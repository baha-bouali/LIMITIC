import { useState } from 'react';
import limticLogo from '@/imports/5b435523-dd99-4ba8-9226-dcd9ab960a41-removebg-preview.png';
import iconLogo from '@/imports/icon.png';
import { useNavigate, Link } from 'react-router-dom';
import { Button } from '../../components/ui/Button';
import { useAuth } from '../../contexts/AuthContext';
import { useLanguage } from '../../contexts/LanguageContext';
import { LanguageSwitcher } from '../../components/shared/LanguageSwitcher';
import { Eye, EyeOff } from 'lucide-react';
import { toast } from 'sonner';
import { clsx } from 'clsx';

export default function LoginPage() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [showPassword, setShowPassword] = useState(false);
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();
  const { t, language, setLanguage } = useLanguage();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    try {
      const user = await login(email, password);
      toast.success(t('login.connectionSuccess'));

      switch (user.role) {
        case 'SUPER_ADMIN':
          navigate('/dashboard/superadmin');
          break;
        case 'ADMIN':
          navigate('/dashboard/admin');
          break;
        case 'DOCTORANT':
          navigate('/dashboard/doctorant');
          break;
        case 'MASTERIEN':
          navigate('/dashboard/masterien');
          break;
        case 'VISITOR':
          navigate('/dashboard/visitor');
          break;
        default:
          navigate('/dashboard/chercheur');
          break;
      }
    } catch (error) {
      toast.error(t('login.connectionError'));
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen brand-gradient-diagonal flex items-center justify-center p-6">
      <div className="absolute top-6 right-6">
        <div className="flex items-center gap-2 bg-white/20 backdrop-blur-sm rounded-lg p-2">
          <button
            onClick={() => setLanguage('fr')}
            className={clsx(
              'px-3 py-1.5 rounded-lg text-sm font-medium transition-colors',
              language === 'fr'
                ? 'bg-white/30 text-white'
                : 'text-white/70 hover:text-white hover:bg-white/10'
            )}
          >
            FR
          </button>
          <button
            onClick={() => setLanguage('en')}
            className={clsx(
              'px-3 py-1.5 rounded-lg text-sm font-medium transition-colors',
              language === 'en'
                ? 'bg-white/30 text-white'
                : 'text-white/70 hover:text-white hover:bg-white/10'
            )}
          >
            EN
          </button>
        </div>
      </div>
      <div className="w-full max-w-md">
        <div className="text-center mb-8">
          <Link to="/" className="inline-flex items-center gap-3 mb-4">
            <div className="flex items-center justify-center overflow-hidden px-4 py-3">
              <img
                src={limticLogo}
                alt="LIMTIC"
                className="w-64 h-auto object-contain"
                onError={(e) => {
                  e.currentTarget.style.display = 'none';
                  const span = document.createElement('span');
                  span.className = 'text-3xl font-bold text-white';
                  span.textContent = 'L';
                  e.currentTarget.parentElement?.appendChild(span);
                }}
              />
            </div>
          </Link>
          <h1 className="text-3xl font-bold text-white mb-2">{t('login.secure')}</h1>
          <p className="text-white/80">{t('login.secureBackoffice')}</p>
        </div>

        <div className="bg-white dark:bg-[#141c24] rounded-2xl shadow-[0_8px_32px_rgba(0,0,0,.24)] p-8">
          <h2 className="text-2xl font-bold text-navy dark:text-white mb-6">{t('login.title')}</h2>

          <form onSubmit={handleSubmit} className="space-y-6">
            <div>
              <label className="block text-sm font-medium mb-2 text-text-primary dark:text-text-primary">{t('contact.email')}</label>
              <input
                type="email"
                required
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                className="w-full px-4 py-3 border border-surface-border dark:border-[#2d3d4e] rounded-[var(--radius-input)] focus:ring-2 focus:ring-accent-blue focus:border-transparent bg-white dark:bg-[#1e2a35] text-text-primary dark:text-text-primary"
                placeholder="exemple@limtic.tn"
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-2 text-text-primary dark:text-text-primary">{t('login.password')}</label>
              <div className="relative">
                <input
                  type={showPassword ? 'text' : 'password'}
                  required
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  className="w-full px-4 py-3 border border-surface-border dark:border-[#2d3d4e] rounded-[var(--radius-input)] focus:ring-2 focus:ring-accent-blue focus:border-transparent pr-12 bg-white dark:bg-[#1e2a35] text-text-primary dark:text-text-primary"
                  placeholder="••••••••"
                />
                <button
                  type="button"
                  onClick={() => setShowPassword(!showPassword)}
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-text-muted hover:text-text-primary dark:hover:text-text-primary"
                >
                  {showPassword ? <EyeOff size={20} /> : <Eye size={20} />}
                </button>
              </div>
            </div>

            <div className="flex items-center justify-between text-sm">
              <label className="flex items-center gap-2 text-text-primary dark:text-text-primary">
                <input type="checkbox" className="w-4 h-4 rounded border-surface-border" />
                <span>{t('login.rememberMe')}</span>
              </label>
              <Link to="/forgot-password" className="text-accent-blue hover:underline">
                {t('login.forgotPassword')}
              </Link>
            </div>

            <Button type="submit" className="w-full" disabled={loading}>
              {loading ? t('login.connecting') : t('login.submit')}
            </Button>
          </form>

          <div className="mt-6 p-4 bg-light-gray dark:bg-[#1e2a35] rounded-lg">
            <p className="text-xs text-text-secondary dark:text-text-secondary mb-2">{t('login.testAccounts')}</p>
            <ul className="text-xs text-text-secondary dark:text-text-secondary space-y-1">
              <li>• superadmin@limtic.tn (Super Admin)</li>
              <li>• admin@limtic.tn (Admin)</li>
              <li>• chercheur@limtic.tn (Chercheur)</li>
              <li>• doctorant@limtic.tn (Doctorant)</li>
              <li>• masterien@limtic.tn (Mastérien)</li>
            </ul>
          </div>
        </div>

        <div className="text-center mt-6">
          <Link to="/" className="text-white/80 hover:text-white text-sm">
            ← {t('login.backToHome')}
          </Link>
        </div>
      </div>
    </div>
  );
}
