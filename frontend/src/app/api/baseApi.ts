import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import type { BaseQueryFn, FetchArgs, FetchBaseQueryError } from '@reduxjs/toolkit/query';

import { clearAccessToken, getAccessToken, setAccessToken } from '../auth/session';

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? 'https://localhost:7242/api';

// Debug: print resolved API base URL so we can confirm env is read correctly in the browser
if (typeof window !== 'undefined' && window.console) {
  // eslint-disable-next-line no-console
  console.info('[baseApi] apiBaseUrl =', apiBaseUrl);
}

const rawBaseQuery = fetchBaseQuery({
  baseUrl: apiBaseUrl,
  credentials: 'include',
  prepareHeaders: (headers) => {
    const token = getAccessToken();

    if (token) {
      headers.set('authorization', `Bearer ${token}`);
    }

    headers.set('accept', 'application/json');

    return headers;
  },
});

const publicRawBaseQuery = fetchBaseQuery({
  baseUrl: apiBaseUrl,
  credentials: 'omit',
  prepareHeaders: (headers) => {
    headers.set('accept', 'application/json');
    return headers;
  },
});

export const baseQueryWithReauth: BaseQueryFn<string | FetchArgs, unknown, FetchBaseQueryError> = async (
  args,
  api,
  extraOptions,
) => {
  // Debug: log the outgoing request (stringified for clarity)
  if (typeof window !== 'undefined' && window.console) {
    try {
      const argStr = typeof args === 'string' ? args : JSON.stringify(args);
      // eslint-disable-next-line no-console
      console.debug('[baseApi] request ->', argStr);
    } catch (e) {
      // eslint-disable-next-line no-console
      console.debug('[baseApi] request -> (unserializable)', args);
    }
  }

  const requestArgs = typeof args === 'string' ? args : (args as FetchArgs & { skipAuth?: boolean });
  const skipAuth = typeof args !== 'string' && Boolean(requestArgs.skipAuth);
  const sanitizedArgs = typeof args === 'string'
    ? args
    : (({ skipAuth: _skipAuth, ...rest }) => rest)(requestArgs);

  let result = await (skipAuth ? publicRawBaseQuery : rawBaseQuery)(sanitizedArgs, api, extraOptions);

  const requestUrl = typeof args === 'string' ? args : (requestArgs as FetchArgs).url;
  if (!skipAuth && result.error?.status === 401 && requestUrl && !String(requestUrl).includes('auth/refreshToken')) {
    // try to get a new token
    const refreshResult = await rawBaseQuery({ url: 'auth/refreshToken', method: 'POST' }, api, extraOptions);

    const accessToken = (refreshResult.data as { accessToken?: string } | undefined)?.accessToken;
    if (accessToken) {
      setAccessToken(accessToken);
      // retry original query
        result = await rawBaseQuery(sanitizedArgs, api, extraOptions);
    } else {
      clearAccessToken();
    }
  }

  // Debug: log the response or error (status + payload)
  if (typeof window !== 'undefined' && window.console) {
    try {
      if (result.error) {
        // eslint-disable-next-line no-console
        console.debug('[baseApi] result.error.status =', result.error.status, 'data =', JSON.stringify(result.error.data));
      } else {
        // eslint-disable-next-line no-console
        console.debug('[baseApi] result.data =', JSON.stringify(result.data));
      }
    } catch (e) {
      // eslint-disable-next-line no-console
      console.debug('[baseApi] result (unserializable) =', result);
    }
  }

  return result;
};

export const api = createApi({
  reducerPath: 'api',
  baseQuery: baseQueryWithReauth,
  tagTypes: ['Auth', 'User', 'Publication', 'Event', 'Axis', 'Notification', 'AuditLog', 'Settings'],
  endpoints: () => ({}),
});

export { apiBaseUrl, rawBaseQuery };