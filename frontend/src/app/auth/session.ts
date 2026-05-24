export type UserRole = 'SUPER_ADMIN' | 'ADMIN' | 'CHERCHEUR' | 'DOCTORANT' | 'MASTERIEN' | 'VISITOR';

export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  role: UserRole;
  avatar?: string;
}

export const ACCESS_TOKEN_KEY = 'accessToken';

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

function normalizeRole(value: unknown, email?: string): UserRole {
  const candidates = Array.isArray(value) ? value : [value];

  for (const candidate of candidates) {
    const normalized = String(candidate ?? '').toUpperCase();
    if (
      normalized === 'SUPER_ADMIN' ||
      normalized === 'ADMIN' ||
      normalized === 'CHERCHEUR' ||
      normalized === 'DOCTORANT' ||
      normalized === 'MASTERIEN' ||
      normalized === 'VISITOR'
    ) {
      return normalized as UserRole;
    }
  }

  const source = (email ?? '').toLowerCase();
  if (source.includes('superadmin')) return 'SUPER_ADMIN';
  if (source.includes('admin')) return 'ADMIN';
  if (source.includes('doctorant')) return 'DOCTORANT';
  if (source.includes('masterien')) return 'MASTERIEN';
  if (source.includes('visitor')) return 'VISITOR';

  return 'SUPER_ADMIN';
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

const ROLE_KEY = 'userRole';

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
