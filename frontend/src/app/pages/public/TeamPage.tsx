import { useState } from 'react';
import { Link } from 'react-router-dom';
import { PublicNavbar } from '../../components/layout/PublicNavbar';
import { PublicFooter } from '../../components/layout/PublicFooter';
import { Card, CardContent } from '../../components/ui/Card';
import { Badge } from '../../components/ui/Badge';
import { Mail, ExternalLink } from 'lucide-react';
import { useLanguage } from '../../contexts/LanguageContext';

export default function TeamPage() {
  const { t } = useLanguage();
  const [activeTab, setActiveTab] = useState<'chercheurs' | 'doctorants' | 'masteriens'>('chercheurs');

  const tabLabels: Record<'chercheurs' | 'doctorants' | 'masteriens', string> = {
    chercheurs: t('team.researchers'),
    doctorants: t('team.phd'),
    masteriens: t('team.masters')
  };

  // Note: Visitors are intentionally excluded from public team page
  // They are only visible to Admin and SuperAdmin in the dashboard

  return (
    <div className="min-h-screen bg-white dark:bg-background">
      <PublicNavbar />
      <div style={{ height: 'var(--navbar-height)' }} /> {/* Spacer */}

      {/* Page Header */}
      <div className="brand-gradient-diagonal text-white py-20">
        <div className="max-w-[var(--content-max-width)] mx-auto px-6">
          <div className="text-sm text-white/80 mb-3">{t('team.breadcrumb')}</div>
          <h1 className="text-5xl font-bold mb-4">{t('team.title')}</h1>
          <p className="text-xl text-white/90">{t('team.subtitle')}</p>
        </div>
      </div>

      {/* Tabs */}
      <div className="sticky top-[var(--navbar-height)] z-40 bg-white dark:bg-[#141c24] border-b border-surface-border dark:border-[#2d3d4e] shadow-sm">
        <div className="max-w-[var(--content-max-width)] mx-auto px-6">
          <div className="flex gap-8">
            {(['chercheurs', 'doctorants', 'masteriens'] as const).map((tab) => (
              <button
                key={tab}
                onClick={() => setActiveTab(tab)}
                className={`py-4 px-2 border-b-2 transition-colors ${
                  activeTab === tab
                    ? 'border-accent-blue text-accent-blue font-medium'
                    : 'border-transparent text-text-secondary dark:text-text-secondary hover:text-text-primary dark:hover:text-white'
                }`}
              >
                {tabLabels[tab]}
              </button>
            ))}
          </div>
        </div>
      </div>

      {/* Content */}
      <div className="max-w-[var(--content-max-width)] mx-auto px-6 py-12">
        {activeTab === 'chercheurs' && <ChercheursList />}
        {activeTab === 'doctorants' && <DoctorantsList />}
        {activeTab === 'masteriens' && <MasteriensList />}
      </div>

      <PublicFooter />
    </div>
  );
}

function ChercheursList() {
  const { t } = useLanguage();
  const chercheurs = [
    { id: '1', name: 'Ahmed Ben Salem', grade: t('role.researcher'), specialty: 'Intelligence Artificielle', email: 'ahmed.bensalem@isi.utm.tn' },
    { id: '2', name: 'Fatma Gharbi', grade: t('role.researcher'), specialty: 'Systèmes Distribués', email: 'fatma.gharbi@isi.utm.tn' },
    { id: '3', name: 'Mohamed Mezghani', grade: t('role.researcher'), specialty: 'Sécurité Informatique', email: 'mohamed.mezghani@isi.utm.tn' },
  ];

  return (
    <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
      {chercheurs.map((chercheur) => (
        <Card key={chercheur.id} className="hover:shadow-lg transition-shadow">
          <CardContent className="p-6 space-y-4">
            <div className="w-24 h-24 rounded-xl bg-navy text-white flex items-center justify-center text-3xl font-bold mx-auto">
              {chercheur.name.charAt(0)}
            </div>
            <div className="text-center">
              <h3 className="text-lg font-bold text-navy dark:text-white">{chercheur.name}</h3>
              <Badge variant="info" className="mt-2">{chercheur.grade}</Badge>
            </div>
            <div className="text-center">
              <Badge variant="default">{chercheur.specialty}</Badge>
            </div>
            <div className="pt-4 border-t border-surface-border">
              <a href={`mailto:${chercheur.email}`} className="flex items-center justify-center gap-2 text-sm text-text-secondary hover:text-accent-blue">
                <Mail size={16} />
                {t('team.email')}
              </a>
            </div>
            <Link to={`/chercheurs/${chercheur.id}`} className="block text-center text-accent-blue hover:underline">
              {t('team.viewProfile')} →
            </Link>
          </CardContent>
        </Card>
      ))}
    </div>
  );
}

function DoctorantsList() {
  const { t } = useLanguage();
  const doctorants = [
    { name: 'Sarah Trabelsi', thesis: 'Apprentissage profond pour le diagnostic médical', director: 'Dr. Ahmed Ben Salem', year: '2024' },
    { name: 'Karim Jebali', thesis: 'Blockchain pour la sécurisation des données IoT', director: 'Dr. Mohamed Mezghani', year: '2023' },
  ];

  return (
    <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
      {doctorants.map((doc, idx) => (
        <Card key={idx} className="hover:shadow-lg transition-shadow">
          <CardContent className="p-6 space-y-4">
            <div className="w-20 h-20 rounded-full bg-teal text-white flex items-center justify-center text-2xl font-bold mx-auto">
              {doc.name.charAt(0)}
            </div>
            <div className="text-center">
              <h3 className="text-base font-bold text-navy dark:text-white">{doc.name}</h3>
            </div>
            <p className="text-sm text-text-secondary line-clamp-2">{doc.thesis}</p>
            <div className="text-xs text-text-secondary">
              <div>{t('team.director')}: <span className="text-accent-blue">{doc.director}</span></div>
              <div>{t('team.year')}: {doc.year}</div>
            </div>
          </CardContent>
        </Card>
      ))}
    </div>
  );
}

function MasteriensList() {
  const { t } = useLanguage();
  const masteriens = [
    { name: 'Ines Hamdi', subject: 'Système de recommandation basé sur l\'IA', supervisor: 'Dr. Ahmed Ben Salem', year: '2025-2026' },
    { name: 'Youssef Dali', subject: 'Application mobile de gestion de santé', supervisor: 'Dr. Fatma Gharbi', year: '2025-2026' },
  ];

  return (
    <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
      {masteriens.map((master, idx) => (
        <Card key={idx} className="hover:shadow-lg transition-shadow">
          <CardContent className="p-6 space-y-4">
            <div className="w-20 h-20 rounded-full bg-accent-blue text-white flex items-center justify-center text-2xl font-bold mx-auto">
              {master.name.charAt(0)}
            </div>
            <div className="text-center">
              <h3 className="text-base font-bold text-navy dark:text-white">{master.name}</h3>
            </div>
            <p className="text-sm text-text-secondary dark:text-text-secondary line-clamp-2">{master.subject}</p>
            <div className="text-xs text-text-secondary dark:text-text-secondary">
              <div>{t('team.supervisor')}: <span className="text-accent-blue">{master.supervisor}</span></div>
              <div>{t('team.promotion')}: {master.year}</div>
            </div>
          </CardContent>
        </Card>
      ))}
    </div>
  );
}
