import { skipToken } from '@reduxjs/toolkit/query';
import { Link } from 'react-router-dom';
import { Card, CardContent } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { FileText, BookOpen, Target, ChevronRight, Calendar, Users } from 'lucide-react';
import { useAuth } from '../../../contexts/AuthContext';
import { useLanguage } from '../../../contexts/LanguageContext';
import { useGetAllAxesQuery } from '../../../api/axesApi';
import { useGetPhDStudentProfileQuery } from '../../../api/profilesApi';

export default function DoctorantOverview() {
  const { t } = useLanguage();
  const { user } = useAuth();
  const { data: profile } = useGetPhDStudentProfileQuery(user?.id ?? skipToken);
  const { data: axes = [], isLoading: axesLoading } = useGetAllAxesQuery();

  const fullName = [profile?.firstName ?? user?.firstName, profile?.lastName ?? user?.lastName].filter(Boolean).join(' ');
  const labPublications = axes.reduce((sum, axis) => sum + (axis.publicationsCount ?? 0), 0);
  const attachedAxis = profile?.researchAxes?.[0];

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-navy dark:text-white">{t('dash.overviewTitle')}</h1>
        <p className="text-text-secondary mt-1">{t('dash.welcomeUser')} {fullName}</p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-5">
        <StatCard icon={<FileText size={22} />} count={0} label={t('dash.myPublications')} href="/dashboard/doctorant/publications" color="accent-blue" />
        <StatCard icon={<BookOpen size={22} />} count={axesLoading ? '...' : labPublications} label={t('pub.labPublications')} href="/dashboard/doctorant/all-publications" color="teal" />
        <StatCard icon={<Target size={22} />} count={axesLoading ? '...' : axes.length} label={t('dash.researchAxes')} href="/dashboard/doctorant/axes" color="success" />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-5">
        <Card>
          <CardContent className="p-5 text-center space-y-3">
            <div className="w-16 h-16 rounded-full bg-gradient-to-br from-navy to-accent-blue text-white flex items-center justify-center text-2xl font-bold mx-auto">
              {getInitials(profile?.supervisorName)}
            </div>
            <div>
              <h3 className="font-bold text-navy dark:text-white">{profile?.supervisorName || t('axes.notAssigned')}</h3>
              <Badge variant="info" className="mt-1.5">{t('profile.thesisSupervisor')}</Badge>
            </div>
            <p className="text-xs text-text-secondary">{t('role.researcher')}</p>
          </CardContent>
        </Card>

        <div className="lg:col-span-2">
          <Card>
            <CardContent className="p-5 space-y-4">
              <div>
                <h3 className="font-bold text-navy dark:text-white mb-1.5">{t('profile.myThesis')}</h3>
                <p className="text-sm text-text-secondary leading-relaxed">{profile?.thesisSubject || t('common.noData')}</p>
              </div>
              <div className="p-3 bg-light-gray dark:bg-muted rounded-lg text-sm">
                <div className="text-xs text-text-muted mb-0.5">{t('profile.enrollmentLabel')}</div>
                <div className="font-semibold text-navy dark:text-white">{profile?.enrollmentYear || t('common.noData')}</div>
              </div>
              <div className="p-3 bg-accent-blue/5 border border-accent-blue/20 rounded-lg text-xs text-text-secondary">
                <strong className="text-navy dark:text-white">{t('profile.attachedAxis')} :</strong> {attachedAxis?.title || t('axes.notAssigned')}
              </div>
            </CardContent>
          </Card>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-5">
        <Card>
          <div className="p-5 flex items-center justify-between border-b border-surface-border">
            <h3 className="font-bold text-navy dark:text-white">{t('dash.myRecentPublications')}</h3>
            <Link to="/dashboard/doctorant/publications" className="text-xs text-accent-blue hover:underline flex items-center gap-1">
              {t('dash.viewAll')} <ChevronRight size={14} />
            </Link>
          </div>
          <div className="p-5">
            <EmptyPanel label={t('pub.noPublications') || t('pubPage.noPublications') || 'No publications found'} />
          </div>
        </Card>

        <QuickAccessCard basePath="/dashboard/doctorant" />
      </div>
    </div>
  );
}

function QuickAccessCard({ basePath }: { basePath: string }) {
  const { t } = useLanguage();
  const items = [
    { label: t('pub.labPublicationsLong'), href: `${basePath}/all-publications`, icon: <BookOpen size={16} />, color: 'bg-teal/10 text-teal' },
    { label: t('dash.researchAxes'), href: `${basePath}/axes`, icon: <Target size={16} />, color: 'bg-success/10 text-success' },
    { label: t('team.labTeam'), href: `${basePath}/team`, icon: <Users size={16} />, color: 'bg-accent-blue/10 text-accent-blue' },
    { label: t('dash.upcomingEvents'), href: `${basePath}/events`, icon: <Calendar size={16} />, color: 'bg-warning/10 text-warning' },
  ];

  return (
    <Card>
      <div className="p-5 border-b border-surface-border">
        <h3 className="font-bold text-navy dark:text-white">{t('dash.quickAccess')}</h3>
      </div>
      <div className="p-5 space-y-2">
        {items.map((item) => (
          <Link key={item.href} to={item.href} className={`flex items-center justify-between p-3 rounded-lg hover:opacity-80 transition-opacity ${item.color}`}>
            <div className="flex items-center gap-2 text-sm font-medium">
              {item.icon}
              {item.label}
            </div>
            <ChevronRight size={15} />
          </Link>
        ))}
      </div>
    </Card>
  );
}

function StatCard({ icon, count, label, href, color }: any) {
  const colorMap: Record<string, string> = {
    'accent-blue': 'bg-accent-blue/10 text-accent-blue',
    teal: 'bg-teal/10 text-teal',
    success: 'bg-success/10 text-success',
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
            <div className={`w-12 h-12 rounded-xl flex items-center justify-center ${colorMap[color]}`}>{icon}</div>
          </div>
        </CardContent>
      </Card>
    </Link>
  );
}

function EmptyPanel({ label }: { label: string }) {
  return <div className="py-6 text-center text-sm text-text-secondary">{label}</div>;
}

function getInitials(name?: string | null) {
  if (!name) return '?';
  return name.split(' ').filter(Boolean).slice(0, 2).map((part) => part[0]?.toUpperCase()).join('');
}
