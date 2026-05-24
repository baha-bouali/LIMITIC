import { useState } from 'react';
import { Link } from 'react-router-dom';
import { PublicNavbar } from '../../components/layout/PublicNavbar';
import { PublicFooter } from '../../components/layout/PublicFooter';
import { Card, CardContent } from '../../components/ui/Card';
import { Badge } from '../../components/ui/Badge';
import { Mail, Loader2 } from 'lucide-react';
import { useLanguage } from '../../contexts/LanguageContext';
import { useGetPublicUsersQuery, UserDto } from '../../api/usersApi';

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
  const { data: chercheurs = [], isLoading, isError } = useGetPublicUsersQuery({ role: 3, status: 'active', limit: 100 });

  if (isLoading) {
    return <div className="flex justify-center py-12"><Loader2 className="animate-spin text-accent-blue" size={32} /></div>;
  }

  if (isError) {
    return <div className="text-center py-12 text-red-500">{t('common.error') || 'Error loading data'}</div>;
  }

  if (chercheurs.length === 0) {
    return <div className="text-center py-12 text-text-secondary">{t('common.noData') || 'No researchers found'}</div>;
  }

  return (
    <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
      {chercheurs.map((chercheur) => (
        <UserCard key={chercheur.id} user={chercheur} role="chercheur" />
      ))}
    </div>
  );
}

function DoctorantsList() {
  const { t } = useLanguage();
  const { data: doctorants = [], isLoading, isError } = useGetPublicUsersQuery({ role: 4, status: 'active', limit: 100 });

  if (isLoading) {
    return <div className="flex justify-center py-12"><Loader2 className="animate-spin text-teal" size={32} /></div>;
  }

  if (isError) {
    return <div className="text-center py-12 text-red-500">{t('common.error') || 'Error loading data'}</div>;
  }

  if (doctorants.length === 0) {
    return <div className="text-center py-12 text-text-secondary">{t('common.noData') || 'No PhD students found'}</div>;
  }

  return (
    <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
      {doctorants.map((doc) => (
        <UserCard key={doc.id} user={doc} role="doctorant" />
      ))}
    </div>
  );
}

function MasteriensList() {
  const { t } = useLanguage();
  const { data: masteriens = [], isLoading, isError } = useGetPublicUsersQuery({ role: 5, status: 'active', limit: 100 });

  if (isLoading) {
    return <div className="flex justify-center py-12"><Loader2 className="animate-spin text-accent-blue" size={32} /></div>;
  }

  if (isError) {
    return <div className="text-center py-12 text-red-500">{t('common.error') || 'Error loading data'}</div>;
  }

  if (masteriens.length === 0) {
    return <div className="text-center py-12 text-text-secondary">{t('common.noData') || 'No Master students found'}</div>;
  }

  return (
    <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
      {masteriens.map((master) => (
        <UserCard key={master.id} user={master} role="masterien" />
      ))}
    </div>
  );
}

function UserCard({ user, role }: { user: UserDto; role: 'chercheur' | 'doctorant' | 'masterien' }) {
  const { t } = useLanguage();

  // Frontend-only: do not fetch role-specific profiles from backend on public page.
  // Render only the public user fields returned by /api/users/public to avoid hitting
  // endpoints that may require additional DB schema or authorization.
  return (
    <Card className="hover:shadow-lg transition-shadow">
      <CardContent className="p-6 space-y-4">
        <div className="w-24 h-24 rounded-full bg-navy text-white flex items-center justify-center text-3xl font-bold mx-auto">
          {((user.firstName ?? '') || '').charAt(0)}{((user.lastName ?? '') || '').charAt(0)}
        </div>
        <div className="text-center">
          <h3 className="text-lg font-bold text-navy dark:text-white">{user.firstName} {user.lastName}</h3>
          {role === 'chercheur' && <Badge variant="info" className="mt-2">{t('role.researcher')}</Badge>}
          {role === 'doctorant' && <Badge variant="info" className="mt-2">{t('role.phd') || 'Doctorant'}</Badge>}
          {role === 'masterien' && <Badge variant="info" className="mt-2">{t('role.master') || 'Masterien'}</Badge>}
        </div>

        <div className="pt-4 border-t border-surface-border">
          <a href={`mailto:${user.email}`} className="flex items-center justify-center gap-2 text-sm text-text-secondary hover:text-accent-blue">
            <Mail size={16} />
            {t('team.email')}
          </a>
        </div>

        <Link to={role === 'chercheur' ? `/chercheurs/${user.id}` : role === 'doctorant' ? `/doctorants/${user.id}` : `/masteriens/${user.id}`} className="block text-center text-accent-blue hover:underline">
          {t('team.viewProfile')} →
        </Link>
      </CardContent>
    </Card>
  );
}
