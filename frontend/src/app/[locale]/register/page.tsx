'use client';

import { useState } from 'react';
import { useTranslations } from 'next-intl';
import { Link } from '@/i18n/routing';
import { useAuth } from '@/hooks/useAuth';
import { Anchor, Loader2, ArrowRight, ArrowLeft } from 'lucide-react';
import { usePathname } from 'next/navigation';

export default function RegisterPage() {
  const t = useTranslations('Auth');
  const pathname = usePathname();
  const isRtl = pathname.startsWith('/ar');
  const { register } = useAuth();
  
  const [fullName, setFullName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [preferredLanguage, setPreferredLanguage] = useState(isRtl ? 'ar' : 'en');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      await register({ fullName, email, password, preferredLanguage });
      // Redirect handled by AuthProvider
    } catch (err: any) {
      if (err.response?.data?.errors) {
        setError(err.response.data.errors.join(', '));
      } else {
        setError(err.response?.data?.error || 'Registration failed');
      }
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
          <div className="text-center mb-8">
            <Link href="/" className="inline-flex items-center justify-center mb-6">
              <Anchor className="w-10 h-10 text-manar-cyan mb-2" />
            </Link>
            <h2 className="text-2xl font-black text-white tracking-tight">{t('register')}</h2>
            <p className="mt-2 text-sm text-manar-gold font-medium">{t('registerText')}</p>
          </div>

          <form className="space-y-4" onSubmit={handleSubmit}>
            {error && (
              <div className="bg-red-500/10 border border-red-500/50 rounded-lg p-3 text-sm text-red-200 text-center backdrop-blur-md">
                {error}
              </div>
            )}

            <div>
              <label htmlFor="fullName" className="block text-sm font-medium text-white/80 mb-1.5">
                {t('fullName')}
              </label>
              <input
                id="fullName"
                name="fullName"
                type="text"
                autoComplete="name"
                required
                value={fullName}
                onChange={(e) => setFullName(e.target.value)}
                className="appearance-none relative block w-full px-4 py-3 border border-white/10 bg-white/5 placeholder-white/30 text-white rounded-xl focus:outline-none focus:ring-2 focus:ring-manar-cyan focus:border-transparent transition-all backdrop-blur-sm"
                placeholder="John Doe"
              />
            </div>

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
                autoComplete="new-password"
                required
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                className="appearance-none relative block w-full px-4 py-3 border border-white/10 bg-white/5 placeholder-white/30 text-white rounded-xl focus:outline-none focus:ring-2 focus:ring-manar-cyan focus:border-transparent transition-all backdrop-blur-sm"
                placeholder="••••••••"
                dir="ltr"
              />
            </div>
            
            <div>
              <label htmlFor="preferredLanguage" className="block text-sm font-medium text-white/80 mb-1.5">
                {t('preferredLanguage')}
              </label>
              <select
                id="preferredLanguage"
                name="preferredLanguage"
                value={preferredLanguage}
                onChange={(e) => setPreferredLanguage(e.target.value)}
                className="appearance-none relative block w-full px-4 py-3 border border-white/10 bg-[#0a0e1a]/80 text-white rounded-xl focus:outline-none focus:ring-2 focus:ring-manar-cyan focus:border-transparent transition-all backdrop-blur-sm"
              >
                <option value="ar">العربية (Arabic)</option>
                <option value="en">English</option>
                <option value="fr">Français (French)</option>
              </select>
            </div>

            <button
              type="submit"
              disabled={loading}
              className="group relative w-full flex justify-center py-3 px-4 border border-transparent text-sm font-bold rounded-xl text-manar-navy bg-manar-cyan hover:bg-[#45e0f5] focus:outline-none transition-all disabled:opacity-50 disabled:cursor-not-allowed shadow-[0_0_15px_rgba(34,211,238,0.4)] mt-6"
            >
              {loading ? (
                <Loader2 className="w-5 h-5 animate-spin" />
              ) : (
                <span className="flex items-center gap-2">
                  {t('registerAction')}
                  <ArrowIcon className="w-4 h-4 group-hover:translate-x-1 rtl:group-hover:-translate-x-1 transition-transform" />
                </span>
              )}
            </button>
          </form>

          <div className="mt-8 text-center border-t border-white/10 pt-6">
            <p className="text-sm text-white/60">
              {t('haveAccount')}{' '}
              <Link href="/login" className="font-bold text-manar-gold hover:text-white transition-colors">
                {t('loginAction')}
              </Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
