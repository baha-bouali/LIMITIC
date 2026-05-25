import { api } from './baseApi';

export interface DashboardPublicationAxeDto {
  id: string;
  title: string;
}

export interface DashboardPublicationSummaryDto {
  id: string;
  type: string;
  publicationType: string;
  title: string;
  year: number;
  status: string;
  visibility: string;
  quartile?: string | null;
  coreRanking?: string | null;
  authors: string[];
  venue?: string | null;
  doi?: string | null;
  axe: DashboardPublicationAxeDto;
  submittedBy?: string | null;
  rejectionReason?: string | null;
}

export interface DashboardPublicationDetailDto extends DashboardPublicationSummaryDto {
  abstract_?: string | null;
  keywords: string[];
  pdfUrl?: string | null;
  journalName?: string | null;
  volume?: string | null;
  number?: string | null;
  pages?: string | null;
  location?: string | null;
  bookTitle?: string | null;
  publisher?: string | null;
  isbn?: string | null;
  reportNumber?: string | null;
  institution?: string | null;
}

export interface DashboardPublicationsQueryParams {
  scope?: string;
  search?: string;
  status?: string;
  type?: string;
  visibility?: string;
  year?: number;
  axeId?: string;
  page?: number;
  limit?: number;
}

export interface DashboardPaginationDto {
  total: number;
  page: number;
  limit: number;
  totalPages: number;
}

export interface DashboardPublicationsListDto {
  items: DashboardPublicationSummaryDto[];
  pagination: DashboardPaginationDto;
}

interface DashboardPublicationsListResponse {
  data?: DashboardPublicationSummaryDto[];
  Data?: DashboardPublicationSummaryDto[];
  pagination?: Partial<DashboardPaginationDto>;
  Pagination?: Partial<DashboardPaginationDto>;
}

interface DashboardPublicationDetailResponse {
  data?: DashboardPublicationDetailDto;
  Data?: DashboardPublicationDetailDto;
}

interface DashboardPublicationResponse {
  publication?: { id: string; status?: string | null } | null;
  Publication?: { id: string; status?: string | null } | null;
  message?: string;
  Message?: string;
}

interface DashboardBaseResponse {
  success?: boolean;
  Success?: boolean;
  message?: string;
  Message?: string;
}

interface DashboardPdfResponse {
  pdfUrl?: string;
  PdfUrl?: string;
  message?: string;
  Message?: string;
}

interface DashboardPublicationStatusResponse {
  id?: string;
  Id?: string;
  status?: string;
  Status?: string;
  message?: string;
  Message?: string;
  extra?: string;
  Extra?: string;
}

export interface DashboardPublicationResearchAxisRequest {
  id: string;
  title?: string;
}

export interface DashboardPublicationJournalArticleRequest {
  journalName?: string;
  volume?: string;
  number?: string;
  pages?: string;
  ranking?: string;
}

export interface DashboardPublicationTechnicalReportRequest {
  reportNumber?: string;
  institution?: string;
}

export interface DashboardPublicationBookChapterRequest {
  bookTitle?: string;
  publisher?: string;
  isbn?: string;
  pages?: string;
}

export interface DashboardPublicationConferenceRequest {
  conferenceName?: string;
  location?: string;
  pages?: string;
  ranking?: string;
}

export interface CreateDashboardPublicationRequest {
  researchAxisId: string;
  title: string;
  abstract?: string | null;
  keywords: string[];
  doi?: string | null;
  venue?: string | null;
  type: string;
  visibility: string;
  year: number;
  authors: string[];
  journalArticle?: DashboardPublicationJournalArticleRequest | null;
  technicalReport?: DashboardPublicationTechnicalReportRequest | null;
  bookChapter?: DashboardPublicationBookChapterRequest | null;
  nationalConference?: DashboardPublicationConferenceRequest | null;
  internationalConference?: DashboardPublicationConferenceRequest | null;
}

export interface UpdateDashboardPublicationRequest extends CreateDashboardPublicationRequest {
  id?: string;
}

export interface PdfRequest {
  pdfUrl: string;
}

export interface RejectPublicationRequest {
  reason: string;
}

const unwrapList = (response: DashboardPublicationsListResponse): DashboardPublicationsListDto => ({
  items: response.data ?? response.Data ?? [],
  pagination: {
    total: (response.pagination ?? response.Pagination)?.total ?? 0,
    page: (response.pagination ?? response.Pagination)?.page ?? 1,
    limit: (response.pagination ?? response.Pagination)?.limit ?? 10,
    totalPages: (response.pagination ?? response.Pagination)?.totalPages ?? 0,
  },
});

const unwrapDetail = (response: DashboardPublicationDetailResponse | DashboardPublicationDetailDto | null | undefined) => {
  if (!response) return null;

  const record = response as Record<string, unknown>;
  if ('data' in record || 'Data' in record) {
    return (record.data ?? record.Data ?? null) as DashboardPublicationDetailDto | null;
  }

  return response as DashboardPublicationDetailDto;
};

const unwrapPublicationRef = (response: DashboardPublicationResponse) => response.publication ?? response.Publication ?? null;

const unwrapPdfUrl = (response: DashboardPdfResponse) => response.pdfUrl ?? response.PdfUrl ?? null;

const unwrapStatus = (response: DashboardPublicationStatusResponse) => ({
  id: response.id ?? response.Id ?? '',
  status: response.status ?? response.Status ?? '',
  message: response.message ?? response.Message ?? '',
  extra: response.extra ?? response.Extra ?? '',
});

