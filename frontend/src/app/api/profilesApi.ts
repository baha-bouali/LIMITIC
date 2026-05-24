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

export const profilesApi = api.injectEndpoints({
  endpoints: (builder) => ({
    getResearcherProfile: builder.query<ResearcherProfileDto, string>({
      query: (userId) => ({ url: `profiles/researchers/${userId}`, method: 'GET' }),
      transformResponse: (response: any) => response.profile ?? response,
    }),
    getPhDStudentProfile: builder.query<PhDStudentProfileDto, string>({
      query: (userId) => ({ url: `profiles/phd-students/${userId}`, method: 'GET' }),
      transformResponse: (response: any) => response.profile ?? response,
    }),
    getMasterianProfile: builder.query<MasterianProfileDto, string>({
      query: (userId) => ({ url: `profiles/masterians/${userId}`, method: 'GET' }),
      transformResponse: (response: any) => response.profile ?? response,
    }),
  }),
  overrideExisting: false,
});

export const {
  useGetResearcherProfileQuery,
  useGetPhDStudentProfileQuery,
  useGetMasterianProfileQuery,
} = profilesApi;