'use client';

import { useTranslations } from 'next-intl';
import { motion } from 'framer-motion';
import { Book, Library, PlaySquare, Flame } from 'lucide-react';
import { Link } from '@/i18n/routing';
import SearchBar from '@/components/SearchBar';

export default function KnowledgeSpire() {
  const t = useTranslations('Index');

  return (
    <div className="flex flex-col items-center justify-center h-full w-full py-12 relative">
      <motion.div 
        initial={{ height: 0, opacity: 0 }}
        animate={{ height: '80%', opacity: 1 }}
        transition={{ duration: 1.5, ease: 'easeOut' }}
        className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-32 bg-gradient-to-t from-manar-cyan to-transparent rounded-t-full blur-3xl opacity-20 pointer-events-none"
      />
      <div className="z-10 text-center mb-12">
        <motion.h1 
          initial={{ y: -50, opacity: 0 }}
          animate={{ y: 0, opacity: 1 }}
          transition={{ duration: 0.8 }}
          className="text-6xl font-black text-white mb-2"
        >
          MANAR <span className="text-4xl text-manar-gold">منارة</span>
        </motion.h1>
        <motion.p 
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          transition={{ delay: 0.5, duration: 1 }}
          className="text-xl text-white/60 tracking-widest uppercase font-semibold"
        >
          Logistics Educational Web Application
        </motion.p>
      </div>

      <div className="w-full max-w-xl z-10 px-4 mt-8 flex flex-col gap-6">
        <SearchBar />
        
        <div className="flex flex-wrap gap-4 justify-center">
          <Link href="/books">
            <motion.button whileHover={{ scale: 1.05 }} className="glass-panel px-6 py-3 rounded-full text-white font-bold hover:bg-white/10 transition-all shadow-md flex items-center gap-2">
              <Library className="w-5 h-5 text-manar-cyan" />
              Books Library
            </motion.button>
          </Link>
          <Link href="/academic">
            <motion.button whileHover={{ scale: 1.05 }} className="glass-panel px-6 py-3 rounded-full text-white font-bold hover:bg-white/10 transition-all shadow-md flex items-center gap-2">
              <Book className="w-5 h-5 text-manar-gold" />
              Terminology Bay
            </motion.button>
          </Link>
        </div>
      </div>

      {/* Interactive Term of the Day Footer */}
      <motion.div 
        initial={{ opacity: 0, y: 50 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ delay: 1.5, duration: 0.8 }}
        className="mt-16 w-full max-w-lg z-10 [perspective:1000px]"
      >
        <div className="w-full relative h-32 group cursor-pointer [transform-style:preserve-3d]">
          {/* Front of card */}
          <div className="absolute inset-0 w-full h-full glass-panel flex flex-col items-center justify-center p-6 border-b-4 border-manar-gold transition-transform duration-500 ease-in-out [backface-visibility:hidden] [transform:rotateX(0deg)] group-hover:[transform:rotateX(180deg)]">
             <span className="text-xs text-manar-gold font-bold uppercase tracking-[0.2em] mb-2 flex items-center gap-2">
                 <Flame className="w-4 h-4" /> Word of the Day
             </span>
             <h3 className="text-3xl font-black text-white glow-text">Incoterms</h3>
             <p className="text-white/40 text-sm mt-1">Hover to reveal meaning</p>
          </div>
          
          {/* Back of card */}
          <div className="absolute inset-0 w-full h-full glass-panel bg-[#1e3a5f] flex flex-col items-center justify-center p-6 border-b-4 border-manar-cyan transition-transform duration-500 ease-in-out [backface-visibility:hidden] [transform:rotateX(-180deg)] group-hover:[transform:rotateX(0deg)]">
             <p className="text-white text-center font-medium leading-relaxed drop-shadow-md">
               A set of 11 internationally recognized rules which define the responsibilities of sellers and buyers in international transactions.
             </p>
          </div>
        </div>
      </motion.div>
    </div>
  );
}
