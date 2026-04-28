import { api } from '@app/api/apiSlice'

interface LoginRequest {
  email: string
  password: string
}

interface AuthResponse {
  token: string
  user: {
    id: string
    email: string
    role: string
  }
}

export const authApiSlice = api.injectEndpoints({
  endpoints: (builder) => ({
    login: builder.mutation<AuthResponse, LoginRequest>({
      query: (credentials) => ({
        url: '/auth/login',
        method: 'POST',
        body: credentials,
      }),
    }),
    getMe: builder.query<AuthResponse['user'], void>({
      query: () => '/auth/me',
      providesTags: ['Auth'],
    }),
  }),
  overrideExisting: false,
})

export const { useLoginMutation, useGetMeQuery } = authApiSlice