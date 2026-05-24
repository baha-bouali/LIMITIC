import { createContext, useContext, useEffect, useMemo, useState, type ReactNode } from 'react';

import { useLoginMutation, useLogoutMutation, useRefreshTokenMutation } from '../api/authApi';
import { clearAccessToken, getAccessToken, userFromAccessToken, type User, setRole, getRole, clearRole } from '../auth/session';

interface AuthContextType {
  user: User | null;
  login: (email: string, password: string) => Promise<User>;
  logout: () => Promise<void>;
  isAuthenticated: boolean;
  isReady: boolean;
  role: User['role'] | null;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [isReady, setIsReady] = useState(false);

  const [loginMutation] = useLoginMutation();
  const [logoutMutation] = useLogoutMutation();
  const [refreshTokenMutation] = useRefreshTokenMutation();

  useEffect(() => {
    let active = true;

    async function bootstrapSession() {
      const storedToken = getAccessToken();
      if (storedToken) {
        const storedUser = userFromAccessToken(storedToken);
        if (storedUser && active) setUser(storedUser);
      }

      try {
        const refreshed = await refreshTokenMutation().unwrap();
        // prefer user DTO from refresh response when available
        const refreshedUser = (refreshed as any).user ? {
          id: String((refreshed as any).user.id),
          email: String((refreshed as any).user.email),
          firstName: String((refreshed as any).user.firstName ?? ''),
          lastName: String((refreshed as any).user.lastName ?? ''),
          role: (refreshed as any).user.role as User['role'],
        } : userFromAccessToken(refreshed.accessToken);
        if (refreshedUser && active) {
          setUser(refreshedUser);
          try { setRole(refreshedUser.role); } catch {}
        } else {
          // if no refreshed user but role stored, try to set a minimal role-only state
          try {
            const storedRole = getRole();
            if (storedRole && active && !refreshedUser) {
              setUser((prev) => prev ?? null);
            }
          } catch {}
        }
      } catch {
        if (!storedToken) clearAccessToken();
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
    // Prefer user returned by the API (DTO) if present, otherwise decode access token
    let nextUser = null as User | null;
    if ((result as any).user) {
      const u = (result as any).user;
      nextUser = {
        id: String(u.id),
        email: String(u.email),
        firstName: String(u.firstName ?? ''),
        lastName: String(u.lastName ?? ''),
        role: u.role as User['role'],
      };
    } else if (result.accessToken) {
      nextUser = userFromAccessToken(result.accessToken);
    }

    if (!nextUser) throw new Error('Invalid access token or missing user');
    setUser(nextUser);
    try { setRole(nextUser.role); } catch {}
    return nextUser;
  };

  const logout = async () => {
    try {
      await logoutMutation().unwrap();
    } finally {
      clearAccessToken();
      try { clearRole(); } catch {}
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
