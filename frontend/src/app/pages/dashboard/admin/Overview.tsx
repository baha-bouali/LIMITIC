import { Card, CardContent } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { Button } from '../../../components/ui/Button';
import { FileText, Calendar, Users, Target, Clock, TrendingUp, ArrowRight } from 'lucide-react';
import { Link } from 'react-router-dom';
import { useLanguage } from '../../../contexts/LanguageContext';
import { useGetAllAxesQuery } from '../../../api/axesApi';
import { useGetEventsQuery } from '../../../api/eventsApi';
import { useGetUsersQuery } from '../../../api/usersApi';
import { useGetDashboardPublicationsQuery } from '../../../api/dashboardPublicationsApi';

export default function AdminOverview() {
  const { t } = useLanguage();
  const { data: axes = [], isLoading: axesLoading } = useGetAllAxesQuery();
  const { data: events = [], isLoading: eventsLoading } = useGetEventsQuery({ limit: 1000 });
  const { data: users = [], isLoading: usersLoading } = useGetUsersQuery({ status: 'active', limit: 1000 });
  const { data: publicationPage, isLoading: publicationsLoading } = useGetDashboardPublicationsQuery({ scope: 'all', limit: 1000 });

  const activeMembers = users.filter((user) => user.isActive !== false).length;
  const publications = publicationPage?.items ?? [];
  const pendingPublications = publications.filter((publication) => publication.status === 'Submitted');
  const upcomingEvents = events
    .filter((event) => event.status === 'A_VENIR')
    .sort((a, b) => new Date(a.startDate).getTime() - new Date(b.startDate).getTime());

  const stats = [
    { label: t('dash.pendingPublicationsCount'), value: publicationsLoading ? '...' : pendingPublications.length, icon: Clock, color: 'bg-warning/10 text-warning', link: '/dashboard/admin/publications' },
    { label: t('dash.upcomingEvents'), value: eventsLoading ? '...' : upcomingEvents.length, icon: Calendar, color: 'bg-accent-blue/10 text-accent-blue', link: '/dashboard/admin/events' },
    { label: t('dash.activeMembers'), value: usersLoading ? '...' : activeMembers, icon: Users, color: 'bg-teal/10 text-teal', link: '/dashboard/admin/users' },
    { label: t('dash.researchAxes'), value: axesLoading ? '...' : axes.length, icon: Target, color: 'bg-success/10 text-success', link: '/dashboard/admin/axes' },
  ];

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-navy dark:text-white">{t('dash.adminDashboard')}</h1>
        <p className="text-text-secondary mt-1">{t('dash.manageLabContent')}</p>
      </div>

      <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
        {stats.map((stat) => {
          const Icon = stat.icon;
          return (
            <Link to={stat.link} key={stat.label}>
              <Card className="hover:shadow-card-hover transition-shadow cursor-pointer">
                <CardContent className="p-5">
                  <div className="flex items-center justify-between mb-3">
                    <div className={`w-10 h-10 rounded-xl flex items-center justify-center ${stat.color}`}>
                      <Icon size={20} />
                    </div>
                    <TrendingUp size={16} className="text-text-muted" />
                  </div>
                  <div className="text-2xl font-bold text-navy dark:text-white">{stat.value}</div>
                  <div className="text-sm text-text-secondary mt-1">{stat.label}</div>
                </CardContent>
              </Card>
            </Link>
          );
        })}
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <Card>
          <CardContent className="p-6">
            <div className="flex items-center justify-between mb-5">
              <h2 className="text-lg font-bold text-navy dark:text-white flex items-center gap-2">
                <FileText size={20} className="text-accent-blue" />
                {t('dash.pendingPublications')}
                <Badge variant={pendingPublications.length > 0 ? 'warning' : 'default'}>{publicationsLoading ? '...' : pendingPublications.length}</Badge>
              </h2>
              <Link to="/dashboard/admin/publications" className="text-sm text-accent-blue hover:underline flex items-center gap-1">
                {t('dash.viewAll')} <ArrowRight size={14} />
              </Link>
            </div>
            <div className="space-y-3">
              {publicationsLoading ? (
                <LoadingPanel />
              ) : pendingPublications.length === 0 ? (
                <EmptyPanel label={t('dash.noPendingPublications')} />
              ) : (
                pendingPublications.slice(0, 3).map((publication) => (
                  <Link key={publication.id} to="/dashboard/admin/publications" className="flex items-start gap-3 p-3 bg-light-gray dark:bg-muted rounded-xl hover:opacity-80">
                    <div className="flex-1 min-w-0">
                      <p className="font-medium text-sm text-navy dark:text-white truncate">{publication.title}</p>
                      <p className="text-xs text-text-muted mt-0.5">
                        {[publication.submittedBy, publication.axe?.title, publication.year].filter(Boolean).join(' · ')}
                      </p>
                    </div>
                    <Badge variant="warning" className="flex-shrink-0">Soumis</Badge>
                  </Link>
                ))
              )}
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-6">
            <div className="flex items-center justify-between mb-5">
              <h2 className="text-lg font-bold text-navy dark:text-white flex items-center gap-2">
                <Calendar size={20} className="text-teal" />
                {t('dash.upcomingEvents')}
              </h2>
              <Link to="/dashboard/admin/events" className="text-sm text-accent-blue hover:underline flex items-center gap-1">
                {t('dash.viewAll')} <ArrowRight size={14} />
              </Link>
            </div>
            <div className="space-y-3">
              {eventsLoading ? (
                <LoadingPanel />
              ) : upcomingEvents.length === 0 ? (
                <EmptyPanel label={t('eventPage.noEvents') || 'No events'} />
              ) : (
                upcomingEvents.slice(0, 3).map((event) => (
                  <Link key={event.id} to={`/dashboard/admin/events`} className="flex items-center gap-3 p-3 bg-light-gray dark:bg-muted rounded-xl hover:opacity-80">
                    <div className="w-12 h-12 rounded-xl bg-gradient-to-br from-navy to-accent-blue flex flex-col items-center justify-center text-white flex-shrink-0">
                      <div className="text-xs font-bold leading-none">{formatDay(event.startDate)}</div>
                      <div className="text-xs opacity-80">{formatMonth(event.startDate)}</div>
                    </div>
                    <div className="flex-1 min-w-0">
                      <p className="font-medium text-sm text-navy dark:text-white truncate">{event.title}</p>
                      <Badge variant="info" className="text-xs mt-1">{event.type}</Badge>
                    </div>
                  </Link>
                ))
              )}
            </div>
          </CardContent>
        </Card>
      </div>

      <Card>
        <CardContent className="p-6">
          <h2 className="text-lg font-bold text-navy dark:text-white mb-4">{t('dash.quickActions')}</h2>
          <div className="flex flex-wrap gap-3">
            <Link to="/dashboard/admin/publications">
              <Button className="flex items-center gap-2"><FileText size={16} /> {t('dash.managePublications')}</Button>
            </Link>
            <Link to="/dashboard/admin/events">
              <Button variant="outlined" className="flex items-center gap-2"><Calendar size={16} /> {t('dash.manageEvents')}</Button>
            </Link>
            <Link to="/dashboard/admin/users">
              <Button variant="outlined" className="flex items-center gap-2"><Users size={16} /> {t('dash.manageMembers')}</Button>
            </Link>
            <Link to="/dashboard/admin/axes">
              <Button variant="outlined" className="flex items-center gap-2"><Target size={16} /> {t('dash.manageAxes')}</Button>
            </Link>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}

function LoadingPanel() {
  return <div className="py-6 text-center text-sm text-text-secondary">Loading...</div>;
}

function EmptyPanel({ label }: { label: string }) {
  return <div className="py-6 text-center text-sm text-text-secondary">{label}</div>;
}

function formatDay(value: string) {
  return new Date(value).toLocaleDateString('fr-FR', { day: '2-digit' });
}

function formatMonth(value: string) {
  return new Date(value).toLocaleDateString('fr-FR', { month: 'short' });
}
