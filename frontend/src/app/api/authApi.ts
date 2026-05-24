import { api } from './baseApi';
import { clearAccessToken, setAccessToken } from '../auth/session';
import type { UserDto } from './usersApi';

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  user?: UserDto;
  message?: string;
  validationErrors?: string[];
}

export interface ForgotPasswordRequest {
  email: string;
}

export interface VerifyResetCodeRequest {
  email: string;
  otpToken: string;
}

export interface VerifyResetCodeResponse {
  resetToken: string;
}

export interface ResetPasswordRequest {
  email: string;
  newPassword: string;
  resetToken: string;
}

export interface BaseResponse {
  success?: boolean;
  message?: string;
  validationErrors?: string[];
}

export const authApi = api.injectEndpoints({
  endpoints: (builder) => ({
    login: builder.mutation<LoginResponse, LoginRequest>({
      query: (body) => ({ url: 'auth/login', method: 'POST', body }),
      async onQueryStarted(_, { queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          if (data?.accessToken) setAccessToken(data.accessToken);
        } catch {
          // ignore
        }
      },
    }),
    refreshToken: builder.mutation<LoginResponse, void>({
      query: () => ({ url: 'auth/refreshToken', method: 'POST' }),
      async onQueryStarted(_, { queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          if (data?.accessToken) setAccessToken(data.accessToken);
        } catch {
          clearAccessToken();
        }
      },
    }),
    logout: builder.mutation<BaseResponse, void>({
      query: () => ({ url: 'auth/logout', method: 'POST' }),
      async onQueryStarted(_, { queryFulfilled }) {
        try {
          await queryFulfilled;
        } finally {
          clearAccessToken();
        }
      },
    }),
    forgotPassword: builder.mutation<BaseResponse, ForgotPasswordRequest>({
      query: (body) => ({ url: 'auth/forgotPassword', method: 'POST', body }),
    }),
    verifyResetCode: builder.mutation<VerifyResetCodeResponse, VerifyResetCodeRequest>({
      query: (body) => ({ url: 'auth/verifyOTP', method: 'POST', body }),
    }),
    resetPassword: builder.mutation<BaseResponse, ResetPasswordRequest>({
      query: (body) => ({ url: 'auth/resetPassword', method: 'POST', body }),
    }),
  }),
  overrideExisting: false,
});

export const {
  useLoginMutation,
  useRefreshTokenMutation,
  useLogoutMutation,
  useForgotPasswordMutation,
  useVerifyResetCodeMutation,
  useResetPasswordMutation,
} = authApi;
