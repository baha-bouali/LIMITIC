import { Link } from 'react-router-dom';
import limticLogo from '@/imports/5b435523-dd99-4ba8-9226-dcd9ab960a41-removebg-preview.png';
import { Mail, Phone, MapPin, Linkedin, Twitter, ArrowUp } from 'lucide-react';
import { useLanguage } from '../../contexts/LanguageContext';

export function PublicFooter() {
  const { t } = useLanguage();
  const scrollToTop = () => {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  return (
    <footer className="brand-gradient-diagonal text-white">
      <div className="max-w-[var(--content-max-width)] mx-auto px-6 py-16">
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-12">
          {/* Logo & Mission */}
          <div className="space-y-4">
            <div className="flex items-center gap-3">
              <div className="flex items-center justify-center overflow-hidden px-3 py-2">
                <img
                  src={limticLogo}
                  alt="LIMTIC"
                  className="w-56 h-auto object-contain"
                  onError={(e) => {
                    e.currentTarget.style.display = 'none';
                    const span = document.createElement('span');
                    span.className = 'text-xl font-bold text-white';
                    span.textContent = 'L';
                    e.currentTarget.parentElement?.appendChild(span);
                  }}
                />
              </div>
              <div>
                <div className="font-bold text-lg leading-tight">LIMTIC</div>
                <div className="text-xs text-gray-400 leading-tight">{t('lab.shortName')}</div>
              </div>
            </div>
            <p className="text-sm text-gray-400 leading-relaxed">
              {t('lab.fullName')}
            </p>
          </div>

          {/* Quick Links */}
          <div>
            <h4 className="font-semibold mb-4">{t('footer.quickLinks')}</h4>
            <ul className="space-y-2">
              <li><Link to="/" className="text-sm text-gray-400 hover:text-white transition-colors">{t('nav.home')}</Link></li>
              <li><Link to="/equipe" className="text-sm text-gray-400 hover:text-white transition-colors">{t('nav.team')}</Link></li>
              <li><Link to="/publications" className="text-sm text-gray-400 hover:text-white transition-colors">{t('nav.publications')}</Link></li>
              <li><Link to="/evenements" className="text-sm text-gray-400 hover:text-white transition-colors">{t('nav.events')}</Link></li>
              <li><Link to="/contact" className="text-sm text-gray-400 hover:text-white transition-colors">{t('nav.contact')}</Link></li>
            </ul>
          </div>

          {/* Contact Info */}
          <div>
            <h4 className="font-semibold mb-4">{t('footer.contact')}</h4>
            <ul className="space-y-3">
              <li className="flex items-start gap-3 text-sm text-gray-400">
                <MapPin size={16} className="mt-0.5 flex-shrink-0" />
                <span>{t('lab.address')}</span>
              </li>
              <li className="flex items-center gap-3 text-sm text-gray-400">
                <Phone size={16} className="flex-shrink-0" />
                <span>+216 71 123 456</span>
              </li>
              <li className="flex items-center gap-3 text-sm text-gray-400">
                <Mail size={16} className="flex-shrink-0" />
                <span>contact@limtic.tn</span>
              </li>
            </ul>
          </div>

          {/* Social Links */}
          <div>
            <h4 className="font-semibold mb-4">{t('footer.followUs')}</h4>
            <div className="flex items-center gap-3">
              <a
                href="https://linkedin.com"
                target="_blank"
                rel="noopener noreferrer"
                className="w-10 h-10 flex items-center justify-center bg-white/10 hover:bg-accent-blue rounded-lg transition-colors"
              >
                <Linkedin size={20} />
              </a>
              <a
                href="https://twitter.com"
                target="_blank"
                rel="noopener noreferrer"
                className="w-10 h-10 flex items-center justify-center bg-white/10 hover:bg-accent-blue rounded-lg transition-colors"
              >
                <Twitter size={20} />
              </a>
            </div>
          </div>
        </div>

        {/* Bottom Bar */}
        <div className="mt-12 pt-8 border-t border-white/10 flex flex-col md:flex-row items-center justify-between gap-4">
          <p className="text-sm text-gray-400">
            © {new Date().getFullYear()} LIMTIC. {t('footer.allRights')}
          </p>
          <div className="flex items-center gap-6">
            <Link to="#" className="text-sm text-gray-400 hover:text-white transition-colors">{t('footer.legal')}</Link>
            <Link to="#" className="text-sm text-gray-400 hover:text-white transition-colors">{t('footer.privacy')}</Link>
            <button
              onClick={scrollToTop}
              className="flex items-center gap-2 text-sm text-gray-400 hover:text-white transition-colors"
            >
              <ArrowUp size={16} />
              {t('footer.backToTop')}
            </button>
          </div>
        </div>
      </div>
    </footer>
  );
}
