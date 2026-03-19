'use client';

import { useState } from 'react';
import { useTranslations } from 'next-intl';
import { TermsService, ExplanationResponse } from '@/api/client';
import { Loader2, Sparkles } from 'lucide-react';

export default function ExplanationPanel({ termId, initialLanguage = 'ar' }: { termId: string, initialLanguage?: string }) {
  const t = useTranslations('Explanation');
  const [explanation, setExplanation] = useState<ExplanationResponse | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [style, setStyle] = useState('Standard');

  const generateExplanation = async () => {
    setLoading(true);
    setError('');
    try {
      const res = await TermsService.explainTerm(termId, initialLanguage, style);
      setExplanation(res.data);
    } catch (err: any) {
      setError(err?.message || 'Failed to generate explanation');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="glass-card-gold rounded-2xl p-6 mt-8 relative overflow-hidden">
      <div className="absolute top-0 right-0 rtl:right-auto rtl:left-0 p-4 opacity-20 text-amber-400">
        <Sparkles className="w-24 h-24" />
      </div>
      
      <div className="relative z-10">
        <div className="flex items-center justify-between mb-6">
          <h3 className="text-xl font-bold flex items-center gap-2 text-white">
            <Sparkles className="w-5 h-5 text-amber-400" />
            {t('title')}
          </h3>
          <div className="flex gap-2">
            <select 
              value={style}
              onChange={(e) => setStyle(e.target.value)}
              className="bg-white/10 border border-white/20 rounded-lg px-3 py-1.5 text-sm text-white backdrop-blur-sm"
            >
              <option value="Standard">{t('standard')}</option>
              <option value="Simplified">{t('simplified')}</option>
              <option value="Academic">{t('academic')}</option>
              <option value="Storytelling">{t('storytelling')}</option>
            </select>
            <button 
              onClick={generateExplanation}
              disabled={loading}
              className="manar-btn-gold px-4 py-1.5 rounded-lg text-sm font-medium transition-all disabled:opacity-50"
            >
              {loading ? <Loader2 className="w-4 h-4 animate-spin" /> : t('generate')}
            </button>
          </div>
        </div>

        {error && <div className="text-red-400 mb-4">{error}</div>}

        {explanation ? (
          <div className="prose prose-invert max-w-none bg-white/5 p-6 rounded-xl border border-white/10 backdrop-blur-sm">
            {explanation.explanation.split('\n').map((paragraph, index) => (
              <p key={index} className="mb-4 last:mb-0 leading-relaxed text-blue-100/80">{paragraph}</p>
            ))}
          </div>
        ) : (
          !loading && (
            <div className="text-center py-12 text-blue-200/50">
              <p>{t('placeholder')}</p>
            </div>
          )
        )}
      </div>
    </div>
  );
}
