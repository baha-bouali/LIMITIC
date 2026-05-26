import { PublicNavbar } from '../../components/layout/PublicNavbar';
import { PublicFooter } from '../../components/layout/PublicFooter';
import { Card, CardContent, CardHeader } from '../../components/ui/Card';
import { Badge } from '../../components/ui/Badge';
import { Button } from '../../components/ui/Button';
import { Link } from 'react-router-dom';
import { Users, FileText, Eye, Loader2 } from 'lucide-react';
import { useLanguage } from '../../contexts/LanguageContext';
import { useGetAllAxesQuery } from '../../api/axesApi';

export default function AxesPage() {
  const { t } = useLanguage();
  const { data: axes = [], isLoading, error } = useGetAllAxesQuery();

  const totalMembers = axes.reduce((sum, axe) => sum + (axe.members?.length || 0), 0);
  const totalPublications = axes.reduce((sum, axe) => sum + (axe.publicationsCount || 0), 0);

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

  if (error) {
    return (
      <div className="min-h-screen bg-white dark:bg-background">
        <PublicNavbar />
        <div style={{ height: 'var(--navbar-height)' }} />
        <div className="max-w-[var(--content-max-width)] mx-auto px-6 py-20 text-center">
          <h1 className="text-2xl font-bold text-navy dark:text-white mb-4">{t('axes.errorLoading')}</h1>
          <p className="text-text-secondary">{t('axes.tryAgainLater')}</p>
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
          <div className="text-sm text-white/80 mb-3">
            {t('axes.breadcrumb')}
          </div>
          <h1 className="text-5xl font-bold mb-4">
            {t('axes.title')}
          </h1>
          <p className="text-xl text-white/90">
            {t('axes.discoverDomains')}
          </p>
          <div className="flex gap-8 mt-8">
            <div className="text-center">
              <div className="text-3xl font-bold">
                {axes.length}
              </div>
              <div className="text-white/80">
                {t('axes.activeAxes')}
              </div>
            </div>
            <div className="text-center">
              <div className="text-3xl font-bold">
                {totalMembers}
              </div>
              <div className="text-white/80">
                {t('axes.researchers')}
              </div>
            </div>
            <div className="text-center">
              <div className="text-3xl font-bold">
                {totalPublications}
              </div>
              <div className="text-white/80">
                {t('pub.publications')}
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Content */}
      <div className="max-w-[var(--content-max-width)] mx-auto px-6 py-12">
        <div className="space-y-8">
          {axes.map((axe) => (
            <Card key={axe.id} className="hover:shadow-lg transition-shadow overflow-hidden">
              <div className={`h-1 bg-${axe.color || 'accent-blue'}`} />
              <CardHeader>
                <div className="flex items-start justify-between gap-4">
                  <div className="flex-1">
                    <h2 className="text-2xl font-bold text-navy dark:text-white mb-3">{axe.title}</h2>
                    <p className="text-text-secondary dark:text-text-secondary leading-relaxed mb-4">
                      {axe.description}
                    </p>
                    {axe.themes && axe.themes.length > 0 && (
                      <div className="flex flex-wrap gap-2 mb-4">
                        {axe.themes.map((theme, idx) => (
                          <Badge key={idx} variant="default">{theme}</Badge>
                        ))}
                      </div>
                    )}
                  </div>
                  <div className="text-right space-y-4 flex-shrink-0">
                    <div>
                      <div className="text-3xl font-bold text-navy dark:text-white">
                        {axe.members?.length || 0}
                      </div>
                      <div className="text-sm text-text-secondary">
                        {t('axes.members').toLowerCase()}
                      </div>
                    </div>
                    <div>
                      <div className="text-2xl font-bold text-accent-blue">
                        {axe.publicationsCount || 0}
                      </div>
                      <div className="text-sm text-text-secondary">
                        {t('axes.publicationsCount')}
                      </div>
                    </div>
                  </div>
                </div>
              </CardHeader>
              <CardContent>
                <div className="flex items-center justify-between">
                  <div className="flex items-center gap-3">
                    <Users size={20} className="text-accent-blue" />
                    <div>
                      <div className="text-sm text-text-secondary">{t('axes.responsible')}</div>
                      {axe.responsibleId && axe.responsibleName ? (
                        <Link
                          to={`/chercheurs/${axe.responsibleId}`}
                          className="font-medium text-accent-blue hover:underline"
                        >
                          {axe.responsibleName}
                        </Link>
                      ) : (
                        <span className="font-medium text-text-secondary">{t('axes.notAssigned')}</span>
                      )}
                    </div>
                  </div>
                  <div className="flex gap-3">
                    <Link to={`/axes-recherche/${axe.id}`}>
                      <Button variant="outlined" className="flex items-center gap-2">
                        <Eye size={18} />
                        {t('axes.viewDetails')}
                      </Button>
                    </Link>
                    <Link
                      to={`/publications?axe=${axe.id}`}
                      className="flex items-center gap-2 px-4 py-2 bg-accent-blue text-white rounded-lg hover:bg-accent-blue/90 transition-colors"
                    >
                      <FileText size={18} />
                      {t('axes.viewPublications')}
                    </Link>
                  </div>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      </div>

      <PublicFooter />
    </div>
  );
}
