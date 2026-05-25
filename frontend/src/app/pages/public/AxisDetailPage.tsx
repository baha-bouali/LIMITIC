import { useParams, Link } from 'react-router-dom';
import { PublicNavbar } from '../../components/layout/PublicNavbar';
import { PublicFooter } from '../../components/layout/PublicFooter';
import { Card, CardContent } from '../../components/ui/Card';
import { Badge } from '../../components/ui/Badge';
import { Button } from '../../components/ui/Button';
import { Users, FileText, Target, Mail, ArrowLeft, Loader2 } from 'lucide-react';
import { useLanguage } from '../../contexts/LanguageContext';
import { useGetAxisByIdQuery } from '../../api/axesApi';

export default function AxisDetailPage() {
  const { id } = useParams<{ id: string }>();
  const { t } = useLanguage();
  const { data: axis, isLoading, error } = useGetAxisByIdQuery(id || '', { skip: !id });

  if (!id) {
    return (
      <div className="min-h-screen bg-white dark:bg-background">
        <PublicNavbar />
        <div style={{ height: 'var(--navbar-height)' }} />
        <div className="max-w-[var(--content-max-width)] mx-auto px-6 py-20 text-center">
          <h1 className="text-4xl font-bold text-navy dark:text-white mb-4">{t('axes.axisNotFound')}</h1>
          <Link to="/axes-recherche">
            <Button>{t('axes.backToAxes')}</Button>
          </Link>
        </div>
        <PublicFooter />
      </div>
    );
  }

  if (isLoading) {
    return (
      <div className="min-h-screen bg-white dark:bg-background">
        <PublicNavbar />
        <div style={{ height: 'var(--navbar-height)' }} />
        <div className="flex items-center justify-center min-h-[60vh]">
          <Loader2 className="animate-spin text-accent-blue" size={48} />
        </div>
        <PublicFooter />
      </div>
    );
  }

  if (error || !axis) {
    return (
      <div className="min-h-screen bg-white dark:bg-background">
        <PublicNavbar />
        <div style={{ height: 'var(--navbar-height)' }} />
        <div className="max-w-[var(--content-max-width)] mx-auto px-6 py-20 text-center">
          <h1 className="text-4xl font-bold text-navy dark:text-white mb-4">{t('axes.axisNotFound')}</h1>
          <Link to="/axes-recherche">
            <Button>{t('axes.backToAxes')}</Button>
          </Link>
        </div>
        <PublicFooter />
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-white dark:bg-background">
      <PublicNavbar />
      <div style={{ height: 'var(--navbar-height)' }} />

      {/* Header */}
      <div className="brand-gradient-diagonal text-white py-20">
        <div className="max-w-[var(--content-max-width)] mx-auto px-6">
          <Link to="/axes-recherche" className="inline-flex items-center gap-2 text-white/80 hover:text-white mb-4 text-sm">
            <ArrowLeft size={16} /> {t('axes.backToAxes')}
          </Link>
          <h1 className="text-5xl font-bold mb-4">{axis.title}</h1>
          <p className="text-xl text-white/90 max-w-3xl">{axis.description}</p>

          <div className="flex gap-8 mt-8">
            <div className="text-center">
              <div className="text-3xl font-bold">{axis.members?.length || 0}</div>
              <div className="text-white/80">{t('axes.members')}</div>
            </div>
            <div className="text-center">
              <div className="text-3xl font-bold">{axis.publicationsCount || 0}</div>
              <div className="text-white/80">{t('pub.publications')}</div>
            </div>
          </div>
        </div>
      </div>

      {/* Content */}
      <div className="max-w-[var(--content-max-width)] mx-auto px-6 py-12 space-y-8">
        {/* Responsable */}
        {axis.responsibleName && (
          <Card>
            <CardContent className="p-6">
              <h2 className="text-2xl font-bold text-navy dark:text-white mb-6 flex items-center gap-2">
                <Target size={24} className="text-accent-blue" />
                {t('axes.axisHead')}
              </h2>
              <div className="flex items-center gap-4">
                <div className="w-16 h-16 rounded-full bg-accent-blue text-white flex items-center justify-center text-2xl font-bold">
                  {axis.responsibleName.charAt(0)}
                </div>
                <div>
                  <h3 className="text-lg font-bold text-navy dark:text-white">{axis.responsibleName}</h3>
                  {axis.responsibleId && (
                    <Link
                      to={`/chercheurs/${axis.responsibleId}`}
                      className="text-sm text-accent-blue hover:underline"
                    >
                      {t('axes.viewProfile')}
                    </Link>
                  )}
                </div>
              </div>
            </CardContent>
          </Card>
        )}

        {/* Thématiques */}
        {axis.themes && axis.themes.length > 0 && (
          <Card>
            <CardContent className="p-6">
              <h2 className="text-2xl font-bold text-navy dark:text-white mb-4">{t('axes.themesTitle')}</h2>
              <div className="flex flex-wrap gap-3">
                {axis.themes.map((theme, idx) => (
                  <Badge key={idx} variant="default" className="text-sm">{theme}</Badge>
                ))}
              </div>
            </CardContent>
          </Card>
        )}

        {/* Membres */}
        {axis.members && axis.members.length > 0 && (
          <Card>
            <CardContent className="p-6">
              <h2 className="text-2xl font-bold text-navy dark:text-white mb-6 flex items-center gap-2">
                <Users size={24} className="text-accent-blue" />
                {t('axes.axisMembers')} ({axis.members.length})
              </h2>
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                {axis.members.map((member) => (
                  <div key={member.id} className="p-4 border border-surface-border rounded-lg hover:shadow-md transition-shadow">
                    <div className="flex items-center gap-3">
                      <div className="w-12 h-12 rounded-full bg-gradient-to-br from-navy to-accent-blue text-white flex items-center justify-center font-bold">
                        {member.firstName.charAt(0)}
                      </div>
                      <div>
                        <h4 className="font-medium text-navy dark:text-white">
                          {member.firstName} {member.lastName}
                        </h4>
                        <Link
                          to={`/chercheurs/${member.id}`}
                          className="text-sm text-accent-blue hover:underline"
                        >
                          {t('axes.viewProfile')}
                        </Link>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            </CardContent>
          </Card>
        )}

        {/* Publications */}
        <Card>
          <CardContent className="p-6">
            <div className="flex items-center justify-between mb-6">
              <h2 className="text-2xl font-bold text-navy dark:text-white flex items-center gap-2">
                <FileText size={24} className="text-accent-blue" />
                {t('axes.axisPublications')}
              </h2>
              <Link to={`/publications?axe=${axis.id}`} className="text-accent-blue hover:underline text-sm">
                {t('axes.viewAllPublications')} →
              </Link>
            </div>
            <div className="text-center py-8">
              <div className="text-5xl font-bold text-accent-blue mb-2">{axis.publicationsCount || 0}</div>
              <div className="text-text-secondary">{t('axes.scientificPublications')}</div>
              <Link to={`/publications?axe=${axis.id}`} className="mt-4 inline-block">
                <Button>{t('axes.explorePublications')}</Button>
              </Link>
            </div>
          </CardContent>
        </Card>
      </div>

      <PublicFooter />
    </div>
  );
}
