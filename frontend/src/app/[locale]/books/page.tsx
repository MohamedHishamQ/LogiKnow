import { getTranslations } from 'next-intl/server';
import { BooksService, BookDto } from '@/api/client';
import BookCard from '@/components/BookCard';

export const revalidate = 60; // Revalidate every minute

export default async function BooksPage({ params }: any) {
  const { locale } = await params;
  const t = await getTranslations('Books');
  
  let books: BookDto[] = [];
  try {
    const res = await BooksService.getBooks(1, 50);
    books = res.data?.data || [];
  } catch (error) {
    console.error('Failed to fetch books', error instanceof Error ? error.message : String(error));
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

        {books.length > 0 ? (
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
            {books.map((book: any) => (
              <BookCard key={book.id} book={book} locale={locale} />
            ))}
          </div>
        ) : (
          <div className="text-center py-20 glass-card rounded-3xl">
            <h3 className="text-xl font-medium text-white mb-2">{t('noBooks')}</h3>
            <p className="text-blue-200/60">{t('noBooksDesc')}</p>
          </div>
        )}
      </div>
    </div>
  );
}
