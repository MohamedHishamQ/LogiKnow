import { useTranslations } from 'next-intl';
import { AcademicEntryDto } from '@/api/client';
import { Badge } from '@/components/ui/Badge';
import { FileText, GraduationCap, Link2 } from 'lucide-react';

export default function AcademicCard({ entry, locale }: { entry: AcademicEntryDto, locale: string }) {
  const t = useTranslations('Academic');
  const tCommon = useTranslations('Common');

  return (
    <div className="flex flex-col glass-panel-heavy border border-white/10 rounded-2xl p-6 transition-all hover:shadow-xl hover:shadow-amber-500/10 group">
      <div className="flex justify-between items-start mb-5">
        <div className="flex items-center gap-3">
          <div className="p-3 bg-white/5 text-amber-400 rounded-xl border border-white/10 group-hover:bg-amber-400/10 transition-colors">
            {entry.type.toLowerCase().includes('thesis') ? <GraduationCap className="w-6 h-6" /> : <FileText className="w-6 h-6" />}
          </div>
          <div>
            <span className="block text-xs font-bold uppercase tracking-wider text-amber-400 bg-amber-400/10 px-2 py-0.5 rounded-full mb-1">{entry.type}</span>
            <span className="block text-xs font-medium text-blue-200/50">{entry.year}</span>
          </div>
        </div>
        <span className={`px-2 py-1 text-xs font-bold rounded-full ${entry.status === 'Published' ? 'bg-emerald-500/20 text-emerald-400 border border-emerald-500/20' : 'bg-white/10 text-blue-200/50'}`}>
          {entry.status}
        </span>
      </div>

      <h3 className="text-xl font-bold mb-3 text-white line-clamp-2">{entry.title}</h3>
      
      <div className="mb-4">
        <p className="text-sm font-medium text-blue-100">{entry.author}</p>
        <p className="text-xs text-blue-200/60 mt-1">{entry.university}{entry.supervisor ? ` • ${t('supervisor')}: ${entry.supervisor}` : ''}</p>
      </div>

      <p className="text-sm text-blue-100/70 leading-relaxed mb-6 line-clamp-3 flex-1">
        {entry.abstract}
      </p>

      <div className="flex items-end justify-between mt-auto pt-4 border-t border-white/10">
        <div className="flex flex-wrap gap-2">
          {entry.tags.slice(0, 2).map(tag => (
            <span key={tag} className="text-xs text-blue-200/80 bg-white/5 border border-white/10 px-2.5 py-1 rounded-md font-medium">
              {tag}
            </span>
          ))}
          {entry.tags.length > 2 && (
            <span className="text-xs text-blue-200/40 flex items-center px-1">+{entry.tags.length - 2}</span>
          )}
        </div>

        {entry.documentUrl && (
          <a href={entry.documentUrl} target="_blank" rel="noopener noreferrer" className="p-2 text-amber-400/70 hover:text-amber-400 hover:bg-amber-400/10 rounded-lg transition-colors">
            <Link2 className="w-5 h-5" />
          </a>
        )}
      </div>
    </div>
  );
}
