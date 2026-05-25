import { PublicNavbar } from '../../components/layout/PublicNavbar';
import { PublicFooter } from '../../components/layout/PublicFooter';
import { Badge } from '../../components/ui/Badge';
import { Mail, Phone, ExternalLink, Loader2 } from 'lucide-react';
import { useState } from 'react';
import { useParams } from 'react-router-dom';
import { useGetResearcherProfileQuery } from '../../api/profilesApi';
import { useLanguage } from '../../contexts/LanguageContext';

export default function ResearcherPage() {
  const { id } = useParams<{ id: string }>();
  const { t } = useLanguage();
  const [activeTab, setActiveTab] = useState('bio');

  const { data: researcher, isLoading, isError } = useGetResearcherProfileQuery(id || '', {
    skip: !id,
  });

  if (isLoading) {
    return (
      <div className="min-h-screen bg-white dark:bg-background flex flex-col">
        <PublicNavbar />
        <div className="flex-1 flex items-center justify-center">
          <Loader2 className="animate-spin text-accent-blue" size={48} />
        </div>
        <PublicFooter />
      </div>
    );
  }

  if (isError || !researcher) {
    return (
      <div className="min-h-screen bg-white dark:bg-background flex flex-col">
        <PublicNavbar />
        <div className="flex-1 flex items-center justify-center">
          <div className="text-xl text-red-500">{t('common.error') || 'Error loading profile data.'}</div>
        </div>
        <PublicFooter />
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-white dark:bg-background">
      <PublicNavbar />
      <div style={{ height: 'var(--navbar-height)' }} />

      <div className="max-w-[var(--content-max-width)] mx-auto px-6 py-12">
        <div className="grid grid-cols-1 lg:grid-cols-4 gap-8">
          <div className="lg:col-span-1">
            <div className="sticky top-24 space-y-6">
              <div className="w-48 h-48 rounded-full bg-navy text-white flex items-center justify-center text-6xl font-bold mx-auto border-4 border-accent-blue">
                {researcher.firstName.charAt(0)}{researcher.lastName.charAt(0)}
              </div>
              <div className="text-center">
                <h2 className="text-2xl font-bold text-navy dark:text-white">
                  {researcher.rank ? `${researcher.rank} ` : ''}{researcher.firstName} {researcher.lastName}
                </h2>
                <Badge variant="info" className="mt-2 text-white">{t('role.researcher') || 'Chercheur'}</Badge>
                {researcher.specialty && (
                  <Badge variant="default" className="mt-2 ml-2">{researcher.specialty}</Badge>
                )}
              </div>
              <div className="space-y-3">
                <div className="flex items-center gap-3 text-sm">
                  <Mail size={16} className="text-accent-blue" />
                  <a href={`mailto:${researcher.email}`} className="hover:underline text-text-secondary hover:text-accent-blue dark:text-text-secondary dark:hover:text-white break-all">{researcher.email}</a>
                </div>
                {researcher.phoneNumber && (
                  <div className="flex items-center gap-3 text-sm text-text-secondary">
                    <Phone size={16} className="text-accent-blue" />
                    <span>{researcher.phoneNumber}</span>
                  </div>
                )}
                {researcher.office && (
                  <div className="flex items-center gap-3 text-sm text-text-secondary">
                    <div className="w-4 flex justify-center">-</div>
                    <span>Bureau: {researcher.office}</span>
                  </div>
                )}
              </div>
              <div className="flex gap-2 flex-wrap">
                {researcher.orcid && (
                  <a href={researcher.orcid} target="_blank" rel="noreferrer" className="flex-1 text-center px-4 py-2 border border-accent-blue text-accent-blue rounded-lg hover:bg-accent-blue hover:text-white transition-colors text-sm">ORCID</a>
                )}
                {researcher.googleScholar && (
                  <a href={researcher.googleScholar} target="_blank" rel="noreferrer" className="flex-1 text-center px-4 py-2 border border-accent-blue text-accent-blue rounded-lg hover:bg-accent-blue hover:text-white transition-colors text-sm">Scholar</a>
                )}
                {researcher.researchGate && (
                  <a href={researcher.researchGate} target="_blank" rel="noreferrer" className="flex-1 text-center px-4 py-2 border border-accent-blue text-accent-blue rounded-lg hover:bg-accent-blue hover:text-white transition-colors text-sm">ResearchGate</a>
                )}
                {researcher.linkedIn && (
                  <a href={researcher.linkedIn} target="_blank" rel="noreferrer" className="flex-1 text-center px-4 py-2 border border-accent-blue text-accent-blue rounded-lg hover:bg-accent-blue hover:text-white transition-colors text-sm">LinkedIn</a>
                )}
              </div>
            </div>
          </div>

          <div className="lg:col-span-3">
            <div className="border-b border-surface-border mb-6">
              <div className="flex gap-6">
                {['bio', 'publications', 'encadrements'].map((tab) => (
                  <button
                    key={tab}
                    onClick={() => setActiveTab(tab)}
                    className={`py-3 px-2 border-b-2 transition-colors capitalize ${
                      activeTab === tab ? 'border-accent-blue text-accent-blue font-medium' : 'border-transparent text-text-secondary hover:text-text-primary dark:hover:text-white'
                    }`}
                  >
                    {tab === 'bio' ? 'Biographie' : tab === 'publications' ? 'Publications' : 'Encadrements'}
                  </button>
                ))}
              </div>
            </div>

            {activeTab === 'bio' && (
              <div className="prose max-w-none">
                <p className="text-text-secondary leading-relaxed whitespace-pre-wrap">
                  {researcher.biography || t('common.noData') || 'Aucune biographie disponible.'}
                </p>
              </div>
            )}

            {activeTab === 'publications' && (
              <div className="space-y-4">
                <div className="text-center py-12 text-text-secondary">
                  {t('common.noData') || 'Aucune publication disponible.'}
                </div>
              </div>
            )}

            {activeTab === 'encadrements' && (
              <div className="space-y-4">
                <div className="text-center py-12 text-text-secondary">
                  {t('common.noData') || 'Aucun encadrement disponible.'}
                </div>
              </div>
            )}
          </div>
        </div>
      </div>

      <PublicFooter />
    </div>
  );
}
