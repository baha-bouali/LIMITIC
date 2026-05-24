import { Card, CardContent } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { Button } from '../../../components/ui/Button';
import { FileText, Calendar, Users, Target, CheckCircle, XCircle, Clock, TrendingUp, ArrowRight } from 'lucide-react';
import { Link } from 'react-router-dom';
import { useLanguage } from '../../../contexts/LanguageContext';

const pendingPublications = [
  { id: '1', title: 'Blockchain Security Analysis for Healthcare Data', author: 'Mohamed Najjar', type: 'CONFÉRENCE INT.', submitted: 'Il y a 2h' },
  { id: '2', title: 'Deep Learning for MRI Segmentation', author: 'Sarah Trabelsi', type: 'ARTICLE JOURNAL', submitted: 'Il y a 5h' },
  { id: '3', title: 'Federated Privacy-Preserving Models', author: 'Dr. Ahmed Ben Salem', type: 'ARTICLE JOURNAL', submitted: 'Il y a 1j' },
];

const upcomingEvents = [
  { id: '1', title: 'Séminaire IA & Santé Numérique', date: '26 Mai 2026', type: 'Séminaire', status: 'A_VENIR' },
  { id: '2', title: 'Soutenance de thèse — Sarah Trabelsi', date: '15 Juin 2026', type: 'Soutenance', status: 'A_VENIR' },
  { id: '3', title: 'Journée Portes Ouvertes LIMTIC', date: '10 Juillet 2026', type: 'Conférence', status: 'A_VENIR' },
];

export default function AdminOverview() {
  const { t } = useLanguage();

  return (
    <div className="space-y-6">
      {/* Header */}
      <div>
        <h1 className="text-3xl font-bold text-navy dark:text-white">{t('dash.adminDashboard')}</h1>
        <p className="text-text-secondary mt-1">{t('dash.manageLabContent')}</p>
      </div>

      {/* Stats */}
      <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
        {[
          { label: t('dash.pendingPublicationsCount'), value: 3, icon: Clock, color: 'bg-warning/10 text-warning', link: '/dashboard/admin/publications' },
          { label: t('dash.upcomingEvents'), value: 5, icon: Calendar, color: 'bg-accent-blue/10 text-accent-blue', link: '/dashboard/admin/events' },
          { label: t('dash.activeMembers'), value: 34, icon: Users, color: 'bg-teal/10 text-teal', link: '/dashboard/admin/members' },
          { label: t('dash.researchAxes'), value: 5, icon: Target, color: 'bg-success/10 text-success', link: '/dashboard/admin/axes' },
        ].map(stat => {
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
        {/* Pending Publications */}
        <Card>
          <CardContent className="p-6">
            <div className="flex items-center justify-between mb-5">
              <h2 className="text-lg font-bold text-navy dark:text-white flex items-center gap-2">
                <FileText size={20} className="text-accent-blue" />
                {t('dash.pendingPublications')}
                <Badge variant="warning">{pendingPublications.length}</Badge>
              </h2>
              <Link to="/dashboard/admin/publications" className="text-sm text-accent-blue hover:underline flex items-center gap-1">
                {t('dash.viewAll')} <ArrowRight size={14} />
              </Link>
            </div>
            <div className="space-y-3">
              {pendingPublications.map(pub => (
                <div key={pub.id} className="flex items-start gap-3 p-3 bg-light-gray dark:bg-muted rounded-xl">
                  <div className="flex-1 min-w-0">
                    <p className="font-medium text-sm text-navy dark:text-white truncate">{pub.title}</p>
                    <p className="text-xs text-text-muted mt-0.5">{pub.author} · {pub.type} · {pub.submitted}</p>
                  </div>
                  <div className="flex gap-1 flex-shrink-0">
                    <button className="p-1.5 bg-success/10 text-success rounded-lg hover:bg-success/20 transition-colors" title={t('pub.approve')}>
                      <CheckCircle size={16} />
                    </button>
                    <button className="p-1.5 bg-error/10 text-error rounded-lg hover:bg-error/20 transition-colors" title={t('pub.reject')}>
                      <XCircle size={16} />
                    </button>
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>

        {/* Upcoming Events */}
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
              {upcomingEvents.map(evt => (
                <div key={evt.id} className="flex items-center gap-3 p-3 bg-light-gray dark:bg-muted rounded-xl">
                  <div className="w-12 h-12 rounded-xl bg-gradient-to-br from-navy to-accent-blue flex flex-col items-center justify-center text-white flex-shrink-0">
                    <div className="text-xs font-bold leading-none">{evt.date.split(' ')[0]}</div>
                    <div className="text-xs opacity-80">{evt.date.split(' ')[1].slice(0, 3)}</div>
                  </div>
                  <div className="flex-1 min-w-0">
                    <p className="font-medium text-sm text-navy dark:text-white">{evt.title}</p>
                    <Badge variant="info" className="text-xs mt-1">{evt.type}</Badge>
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Quick actions */}
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
            <Link to="/dashboard/admin/members">
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
