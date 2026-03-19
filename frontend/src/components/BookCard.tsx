import { useTranslations } from 'next-intl';
import { BookDto } from '@/api/client';
import { Badge } from '@/components/ui/Badge';
import { Library, ExternalLink } from 'lucide-react';

export default function BookCard({ book, locale }: { book: BookDto, locale: string }) {
  const t = useTranslations('Common'); // Using common read, or Books if available

  return (
    <div className="flex flex-col glass-panel-heavy border border-white/10 rounded-2xl overflow-hidden transition-all hover:shadow-xl hover:shadow-amber-500/10 group">
      <div className="h-48 bg-white/5 flex items-center justify-center p-6 border-b border-white/10 relative overflow-hidden">
        <div className="absolute inset-0 bg-gradient-to-t from-black/40 to-transparent z-0" />
        {book.coverUrl ? (
          <img src={book.coverUrl} alt={book.title} className="h-full object-contain drop-shadow-2xl relative z-10" />
        ) : (
          <Library className="w-16 h-16 text-amber-500/40 relative z-10 transition-transform group-hover:scale-110 duration-500" />
        )}
      </div>
      
      <div className="p-6 flex-1 flex flex-col relative z-10">
        <div className="flex justify-between items-start mb-3">
          <span className="text-xs font-bold uppercase tracking-wider text-amber-400 bg-amber-400/10 px-3 py-1 rounded-full">{book.category}</span>
          <span className="text-sm font-medium text-blue-200/50">{book.year}</span>
        </div>
        
        <h3 className="text-xl font-bold mb-2 text-white line-clamp-2">{book.title}</h3>
        
        <p className="text-sm text-blue-100/70 mb-6 flex-1">
          {book.authors.join(', ')}
        </p>

        <div className="flex items-center justify-between mt-auto pt-4 border-t border-white/10">
          <span className="font-mono text-xs text-blue-300/50 bg-white/5 px-2 py-1 rounded-md">{book.language.toUpperCase()}</span>
          
          <div className="flex gap-2">
            {book.externalLink && (
              <a href={book.externalLink} target="_blank" rel="noopener noreferrer" className="text-blue-300 hover:text-white flex items-center gap-1.5 text-sm font-bold transition-colors">
                {t('read')} <ExternalLink className="w-4 h-4" />
              </a>
            )}
            
            {/* Download button for uploaded PDFs */}
            {(book as any).blobStoragePath && (
              <a href={`${process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5038'}${(book as any).blobStoragePath}`} target="_blank" rel="noopener noreferrer" 
                 className="manar-btn-gold px-3 py-1.5 rounded-lg flex items-center gap-1.5 text-sm font-bold shadow-lg">
                PDF <ExternalLink className="w-4 h-4" />
              </a>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