const unwrapBaseMessage = (response: DashboardBaseResponse) => response.message ?? response.Message ?? '';

export const dashboardPublicationsApi = api.injectEndpoints({
  endpoints: (builder) => ({
    getDashboardPublications: builder.query<DashboardPublicationsListDto, DashboardPublicationsQueryParams | void>({
      query: (params) => ({
        url: 'publications',
        method: 'GET',
        params: params ?? undefined,
      }),
      transformResponse: (response: DashboardPublicationsListResponse) => unwrapList(response),
      providesTags: ['Publication'],
    }),

    getDashboardPublicationById: builder.query<DashboardPublicationDetailDto | null, string>({
      query: (id) => ({
        url: `publications/${id}`,
        method: 'GET',
      }),
      transformResponse: (response: DashboardPublicationDetailResponse | DashboardPublicationDetailDto) => unwrapDetail(response),
      providesTags: (_result, _error, id) => [{ type: 'Publication' as const, id }],
    }),

    createDashboardPublication: builder.mutation<{ id: string; status?: string | null } | null, CreateDashboardPublicationRequest>({
      query: (body) => ({
        url: 'publications',
        method: 'POST',
        body,
      }),
      transformResponse: (response: DashboardPublicationResponse) => unwrapPublicationRef(response),
      invalidatesTags: ['Publication'],
    }),

    updateDashboardPublication: builder.mutation<{ success: boolean; message: string }, { id: string; body: UpdateDashboardPublicationRequest }>({
      query: ({ id, body }) => ({
        url: `publications/${id}`,
        method: 'PUT',
        body,
      }),
      transformResponse: (response: DashboardBaseResponse) => ({
        success: response.success ?? response.Success ?? false,
        message: unwrapBaseMessage(response),
      }),
      invalidatesTags: (_result, _error, { id }) => ['Publication', { type: 'Publication', id }],
    }),

    deleteDashboardPublication: builder.mutation<{ success: boolean; message: string }, string>({
      query: (id) => ({
        url: `publications/${id}`,
        method: 'DELETE',
      }),
      transformResponse: (response: DashboardBaseResponse) => ({
        success: response.success ?? response.Success ?? false,
        message: unwrapBaseMessage(response),
      }),
      invalidatesTags: ['Publication'],
    }),

    addDashboardPublicationPdf: builder.mutation<{ pdfUrl: string | null; message: string }, { id: string; body: PdfRequest }>({
      query: ({ id, body }) => ({
        url: `publications/${id}/pdf`,
        method: 'POST',
        body,
      }),
      transformResponse: (response: DashboardPdfResponse) => ({
        pdfUrl: unwrapPdfUrl(response),
        message: response.message ?? response.Message ?? '',
      }),
      invalidatesTags: (_result, _error, { id }) => ['Publication', { type: 'Publication', id }],
    }),

    removeDashboardPublicationPdf: builder.mutation<{ success: boolean; message: string }, { id: string; body: PdfRequest }>({
      query: ({ id, body }) => ({
        url: `publications/${id}/pdf`,
        method: 'DELETE',
        body,
      }),
      transformResponse: (response: DashboardBaseResponse) => ({
        success: response.success ?? response.Success ?? false,
        message: unwrapBaseMessage(response),
      }),
      invalidatesTags: (_result, _error, { id }) => ['Publication', { type: 'Publication', id }],
    }),

    submitDashboardPublication: builder.mutation<{ id: string; status: string; message: string }, string>({
      query: (id) => ({
        url: `publications/${id}/submit`,
        method: 'POST',
      }),
      transformResponse: (response: DashboardPublicationStatusResponse) => ({
        ...unwrapStatus(response),
        id: response.id ?? response.Id ?? '',
        status: response.status ?? response.Status ?? '',
        message: response.message ?? response.Message ?? '',
      }),
      invalidatesTags: (_result, _error, id) => ['Publication', { type: 'Publication', id }],
    }),

    validateDashboardPublication: builder.mutation<{ id: string; status: string; message: string; validatedBy?: string }, string>({
      query: (id) => ({
        url: `publications/${id}/validate`,
        method: 'POST',
      }),
      transformResponse: (response: DashboardPublicationStatusResponse) => ({
        ...unwrapStatus(response),
        validatedBy: response.extra ?? response.Extra,
      }),
      invalidatesTags: (_result, _error, id) => ['Publication', { type: 'Publication', id }],
    }),

    rejectDashboardPublication: builder.mutation<{ id: string; status: string; message: string; rejectionReason?: string }, { id: string; body: RejectPublicationRequest }>({
      query: ({ id, body }) => ({
        url: `publications/${id}/reject`,
        method: 'POST',
        body,
      }),
      transformResponse: (response: DashboardPublicationStatusResponse) => ({
        ...unwrapStatus(response),
        rejectionReason: response.extra ?? response.Extra,
      }),
      invalidatesTags: (_result, _error, { id }) => ['Publication', { type: 'Publication', id }],
    }),
  }),
  overrideExisting: false,
});

export const {
  useGetDashboardPublicationsQuery,
  useGetDashboardPublicationByIdQuery,
  useCreateDashboardPublicationMutation,
  useUpdateDashboardPublicationMutation,
  useDeleteDashboardPublicationMutation,
  useAddDashboardPublicationPdfMutation,
  useRemoveDashboardPublicationPdfMutation,
  useSubmitDashboardPublicationMutation,
  useValidateDashboardPublicationMutation,
  useRejectDashboardPublicationMutation,
} = dashboardPublicationsApi;