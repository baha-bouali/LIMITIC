import { configureStore } from '@reduxjs/toolkit'
import { api } from '@app/api/apiSlice'
import { authReducer } from '@features/auth/store/authStore'

export const store = configureStore({
  reducer: {
    [api.reducerPath]: api.reducer,
    auth: authReducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware().concat(api.middleware),
  devTools: import.meta.env.VITE_ENABLE_DEVTOOLS === 'true',
})

export type RootState = ReturnType<typeof store.getState>
export type AppDispatch = typeof store.dispatch