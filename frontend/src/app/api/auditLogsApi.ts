import { api } from './baseApi';

export interface AuditLogDto {
  id: string;
  actorId: string;
  actorName?: string | null;
  action: string;
  resource: string;
  timestamp: string;
}

interface GetAuditLogsResponse {
  success: boolean;
  items: AuditLogDto[];
  fromUtc: string;
  toUtc: string;
}

interface GetAuditLogsParams {
  fromUtc: string;
  toUtc?: string;
}

export const auditLogsApi = api.injectEndpoints({
  endpoints: (builder) => ({
    getAuditLogs: builder.query<AuditLogDto[], GetAuditLogsParams>({
      query: ({ fromUtc, toUtc }) => ({
        url: 'auditLogs',
        method: 'GET',
        params: toUtc ? { fromUtc, toUtc } : { fromUtc },
      }),
      transformResponse: (response: GetAuditLogsResponse) => response.items,
      providesTags: ['AuditLog'],
    }),
  }),
  overrideExisting: false,
});

export const { useGetAuditLogsQuery } = auditLogsApi;
