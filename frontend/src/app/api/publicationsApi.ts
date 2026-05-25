import { api } from './baseApi';

export interface PublicationAxeDto {
  id: string;
  title: string;
}

export interface PublicPublicationSummaryDto {
  id: string;
  type: string;
  publicationType: string;
  year: number;
  title: string;
  authors: string[];
  venue?: string | null;
  doi?: string | null;
  ranking?: string | null;
  coreRanking?: string | null;
  axe: PublicationAxeDto;
}

export interface PublicPublicationCardDto {
  id: string;
  type: string;
  publicationType: string;
  year: number;
  title: string;
  authors: string[];
  venue?: string | null;
  doi?: string | null;
  ranking?: string | null;
  coreRanking?: string | null;
}

export interface PublicPublicationDetailDto extends PublicPublicationSummaryDto {
  status?: string | null;
  visibility?: string | null;
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

export interface PublicPublicationsQueryParams {
  page?: number;
  limit?: number;
  search?: string;
  type?: string;
  year?: number;
  axeId?: string;
}

interface PublicationsListResponse {
  data?: PublicPublicationSummaryDto[];
  Data?: PublicPublicationSummaryDto[];
  stats?: {
    total?: number;
    journals?: number;
    conferences?: number;
  };
  Stats?: {
    total?: number;
    journals?: number;
    conferences?: number;
  };
  pagination?: {
    total?: number;
    page?: number;
    limit?: number;
    totalPages?: number;
  };
  Pagination?: {
    total?: number;
    page?: number;
    limit?: number;
    totalPages?: number;
  };
}

interface RecentPublicationsResponse {
  data?: PublicPublicationCardDto[];
  Data?: PublicPublicationCardDto[];
}

interface PublicationDetailResponse {
  data?: PublicPublicationDetailDto;
  Data?: PublicPublicationDetailDto;
}

const unwrapPublicationsList = (response: PublicationsListResponse) => response.data ?? response.Data ?? [];

const unwrapRecentPublications = (response: RecentPublicationsResponse) => response.data ?? response.Data ?? [];

const unwrapPublicationDetail = (
  response: PublicationDetailResponse | PublicPublicationDetailDto | null | undefined,
): PublicPublicationDetailDto | null => {
  if (!response) {
    return null;
  }

  const record = response as Record<string, unknown>;

  if ('data' in record || 'Data' in record) {
    return (record.data ?? record.Data ?? null) as PublicPublicationDetailDto | null;
  }

  return response as PublicPublicationDetailDto;
};

export const publicationsApi = api.injectEndpoints({
  endpoints: (builder) => ({
    getPublicPublications: builder.query<{
      items: PublicPublicationSummaryDto[];
      stats: { total: number; journals: number; conferences: number };
      pagination: { total: number; page: number; limit: number; totalPages: number };
    }, PublicPublicationsQueryParams | void>({
      query: (params) => ({
        url: 'v1/public/publications',
        method: 'GET',
        skipAuth: true,
        params: params ?? undefined,
      }),
      transformResponse: (response: PublicationsListResponse) => ({
        items: unwrapPublicationsList(response),
        stats: {
          total: (response.stats ?? response.Stats)?.total ?? 0,
          journals: (response.stats ?? response.Stats)?.journals ?? 0,
          conferences: (response.stats ?? response.Stats)?.conferences ?? 0,
        },
        pagination: {
          total: (response.pagination ?? response.Pagination)?.total ?? 0,
          page: (response.pagination ?? response.Pagination)?.page ?? 1,
          limit: (response.pagination ?? response.Pagination)?.limit ?? 10,
          totalPages: (response.pagination ?? response.Pagination)?.totalPages ?? 0,
        },
      }),
      providesTags: ['Publication'],
    }),

    getRecentPublicPublications: builder.query<PublicPublicationCardDto[], number | void>({
      query: (limit) => ({
        url: 'v1/public/publications/recent',
        method: 'GET',
        skipAuth: true,
        params: typeof limit === 'number' ? { limit } : undefined,
      }),
      transformResponse: (response: RecentPublicationsResponse) => unwrapRecentPublications(response),
      providesTags: ['Publication'],
    }),

    getPublicPublicationById: builder.query<PublicPublicationDetailDto | null, string>({
      query: (id) => ({
        url: `v1/public/publications/${id}`,
        method: 'GET',
        skipAuth: true,
      }),
      transformResponse: (response: PublicationDetailResponse | PublicPublicationDetailDto) => unwrapPublicationDetail(response),
      providesTags: (_result, _error, id) => [{ type: 'Publication' as const, id }],
    }),
  }),
  overrideExisting: false,
});

export const {
  useGetPublicPublicationsQuery,
  useGetRecentPublicPublicationsQuery,
  useGetPublicPublicationByIdQuery,
} = publicationsApi;