'use client';

import { useState, useEffect, useCallback, useMemo } from 'react';
import axios from 'axios';
import { useParams } from 'next/navigation';
import { 
  Search, 
  Loader2, 
  BookOpen, 
  Quote, 
  SearchX, 
  ChevronLeft, 
  ChevronRight, 
  Sparkles, 
  Filter, 
  Download,
  ArrowRight,
  Library
} from 'lucide-react';

/**
 * PRODUCTION-READY STANDALONE QUOTE SEARCH PAGE
 * This page is entirely self-contained. It does not import from existing components
 * or global state to ensure zero conflict with existing code.
 * It detects the locale (en/ar) and shows translated strings.
 */

// --- Types ---
interface QuoteResult {
  bookId: string;
  bookTitle: string;
  bookAuthors: string;
  bookCategory: string;
  bookCoverUrl?: string;
  pageNumber: number;
  snippet: string;
  highlight: string;
}

interface BookFilterItem {
  id: string;
  title: string;
  category: string;
  language: string;
  hasPages: boolean;
}

// --- Translations ---
const translations = {
  en: {
    badge: 'Deep Library Search',
    title: 'Search Inside <span class="bg-gradient-to-r from-amber-400 via-yellow-300 to-amber-500 bg-clip-text text-transparent">Books</span>',
    subtitle: 'Find specific quotes, terms, or passages across our entire indexed library of logistics literature.',
    placeholder: 'Enter a quote, term, or phrase to search...',
    allBooks: 'All Books',
    filteringBy: 'Filtering by:',
    clear: 'Clear',
    found: 'Found',
    results: 'results',
    page: 'Page',
    of: 'of',
    searching: 'Searching through pages...',
    noResults: 'No quotes found',
    noResultsDesc: 'Try different search terms, or select a specific book with indexed pages.',
    initialTitle: 'Search Inside Books',
    initialDesc: 'Search for specific phrases, terminology, or topics across our entire indexed library of logistics and supply chain literature.',
    prev: 'Previous',
    next: 'Next',
    searchBtn: 'Search',
    viewBook: 'View Book',
    author: 'Author',
    category: 'Category',
  },
  ar: {
    badge: 'البحث العميق في المكتبة',
    title: 'بحث في محتوى <span class="bg-gradient-to-r from-amber-400 via-yellow-300 to-amber-500 bg-clip-text text-transparent">الكتب</span>',
    subtitle: 'ابحث عن اقتباسات أو مصطلحات أو فقرات محددة عبر كامل مكتبتنا المفهرسة في أدبيات اللوجستيات.',
    placeholder: 'أدخل اقتباساً، مصطلحاً، أو عبارة للبحث...',
    allBooks: 'جميع الكتب',
    filteringBy: 'تصفية حسب:',
    clear: 'مسح',
    found: 'تم العثور على',
    results: 'نتائج',
    page: 'صفحة',
    of: 'من',
    searching: 'جاري البحث في الصفحات...',
    noResults: 'لم يتم العثور على نتائج',
    noResultsDesc: 'جرّب كلمات بحث مختلفة، أو اختر كتاباً محدداً يحتوي على صفحات مفهرسة.',
    initialTitle: 'ابحث في محتوى الكتب',
    initialDesc: 'ابحث عن عبارات أو مصطلحات أو مواضيع محددة عبر كامل مكتبتنا المفهرسة في مجال اللوجستيات وسلاسل الإمداد.',
    prev: 'السابق',
    next: 'التالي',
    searchBtn: 'بحث',
    viewBook: 'عرض الكتاب',
    author: 'المؤلف',
    category: 'التصنيف',
  }
};

type LocaleKey = keyof typeof translations;

