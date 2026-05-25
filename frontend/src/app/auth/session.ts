export type UserRole = 'SUPER_ADMIN' | 'ADMIN' | 'CHERCHEUR' | 'DOCTORANT' | 'MASTERIEN' | 'VISITOR';

export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  role: UserRole;
  avatar?: string;
}

export const DASHBOARD_PATH_BY_ROLE: Record<UserRole, string> = {
  SUPER_ADMIN: '/dashboard/superadmin',
  ADMIN: '/dashboard/admin',
  CHERCHEUR: '/dashboard/chercheur',
  DOCTORANT: '/dashboard/doctorant',
  MASTERIEN: '/dashboard/masterien',
  VISITOR: '/dashboard/visitor',
};

const DASHBOARD_SLUG_BY_ROLE: Record<UserRole, string> = {
  SUPER_ADMIN: 'superadmin',
  ADMIN: 'admin',
  CHERCHEUR: 'chercheur',
  DOCTORANT: 'doctorant',
  MASTERIEN: 'masterien',
  VISITOR: 'visitor',
};

export const ACCESS_TOKEN_KEY = 'accessToken';
export const USER_ID_KEY = 'userId';
export const ROLE_KEY = 'userRole';
export const STORED_USER_KEY = 'storedUser';

function normalizeStoredIdentifier(value: unknown): string | null {
  if (typeof value !== 'string') {
    return null;
  }

  const trimmed = value.trim();
  if (!trimmed || trimmed === 'undefined' || trimmed === 'null') {
    return null;
  }

  return trimmed;
}

function readJwtPayload(token: string): Record<string, unknown> | null {
  const parts = token.split('.');
  if (parts.length < 2) return null;

  try {
    const payload = parts[1].replace(/-/g, '+').replace(/_/g, '/');
    const base64 = payload.padEnd(payload.length + (4 - (payload.length % 4 || 4)) % 4, '=');
    const json = decodeURIComponent(
      atob(base64)
        .split('')
        .map((character) => `%${character.charCodeAt(0).toString(16).padStart(2, '0')}`)
        .join('')
    );

    return JSON.parse(json) as Record<string, unknown>;
  } catch {
    return null;
  }
}

const ROLE_BY_BACKEND_ENUM: Record<number, UserRole> = {
  1: 'SUPER_ADMIN',
  2: 'ADMIN',
  3: 'CHERCHEUR',
  4: 'DOCTORANT',
  5: 'MASTERIEN',
  6: 'VISITOR',
};

export function normalizeRole(value: unknown, email?: string): UserRole {
  if (typeof value === 'number' && ROLE_BY_BACKEND_ENUM[value]) {
    return ROLE_BY_BACKEND_ENUM[value];
  }

  if (typeof value === 'string' && /^\d+$/.test(value) && ROLE_BY_BACKEND_ENUM[Number(value)]) {
    return ROLE_BY_BACKEND_ENUM[Number(value)];
  }

  const candidates = Array.isArray(value) ? value : [value];

  for (const candidate of candidates) {
    const normalized = String(candidate ?? '').toUpperCase();
    if (
      normalized === 'SUPER_ADMIN' ||
      normalized === 'ADMIN' ||
      normalized === 'CHERCHEUR' ||
      normalized === 'DOCTORANT' ||
      normalized === 'MASTERIEN' ||
      normalized === 'VISITOR' ||
      normalized === 'SUPERADMIN' ||
      normalized === 'RESEARCHER' ||
      normalized === 'PHDSTUDENT' ||
      normalized === 'MASTERIAN'
    ) {
      switch (normalized) {
        case 'SUPERADMIN':
          return 'SUPER_ADMIN';
        case 'RESEARCHER':
          return 'CHERCHEUR';
        case 'PHDSTUDENT':
          return 'DOCTORANT';
        case 'MASTERIAN':
          return 'MASTERIEN';
        default:
          return normalized as UserRole;
      }
    }
  }

  const source = (email ?? '').toLowerCase();
  if (source.includes('superadmin')) return 'SUPER_ADMIN';
  if (source.includes('admin')) return 'ADMIN';
  if (source.includes('doctorant')) return 'DOCTORANT';
  if (source.includes('masterien')) return 'MASTERIEN';
  if (source.includes('visitor')) return 'VISITOR';

  return 'CHERCHEUR';
}

export function getDashboardPathForRole(role?: UserRole | string | null) {
  const normalizedRole = normalizeRole(role);

  if (normalizedRole in DASHBOARD_PATH_BY_ROLE) {
    return DASHBOARD_PATH_BY_ROLE[normalizedRole];
  }

  return DASHBOARD_PATH_BY_ROLE.CHERCHEUR;
}

