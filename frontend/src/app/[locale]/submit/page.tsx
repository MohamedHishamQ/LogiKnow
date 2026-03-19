'use client';

import { useState } from 'react';
import { useTranslations } from 'next-intl';
import { apiClient } from '@/api/client';
import { Loader2, CheckCircle2 } from 'lucide-react';

export default function SubmitPage() {
  const t = useTranslations('Submit');
  const [loading, setLoading] = useState(false);
  const [success, setSuccess] = useState(false);
  const [error, setError] = useState('');

  const [formData, setFormData] = useState({
    title: '',
    author: '',
    university: '',
    year: new Date().getFullYear(),
    abstract: '',
    documentUrl: '',
    type: 'Thesis',
    tags: ''
  });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    
    try {
      const tags = formData.tags.split(',').map(tag => tag.trim()).filter(Boolean);
      const payload = { ...formData, tags };
      await apiClient.post('/academic/submit', payload);
      setSuccess(true);
    } catch (err: any) {
      setError(err?.response?.data?.message || err.message || 'Failed to submit entry');
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  if (success) {
    return (
      <div className="min-h-screen manar-page-bg p-8 flex items-center justify-center">
        <div className="max-w-md w-full glass-card rounded-3xl p-8 text-center">
          <div className="w-20 h-20 bg-emerald-500/20 text-emerald-400 rounded-full flex items-center justify-center mx-auto mb-6">
            <CheckCircle2 className="w-10 h-10" />
          </div>
          <h2 className="text-3xl font-bold mb-4 text-white">{t('successTitle')}</h2>
          <p className="text-blue-200/70 mb-8">
            {t('successMessage')}
          </p>
          <button 
            onClick={() => setSuccess(false)}
            className="w-full manar-btn-gold py-3 rounded-xl font-semibold transition-all"
          >
            {t('submitAnother')}
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen manar-page-bg p-8 py-12">
      <div className="max-w-3xl mx-auto space-y-8">
        <header className="mb-10 text-center">
          <h1 className="text-4xl md:text-5xl font-black mb-4 text-white tracking-tight">{t('title')}</h1>
          <p className="text-lg text-blue-200/70">
            {t('description')}
          </p>
        </header>

        <div className="glass-card rounded-3xl p-8 md:p-12">
          {error && (
            <div className="bg-red-500/10 text-red-400 p-4 rounded-xl mb-8 border border-red-500/20">
              {error}
            </div>
          )}

          <form onSubmit={handleSubmit} className="space-y-6">
            <div className="grid grid-cols-1 select-none gap-6 md:grid-cols-2">
              <div className="space-y-2">
                <label className="text-sm font-semibold text-blue-100">{t('titleLabel')} *</label>
                <input required type="text" name="title" value={formData.title} onChange={handleChange} className="manar-input" placeholder={t('titlePlaceholder')} />
              </div>
              
              <div className="space-y-2">
                <label className="text-sm font-semibold text-blue-100">{t('authorLabel')} *</label>
                <input required type="text" name="author" value={formData.author} onChange={handleChange} className="manar-input" placeholder={t('authorPlaceholder')} />
              </div>

              <div className="space-y-2">
                <label className="text-sm font-semibold text-blue-100">{t('universityLabel')} *</label>
                <input required type="text" name="university" value={formData.university} onChange={handleChange} className="manar-input" placeholder={t('universityPlaceholder')} />
              </div>

              <div className="space-y-2">
                <label className="text-sm font-semibold text-blue-100">{t('yearLabel')} *</label>
                <input required type="number" name="year" value={formData.year} onChange={handleChange} className="manar-input" min="1900" max="2100" />
              </div>

              <div className="space-y-2">
                <label className="text-sm font-semibold text-blue-100">{t('typeLabel')} *</label>
                <select name="type" value={formData.type} onChange={handleChange} className="manar-input">
                  <option value="Thesis">{t('typeThesis')}</option>
                  <option value="Paper">{t('typePaper')}</option>
                  <option value="Article">{t('typeArticle')}</option>
                  <option value="Report">{t('typeReport')}</option>
                </select>
              </div>

              <div className="space-y-2">
                <label className="text-sm font-semibold text-blue-100">{t('tagsLabel')}</label>
                <input type="text" name="tags" value={formData.tags} onChange={handleChange} className="manar-input" placeholder={t('tagsPlaceholder')} />
              </div>
            </div>

            <div className="space-y-2">
              <label className="text-sm font-semibold text-blue-100">{t('documentUrlLabel')}</label>
              <input type="url" name="documentUrl" value={formData.documentUrl} onChange={handleChange} className="manar-input" placeholder={t('documentUrlPlaceholder')} />
            </div>

            <div className="space-y-2">
              <label className="text-sm font-semibold text-blue-100">{t('abstractLabel')} *</label>
              <textarea required name="abstract" value={formData.abstract} onChange={handleChange} rows={5} className="manar-input" placeholder={t('abstractPlaceholder')} />
            </div>

            <button 
              type="submit" 
              disabled={loading}
              className="w-full manar-btn-gold flex justify-center items-center py-4 rounded-xl font-bold transition-all mt-8 disabled:opacity-70 disabled:cursor-not-allowed"
            >
              {loading ? (
                <>
                  <Loader2 className="w-6 h-6 animate-spin mr-2 rtl:mr-0 rtl:ml-2" />
                  {t('submitting')}
                </>
              ) : (
                t('submitButton')
              )}
            </button>
          </form>
        </div>
      </div>
    </div>
  );
}
