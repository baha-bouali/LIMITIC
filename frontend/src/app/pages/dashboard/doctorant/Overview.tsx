import { Link } from 'react-router-dom';
import { Card, CardContent } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { FileText, BookOpen, Target, ChevronRight, Calendar, Users } from 'lucide-react';
import { useLanguage } from '../../../contexts/LanguageContext';

export default function DoctorantOverview() {
  const { t } = useLanguage();

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-navy dark:text-white">{t('dash.overviewTitle')}</h1>
        <p className="text-text-secondary mt-1">{t('dash.welcomeUser')} Sarah Trabelsi</p>
      </div>

      {/* Stats */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-5">
        <StatCard icon={<FileText size={22} />} count={3} label={t('dash.myPublications')} href="/dashboard/doctorant/publications" color="accent-blue" />
        <StatCard icon={<BookOpen size={22} />} count={6} label={t('pub.labPublications')} href="/dashboard/doctorant/all-publications" color="teal" />
        <StatCard icon={<Target size={22} />} count={5} label={t('dash.researchAxes')} href="/dashboard/doctorant/axes" color="success" />
      </div>

      {/* Thesis & Director */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-5">
        <Card>
          <CardContent className="p-5 text-center space-y-3">
            <div className="w-16 h-16 rounded-full bg-gradient-to-br from-navy to-accent-blue text-white flex items-center justify-center text-2xl font-bold mx-auto">
              AB
            </div>
            <div>
              <h3 className="font-bold text-navy dark:text-white">Dr. Ahmed Ben Salem</h3>
              <Badge variant="info" className="mt-1.5">{t('profile.thesisSupervisor')}</Badge>
            </div>
            <p className="text-xs text-text-secondary">{t('role.researcher')} — Intelligence Artificielle</p>
          </CardContent>
        </Card>

        <div className="lg:col-span-2">
          <Card>
            <CardContent className="p-5 space-y-4">
              <div>
                <h3 className="font-bold text-navy dark:text-white mb-1.5">{t('profile.myThesis')}</h3>
                <p className="text-sm text-text-secondary leading-relaxed">
                  Apprentissage profond pour le diagnostic médical assisté par intelligence artificielle
                </p>
              </div>
              <div className="p-3 bg-light-gray dark:bg-muted rounded-lg text-sm">
                <div className="text-xs text-text-muted mb-0.5">{t('profile.enrollmentLabel')}</div>
                <div className="font-semibold text-navy dark:text-white">2024</div>
              </div>
              <div className="p-3 bg-accent-blue/5 border border-accent-blue/20 rounded-lg text-xs text-text-secondary">
                <strong className="text-navy dark:text-white">{t('profile.attachedAxis')} :</strong> Intelligence Artificielle et Apprentissage Automatique
              </div>
            </CardContent>
          </Card>
        </div>
      </div>

      {/* Recent Publications & Quick Access */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-5">
        <Card>
          <div className="p-5 flex items-center justify-between border-b border-surface-border">
            <h3 className="font-bold text-navy dark:text-white">{t('dash.myRecentPublications')}</h3>
            <Link to="/dashboard/doctorant/publications" className="text-xs text-accent-blue hover:underline flex items-center gap-1">
              {t('dash.viewAll')} <ChevronRight size={14} />
            </Link>
          </div>
          <div className="p-5 space-y-2.5">
            <PubItem title="Medical Image Classification using CNNs" status="SOUMIS" type="Q2" t={t} />
            <PubItem title="Deep Learning for Chest X-Ray Analysis" status="BROUILLON" type="CORE A" t={t} />
            <div className="pt-2 border-t border-surface-border">
              <Link
                to="/dashboard/doctorant/publications"
                className="flex items-center justify-center gap-2 py-2 text-sm text-accent-blue hover:bg-accent-blue/5 rounded-lg transition-colors"
              >
                <FileText size={16} /> {t('dash.createNewPublication')}
              </Link>
            </div>
          </div>
        </Card>

        <Card>
          <div className="p-5 border-b border-surface-border">
            <h3 className="font-bold text-navy dark:text-white">{t('dash.quickAccess')}</h3>
          </div>
          <div className="p-5 space-y-2">
            {[
              { label: t('pub.labPublicationsLong'), href: '/dashboard/doctorant/all-publications', icon: <BookOpen size={16} />, color: 'bg-teal/10 text-teal' },
              { label: t('dash.researchAxes'), href: '/dashboard/doctorant/axes', icon: <Target size={16} />, color: 'bg-success/10 text-success' },
              { label: t('team.labTeam'), href: '/dashboard/doctorant/team', icon: <Users size={16} />, color: 'bg-accent-blue/10 text-accent-blue' },
              { label: t('dash.upcomingEvents'), href: '/dashboard/doctorant/events', icon: <Calendar size={16} />, color: 'bg-warning/10 text-warning' },
            ].map(item => (
              <Link
                key={item.href}
                to={item.href}
                className={`flex items-center justify-between p-3 rounded-lg hover:opacity-80 transition-opacity ${item.color}`}
              >
                <div className="flex items-center gap-2 text-sm font-medium">
                  {item.icon}
                  {item.label}
                </div>
                <ChevronRight size={15} />
              </Link>
            ))}
          </div>
        </Card>
      </div>
    </div>
  );
}

function StatCard({ icon, count, label, href, color }: any) {
  const colorMap: Record<string, string> = {
    'accent-blue': 'bg-accent-blue/10 text-accent-blue',
    'teal': 'bg-teal/10 text-teal',
    'success': 'bg-success/10 text-success',
  };
  return (
    <Link to={href}>
      <Card className="hover:shadow-card-hover transition-shadow cursor-pointer">
        <CardContent className="p-5">
          <div className="flex items-center justify-between">
            <div>
              <div className="text-3xl font-bold text-navy dark:text-white mb-0.5">{count}</div>
              <div className="text-sm text-text-secondary">{label}</div>
            </div>
            <div className={`w-12 h-12 rounded-xl flex items-center justify-center ${colorMap[color]}`}>
              {icon}
            </div>
          </div>
        </CardContent>
      </Card>
    </Link>
  );
}

function PubItem({ title, status, type, t }: any) {
  const getVariant = (s: string) => s === 'PUBLIE' ? 'success' : s === 'SOUMIS' ? 'warning' : 'default';
  const getLabel = (s: string) => s === 'PUBLIE' ? t('pub.published') : s === 'SOUMIS' ? t('pub.underReview') : t('pub.draft');
  return (
    <div className="flex items-center justify-between p-3 bg-light-gray dark:bg-muted rounded-lg gap-3">
      <div className="flex-1 min-w-0">
        <p className="text-sm font-medium text-navy dark:text-white truncate">{title}</p>
        <span className="text-xs text-text-muted">{type}</span>
      </div>
      <Badge variant={getVariant(status)} className="flex-shrink-0 text-xs">{getLabel(status)}</Badge>
    </div>
  );
}
