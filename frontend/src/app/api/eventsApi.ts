import { api } from './baseApi';

export interface SpeakerDto {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  institution?: string | null;
  role?: string | null;
  subject?: string | null;
}

export type EventTypeValue = 'Seminar' | 'Conference' | 'StudyDay' | 'Workshop' | 'Defense' | 'Other';

export const EVENT_TYPE_OPTIONS: Array<{ value: EventTypeValue; label: string }> = [
  { value: 'Seminar', label: 'Séminaire' },
  { value: 'Conference', label: 'Conférence' },
  { value: 'Workshop', label: 'Atelier' },
  { value: 'StudyDay', label: "Journée d'étude" },
  { value: 'Defense', label: 'Soutenance' },
  { value: 'Other', label: 'Autre' },
];

export const formatEventDate = (
  value?: string | null,
  options?: Intl.DateTimeFormatOptions,
  fallback = '—',
) => {
  if (!value) return fallback;

  const parsedDate = new Date(value);
  if (Number.isNaN(parsedDate.getTime())) return fallback;

  return parsedDate.toLocaleDateString('fr-FR', options);
};

export const formatEventDateRange = (
  startDate?: string | null,
  endDate?: string | null,
  options?: Intl.DateTimeFormatOptions,
  fallback = '—',
) => {
  const formattedStartDate = formatEventDate(startDate, options, fallback);

  if (!endDate || endDate === startDate) {
    return formattedStartDate;
  }

  const formattedEndDate = formatEventDate(endDate, options, fallback);
  return `${formattedStartDate} — ${formattedEndDate}`;
};

export interface EventDto {
  id: string;
  type: string;
  title: string;
  startDate: string;
  endDate: string;
  location: string;
  status: 'A_VENIR' | 'EN_COURS' | 'PASSE' | string;
  description: string;
  program?: string | null;
  photoFileNames: string[];
  speakers?: SpeakerDto[];
  researchAxisId?: string;
}

const normalizeEventStatus = (status: string): EventDto['status'] => {
  const normalized = status?.toLowerCase();
  if (normalized === 'upcoming' || normalized === 'a_venir') return 'A_VENIR';
  if (normalized === 'ongoing'  || normalized === 'en_cours') return 'EN_COURS';
  if (normalized === 'past'     || normalized === 'passe')    return 'PASSE';
  return status;
};

