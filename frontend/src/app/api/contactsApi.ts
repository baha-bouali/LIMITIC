import { api } from './baseApi';

export interface SendContactMessageRequest {
  fullName: string;
  email: string;
  subject: string;
  message: string;
}

export interface BaseResponse {
  success?: boolean;
  message?: string;
  validationErrors?: Record<string, string[]>;
}

export const contactsApi = api.injectEndpoints({
  endpoints: (builder) => ({
    sendContactMessage: builder.mutation<BaseResponse, SendContactMessageRequest>({
      query: (body) => ({ url: 'contact/send', method: 'POST', body }),
    }),
  }),
  overrideExisting: false,
});

export const { useSendContactMessageMutation } = contactsApi;