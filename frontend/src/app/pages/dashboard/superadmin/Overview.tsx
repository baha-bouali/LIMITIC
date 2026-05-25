import { Link } from 'react-router-dom';
import { Card, CardContent } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { Users, FileText, Calendar, Target, ChevronRight } from 'lucide-react';
import { useLanguage } from '../../../contexts/LanguageContext';
import { useGetAuditLogsQuery } from '../../../api/auditLogsApi';
import { useGetAllAxesQuery } from '../../../api/axesApi';
import { useGetEventsQuery } from '../../../api/eventsApi';
import { useGetUsersQuery } from '../../../api/usersApi';
import { useGetDashboardPublicationsQuery } from '../../../api/dashboardPublicationsApi';

export default function SuperAdminOverview() {
  const { t } = useLanguage();
  const fromUtc = new Date(Date.now() - 7 * 24 * 60 * 60 * 1000).toISOString();
  const { data: users = [], isLoading: usersLoading } = useGetUsersQuery({ limit: 1000 });
  const { data: axes = [], isLoading: axesLoading } = useGetAllAxesQuery();
  const { data: events = [], isLoading: eventsLoading } = useGetEventsQuery({ limit: 1000 });
  const { data: auditLogs = [], isLoading: logsLoading } = useGetAuditLogsQuery({ fromUtc });
  const { data: publicationPage, isLoading: publicationsLoading } = useGetDashboardPublicationsQuery({ scope: 'all', limit: 1000 });

  const activeMembers = users.filter((user) => user.isActive !== false).length;
  const publications = publicationPage?.items ?? [];
  const pendingPublications = publications.filter((publication) => publication.status === 'Submitted');
  const upcomingEvents = events
    .filter((event) => event.status === 'A_VENIR')
    .sort((a, b) => new Date(a.startDate).getTime() - new Date(b.startDate).getTime());
  const recentLogs = [...auditLogs]
    .sort((a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime())
    .slice(0, 4);

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-navy dark:text-white">{t('dash.overviewTitle')}</h1>
        <p className="text-text-secondary mt-1">{t('dash.superadminDashboard')}</p>
      </div>

      <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
        <StatCard icon={<Users size={22} />} count={usersLoading ? '...' : activeMembers} label={t('dash.users')} color="accent-blue" href="/dashboard/superadmin/users" />
        <StatCard icon={<FileText size={22} />} count={publicationsLoading ? '...' : publicationPage?.pagination.total ?? publications.length} label={t('nav.publications')} color="teal" href="/dashboard/superadmin/publications" />
        <StatCard icon={<Calendar size={22} />} count={eventsLoading ? '...' : upcomingEvents.length} label={t('dash.events')} color="success" href="/dashboard/superadmin/events" />
        <StatCard icon={<Target size={22} />} count={axesLoading ? '...' : axes.length} label={t('dash.researchAxes')} color="warning" href="/dashboard/superadmin/axes" />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-5">
        <Card>
          <div className="p-5 flex items-center justify-between border-b border-surface-border">
            <h3 className="font-bold text-navy dark:text-white flex items-center gap-2">
              {t('dash.pendingPublications')}
              <Badge variant={pendingPublications.length > 0 ? 'warning' : 'default'}>{publicationsLoading ? '...' : pendingPublications.length}</Badge>
            </h3>
            <Link to="/dashboard/superadmin/publications" className="text-xs text-accent-blue hover:underline flex items-center gap-1">
              {t('dash.viewAll')} <ChevronRight size={13} />
            </Link>
          </div>
          <div className="p-5 space-y-3">
            {publicationsLoading ? (
              <LoadingPanel />
            ) : pendingPublications.length === 0 ? (
              <EmptyPanel label={t('dash.noPendingPublications')} compact />
            ) : (
              pendingPublications.slice(0, 3).map((publication) => (
                <PublicationRow key={publication.id} publication={publication} href="/dashboard/superadmin/publications" />
              ))
            )}
          </div>
        </Card>

        <Card>
          <div className="p-5 flex items-center justify-between border-b border-surface-border">
            <h3 className="font-bold text-navy dark:text-white">{t('dash.upcomingEvents')}</h3>
            <Link to="/dashboard/superadmin/events" className="text-xs text-accent-blue hover:underline flex items-center gap-1">
              {t('dash.viewAll')} <ChevronRight size={13} />
            </Link>
          </div>
          <div className="p-5 space-y-3">
            {eventsLoading ? (
              <LoadingPanel />
            ) : upcomingEvents.length === 0 ? (
              <EmptyPanel label={t('eventPage.noEvents') || 'No events'} compact />
            ) : (
              upcomingEvents.slice(0, 3).map((event) => (
                <EventRow key={event.id} id={event.id} title={event.title} date={event.startDate} type={event.type} />
              ))
            )}
          </div>
        </Card>
      </div>

      <Card>
        <div className="p-5 border-b border-surface-border">
          <h3 className="font-bold text-navy dark:text-white">{t('dash.recentActivity')}</h3>
        </div>
        <div className="p-5 space-y-4">
          {logsLoading ? (
            <LoadingPanel />
          ) : recentLogs.length === 0 ? (
            <EmptyPanel label={t('common.noData') || 'No activity found'} compact />
          ) : (
            recentLogs.map((item) => (
              <div key={item.id} className="flex items-start gap-3">
                <div className="w-9 h-9 rounded-full bg-gradient-to-br from-navy to-accent-blue text-white flex items-center justify-center text-xs font-bold flex-shrink-0">
                  {getInitials(item.actorName)}
                </div>
                <div className="flex-1 min-w-0">
                  <p className="text-sm text-text-secondary">
                    <span className="font-semibold text-navy dark:text-white">{item.actorName || item.actorId}</span>{' '}
                    {item.action} {item.resource}
                  </p>
                  <p className="text-xs text-text-muted mt-0.5">{formatDateTime(item.timestamp)}</p>
                </div>
              </div>
            ))
          )}
        </div>
      </Card>

      <div className="grid grid-cols-2 md:grid-cols-4 gap-3">
        {[
          { label: t('dash.manageUsers'), href: '/dashboard/superadmin/users', color: 'bg-accent-blue/10 text-accent-blue' },
          { label: t('dash.viewStatistics'), href: '/dashboard/superadmin/statistics', color: 'bg-teal/10 text-teal' },
          { label: t('dash.auditLog'), href: '/dashboard/superadmin/audit', color: 'bg-warning/10 text-warning' },
          { label: t('dash.systemSettings'), href: '/dashboard/superadmin/settings', color: 'bg-error/10 text-error' },
        ].map((item) => (
          <Link
            key={item.href}
            to={item.href}
            className={`flex items-center justify-between p-3 rounded-xl hover:opacity-80 transition-opacity text-sm font-medium ${item.color}`}
          >
            {item.label}
            <ChevronRight size={15} />
          </Link>
        ))}
      </div>
    </div>
  );
}

