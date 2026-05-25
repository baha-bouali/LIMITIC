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
  // Handle both camelCase (ASP.NET Core default) and PascalCase defensively
  items?: AuditLogDto[];
  Items?: AuditLogDto[];
  message?: string;
}

export const auditLogsApi = api.injectEndpoints({
  endpoints: (builder) => ({
    getAuditLogs: builder.query<AuditLogDto[], { fromUtc: string; toUtc?: string }>({
      // Build URL directly — avoids URLSearchParams encoding colons in DateTime strings
      query: ({ fromUtc, toUtc }) => {
        const qs = toUtc
          ? `fromUtc=${fromUtc}&toUtc=${toUtc}`
          : `fromUtc=${fromUtc}`;
        return { url: `auditLogs?${qs}`, method: 'GET' };
      },
      transformResponse: (response: GetAuditLogsResponse) =>
        response.items ?? response.Items ?? [],
      providesTags: ['AuditLog'],
    }),
  }),
  overrideExisting: false,
});

export const { useGetAuditLogsQuery } = auditLogsApi;