const normalizeTypeKey = (value: string) =>
  value
    ?.toLowerCase()
    .trim()
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '')
    .replace(/['’\s-]/g, '');

/**
 * Map English PascalCase type names returned by the backend to the French
 * uppercase display names used throughout the UI.
 */
const TYPE_MAP: Record<string, string> = {
  conference:     'CONFÉRENCE',
  seminar:        'SÉMINAIRE',
  séminaire:      'SÉMINAIRE',
  seminaire:      'SÉMINAIRE',
  workshop:       'ATELIER',
  atelier:        'ATELIER',
  'study day':    "JOURNÉE D'ÉTUDE",
  studyday:       "JOURNÉE D'ÉTUDE",
  journeedétude:  "JOURNÉE D'ÉTUDE",
  "journée d'étude": "JOURNÉE D'ÉTUDE",
  defense:        'SOUTENANCE',
  soutenance:     'SOUTENANCE',
  other:          'AUTRE',
  autre:          'AUTRE',
};

const normalizeEventType = (type: string): string =>
  TYPE_MAP[normalizeTypeKey(type)] ?? type;

const EVENT_TYPE_VALUE_MAP: Record<string, EventTypeValue> = {
  seminar: 'Seminar',
  seminaire: 'Seminar',
  séminaire: 'Seminar',
  conference: 'Conference',
  workshop: 'Workshop',
  atelier: 'Workshop',
  studyday: 'StudyDay',
  journeedetude: 'StudyDay',
  "journéed'étude": 'StudyDay',
  soutenance: 'Defense',
  defense: 'Defense',
  other: 'Other',
  autre: 'Other',
};

export const toBackendEventType = (type: string): EventTypeValue => {
  const normalized = normalizeTypeKey(type);
  return EVENT_TYPE_VALUE_MAP[normalized] ?? 'Other';
};

const normalizeEvent = (event: EventDto): EventDto => ({
  ...event,
  type:   normalizeEventType(event.type),
  status: normalizeEventStatus(event.status),
});

export interface GetEventsParams {
  status?: string;
  type?: EventTypeValue;
  page?: number;
  limit?: number;
  q?: string;
}

interface GetEventsResponse {
  success: boolean;
  items?: EventDto[];
  Items?: EventDto[];
  total?: number;
  Total?: number;
  page?: number;
  Page?: number;
  limit?: number;
  Limit?: number;
  message?: string;
  Message?: string;
}

interface GetEventResponse {
  success: boolean;
  event?: EventDto | null;
  Event?: EventDto | null;
  message?: string;
  Message?: string;
}

interface CreateEventResponse {
  success: boolean;
  event?: EventDto | null;
  Event?: EventDto | null;
  message?: string;
  validationErrors?: string[];
}

interface UpdateEventResponse {
  success: boolean;
  event?: EventDto | null;
  Event?: EventDto | null;
  message?: string;
  validationErrors?: string[];
}

interface BaseResponse {
  success: boolean;
  message?: string;
  validationErrors?: string[];
}

interface AddSpeakerResponse {
  success: boolean;
  speaker?: SpeakerDto | null;
  Speaker?: SpeakerDto | null;
  message?: string;
  validationErrors?: string[];
}

interface UpdateSpeakerResponse {
  success: boolean;
  speaker?: SpeakerDto | null;
  Speaker?: SpeakerDto | null;
  message?: string;
  validationErrors?: string[];
}

export interface CreateEventSpeakerRequest {
  firstName: string;
  lastName: string;
  email: string;
  institution?: string;
  role?: string;
  subject?: string;
}

export interface CreateEventRequest {
  type: EventTypeValue;
  title: string;
  startDate: string;
  endDate: string;
  location: string;
  description: string;
  program?: string;
  researchAxisId: string;
  speakers?: CreateEventSpeakerRequest[];
}

export interface UpdateEventRequest {
  type: EventTypeValue;
  title: string;
  startDate: string;
  endDate: string;
  location: string;
  description: string;
  program?: string;
  researchAxisId: string;
}

export interface SpeakerRequest {
  firstName: string;
  lastName: string;
  email: string;
  institution?: string;
  role?: string;
  subject?: string;
}

export const eventsApi = api.injectEndpoints({
  endpoints: (builder) => ({
    getEvents: builder.query<EventDto[], GetEventsParams | void>({
      query: (params) => ({
        url: 'events',
        method: 'GET',
        skipAuth: true,
        params: params ?? undefined,
      }),
      providesTags: ['Event'],
      transformResponse: (response: GetEventsResponse) =>
        (response.items ?? response.Items ?? []).map(normalizeEvent),
    }),

    getEventById: builder.query<EventDto | null, string>({
      query: (eventId) => ({
        url: `events/${eventId}`,
        method: 'GET',
        skipAuth: true,
      }),
      providesTags: (_result, _error, id) => [{ type: 'Event', id }],
      transformResponse: (response: GetEventResponse) => {
        const event = response.event ?? response.Event ?? null;
        return event ? normalizeEvent(event) : null;
      },
    }),

    createEvent: builder.mutation<EventDto, CreateEventRequest>({
      query: (body) => ({
        url: 'events',
        method: 'POST',
        body,
      }),
      invalidatesTags: ['Event'],
      transformResponse: (response: CreateEventResponse) => {
        const event = response.event ?? response.Event;
        if (!event) throw new Error(response.message ?? 'Failed to create event');
        return normalizeEvent(event);
      },
    }),

    updateEvent: builder.mutation<EventDto, { id: string; body: UpdateEventRequest }>({
      query: ({ id, body }) => ({
        url: `events/${id}`,
        method: 'PUT',
        body,
      }),
      invalidatesTags: (_result, _error, { id }) => ['Event', { type: 'Event', id }],
      transformResponse: (response: UpdateEventResponse) => {
        const event = response.event ?? response.Event;
        if (!event) throw new Error(response.message ?? 'Failed to update event');
        return normalizeEvent(event);
      },
    }),

    deleteEvent: builder.mutation<void, string>({
      query: (id) => ({
        url: `events/${id}`,
        method: 'DELETE',
      }),
      invalidatesTags: ['Event'],
      transformResponse: (response: BaseResponse) => {
        if (!response.success) throw new Error(response.message ?? 'Failed to delete event');
      },
    }),

    addSpeaker: builder.mutation<SpeakerDto, { eventId: string; body: SpeakerRequest }>({
      query: ({ eventId, body }) => ({
        url: `events/${eventId}/speakers`,
        method: 'POST',
        body,
      }),
      invalidatesTags: (_result, _error, { eventId }) => ['Event', { type: 'Event', id: eventId }],
      transformResponse: (response: AddSpeakerResponse) => {
        const speaker = response.speaker ?? response.Speaker;
        if (!speaker) throw new Error(response.message ?? 'Failed to add speaker');
        return speaker;
      },
    }),

    updateSpeaker: builder.mutation<SpeakerDto, { eventId: string; speakerId: string; body: SpeakerRequest }>({
      query: ({ eventId, speakerId, body }) => ({
        url: `events/${eventId}/speakers/${speakerId}`,
        method: 'PUT',
        body,
      }),
      invalidatesTags: (_result, _error, { eventId }) => ['Event', { type: 'Event', id: eventId }],
      transformResponse: (response: UpdateSpeakerResponse) => {
        const speaker = response.speaker ?? response.Speaker;
        if (!speaker) throw new Error(response.message ?? 'Failed to update speaker');
        return speaker;
      },
    }),

    deleteSpeaker: builder.mutation<void, { eventId: string; speakerId: string }>({
      query: ({ eventId, speakerId }) => ({
        url: `events/${eventId}/speakers/${speakerId}`,
        method: 'DELETE',
      }),
      invalidatesTags: (_result, _error, { eventId }) => ['Event', { type: 'Event', id: eventId }],
      transformResponse: (response: BaseResponse) => {
        if (!response.success) throw new Error(response.message ?? 'Failed to delete speaker');
      },
    }),

    uploadEventPhotos: builder.mutation<EventDto, { eventId: string; files: File[] }>({
      query: ({ eventId, files }) => {
        const formData = new FormData();
        files.forEach((file) => formData.append('files', file));
        return {
          url: `events/${eventId}/photos`,
          method: 'POST',
          body: formData,
        };
      },
      invalidatesTags: (_result, _error, { eventId }) => ['Event', { type: 'Event', id: eventId }],
      transformResponse: (response: { success: boolean; event?: EventDto; Event?: EventDto; message?: string }) => {
        const event = response.event ?? response.Event;
        if (!event) throw new Error(response.message ?? 'Failed to upload photos');
        return normalizeEvent(event);
      },
    }),
  }),
  overrideExisting: false,
});

export const {
  useGetEventsQuery,
  useGetEventByIdQuery,
  useCreateEventMutation,
  useUpdateEventMutation,
  useDeleteEventMutation,
  useAddSpeakerMutation,
  useUpdateSpeakerMutation,
  useDeleteSpeakerMutation,
  useUploadEventPhotosMutation,
} = eventsApi;