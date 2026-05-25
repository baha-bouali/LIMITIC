import { Link } from 'react-router-dom';
import { Card, CardContent } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { BookOpen, Target, ChevronRight, Calendar, Users } from 'lucide-react';
import { useLanguage } from '../../../contexts/LanguageContext';
import { useGetAllAxesQuery } from '../../../api/axesApi';
import { useGetPublicUsersQuery } from '../../../api/usersApi';

export default function VisitorOverview() {
  const { t } = useLanguage();
  const { data: axes = [], isLoading: axesLoading } = useGetAllAxesQuery();
  const { data: users = [], isLoading: usersLoading } = useGetPublicUsersQuery({ status: 'active', limit: 1000 });

  const labPublications = axes.reduce((sum, axis) => sum + (axis.publicationsCount ?? 0), 0);
  const featuredAxes = [...axes].filter((axis) => (axis.publicationsCount ?? 0) > 0).sort((a, b) => (b.publicationsCount ?? 0) - (a.publicationsCount ?? 0)).slice(0, 3);

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-navy dark:text-white">{t('dash.overviewTitle')}</h1>
        <p className="text-text-secondary mt-1">{t('dash.welcomeVisitor')}</p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-5">
        <StatCard icon={<BookOpen size={22} />} count={axesLoading ? '...' : labPublications} label={t('pub.labPublications')} href="/dashboard/visitor/all-publications" color="teal" />
        <StatCard icon={<Target size={22} />} count={axesLoading ? '...' : axes.length} label={t('dash.researchAxes')} href="/dashboard/visitor/axes" color="accent-blue" />
        <StatCard icon={<Users size={22} />} count={usersLoading ? '...' : users.length} label={t('team.labMembers')} href="/dashboard/visitor/team" color="success" />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-5">
        <QuickAccessCard basePath="/dashboard/visitor" />

        <Card>
          <div className="p-5 border-b border-surface-border">
            <h3 className="font-bold text-navy dark:text-white">{t('dash.lastLabPublications')}</h3>
          </div>
          <div className="p-5 space-y-2.5">
            {axesLoading ? (
              <EmptyPanel label="Loading..." />
            ) : featuredAxes.length === 0 ? (
              <EmptyPanel label={t('pubPage.noPublications') || 'No publications found'} />
            ) : (
              featuredAxes.map((axis) => <PublicationAxisItem key={axis.id} axis={axis} />)
            )}
            <Link to="/dashboard/visitor/all-publications" className="flex items-center justify-center gap-1.5 pt-2 text-xs text-accent-blue hover:underline">
              {t('dash.viewAllPublications')} <ChevronRight size={13} />
            </Link>
          </div>
        </Card>
      </div>
    </div>
  );
}

function QuickAccessCard({ basePath }: { basePath: string }) {
  const { t } = useLanguage();
  const items = [
    { label: t('pub.labPublicationsLong'), href: `${basePath}/all-publications`, icon: <BookOpen size={16} />, color: 'bg-teal/10 text-teal' },
    { label: t('dash.researchAxes'), href: `${basePath}/axes`, icon: <Target size={16} />, color: 'bg-accent-blue/10 text-accent-blue' },
    { label: t('team.labTeam'), href: `${basePath}/team`, icon: <Users size={16} />, color: 'bg-success/10 text-success' },
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

function PublicationAxisItem({ axis }: any) {
  return (
    <Link to="/dashboard/visitor/all-publications" className="flex items-center gap-3 p-3 bg-light-gray dark:bg-muted rounded-lg hover:opacity-80">
      <div className="flex-1 min-w-0">
        <p className="text-sm font-medium text-navy dark:text-white truncate">{axis.title}</p>
        <span className="text-xs text-text-muted">{axis.description}</span>
      </div>
      <Badge variant="info" className="text-xs flex-shrink-0">{axis.publicationsCount ?? 0}</Badge>
    </Link>
  );
}

function StatCard({ icon, count, label, href, color }: any) {
  const colorMap: Record<string, string> = {
    teal: 'bg-teal/10 text-teal',
    'accent-blue': 'bg-accent-blue/10 text-accent-blue',
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
