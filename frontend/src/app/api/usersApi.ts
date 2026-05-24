import { api } from './baseApi';

export interface UserDto {
  id: string;
  firstName?: string;
  lastName?: string;
  email: string;
  role?: number;
  isActive?: boolean;
}

export const usersApi = api.injectEndpoints({
  endpoints: (builder) => ({
    getUserById: builder.query<UserDto, string>({
      query: (id) => ({ url: `users/getUser?id=${id}`, method: 'GET' }),
    }),
    getUsers: builder.query<UserDto[], void>({
      query: () => ({ url: 'users/getAll', method: 'GET' }),
    }),
    addUser: builder.mutation<UserDto, Partial<UserDto>>({
      query: (body) => ({ url: 'users/addUser', method: 'POST', body }),
    }),
    changePassword: builder.mutation<void, { email: string; oldPassword: string; newPassword: string }>({
      query: (body) => ({ url: 'users/changePassword', method: 'POST', body }),
    }),
  }),
  overrideExisting: false,
});

export const { useGetUserByIdQuery, useLazyGetUserByIdQuery, useGetUsersQuery, useAddUserMutation, useChangePasswordMutation } = usersApi;
