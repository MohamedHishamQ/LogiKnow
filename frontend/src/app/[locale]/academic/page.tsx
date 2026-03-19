import { getTranslations } from 'next-intl/server';
import { AcademicService, AcademicEntryDto } from '@/api/client';
import AcademicCard from '@/components/AcademicCard';

export const revalidate = 60;

export default async function AcademicPage({ params }: any) {
  const { locale } = await params;
  const t = await getTranslations('Academic');
  
  let entries: AcademicEntryDto[] = [];
  try {
    const res = await AcademicService.getEntries(1, 50);
    entries = res.data?.data || [];
  } catch (error) {
    console.error('Failed to fetch academic entries', error instanceof Error ? error.message : String(error));
  }

  return (
    <div className="min-h-screen manar-page-bg p-8">
      <div className="max-w-7xl mx-auto space-y-12">
        <header className="mb-12">
          <h1 className="text-4xl md:text-5xl font-black mb-4 text-white tracking-tight">{t('title')}</h1>
          <p className="text-lg text-blue-200/70 max-w-2xl">
            {t('description')}
          </p>
        </header>

        {entries.length > 0 ? (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {entries.map((entry: any) => (
              <AcademicCard key={entry.id} entry={entry} locale={locale} />
            ))}
          </div>
        ) : (
          <div className="text-center py-20 glass-card rounded-3xl">
            <h3 className="text-xl font-medium text-white mb-2">{t('noEntries')}</h3>
            <p className="text-blue-200/60">{t('noEntriesDesc')}</p>
          </div>
        )}
      </div>
    </div>
  );
}
