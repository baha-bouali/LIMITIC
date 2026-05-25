import { api } from '../../../app/api/baseApi'

export interface Identity {
  labName: string
  labSlogan: string
  contactEmail: string
  address: string
  phone: string
  logoUrl?: string | null
}

export interface SmtpResponse {
  host: string
  port: number
  username: string
  useTls: boolean
}

export interface SettingsResponse {
  success: boolean
  identity: Identity
  smtp: SmtpResponse
}

export interface SmtpUpdateRequest {
  host: string
  port: number
  username: string
  password?: string | null
  useTls: boolean
}

export interface UpdateSettingsRequest {
  identity: Identity
  smtp: SmtpUpdateRequest
}

export interface UpdateSettingsResponse {
  success: boolean
}

export interface TestSmtpResponse {
  success: boolean
  message?: string
}

export const settingsApiSlice = api.injectEndpoints({
  endpoints: (builder) => ({
    getSettings: builder.query<SettingsResponse, void>({
      query: () => '/dashboard/superadmin/settings',
      providesTags: ['Settings'],
    }),
    updateSettings: builder.mutation<UpdateSettingsResponse, UpdateSettingsRequest>({
      query: (body) => ({
        url: '/dashboard/superadmin/settings',
        method: 'PUT',
        body,
      }),
      invalidatesTags: ['Settings'],
    }),
    testSmtp: builder.mutation<TestSmtpResponse, { testEmail: string }>({
      query: (body) => ({
        url: '/dashboard/superadmin/settings/smtp/test',
        method: 'POST',
        body,
      }),
    }),
  }),
  overrideExisting: false,
})

export const { useGetSettingsQuery, useUpdateSettingsMutation, useTestSmtpMutation } = settingsApiSlice