export function getDashboardSlugForRole(role?: unknown) {
  const normalizedRole = normalizeRole(role);
  return DASHBOARD_SLUG_BY_ROLE[normalizedRole];
}

export function getAccessToken() {
  return window.localStorage.getItem(ACCESS_TOKEN_KEY);
}

export function setAccessToken(token: string) {
  window.localStorage.setItem(ACCESS_TOKEN_KEY, token);
}

export function clearAccessToken() {
  window.localStorage.removeItem(ACCESS_TOKEN_KEY);
}

export function getUserId() {
  return normalizeStoredIdentifier(window.localStorage.getItem(USER_ID_KEY));
}

export function setUserId(userId: string | null | undefined) {
  const normalizedUserId = normalizeStoredIdentifier(userId);

  if (normalizedUserId) {
    window.localStorage.setItem(USER_ID_KEY, normalizedUserId);
    return;
  }

  clearUserId();
}

export function clearUserId() {
  window.localStorage.removeItem(USER_ID_KEY);
}

export function getRole(): UserRole | null {
  try {
    const r = window.localStorage.getItem(ROLE_KEY);
    return (r as UserRole) ?? null;
  } catch {
    return null;
  }
}

export function setRole(role: UserRole) {
  try {
    window.localStorage.setItem(ROLE_KEY, role);
  } catch {
    // ignore
  }
}

export function clearRole() {
  try {
    window.localStorage.removeItem(ROLE_KEY);
  } catch {
    // ignore
  }
}

export function getStoredUser(): User | null {
  try {
    const raw = window.localStorage.getItem(STORED_USER_KEY);
    if (!raw) return null;

    const parsed = JSON.parse(raw) as Partial<User>;
    const email = String(parsed.email ?? '').trim();
    const id = (normalizeStoredIdentifier(parsed.id) ?? email) || null;

    if (!id || !email) return null;

    return {
      id,
      email,
      firstName: String(parsed.firstName ?? email.split('@')[0] ?? 'Utilisateur'),
      lastName: String(parsed.lastName ?? ''),
      role: normalizeRole(parsed.role, email),
      avatar: parsed.avatar,
    };
  } catch {
    return null;
  }
}

export function setStoredUser(user: User | null | undefined) {
  try {
    if (!user) {
      window.localStorage.removeItem(STORED_USER_KEY);
      return;
    }

    window.localStorage.setItem(STORED_USER_KEY, JSON.stringify(user));
  } catch {
    // ignore
  }
}

export function clearStoredUser() {
  try {
    window.localStorage.removeItem(STORED_USER_KEY);
  } catch {
    // ignore
  }
}

export function resolveUserIdFromAuthSource(source?: { userId?: unknown; user?: { id?: unknown } | null; accessToken?: string | null } | null) {
  const directUserId = normalizeStoredIdentifier(source?.userId);
  if (directUserId) {
    return directUserId;
  }

  const nestedUserId = normalizeStoredIdentifier(source?.user?.id);
  if (nestedUserId) {
    return nestedUserId;
  }

  const token = normalizeStoredIdentifier(source?.accessToken);
  if (token) {
    const tokenUser = userFromAccessToken(token);
    if (tokenUser?.id) {
      return tokenUser.id;
    }
  }

  return null;
}

export function userFromAccessToken(token: string): User | null {
  const payload = readJwtPayload(token);
  if (!payload) return null;

  const email = String(payload.email ?? payload.upn ?? payload.preferred_username ?? '').trim();
  const firstName = String(payload.given_name ?? payload.firstName ?? payload.name ?? email.split('@')[0] ?? 'Utilisateur');
  const lastName = String(payload.family_name ?? payload.lastName ?? '');
  const id = String(payload.sub ?? payload.nameid ?? payload.id ?? email ?? '1');

  return {
    id,
    email,
    firstName,
    lastName,
    role: normalizeRole(payload.role ?? payload.roles ?? payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'], email),
  };
}

export function userFromAuthResult(result: { accessToken?: string | null; user?: Partial<User> | null } | null | undefined): User | null {
  if (!result) return null;

  if (result.user) {
    const user = result.user;
    const email = String(user.email ?? '').trim();
    const resolvedId = resolveUserIdFromAuthSource({ userId: user.id, user, accessToken: result.accessToken });

    return {
      id: resolvedId ?? String(user.id ?? email ?? '1'),
      email,
      firstName: String(user.firstName ?? email.split('@')[0] ?? 'Utilisateur'),
      lastName: String(user.lastName ?? ''),
      role: normalizeRole(user.role, email),
      avatar: user.avatar,
    };
  }

  if (result.accessToken) {
    return userFromAccessToken(result.accessToken);
  }

  return null;
}
