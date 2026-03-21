'use client';

import { useAuth } from '@/components/AuthProvider';
import { useRouter } from '@/i18n/routing';
import { useEffect } from 'react';
import { Loader2 } from 'lucide-react';

export default function ProtectedRoute({ children }: { children: React.ReactNode }) {
  const { isAuthenticated, loading } = useAuth();
  const router = useRouter();

  useEffect(() => {
    if (!loading && !isAuthenticated) {
      router.push('/login');
    }
  }, [loading, isAuthenticated, router]);

  if (loading) {
    return (
      <div className="flex h-screen items-center justify-center">
        <Loader2 className="w-8 h-8 animate-spin text-manar-cyan" />
      </div>
    );
  }

  if (!isAuthenticated) return null;

  return <>{children}</>;
}
