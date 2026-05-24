import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react'
import type { RootState } from '@app/store/store'

export const api = createApi({
  reducerPath: 'api',
  baseQuery: fetchBaseQuery({
    baseUrl: import.meta.env.VITE_API_URL,
    prepareHeaders: (headers, { getState }) => {
    // Get the token from the auth slice
    },
  }),
  tagTypes: ['Auth', 'Settings'],
  endpoints: () => ({}),
})