function StatCard({ icon, count, label, color, href }: any) {
  const colorMap: Record<string, string> = {
    'accent-blue': 'bg-accent-blue/10 text-accent-blue',
    teal: 'bg-teal/10 text-teal',
    success: 'bg-success/10 text-success',
    warning: 'bg-warning/10 text-warning',
  };

  return (
    <Link to={href}>
      <Card className="hover:shadow-card-hover transition-shadow cursor-pointer">
        <CardContent className="p-5">
          <div className="flex items-center justify-between">
            <div>
              <div className="text-2xl font-bold text-navy dark:text-white mb-0.5">{count}</div>
              <div className="text-xs text-text-secondary">{label}</div>
            </div>
            <div className={`w-11 h-11 rounded-xl flex items-center justify-center ${colorMap[color]}`}>{icon}</div>
          </div>
        </CardContent>
      </Card>
    </Link>
  );
}

function EventRow({ id, title, date, type }: any) {
  const parsed = new Date(date);

  return (
    <Link to={`/dashboard/superadmin/events`} className="flex items-center gap-3 p-3 bg-light-gray dark:bg-muted rounded-lg hover:opacity-80">
      <div className="w-11 h-11 rounded-xl bg-accent-blue/10 flex flex-col items-center justify-center flex-shrink-0">
        <span className="text-sm font-bold text-accent-blue leading-none">{parsed.toLocaleDateString('fr-FR', { day: '2-digit' })}</span>
        <span className="text-xs text-accent-blue/70 leading-none">{parsed.toLocaleDateString('fr-FR', { month: 'short' })}</span>
      </div>
      <div className="flex-1 min-w-0">
        <p className="text-sm font-medium text-navy dark:text-white truncate">{title}</p>
        <p className="text-xs text-text-secondary">{type}</p>
      </div>
    </Link>
  );
}

function PublicationRow({ publication, href }: any) {
  return (
    <Link to={href} className="flex items-start gap-3 p-3 bg-light-gray dark:bg-muted rounded-lg hover:opacity-80">
      <div className="flex-1 min-w-0">
        <p className="text-sm font-medium text-navy dark:text-white truncate">{publication.title}</p>
        <p className="text-xs text-text-secondary">
          {[publication.submittedBy, publication.axe?.title, publication.year].filter(Boolean).join(' · ')}
        </p>
      </div>
      <Badge variant="warning" className="flex-shrink-0">Soumis</Badge>
    </Link>
  );
}

function LoadingPanel() {
  return <div className="py-6 text-center text-sm text-text-secondary">Loading...</div>;
}

function EmptyPanel({ label, compact = false }: { label: string; compact?: boolean }) {
  return (
    <div className={compact ? 'py-6 text-center text-sm text-text-secondary' : 'p-5 py-8 text-center text-sm text-text-secondary'}>
      {label}
    </div>
  );
}

function getInitials(name?: string | null) {
  if (!name) return '?';
  return name.split(' ').filter(Boolean).slice(0, 2).map((part) => part[0]?.toUpperCase()).join('');
}

function formatDateTime(value: string) {
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return value;
  return date.toLocaleString('fr-FR', { dateStyle: 'short', timeStyle: 'short' });
}
