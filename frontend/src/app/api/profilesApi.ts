import { api } from './baseApi';

export interface ResearchAxisDto {
  id: string;
  title: string;
  description: string;
  themes: string[];
  color?: string;
  responsibleId?: string | null;
}

export interface ResearcherProfileDto {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: number;
  isActive: boolean;
  rank?: string;
  specialty?: string;
  office?: string;
  phoneNumber?: string;
  biography?: string;
  orcid?: string;
  googleScholar?: string;
  researchGate?: string;
  linkedIn?: string;
  researchAxes?: ResearchAxisDto[];
}

export interface PhDStudentProfileDto {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: number;
  isActive: boolean;
  thesisSubject?: string;
  enrollmentYear: number;
  supervisorId?: string | null;
  supervisorName?: string | null;
  researchAxes?: ResearchAxisDto[];
}

export interface MasterianProfileDto {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: number;
  isActive: boolean;
  dissertationSubject?: string;
  cohort?: string;
  supervisorId?: string | null;
  supervisorName?: string | null;
}

// ── Request body types (mirror backend UpdateXxxProfileRequest) ────────────

export interface UpdateResearcherProfileBody {
  rank: string;
  specialty: string;
  office: string;
  phoneNumber: string;
  biography?: string;
  orcid?: string;
  googleScholar?: string;
  researchGate?: string;
  linkedIn?: string;
  researchAxisIds?: string[];
}

export interface UpdatePhDStudentProfileBody {
  thesisSubject?: string;
  enrollmentYear: number;
  supervisorId?: string;
  researchAxisIds?: string[];
}

export interface UpdateMasterianProfileBody {
  dissertationSubject: string;
  cohort: string;
  supervisorId?: string;
}

// ── Raw backend response shapes ────────────────────────────────────────────

interface ProfileResponse<T> {
  success: boolean;
  profile: T;
  message?: string;
}

interface ResearchAxesListResponse {
  success: boolean;
  researchAxes: ResearchAxisDto[];
}

// ── Injected endpoints ─────────────────────────────────────────────────────

export const profilesApi = api.injectEndpoints({
  endpoints: (builder) => ({

    // ── Profile GET endpoints ─────────────────────────────────────────────
    // Backend returns { success, profile: <Dto> } — transformResponse unwraps it.

    getResearcherProfile: builder.query<ResearcherProfileDto, string>({
      query: (userId) => ({ url: `profiles/researchers/${userId}`, method: 'GET' }),
      transformResponse: (response: ProfileResponse<ResearcherProfileDto>) => response.profile,
    }),

    getPhDStudentProfile: builder.query<PhDStudentProfileDto, string>({
      query: (userId) => ({ url: `profiles/phd-students/${userId}`, method: 'GET' }),
      transformResponse: (response: ProfileResponse<PhDStudentProfileDto>) => response.profile,
    }),

    getMasterianProfile: builder.query<MasterianProfileDto, string>({
      query: (userId) => ({ url: `profiles/masterians/${userId}`, method: 'GET' }),
      transformResponse: (response: ProfileResponse<MasterianProfileDto>) => response.profile,
    }),

    // ── Research axes list ────────────────────────────────────────────────
    // Backend: GET /api/research-axes → { success, researchAxes: [] }

    getResearchAxes: builder.query<ResearchAxisDto[], void>({
      query: () => ({ url: 'research-axes', method: 'GET' }),
      transformResponse: (response: ResearchAxesListResponse) => response.researchAxes,
    }),

    // ── Profile update mutations ──────────────────────────────────────────

    updateResearcherProfile: builder.mutation<
      ResearcherProfileDto | undefined,
      { userId: string; data: UpdateResearcherProfileBody }
    >({
      query: ({ userId, data }) => ({
        url: `profiles/researchers/${userId}`,
        method: 'PUT',
        body: data,
      }),
      transformResponse: (response: ProfileResponse<ResearcherProfileDto>) => response.profile,
    }),

    updatePhDStudentProfile: builder.mutation<
      PhDStudentProfileDto | undefined,
      { userId: string; data: UpdatePhDStudentProfileBody }
    >({
      query: ({ userId, data }) => ({
        url: `profiles/phd-students/${userId}`,
        method: 'PUT',
        body: data,
      }),
      transformResponse: (response: ProfileResponse<PhDStudentProfileDto>) => response.profile,
    }),

    updateMasterianProfile: builder.mutation<
      MasterianProfileDto | undefined,
      { userId: string; data: UpdateMasterianProfileBody }
    >({
      query: ({ userId, data }) => ({
        url: `profiles/masterians/${userId}`,
        method: 'PUT',
        body: data,
      }),
      transformResponse: (response: ProfileResponse<MasterianProfileDto>) => response.profile,
    }),
  }),
  overrideExisting: false,
});

export const {
  useGetResearcherProfileQuery,
  useLazyGetResearcherProfileQuery,
  useGetPhDStudentProfileQuery,
  useLazyGetPhDStudentProfileQuery,
  useGetMasterianProfileQuery,
  useLazyGetMasterianProfileQuery,
  useGetResearchAxesQuery,
  useUpdateResearcherProfileMutation,
  useUpdatePhDStudentProfileMutation,
  useUpdateMasterianProfileMutation,
} = profilesApi;
