'use client';

import { useState } from 'react';
import { useTranslations } from 'next-intl';
import { Link } from '@/i18n/routing';
import { useAuth } from '@/hooks/useAuth';
import { Anchor, Loader2, ArrowRight, ArrowLeft } from 'lucide-react';
import { usePathname } from 'next/navigation';

export default function LoginPage() {
  const t = useTranslations('Auth');
  const pathname = usePathname();
  const isRtl = pathname.startsWith('/ar');
  const { login } = useAuth();
  
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      await login({ email, password });
      // Redirect handled by AuthProvider
    } catch (err: any) {
      setError(err.response?.data?.error || 'Invalid credentials');
      setLoading(false);
    }
  };

  const ArrowIcon = isRtl ? ArrowLeft : ArrowRight;

  return (
    <div className="min-h-screen flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-md w-full glass-panel-heavy p-8 relative overflow-hidden">
        {/* Background decorative element */}
        <div className="absolute -top-24 -right-24 rtl:-right-auto rtl:-left-24 w-48 h-48 bg-manar-cyan/20 rounded-full blur-3xl pointer-events-none"></div>
        <div className="absolute -bottom-24 -left-24 rtl:-left-auto rtl:-right-24 w-48 h-48 bg-manar-gold/20 rounded-full blur-3xl pointer-events-none"></div>

        <div className="relative z-10">
          <div className="text-center mb-10">
            <Link href="/" className="inline-flex items-center justify-center mb-6">
              <Anchor className="w-12 h-12 text-manar-cyan mb-2" />
            </Link>
            <h2 className="text-3xl font-black text-white tracking-tight">MANARA</h2>
            <p className="mt-2 text-sm text-manar-gold font-medium">{t('loginText')}</p>
          </div>

          <form className="space-y-6" onSubmit={handleSubmit}>
            {error && (
              <div className="bg-red-500/10 border border-red-500/50 rounded-lg p-3 text-sm text-red-200 text-center backdrop-blur-md">
                {error}
              </div>
            )}

            <div>
              <label htmlFor="email" className="block text-sm font-medium text-white/80 mb-1.5">
                {t('email')}
              </label>
              <input
                id="email"
                name="email"
                type="email"
                autoComplete="email"
                required
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                className="appearance-none relative block w-full px-4 py-3 border border-white/10 bg-white/5 placeholder-white/30 text-white rounded-xl focus:outline-none focus:ring-2 focus:ring-manar-cyan focus:border-transparent transition-all backdrop-blur-sm"
                placeholder="name@example.com"
                dir="ltr"
              />
            </div>

            <div>
              <label htmlFor="password" className="block text-sm font-medium text-white/80 mb-1.5">
                {t('password')}
              </label>
              <input
                id="password"
                name="password"
                type="password"
                autoComplete="current-password"
                required
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                className="appearance-none relative block w-full px-4 py-3 border border-white/10 bg-white/5 placeholder-white/30 text-white rounded-xl focus:outline-none focus:ring-2 focus:ring-manar-cyan focus:border-transparent transition-all backdrop-blur-sm"
                placeholder="••••••••"
                dir="ltr"
              />
            </div>

            <button
              type="submit"
              disabled={loading}
              className="group relative w-full flex justify-center py-3 px-4 border border-transparent text-sm font-bold rounded-xl text-manar-navy bg-manar-cyan hover:bg-[#45e0f5] focus:outline-none transition-all disabled:opacity-50 disabled:cursor-not-allowed shadow-[0_0_15px_rgba(34,211,238,0.4)]"
            >
              {loading ? (
                <Loader2 className="w-5 h-5 animate-spin" />
              ) : (
                <span className="flex items-center gap-2">
                  {t('loginAction')}
                  <ArrowIcon className="w-4 h-4 group-hover:translate-x-1 rtl:group-hover:-translate-x-1 transition-transform" />
                </span>
              )}
            </button>
          </form>

          <div className="mt-8 text-center border-t border-white/10 pt-6">
            <p className="text-sm text-white/60">
              {t('noAccount')}{' '}
              <Link href="/register" className="font-bold text-manar-gold hover:text-white transition-colors">
                {t('register')}
              </Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
