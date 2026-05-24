import { Link } from 'react-router-dom';
import { Card, CardContent } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { Users, FileText, Calendar, GraduationCap, Target, ChevronRight, Check, X } from 'lucide-react';
import { toast } from 'sonner';
import { useState } from 'react';
import { useLanguage } from '../../../contexts/LanguageContext';

interface PendingPub {
  id: string;
  title: string;
  author: string;
  type: string;
}

const initialPending: PendingPub[] = [
  { id: '1', title: 'Deep Learning for Medical Imaging Analysis', author: 'Dr. Ahmed Ben Salem', type: 'Journal Q2' },
  { id: '2', title: 'Blockchain Security for Healthcare Ecosystems', author: 'Dr. Fatma Gharbi', type: 'Conférence CORE A' },
  { id: '3', title: 'Federated Learning Privacy Mechanisms', author: 'Dr. Mohamed Mezghani', type: 'Journal Q1' },
];

export default function SuperAdminOverview() {
  const { t } = useLanguage();
  const [pending, setPending] = useState(initialPending);

  function handleApprove(id: string, title: string) {
    setPending(p => p.filter(x => x.id !== id));
    toast.success(`Publication "${title}" ${t('pub.approve').toLowerCase()}ée`);
  }

  function handleReject(id: string, title: string) {
    setPending(p => p.filter(x => x.id !== id));
    toast.error(`Publication "${title}" ${t('pub.reject').toLowerCase()}ée`);
  }

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-navy dark:text-white">{t('dash.overviewTitle')}</h1>
        <p className="text-text-secondary mt-1">{t('dash.superadminDashboard')}</p>
      </div>

      <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
        <StatCard icon={<Users size={22} />} count={45} label={t('dash.users')} color="accent-blue" href="/dashboard/superadmin/users" />
        <StatCard icon={<FileText size={22} />} count={156} label={t('nav.publications')} color="teal" href="/dashboard/superadmin/publications" />
        <StatCard icon={<Calendar size={22} />} count={12} label={t('dash.events')} color="success" href="/dashboard/superadmin/events" />
        <StatCard icon={<Target size={22} />} count={5} label={t('dash.researchAxes')} color="warning" href="/dashboard/superadmin/axes" />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-5">
        {/* Pending Publications */}
        <Card>
          <div className="p-5 flex items-center justify-between border-b border-surface-border">
            <h3 className="font-bold text-navy dark:text-white flex items-center gap-2">
              {t('dash.pendingPublications')}
              {pending.length > 0 && (
                <span className="text-xs bg-warning/10 text-warning px-2 py-0.5 rounded-full font-medium">{pending.length}</span>
              )}
            </h3>
            <Link to="/dashboard/superadmin/publications" className="text-xs text-accent-blue hover:underline flex items-center gap-1">
              {t('dash.viewAll')} <ChevronRight size={13} />
            </Link>
          </div>
          <div className="p-5 space-y-3">
            {pending.length === 0 ? (
              <div className="py-8 text-center">
                <div className="bg-accent-blue/5 dark:bg-accent-blue/10 rounded-xl p-6 inline-block">
                  <p className="text-navy dark:text-white font-medium text-sm">{t('dash.noPendingPublications')}</p>
                </div>
              </div>
            ) : (
              pending.map(pub => (
                <div key={pub.id} className="flex items-start gap-3 p-3 bg-light-gray dark:bg-muted rounded-lg">
                  <div className="flex-1 min-w-0">
                    <p className="text-sm font-medium text-navy dark:text-white truncate">{pub.title}</p>
                    <p className="text-xs text-text-secondary">{pub.author} · {pub.type}</p>
                  </div>
                  <div className="flex gap-1.5 flex-shrink-0">
                    <button
                      onClick={() => handleApprove(pub.id, pub.title)}
                      className="p-1.5 bg-success/10 text-success hover:bg-success/20 rounded-lg transition-colors"
                      title={t('pub.approve')}
                    >
                      <Check size={14} />
                    </button>
                    <button
                      onClick={() => handleReject(pub.id, pub.title)}
                      className="p-1.5 bg-error/10 text-error hover:bg-error/20 rounded-lg transition-colors"
                      title={t('pub.reject')}
                    >
                      <X size={14} />
                    </button>
                  </div>
                </div>
              ))
            )}
          </div>
        </Card>

        {/* Upcoming Events */}
        <Card>
          <div className="p-5 flex items-center justify-between border-b border-surface-border">
            <h3 className="font-bold text-navy dark:text-white">{t('dash.upcomingEvents')}</h3>
            <Link to="/dashboard/superadmin/events" className="text-xs text-accent-blue hover:underline flex items-center gap-1">
              {t('dash.viewAll')} <ChevronRight size={13} />
            </Link>
          </div>
          <div className="p-5 space-y-3">
            {[
              { date: '15', month: 'Juin', title: 'Séminaire IA et Santé', participants: 45 },
              { date: '22', month: 'Juin', title: 'Atelier Deep Learning', participants: 30 },
              { date: '05', month: 'Juil', title: 'LIMTIC Research Day 2026', participants: 80 },
            ].map((event, i) => (
              <div key={i} className="flex items-center gap-3 p-3 bg-light-gray dark:bg-muted rounded-lg">
                <div className="w-11 h-11 rounded-xl bg-accent-blue/10 flex flex-col items-center justify-center flex-shrink-0">
                  <span className="text-sm font-bold text-accent-blue leading-none">{event.date}</span>
                  <span className="text-xs text-accent-blue/70 leading-none">{event.month}</span>
                </div>
                <div className="flex-1 min-w-0">
                  <p className="text-sm font-medium text-navy dark:text-white">{event.title}</p>
                  <p className="text-xs text-text-secondary">{event.participants} {t('dash.expectedParticipants')}</p>
                </div>
              </div>
            ))}
          </div>
        </Card>
      </div>

      {/* Recent Activity */}
      <Card>
        <div className="p-5 border-b border-surface-border">
          <h3 className="font-bold text-navy dark:text-white">{t('dash.recentActivity')}</h3>
        </div>
        <div className="p-5 space-y-4">
          {[
            { user: 'Ahmed Ben Salem', initials: 'AB', action: 'a soumis une nouvelle publication pour révision', time: 'Il y a 2h', variant: 'info' },
            { user: 'Fatma Gharbi', initials: 'FG', action: 'a créé un événement "Atelier Sécurité"', time: 'Il y a 5h', variant: 'success' },
            { user: 'Sarah Trabelsi', initials: 'ST', action: 'a rejoint le laboratoire comme doctorant', time: 'Il y a 1j', variant: 'warning' },
            { user: 'Mohamed Mezghani', initials: 'MM', action: 'a mis à jour ses informations de profil', time: 'Il y a 2j', variant: 'default' },
          ].map((item, i) => (
            <div key={i} className="flex items-start gap-3">
              <div className="w-9 h-9 rounded-full bg-gradient-to-br from-navy to-accent-blue text-white flex items-center justify-center text-xs font-bold flex-shrink-0">
                {item.initials}
              </div>
              <div className="flex-1 min-w-0">
                <p className="text-sm text-text-secondary">
                  <span className="font-semibold text-navy dark:text-white">{item.user}</span> {item.action}
                </p>
                <p className="text-xs text-text-muted mt-0.5">{item.time}</p>
              </div>
            </div>
          ))}
        </div>
      </Card>

      {/* Quick Links */}
      <div className="grid grid-cols-2 md:grid-cols-4 gap-3">
        {[
          { label: t('dash.manageUsers'), href: '/dashboard/superadmin/users', color: 'bg-accent-blue/10 text-accent-blue' },
          { label: t('dash.viewStatistics'), href: '/dashboard/superadmin/statistics', color: 'bg-teal/10 text-teal' },
          { label: t('dash.auditLog'), href: '/dashboard/superadmin/audit', color: 'bg-warning/10 text-warning' },
          { label: t('dash.systemSettings'), href: '/dashboard/superadmin/settings', color: 'bg-error/10 text-error' },
        ].map(item => (
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
    'teal': 'bg-teal/10 text-teal',
    'success': 'bg-success/10 text-success',
    'warning': 'bg-warning/10 text-warning',
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
            <div className={`w-11 h-11 rounded-xl flex items-center justify-center ${colorMap[color]}`}>
              {icon}
            </div>
          </div>
        </CardContent>
      </Card>
    </Link>
  );
}
