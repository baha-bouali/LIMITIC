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
}

const normalizeEventStatus = (status: string): EventDto['status'] => {
  const normalized = status?.toLowerCase();

  if (normalized === 'upcoming') return 'A_VENIR';
  if (normalized === 'ongoing') return 'EN_COURS';
  if (normalized === 'past') return 'PASSE';

  return status;
};

const normalizeEvent = (event: EventDto): EventDto => ({
  ...event,
  status: normalizeEventStatus(event.status),
});

export interface GetEventsParams {
  status?: string;
  type?: string;
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

export const eventsApi = api.injectEndpoints({
  endpoints: (builder) => ({
    getEvents: builder.query<EventDto[], GetEventsParams | void>({
      query: (params) => ({
        url: 'events',
        method: 'GET',
        skipAuth: true,
        params: params ?? undefined,
      }),
      transformResponse: (response: GetEventsResponse) => ((response.items ?? response.Items ?? []).map(normalizeEvent)),
    }),
    getEventById: builder.query<EventDto | null, string>({
      query: (eventId) => ({
        url: `events/${eventId}`,
        method: 'GET',
        skipAuth: true,
      }),
      transformResponse: (response: GetEventResponse) => {
        const event = response.event ?? response.Event ?? null;
        return event ? normalizeEvent(event) : null;
      },
    }),
  }),
  overrideExisting: false,
});

export const { useGetEventsQuery, useGetEventByIdQuery } = eventsApi;