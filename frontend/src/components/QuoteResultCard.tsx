import { QuoteSearchResultDto } from '@/api/client';
import { BookOpen, Quote } from 'lucide-react';

export default function QuoteResultCard({ result }: { result: QuoteSearchResultDto }) {
  // Use highlight if available, otherwise just use surrounding context
  const htmlContent = result.highlight || (
    result.surroundingContext ? 
      result.surroundingContext : ''
  );

  return (
    <div className="glass-card p-6 md:p-8 rounded-2xl border border-white/10 hover:border-manar-gold/30 transition-all duration-300 group">
      <div className="flex flex-col md:flex-row gap-6">
        
        {/* Quote Content */}
        <div className="flex-1 space-y-4">
          <div className="relative">
            <Quote className="absolute -top-2 -left-3 rtl:-left-auto rtl:-right-3 w-8 h-8 text-manar-gold/20 rotate-180 rtl:rotate-0" />
            
            <p className="text-white/80 leading-relaxed text-lg italic z-10 relative pl-4 rtl:pl-0 rtl:pr-4 border-l-2 rtl:border-l-0 rtl:border-r-2 border-manar-gold/50">
              <span dangerouslySetInnerHTML={{ __html: htmlContent }} />
            </p>
          </div>
          
          <div className="flex flex-wrap items-center gap-4 text-xs font-medium text-white/50 pt-4 border-t border-white/10">
            <div className="flex items-center gap-2 text-manar-cyan">
              <BookOpen className="w-4 h-4" />
              <span className="font-bold text-sm tracking-wide bg-gradient-to-r from-manar-cyan to-blue-400 bg-clip-text text-transparent">
                {result.bookTitle}
              </span>
            </div>
            
            <div className="w-1.5 h-1.5 rounded-full bg-white/20"></div>
            
            <span className="px-2 py-1 rounded bg-white/5 border border-white/10">
              Page {result.pageNumber}
            </span>
          </div>
        </div>
      </div>
    </div>
  );
}
