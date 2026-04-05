'use client';

import { useState } from 'react';
import { useTranslations } from 'next-intl';
import { BooksService } from '@/api/client';
import { Loader2, CheckCircle2, BookOpen } from 'lucide-react';
import ProtectedRoute from '@/components/ProtectedRoute';

export default function SubmitBookPage() {
  const t = useTranslations('SubmitBook');
  const [loading, setLoading] = useState(false);
  const [success, setSuccess] = useState(false);
  const [error, setError] = useState('');

  const [formData, setFormData] = useState({
    title: '',
    authors: '',
    year: new Date().getFullYear(),
    isbn: '',
    language: 'English',
    category: 'Logistics',
    coverUrl: '',
    externalLink: ''
  });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    
    try {
      const authors = formData.authors.split(',').map(a => a.trim()).filter(Boolean);
      const payload = { ...formData, authors };
      await BooksService.submitBook(payload);
      setSuccess(true);
    } catch (err: any) {
      setError(err?.response?.data?.message || err.message || 'Failed to submit book');
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
          <h2 className="text-3xl font-bold mb-4 text-white">Submitted Successfully</h2>
          <p className="text-blue-200/70 mb-8">
            Your book has been submitted. It will appear in the library once an admin approves it and uploads the PDF.
          </p>
          <button 
            onClick={() => setSuccess(false)}
            className="w-full manar-btn-gold py-3 rounded-xl font-semibold transition-all"
          >
            Submit Another Book
          </button>
        </div>
      </div>
    );
  }

  return (
    <ProtectedRoute>
      <div className="min-h-screen manar-page-bg p-8 py-12">
        <div className="max-w-3xl mx-auto space-y-8">
          <header className="mb-10 text-center">
            <div className="w-16 h-16 bg-gradient-to-br from-manar-gold/30 to-manar-gold/10 rounded-2xl flex items-center justify-center mx-auto mb-6 border border-manar-gold/20">
              <BookOpen className="w-8 h-8 text-manar-gold" />
            </div>
            <h1 className="text-4xl md:text-5xl font-black mb-4 text-white tracking-tight">Submit a Book</h1>
            <p className="text-lg text-blue-200/70">
              Contribute to the MANAR Library. Submitted books will be reviewed by administrators.
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
                  <label className="text-sm font-semibold text-blue-100">Title *</label>
                  <input required type="text" name="title" value={formData.title} onChange={handleChange} className="manar-input" placeholder="Book Title" />
                </div>
                
                <div className="space-y-2">
                  <label className="text-sm font-semibold text-blue-100">Authors *</label>
                  <input required type="text" name="authors" value={formData.authors} onChange={handleChange} className="manar-input" placeholder="e.g. John Doe, Jane Smith" />
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-semibold text-blue-100">Year *</label>
                  <input required type="number" name="year" value={formData.year} onChange={handleChange} className="manar-input" min="1900" max="2100" />
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-semibold text-blue-100">ISBN</label>
                  <input type="text" name="isbn" value={formData.isbn} onChange={handleChange} className="manar-input" placeholder="ISBN number" />
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-semibold text-blue-100">Language *</label>
                  <select name="language" value={formData.language} onChange={handleChange} className="manar-input">
                    <option value="English">English</option>
                    <option value="Arabic">Arabic</option>
                    <option value="French">French</option>
                  </select>
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-semibold text-blue-100">Category *</label>
                  <input required type="text" name="category" value={formData.category} onChange={handleChange} className="manar-input" placeholder="Logistics, Supply Chain, etc." />
                </div>
              </div>

              <div className="space-y-2">
                <label className="text-sm font-semibold text-blue-100">Cover URL</label>
                <input type="url" name="coverUrl" value={formData.coverUrl} onChange={handleChange} className="manar-input" placeholder="https://example.com/cover.jpg" />
              </div>

              <div className="space-y-2">
                <label className="text-sm font-semibold text-blue-100">External Link</label>
                <input type="url" name="externalLink" value={formData.externalLink} onChange={handleChange} className="manar-input" placeholder="Link to purchase or read online" />
              </div>

              <button 
                type="submit" 
                disabled={loading}
                className="w-full manar-btn-gold flex justify-center items-center py-4 rounded-xl font-bold transition-all mt-8 disabled:opacity-70 disabled:cursor-not-allowed"
              >
                {loading ? (
                  <>
                    <Loader2 className="w-6 h-6 animate-spin mr-2 rtl:mr-0 rtl:ml-2" />
                    Submitting...
                  </>
                ) : (
                  "Submit Book"
                )}
              </button>
            </form>
          </div>
        </div>
      </div>
    </ProtectedRoute>
  );
}
