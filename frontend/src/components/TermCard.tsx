import { TermDto } from '@/api/client';
import { BookOpen } from 'lucide-react';
import { Link } from '@/i18n/routing';

export default function TermCard({ term, locale }: { term: TermDto, locale: string }) {
  const title = locale === 'ar' ? term.nameAr : (locale === 'fr' ? (term.nameFr || term.nameEn) : term.nameEn);
  const definition = locale === 'ar' ? term.definitionAr : term.definitionEn;

  return (
    <Link href={`/terms/${term.id}`} className="group block glass-card border border-white/10 rounded-2xl p-6 transition-all hover:shadow-xl hover:shadow-amber-500/10 hover:border-amber-500/30">
      <div className="flex items-start justify-between mb-5">
        <div>
          <h3 className="text-2xl font-bold text-white group-hover:text-amber-400 transition-colors">{title}</h3>
          <p className="text-sm text-blue-200/50 font-mono mt-1 uppercase tracking-wider">{term.category}</p>
        </div>
        <div className="p-3 bg-white/5 text-amber-500 rounded-xl border border-white/10 group-hover:bg-amber-500/10 transition-colors">
          <BookOpen className="w-6 h-6" />
        </div>
      </div>
      
      <p className="text-blue-100/70 leading-relaxed mb-6 line-clamp-3">
        {definition}
      </p>

      {term.tags && term.tags.length > 0 && (
        <div className="flex flex-wrap gap-2 pt-4 border-t border-white/10">
          {term.tags.slice(0, 3).map(tag => (
            <span key={tag} className="text-xs font-medium px-2.5 py-1 rounded-md bg-white/5 border border-white/10 text-blue-200/80">
              {tag}
            </span>
          ))}
          {term.tags.length > 3 && (
            <span className="text-xs text-blue-200/40 flex items-center px-1">+{term.tags.length - 3}</span>
          )}
        </div>
      )}
    </Link>
  );
}
