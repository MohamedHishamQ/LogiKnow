import { Anchor } from 'lucide-react';
import Link from 'next/link';

export default function NotFoundPage() {
  return (
    <div className="min-h-screen manar-page-bg flex flex-col items-center justify-center p-4 text-center">
      <div className="glass-card-gold p-12 rounded-3xl max-w-lg w-full relative overflow-hidden">
        <div className="absolute inset-0 bg-gradient-to-br from-manar-gold/10 to-transparent"></div>
        
        <div className="relative z-10 flex flex-col items-center">
          <div className="w-24 h-24 bg-white/5 rounded-full flex items-center justify-center mb-6 border border-white/10 shadow-[0_0_30px_rgba(212,168,67,0.2)]">
            <Anchor className="w-12 h-12 text-manar-gold" />
          </div>
          
          <h1 className="text-6xl font-black text-white mb-4 tracking-tighter text-glow">404</h1>
          <h2 className="text-2xl font-bold text-white mb-2">Lost at Sea</h2>
          
          <p className="text-blue-200/70 mb-8 max-w-sm">
            The page you are looking for has drifted away or doesn't exist in our logs.
          </p>
          
          <Link href="/" className="manar-btn-gold px-8 py-3 rounded-xl font-bold flex items-center justify-center gap-2 transition-all shadow-[0_0_15px_rgba(212,168,67,0.3)] hover:shadow-[0_0_25px_rgba(212,168,67,0.5)] w-full sm:w-auto">
            Return to Port
          </Link>
        </div>
      </div>
    </div>
  );
}
