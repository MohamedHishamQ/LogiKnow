'use client';

import { createContext, useContext, useEffect, useState } from 'react';
import { AuthService, AuthUser } from '@/api/client';
import { useRouter } from '@/i18n/routing';

interface AuthContextType {
  user: AuthUser | null;
  loading: boolean;
  login: (data: any) => Promise<void>;
  register: (data: any) => Promise<void>;
  logout: () => Promise<void>;
  isAuthenticated: boolean;
}

const AuthContext = createContext<AuthContextType>({
  user: null,
  loading: true,
  login: async () => {},
  register: async () => {},
  logout: async () => {},
  isAuthenticated: false,
});

export const useAuth = () => useContext(AuthContext);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<AuthUser | null>(null);
  const [loading, setLoading] = useState(true);
  const router = useRouter();

  useEffect(() => {
    // Try to restore session on mount using the httpOnly refresh/access cookies
    const checkSession = async () => {
      try {
        const { data } = await AuthService.me();
        setUser(data);
      } catch (err: any) {
        // If 401, token might be expired. Try to refresh
        if (err.response?.status === 401) {
          try {
            await AuthService.refresh();
            const { data } = await AuthService.me();
            setUser(data);
          } catch (refreshErr) {
            setUser(null);
          }
        } else {
          setUser(null);
        }
      } finally {
        setLoading(false);
      }
    };

    checkSession();
  }, []);

  const login = async (data: any) => {
    const res = await AuthService.login(data);
    const meRes = await AuthService.me();
    setUser(meRes.data);
    router.push('/');
    router.refresh();
  };

  const register = async (data: any) => {
    await AuthService.register(data);
    const meRes = await AuthService.me();
    setUser(meRes.data);
    router.push('/');
    router.refresh();
  };

  const logout = async () => {
    try {
      await AuthService.logout();
    } catch {
      // Ignore errors on logout
    } finally {
      setUser(null);
      router.push('/login');
      router.refresh();
    }
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        loading,
        login,
        register,
        logout,
        isAuthenticated: !!user,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}
