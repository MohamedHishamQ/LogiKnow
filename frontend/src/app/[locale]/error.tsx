'use client';

import { useEffect } from 'react';
import { useTranslations } from 'next-intl';
import { AlertCircle, RotateCw } from 'lucide-react';

export default function Error({
  error,
  reset,
}: {
  error: Error & { digest?: string };
  reset: () => void;
}) {
  const t = useTranslations('Common');

  useEffect(() => {
    console.error('Global Error Boundary caught:', error);
  }, [error]);

  return (
    <div className="min-h-screen manar-page-bg flex flex-col items-center justify-center p-4 text-center">
      <div className="bg-red-500/10 border border-red-500/20 p-12 rounded-3xl max-w-lg w-full relative overflow-hidden backdrop-blur-xl">
        <div className="absolute inset-0 bg-gradient-to-br from-red-500/5 to-transparent"></div>
        
        <div className="relative z-10 flex flex-col items-center">
          <div className="w-20 h-20 bg-red-500/20 rounded-full flex items-center justify-center mb-6">
            <AlertCircle className="w-10 h-10 text-red-500" />
          </div>
          
          <h2 className="text-2xl font-bold text-white mb-4">Something went wrong</h2>
          
          <p className="text-red-200/70 mb-8 max-w-sm">
            We encountered an unexpected error. Please try again or return to the homepage.
          </p>
          
          <div className="flex flex-col sm:flex-row gap-4 w-full sm:w-auto">
            <button
              onClick={() => reset()}
              className="bg-white/10 hover:bg-white/20 text-white px-6 py-3 rounded-xl font-medium flex items-center justify-center gap-2 transition-colors border border-white/10"
            >
              <RotateCw className="w-4 h-4" />
              Try Again
            </button>
            
            <a href="/" className="manar-btn-gold px-6 py-3 rounded-xl font-bold flex items-center justify-center transition-all shadow-[0_0_15px_rgba(212,168,67,0.3)] hover:shadow-[0_0_25px_rgba(212,168,67,0.5)] border border-manar-gold/50">
              Go Home
            </a>
          </div>
        </div>
      </div>
    </div>
  );
}
