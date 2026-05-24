import { createContext, useContext, useEffect, useMemo, useState, type ReactNode } from 'react';

import { useLoginMutation, useLogoutMutation, useRefreshTokenMutation } from '../api/authApi';
import { useLazyGetUserByIdQuery } from '../api/usersApi';
import { clearAccessToken, clearRole, clearStoredUser, clearUserId, getAccessToken, getRole, getStoredUser, getUserId, normalizeRole, resolveUserIdFromAuthSource, setRole, setStoredUser, setUserId, type User } from '../auth/session';
import type { UserDto } from '../api/usersApi';

interface AuthContextType {
  user: User | null;
  login: (email: string, password: string) => Promise<User>;
  logout: () => Promise<void>;
  isAuthenticated: boolean;
  isReady: boolean;
  role: User['role'] | null;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

function userFromUserDto(user: UserDto): User {
  const email = String(user.email ?? '').trim();
  const resolvedId = String(user.id ?? '').trim();

  return {
    id: resolvedId || email || '1',
    email,
    firstName: String(user.firstName ?? email.split('@')[0] ?? 'Utilisateur'),
    lastName: String(user.lastName ?? ''),
    role: normalizeRole(user.role, email),
  };
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [isReady, setIsReady] = useState(false);

  const [loginMutation] = useLoginMutation();
  const [logoutMutation] = useLogoutMutation();
  const [refreshTokenMutation] = useRefreshTokenMutation();
  const [loadUserById] = useLazyGetUserByIdQuery();

  const resolveUserById = async (userId?: string | null) => {
    if (!userId) return null;

    try {
      const userDto = await loadUserById(userId).unwrap();
      return userFromUserDto(userDto);
    } catch {
      return null;
    }
  };

  useEffect(() => {
    let active = true;

    async function bootstrapSession() {
      const storedSnapshot = getStoredUser();
      const storedUserId = getUserId();
      const storedUser = storedSnapshot ?? (await resolveUserById(storedUserId));

      if (storedUser && active) {
        setUser(storedUser);
        setUserId(storedUser.id);
        setStoredUser(storedUser);
        try {
          setRole(storedUser.role);
        } catch {
          // ignore storage failures
        }
      }

      try {
        const refreshed = await refreshTokenMutation().unwrap();
        const refreshedUserId = resolveUserIdFromAuthSource({
          userId: refreshed.userId ?? storedUserId ?? null,
          user: refreshed.user ?? null,
          accessToken: refreshed.accessToken ?? getAccessToken(),
        }) ?? storedUserId ?? null;
        const fetchedUser = await resolveUserById(refreshedUserId);

        if (fetchedUser && active) {
          setUser(fetchedUser);
          setUserId(fetchedUser.id);
          setStoredUser(fetchedUser);
          try {
            setRole(fetchedUser.role);
          } catch {
            // ignore storage failures
          }
        } else if (active && !storedUser) {
          clearRole();
          clearUserId();
          clearStoredUser();
          setUser(null);
        }
      } catch {
        if (!storedUser) {
          clearAccessToken();
          clearRole();
          clearUserId();
          clearStoredUser();
          if (active) setUser(null);
        }
      } finally {
        if (active) setIsReady(true);
      }
    }

    bootstrapSession();

    return () => {
      active = false;
    };
  }, [refreshTokenMutation]);

  const login = async (email: string, password: string) => {
    const result = await loginMutation({ username: email, password }).unwrap();
    const resolvedUserId = resolveUserIdFromAuthSource({
      userId: result.userId ?? null,
      user: result.user ?? null,
      accessToken: result.accessToken,
    });
    const fetchedUser = await resolveUserById(resolvedUserId);

    if (!fetchedUser) throw new Error('Unable to load user by id');
    setUser(fetchedUser);
    setUserId(fetchedUser.id);
    setStoredUser(fetchedUser);
    try {
      setRole(fetchedUser.role);
    } catch {
      // ignore storage failures
    }
    return fetchedUser;
  };

  const logout = async () => {
    try {
      await logoutMutation().unwrap();
    } finally {
      clearAccessToken();
      try { clearRole(); } catch {}
      clearUserId();
      clearStoredUser();
      setUser(null);
    }
  };

  const value = useMemo(() => ({ user, login, logout, isAuthenticated: !!user, isReady, role: user?.role ?? getRole() }), [isReady, user]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) throw new Error('useAuth must be used within an AuthProvider');
  return context;
}
