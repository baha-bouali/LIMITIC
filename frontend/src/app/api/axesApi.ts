import { api } from './baseApi';
import type { ResearchAxisDto } from './profilesApi';

interface AxisResponse {
  success: boolean;
  researchAxis?: ResearchAxisDto;
  message?: string;
}

interface AxesListResponse {
  success: boolean;
  researchAxes?: ResearchAxisDto[];
  message?: string;
}

export interface AxisUpsertBody {
  title: string;
  description: string;
  themes: string[];
  color?: string;
  responsibleId?: string | null;
  memberIds?: string[];
}

export const axesApi = api.injectEndpoints({
  endpoints: (builder) => ({

    getAllAxes: builder.query<ResearchAxisDto[], void>({
      query: () => ({ url: 'research-axes', skipAuth: true }),
      transformResponse: (response: AxesListResponse) => response.researchAxes || [],
      providesTags: ['Axis'],
    }),

    getAxisById: builder.query<ResearchAxisDto, string>({
      query: (id) => ({ url: `research-axes/${id}`, skipAuth: true }),
      transformResponse: (response: AxisResponse) => response.researchAxis!,
      providesTags: (_result, _error, id) => [{ type: 'Axis', id }],
    }),

    createAxis: builder.mutation<ResearchAxisDto, AxisUpsertBody>({
      query: (body) => ({ url: 'research-axes', method: 'POST', body }),
      transformResponse: (response: AxisResponse) => response.researchAxis!,
      invalidatesTags: ['Axis'],
    }),

    updateAxis: builder.mutation<ResearchAxisDto, { id: string; data: AxisUpsertBody }>({
      query: ({ id, data }) => ({ url: `research-axes/${id}`, method: 'PUT', body: data }),
      transformResponse: (response: AxisResponse) => response.researchAxis!,
      invalidatesTags: (_result, _error, { id }) => ['Axis', { type: 'Axis', id }],
    }),

    deleteAxis: builder.mutation<void, string>({
      query: (id) => ({ url: `research-axes/${id}`, method: 'DELETE' }),
      invalidatesTags: ['Axis'],
    }),

  }),
  overrideExisting: false,
});

export const {
  useGetAllAxesQuery,
  useGetAxisByIdQuery,
  useCreateAxisMutation,
  useUpdateAxisMutation,
  useDeleteAxisMutation,
} = axesApi;
