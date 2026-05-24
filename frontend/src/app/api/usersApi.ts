import { api } from './baseApi';

export interface UserDto {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role?: number;
  isActive?: boolean;
  avatarBlobName?: string;
}

export interface CreateUserRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  isActive: boolean;
}

export interface UpdateUserRoleRequest {
  role: number;
  rank?: string;
  specialty?: string;
  office?: string;
  phoneNumber?: string;
  researchAxisIds?: string[];
  enrollmentYear?: number;
  cohort?: string;
  dissertationSubject?: string;
}

// Backend wraps single-user responses in { success, user }
interface GetUserResponse {
  success: boolean;
  user: UserDto;
  message?: string;
}

// Backend wraps list responses in { success, items, total, counts, page, limit }
interface GetUsersResponse {
  success: boolean;
  items: UserDto[];
  total: number;
  counts: Record<string, number>;
  page: number;
  limit: number;
}

interface CreateUserResponse {
  success: boolean;
  user?: UserDto;
  message?: string;
  validationErrors?: Record<string, string[]>;
}

export interface GetUsersParams {
  role?: number;
  status?: 'active' | 'inactive';
  q?: string;
  page?: number;
  limit?: number;
}

export const usersApi = api.injectEndpoints({
  endpoints: (builder) => ({
    getUserById: builder.query<UserDto, string>({
      query: (id) => ({ url: `users/getUser?id=${id}`, method: 'GET' }),
      // Backend returns { success, user: UserDto } — extract the nested user object
      transformResponse: (response: GetUserResponse) => response.user,
      providesTags: (_result, _error, id) => [{ type: 'User', id }],
    }),

    getUsers: builder.query<UserDto[], GetUsersParams | void>({
      // Backend: GET /api/users/ (not /users/getAll)
      query: (params) => ({ url: 'users', method: 'GET', params: params ?? undefined }),
      transformResponse: (response: GetUsersResponse) => response.items,
      providesTags: ['User'],
    }),

    addUser: builder.mutation<UserDto | null, CreateUserRequest>({
      query: (body) => ({ url: 'users/addUser', method: 'POST', body }),
      transformResponse: (response: CreateUserResponse) => response.user ?? null,
      invalidatesTags: ['User'],
    }),

    updateUserRole: builder.mutation<void, { userId: string; data: UpdateUserRoleRequest }>({
      query: ({ userId, data }) => ({ url: `users/updateRole/${userId}`, method: 'PUT', body: data }),
      invalidatesTags: (_result, _error, { userId }) => [{ type: 'User', id: userId }],
    }),

    // userId passed as query param — backend simple-type inference binds it from query string
    activateUser: builder.mutation<void, string>({
      query: (userId) => ({ url: `users/activateUser?userId=${userId}`, method: 'POST' }),
      invalidatesTags: (_result, _error, userId) => [{ type: 'User', id: userId }],
    }),

    deactivateUser: builder.mutation<void, string>({
      query: (userId) => ({ url: `users/deactivateUser?userId=${userId}`, method: 'POST' }),
      invalidatesTags: (_result, _error, userId) => [{ type: 'User', id: userId }],
    }),

    changePassword: builder.mutation<void, { email: string; oldPassword: string; newPassword: string }>({
      query: (body) => ({ url: 'users/changePassword', method: 'POST', body }),
    }),

    uploadAvatar: builder.mutation<string, { userId: string; avatar: File }>({
      query: ({ userId, avatar }) => {
        const formData = new FormData();
        formData.append('avatar', avatar);
        return { url: `users/${userId}/avatar`, method: 'POST', body: formData };
      },
      invalidatesTags: (_result, _error, { userId }) => [{ type: 'User', id: userId }],
    }),
  }),
  overrideExisting: false,
});

export const {
  useGetUserByIdQuery,
  useLazyGetUserByIdQuery,
  useGetUsersQuery,
  useAddUserMutation,
  useUpdateUserRoleMutation,
  useActivateUserMutation,
  useDeactivateUserMutation,
  useChangePasswordMutation,
  useUploadAvatarMutation,
} = usersApi;