export default function LibraryDeepSearch() {
  const params = useParams();
  const locale = (params.locale as LocaleKey) || 'en';
  const isRtl = locale === 'ar';
  const t = translations[locale] || translations.en;

  // --- Configuration ---
  const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5039/api';
  const apiClient = useMemo(() => axios.create({ baseURL: API_BASE_URL }), [API_BASE_URL]);

  // --- Search State ---
  const [query, setQuery] = useState('');
  const [selectedBookId, setSelectedBookId] = useState<string>('all');
  const [results, setResults] = useState<QuoteResult[]>([]);
  const [loading, setLoading] = useState(false);
  const [hasSearched, setHasSearched] = useState(false);
  const [error, setError] = useState('');

  // --- Pagination ---
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalResults, setTotalResults] = useState(0);
  const pageSize = 15;

  // --- Book Metadata for filter ---
  const [filterBooks, setFilterBooks] = useState<BookFilterItem[]>([]);
  const [booksLoading, setBooksLoading] = useState(true);

  // --- Saved configuration for stable pagination ---
  const [savedQuery, setSavedQuery] = useState('');
  const [savedBookId, setSavedBookId] = useState<string>('all');

  // 1. Initial Load: Get Books Metadata
  useEffect(() => {
    const fetchMetadata = async () => {
      try {
        const res = await apiClient.get('/quotesearch/books');
        setFilterBooks(res.data?.data || []);
      } catch (err) {
        console.error('Failed to load metadata:', err);
      } finally {
        setBooksLoading(false);
      }
    };
    fetchMetadata();
  }, [apiClient]);

  // 2. Search Logic
  const handleSearch = useCallback(async (searchQuery: string, bookId: string, pg: number) => {
    if (!searchQuery.trim()) return;

    setLoading(true);
    setError('');

    try {
      const paramsObj: Record<string, any> = { 
        q: searchQuery, 
        page: pg, 
        size: pageSize 
      };
      
      if (bookId && bookId !== 'all') {
        paramsObj.bookId = bookId;
      }

      const res = await apiClient.get('/quotesearch', { params: paramsObj });
      const { data, meta } = res.data;

      setResults(data || []);
      setTotalResults(meta?.total || 0);
      setTotalPages(Math.max(1, Math.ceil((meta?.total || 0) / pageSize)));
    } catch (err: any) {
      console.error('Search error:', err);
      setError(err?.response?.data?.error || 'Search failed. Please ensure the backend is running.');
      setResults([]);
    } finally {
      setLoading(false);
    }
  }, [apiClient]);

  // Handle Form Submission
  const onSearchSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!query.trim()) return;

    setHasSearched(true);
    setPage(1);
    setSavedQuery(query.trim());
    setSavedBookId(selectedBookId);
    handleSearch(query.trim(), selectedBookId, 1);
  };

  // Handle Pagination Changes
  useEffect(() => {
    if (hasSearched && savedQuery && page > 0) {
      handleSearch(savedQuery, savedBookId, page);
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }, [page, hasSearched, savedQuery, savedBookId, handleSearch]);

  const goToPage = (newPage: number) => {
    if (newPage >= 1 && newPage <= totalPages) {
      setPage(newPage);
    }
  };

  return (
    <div className={`min-h-screen bg-[#060a15] text-white overflow-x-hidden ${isRtl ? 'font-arabic' : ''}`} dir={isRtl ? 'rtl' : 'ltr'}>
      {/* Background decoration */}
      <div className="fixed inset-0 pointer-events-none opacity-30">
        <div className="absolute top-[-10%] right-[-10%] w-[50%] h-[50%] bg-amber-500/10 blur-[120px] rounded-full" />
        <div className="absolute bottom-[-10%] left-[-10%] w-[50%] h-[50%] bg-blue-500/10 blur-[120px] rounded-full" />
      </div>

      <div className="container mx-auto max-w-5xl px-4 sm:px-8 py-12 relative z-10">
        
        {/* Header Section */}
        <header className="text-center space-y-6 mb-12">
          <div className="inline-flex items-center gap-2.5 px-6 py-2 rounded-full bg-amber-500/10 border border-amber-500/20 text-amber-300 text-sm font-bold tracking-wide animate-fade-in">
            <Sparkles className="w-4 h-4" />
            {t.badge}
          </div>
          
          <h1 
            className="text-4xl md:text-6xl font-black tracking-tight leading-tight"
            dangerouslySetInnerHTML={{ __html: t.title }}
          />
          
          <p className="text-blue-200/50 text-xl max-w-2xl mx-auto leading-relaxed">
            {t.subtitle}
          </p>
        </header>

        {/* Search Control Panel */}
        <div className="glass-panel-deep p-1 rounded-3xl shadow-2xl mb-12">
          <form onSubmit={onSearchSubmit} className="flex flex-col md:flex-row gap-4 p-4">
            {/* Search Input */}
            <div className="relative flex-grow group">
              <div className={`absolute inset-y-0 ${isRtl ? 'right-0 pr-5' : 'left-0 pl-5'} flex items-center pointer-events-none text-white/30 group-focus-within:text-amber-400 transition-colors`}>
                <Search className="w-6 h-6" />
              </div>
              <input
                type="text"
                value={query}
                onChange={(e) => setQuery(e.target.value)}
                placeholder={t.placeholder}
                className={`w-full bg-white/5 border border-white/10 hover:border-amber-400/40 focus:border-amber-400/60 rounded-2xl ${isRtl ? 'pr-14 pl-5' : 'pl-14 pr-5'} py-5 text-white placeholder-white/20 backdrop-blur-xl focus:outline-none focus:ring-2 focus:ring-amber-400/20 transition-all text-lg shadow-inner`}
              />
            </div>

            {/* Book Filter Select */}
            <div className="relative flex-shrink-0 min-w-[240px]">
              <div className={`absolute inset-y-0 ${isRtl ? 'right-0 pr-4' : 'left-0 pl-4'} flex items-center pointer-events-none text-white/30 transition-colors`}>
                {booksLoading ? <Loader2 className="w-5 h-5 animate-spin" /> : <Filter className="w-5 h-5" />}
              </div>
              <select
                value={selectedBookId}
                onChange={(e) => setSelectedBookId(e.target.value)}
                disabled={booksLoading}
                className={`w-full appearance-none bg-white/5 border border-white/10 hover:border-amber-400/40 focus:border-amber-400/60 rounded-2xl ${isRtl ? 'pr-12 pl-10' : 'pl-12 pr-10'} py-5 text-white backdrop-blur-xl focus:outline-none focus:ring-2 focus:ring-amber-400/20 transition-all cursor-pointer shadow-inner disabled:opacity-40`}
              >
                <option value="all" className="bg-[#0f172a]">{t.allBooks}</option>
                {filterBooks.filter(b => b.hasPages).map(book => (
                  <option key={book.id} value={book.id} className="bg-[#0f172a]">
                    {book.title.length > 50 ? book.title.substring(0, 50) + '...' : book.title}
                  </option>
                ))}
              </select>
              <div className={`absolute inset-y-0 ${isRtl ? 'left-0 pl-4' : 'right-0 pr-4'} flex items-center pointer-events-none text-white/40`}>
                <ChevronRight className={`w-5 h-5 rotate-90`} />
              </div>
            </div>

            {/* Submit Button */}
            <button
              type="submit"
              disabled={loading || !query.trim()}
              className="px-10 py-5 rounded-2xl font-black text-lg flex items-center justify-center gap-3 whitespace-nowrap disabled:opacity-30 disabled:cursor-not-allowed transition-all active:scale-[0.98] shadow-lg"
              style={{
                background: 'linear-gradient(135deg, #d4a843 0%, #b8922e 60%, #d4a843 100%)',
                color: '#0a0e1a',
                boxShadow: '0 0 30px rgba(212, 168, 67, 0.2)',
              }}
            >
              {loading ? <Loader2 className="w-6 h-6 animate-spin" /> : <Search className="w-5 h-5" />}
              {t.searchBtn}
            </button>
          </form>

          {/* Active Filter Pill */}
          {selectedBookId !== 'all' && (
            <div className={`px-8 pb-4 flex items-center gap-2 text-sm text-amber-300/80`}>
              <div className="w-2 h-2 rounded-full bg-amber-400 animate-pulse" />
              <span>{t.filteringBy} <strong className="text-amber-200">{filterBooks.find(b => b.id === selectedBookId)?.title}</strong></span>
              <button 
                onClick={() => setSelectedBookId('all')}
                className="ml-2 underline underline-offset-4 hover:text-amber-200 transition-colors"
              >
                {t.clear}
              </button>
            </div>
          )}
        </div>

        {/* Results Body */}
        <div className="min-h-[400px]">
          {loading && (
            <div className="flex flex-col items-center justify-center py-24 gap-6">
              <div className="relative">
                <div className="w-20 h-20 rounded-full border-4 border-amber-400/10 border-t-amber-400 animate-spin" />
                <div className="absolute inset-0 w-20 h-20 rounded-full border border-amber-400/5 animate-ping" />
              </div>
              <p className="text-amber-200/50 font-bold text-xl animate-pulse">{t.searching}</p>
            </div>
          )}

          {error && !loading && (
            <div className="bg-red-500/5 border border-red-500/20 p-10 rounded-3xl text-center space-y-4">
              <SearchX className="w-16 h-16 text-red-500/40 mx-auto" />
              <p className="text-red-400 text-lg font-medium">{error}</p>
            </div>
          )}

          {!loading && !error && hasSearched && results.length > 0 && (
            <div className="space-y-8">
              {/* Stats Bar */}
              <div className="flex items-center justify-between border-b border-white/5 pb-6">
                <div className="flex items-center gap-3">
                  <div className="px-3 py-1 bg-white/5 rounded-lg border border-white/10 text-white/50 text-xs font-bold uppercase tracking-widest">Results</div>
                  <span className="text-lg text-white/70">
                    {t.found} <span className="text-amber-400 font-black">{totalResults}</span> {t.results}
                  </span>
                </div>
                <div className="text-white/30 text-sm font-medium">
                  {t.page} {page} {t.of} {totalPages}
                </div>
              </div>

              {/* Cards Grid */}
              <div className="grid gap-6">
                {results.map((result, idx) => (
                  <article 
                    key={`${result.bookId}-${result.pageNumber}-${idx}`}
                    className="group relative bg-[#0d1324] border border-white/5 hover:border-amber-400/20 rounded-3xl p-8 transition-all duration-500 hover:shadow-[0_20px_60px_-15px_rgba(0,0,0,0.5)] overflow-hidden"
                  >
                    {/* Hover Glow */}
                    <div className="absolute inset-0 bg-gradient-to-br from-amber-400/[0.03] to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-700 pointer-events-none" />
                    
                    <div className="relative flex flex-col md:flex-row gap-8">
                      {/* Left: Metadata & Cover Placeholder */}
                      <div className="w-full md:w-[120px] shrink-0">
                        <div className="aspect-[3/4.5] bg-white/5 rounded-xl border border-white/10 overflow-hidden flex items-center justify-center group-hover:border-amber-400/30 transition-colors relative">
                          {result.bookCoverUrl ? (
                            // eslint-disable-next-line @next/next/no-img-element
                            <img src={result.bookCoverUrl} alt={result.bookTitle} className="w-full h-full object-cover" />
                          ) : (
                            <BookOpen className="w-10 h-10 text-white/10" />
                          )}
                          <div className="absolute bottom-2 left-2 right-2 bg-black/60 backdrop-blur-md px-2 py-1.5 rounded-lg border border-white/10 text-[10px] font-black text-center text-amber-300 tracking-wider uppercase">
                            Page {result.pageNumber}
                          </div>
                        </div>
                      </div>

                      {/* Right: Content */}
                      <div className="flex-grow space-y-6">
                        {/* Book Header */}
                        <div>
                          <h2 className="text-2xl font-black text-white group-hover:text-amber-200 transition-colors leading-tight mb-2">
                            {result.bookTitle}
                          </h2>
                          <div className="flex flex-wrap gap-x-4 gap-y-2">
                            <span className="flex items-center gap-1.5 text-blue-400/60 text-sm">
                              <Sparkles className="w-3.5 h-3.5" />
                              {result.bookCategory || 'Logistics'}
                            </span>
                            <span className="flex items-center gap-1.5 text-white/30 text-sm">
                              <Library className="w-3.5 h-3.5" />
                              {result.bookAuthors}
                            </span>
                          </div>
                        </div>

                        {/* Quote Snippet */}
                        <div className={`relative ${isRtl ? 'pr-6' : 'pl-6'} py-2`}>
                          <div className={`absolute top-0 bottom-0 ${isRtl ? 'right-0' : 'left-0'} w-1 bg-amber-400/20 rounded-full`} />
                          <Quote className={`absolute ${isRtl ? '-right-3' : '-left-3'} -top-2 w-8 h-8 text-amber-400/10 rotate-180`} />
                          <div 
                            className="text-white/80 text-lg leading-relaxed font-light highlights-container"
                            dangerouslySetInnerHTML={{ __html: result.highlight }}
                          />
                        </div>

                        {/* Footer Action */}
                        <div className="pt-2 flex items-center gap-4">
                           <button 
                            className="flex items-center gap-2 px-5 py-2.5 rounded-xl bg-white/5 border border-white/10 hover:bg-white/10 hover:border-white/20 text-white/50 hover:text-white transition-all text-sm font-bold group/btn"
                           >
                            <ArrowRight className={`w-4 h-4 transition-transform group-hover/btn:translate-x-1 ${isRtl ? 'rotate-180' : ''}`} />
                            {t.viewBook}
                           </button>
                        </div>
                      </div>
                    </div>
                  </article>
                ))}
              </div>

              {/* Pagination UI */}
              {totalPages > 1 && (
                <div className="flex justify-center items-center gap-4 pt-12">
                  <button
                    onClick={() => goToPage(page - 1)}
                    disabled={page === 1}
                    className="flex items-center gap-2 px-6 py-3 rounded-2xl bg-white/5 border border-white/10 hover:bg-white/10 disabled:opacity-20 text-white/80 transition-all font-bold"
                  >
                    <ChevronLeft className={`w-5 h-5 ${isRtl ? 'rotate-180' : ''}`} />
                    {t.prev}
                  </button>

                  <div className="hidden sm:flex gap-2">
                    {Array.from({ length: Math.min(5, totalPages) }, (_, i) => {
                       let pNum = i + 1;
                       if (totalPages > 5) {
                         if (page > 3) pNum = page - 2 + i;
                         if (page > totalPages - 2) pNum = totalPages - 4 + i;
                       }
                       if (pNum > totalPages) return null;
                       return (
                        <button
                          key={pNum}
                          onClick={() => goToPage(pNum)}
                          className={`w-12 h-12 rounded-xl border font-bold transition-all ${
                            pNum === page 
                            ? 'bg-amber-400 text-black border-amber-400 shadow-lg shadow-amber-400/20 scale-110' 
                            : 'bg-white/5 border-white/10 text-white/40 hover:text-white'
                          }`}
                        >
                          {pNum}
                        </button>
                       );
                    })}
                  </div>

                  <button
                    onClick={() => goToPage(page + 1)}
                    disabled={page === totalPages}
                    className="flex items-center gap-2 px-6 py-3 rounded-2xl bg-white/5 border border-white/10 hover:bg-white/10 disabled:opacity-20 text-white/80 transition-all font-bold"
                  >
                    {t.next}
                    <ChevronRight className={`w-5 h-5 ${isRtl ? 'rotate-180' : ''}`} />
                  </button>
                </div>
              )}
            </div>
          )}

          {!loading && !error && hasSearched && results.length === 0 && (
            <div className="py-32 text-center border-2 border-dashed border-white/10 rounded-[40px] space-y-6">
              <div className="w-24 h-24 rounded-full bg-white/5 flex items-center justify-center mx-auto opacity-50">
                <SearchX className="w-12 h-12 text-white" />
              </div>
              <div className="space-y-2">
                <h3 className="text-3xl font-black text-white">{t.noResults}</h3>
                <p className="text-blue-200/40 text-lg max-w-sm mx-auto">{t.noResultsDesc}</p>
              </div>
            </div>
          )}

          {!loading && !hasSearched && (
            <div className="py-40 text-center relative rounded-[40px] border border-white/5 bg-gradient-to-b from-[#0d1324] to-transparent overflow-hidden group">
              <div className="absolute top-0 inset-x-0 h-px bg-gradient-to-r from-transparent via-amber-400/20 to-transparent" />
              <div className="space-y-8 relative z-10">
                <div className="relative inline-block">
                  <Quote className="w-24 h-24 text-amber-400/5 mx-auto rotate-180" />
                  <div className="absolute inset-0 bg-amber-400/20 blur-3xl rounded-full opacity-20 group-hover:opacity-40 transition-opacity" />
                </div>
                <div className="space-y-4">
                  <h3 className="text-4xl font-black text-white/90">{t.initialTitle}</h3>
                  <p className="text-blue-200/40 max-w-md mx-auto text-lg leading-relaxed">
                    {t.initialDesc}
                  </p>
                </div>
                
                {/* Suggestions */}
                <div className="flex flex-wrap justify-center gap-3 pt-4">
                  {['logistics management', 'supply chain', 'warehouse operations', 'freight'].map(term => (
                    <button 
                      key={term}
                      onClick={() => setQuery(term)}
                      className="px-5 py-2.5 rounded-xl bg-white/5 border border-white/10 text-white/40 text-sm font-medium hover:border-amber-400/40 hover:text-amber-300 hover:bg-amber-400/10 transition-all"
                    >
                      &ldquo;{term}&rdquo;
                    </button>
                  ))}
                </div>
              </div>
            </div>
          )}
        </div>
      </div>

      <style jsx global>{`
        .glass-panel-deep {
          background: rgba(13, 19, 36, 0.7);
          backdrop-filter: blur(40px);
          -webkit-backdrop-filter: blur(40px);
          border: 1px solid rgba(255, 255, 255, 0.08);
        }

        .highlights-container mark {
          background: linear-gradient(135deg, rgba(212, 168, 67, 0.4), rgba(212, 168, 67, 0.2));
          color: #fcd34d;
          padding: 0 4px;
          border-radius: 4px;
          font-weight: 700;
          box-shadow: 0 0 10px rgba(212, 168, 67, 0.1);
        }

        @keyframes animate-fade-in {
          from { opacity: 0; transform: translateY(10px); }
          to { opacity: 1; transform: translateY(0); }
        }

        .animate-fade-in {
          animation: animate-fade-in 0.8s ease-out forwards;
        }

        @font-face {
          font-family: 'font-arabic';
          src: url('https://fonts.googleapis.com/css2?family=Cairo:wght@400;600;700;900&display=swap');
        }

        .font-arabic {
          font-family: 'Cairo', sans-serif !important;
        }
      `}</style>
    </div>
  );
}
