import { useLanguage } from '../../contexts/LanguageContext';
import { Globe } from 'lucide-react';
import { clsx } from 'clsx';
import iconLogo from '@/imports/icon.png';

interface LanguageSwitcherProps {
  variant?: 'navbar' | 'sidebar' | 'dropdown';
  className?: string;
}

export function LanguageSwitcher({ variant = 'navbar', className }: LanguageSwitcherProps) {
  const { language, setLanguage } = useLanguage();

  if (variant === 'dropdown') {
    return (
      <div className={clsx('relative', className)}>
        <select
          value={language}
          onChange={(e) => setLanguage(e.target.value as 'fr' | 'en')}
          className="pl-8 pr-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm appearance-none cursor-pointer"
        >
          <option value="fr">Français</option>
          <option value="en">English</option>
        </select>
        <Globe size={16} className="absolute left-2.5 top-1/2 -translate-y-1/2 text-text-muted pointer-events-none" />
      </div>
    );
  }

 

  // Default navbar variant
  return (
    <div className={clsx('flex items-center gap-2 bg-light-gray dark:bg-muted rounded-lg p-1.5', className)}>
      <button
        onClick={() => setLanguage('fr')}
        className={clsx(
          'px-3 py-1.5 rounded-lg text-sm font-medium transition-colors',
          language === 'fr'
            ? 'bg-accent-blue text-white'
            : 'text-text-secondary hover:text-navy dark:hover:text-white hover:bg-white dark:hover:bg-[#1e2a35]'
        )}
      >
        FR
      </button>
      <button
        onClick={() => setLanguage('en')}
        className={clsx(
          'px-3 py-1.5 rounded-lg text-sm font-medium transition-colors',
          language === 'en'
            ? 'bg-accent-blue text-white'
            : 'text-text-secondary hover:text-navy dark:hover:text-white hover:bg-white dark:hover:bg-[#1e2a35]'
        )}
      >
        EN
      </button>
    </div>
  );
}
