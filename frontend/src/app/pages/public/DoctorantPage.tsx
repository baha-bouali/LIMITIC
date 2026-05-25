import { useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { Loader2, Mail, BookOpen, GraduationCap, Users } from 'lucide-react';
import { PublicNavbar } from '../../components/layout/PublicNavbar';
import { PublicFooter } from '../../components/layout/PublicFooter';
import { Badge } from '../../components/ui/Badge';
import { Card, CardContent } from '../../components/ui/Card';
import { useGetPhDStudentProfileQuery } from '../../api/profilesApi';
import { useGetPublicUsersQuery } from '../../api/usersApi';
import { useLanguage } from '../../contexts/LanguageContext';

export default function DoctorantPage() {
  const { id } = useParams<{ id: string }>();
  const { t } = useLanguage();
  const [activeTab, setActiveTab] = useState<'thesis' | 'research' | 'supervision'>('thesis');

  const { data: doctorant, isLoading, isError } = useGetPhDStudentProfileQuery(id || '', {
    skip: !id,
  });
  const { data: publicUsers = [] } = useGetPublicUsersQuery({ limit: 100 });

  const fallbackUser = publicUsers.find((user) => user.id === id);
  const displayDoctorant = doctorant ?? (fallbackUser
    ? {
        id: fallbackUser.id,
        firstName: fallbackUser.firstName,
        lastName: fallbackUser.lastName,
        email: fallbackUser.email,
        role: 4,
        isActive: true,
        thesisSubject: undefined,
        enrollmentYear: new Date().getFullYear(),
        supervisorId: null,
        supervisorName: null,
        researchAxes: [],
      }
    : null);

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

  if (!displayDoctorant) {
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

  const hasProfileDetails = Boolean(doctorant);

  return (
    <div className="min-h-screen bg-white dark:bg-background">
      <PublicNavbar />
      <div style={{ height: 'var(--navbar-height)' }} />

      <div className="brand-gradient-diagonal text-white py-20">
        <div className="max-w-[var(--content-max-width)] mx-auto px-6">
          <div className="text-sm text-white/80 mb-3">
            <Link to="/equipe" className="hover:text-white">{t('team.title')}</Link> / {t('team.phd') || 'Doctorants'}
          </div>
          <h1 className="text-5xl font-bold mb-4">{displayDoctorant?.firstName} {displayDoctorant?.lastName}</h1>
          <p className="text-xl text-white/90">{displayDoctorant?.thesisSubject || (hasProfileDetails ? (t('common.noData') || 'Aucun sujet de thèse disponible.') : 'Profil public limité') }</p>
        </div>
      </div>

      <div className="max-w-[var(--content-max-width)] mx-auto px-6 py-12">
        <div className="grid grid-cols-1 lg:grid-cols-4 gap-8">
          <div className="lg:col-span-1">
            <div className="sticky top-24 space-y-6">
              <div className="w-48 h-48 rounded-full bg-navy text-white flex items-center justify-center text-6xl font-bold mx-auto border-4 border-accent-blue">
                {displayDoctorant?.firstName?.charAt(0)}{displayDoctorant?.lastName?.charAt(0)}
              </div>
              <div className="text-center">
                <h2 className="text-2xl font-bold text-navy dark:text-white">
                  {displayDoctorant?.firstName} {displayDoctorant?.lastName}
                </h2>
                <Badge variant="info" className="mt-2 text-white">{t('role.phd') || 'Doctorant'}</Badge>
                {displayDoctorant?.enrollmentYear && hasProfileDetails && <Badge variant="default" className="mt-2 ml-2">{displayDoctorant.enrollmentYear}</Badge>}
              </div>
              <div className="space-y-3">
                <div className="flex items-center gap-3 text-sm">
                  <Mail size={16} className="text-accent-blue" />
                  <a href={`mailto:${displayDoctorant?.email || ''}`} className="hover:underline text-text-secondary hover:text-accent-blue dark:text-text-secondary dark:hover:text-white break-all">
                    {displayDoctorant?.email || 'Email non disponible'}
                  </a>
                </div>
                {hasProfileDetails && displayDoctorant?.supervisorName && (
                  <div className="flex items-center gap-3 text-sm text-text-secondary">
                    <Users size={16} className="text-accent-blue" />
                    <span>{displayDoctorant.supervisorName}</span>
                  </div>
                )}
              </div>
            </div>
          </div>

          <div className="lg:col-span-3">
            <div className="border-b border-surface-border mb-6">
              <div className="flex gap-6 flex-wrap">
                {['thesis', 'research', 'supervision'].map((tab) => (
                  <button
                    key={tab}
                    onClick={() => setActiveTab(tab as 'thesis' | 'research' | 'supervision')}
                    className={`py-3 px-2 border-b-2 transition-colors capitalize ${
                      activeTab === tab ? 'border-accent-blue text-accent-blue font-medium' : 'border-transparent text-text-secondary hover:text-text-primary dark:hover:text-white'
                    }`}
                  >
                    {tab === 'thesis' ? 'Thèse' : tab === 'research' ? 'Axes de recherche' : 'Encadrement'}
                  </button>
                ))}
              </div>
            </div>

            {activeTab === 'thesis' && (
              <div className="space-y-4">
                <Card>
                  <CardContent className="p-6">
                    <div className="flex items-start gap-3">
                      <BookOpen size={20} className="text-accent-blue mt-1 flex-shrink-0" />
                      <div>
                        <div className="font-semibold text-navy dark:text-white mb-2">Sujet de thèse</div>
                        <p className="text-text-secondary whitespace-pre-wrap leading-relaxed">
                          {displayDoctorant?.thesisSubject || (hasProfileDetails ? (t('common.noData') || 'Aucun sujet disponible.') : 'Aucune donnée publique détaillée disponible.')}
                        </p>
                      </div>
                    </div>
                  </CardContent>
                </Card>
              </div>
            )}

            {activeTab === 'research' && (
              <div className="space-y-4">
                {hasProfileDetails && displayDoctorant?.researchAxes && displayDoctorant.researchAxes.length > 0 ? (
                  <div className="flex flex-wrap gap-2">
                    {displayDoctorant.researchAxes.map((axis) => (
                      <Badge key={axis.id} variant="default">{axis.title}</Badge>
                    ))}
                  </div>
                ) : (
                  <div className="text-center py-12 text-text-secondary">
                    {hasProfileDetails ? (t('common.noData') || 'Aucun axe de recherche disponible.') : 'Aucun axe de recherche public disponible.'}
                  </div>
                )}
              </div>
            )}

            {activeTab === 'supervision' && (
              <div className="space-y-4">
                <Card>
                  <CardContent className="p-6">
                    <div className="flex items-start gap-3">
                      <GraduationCap size={20} className="text-accent-blue mt-1 flex-shrink-0" />
                      <div>
                        <div className="font-semibold text-navy dark:text-white mb-2">Encadrant</div>
                        <p className="text-text-secondary">{displayDoctorant?.supervisorName || (hasProfileDetails ? (t('common.noData') || 'Aucun encadrant renseigné.') : 'Aucune donnée publique détaillée disponible.')}</p>
                      </div>
                    </div>
                  </CardContent>
                </Card>
              </div>
            )}
          </div>
        </div>
      </div>

      <PublicFooter />
    </div>
  );
}