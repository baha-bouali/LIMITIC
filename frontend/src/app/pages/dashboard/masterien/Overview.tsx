import { Link } from 'react-router-dom';
import { Card, CardContent } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { BookOpen, Target, ChevronRight, Calendar, Users } from 'lucide-react';
import { useLanguage } from '../../../contexts/LanguageContext';

export default function MasterienOverview() {
  const { t } = useLanguage();

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-navy dark:text-white">{t('dash.overviewTitle')}</h1>
        <p className="text-text-secondary mt-1">{t('dash.welcomeUser')} Ines Hamdi</p>
      </div>

      {/* Stats */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-5">
        <StatCard icon={<BookOpen size={22} />} count={5} label={t('pub.labPublications')} href="/dashboard/masterien/all-publications" color="teal" />
        <StatCard icon={<Target size={22} />} count={5} label={t('dash.researchAxes')} href="/dashboard/masterien/axes" color="accent-blue" />
        <StatCard icon={<Users size={22} />} count={10} label={t('team.labMembers')} href="/dashboard/masterien/team" color="success" />
      </div>

      {/* Encadrant & Mémoire */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-5">
        <Card>
          <CardContent className="p-5 text-center space-y-3">
            <div className="w-16 h-16 rounded-full bg-gradient-to-br from-navy to-accent-blue text-white flex items-center justify-center text-2xl font-bold mx-auto">
              AB
            </div>
            <div>
              <h3 className="font-bold text-navy dark:text-white">Dr. Ahmed Ben Salem</h3>
              <Badge variant="info" className="mt-1.5">{t('profile.supervisor')}</Badge>
            </div>
            <p className="text-xs text-text-secondary">{t('role.researcher')} — Intelligence Artificielle</p>
          </CardContent>
        </Card>

        <div className="lg:col-span-2">
          <Card>
            <CardContent className="p-5 space-y-4">
              <div>
                <h3 className="font-bold text-navy dark:text-white mb-1.5">{t('profile.myProject')}</h3>
                <p className="text-sm text-text-secondary leading-relaxed">
                  Système de recommandation basé sur l'intelligence artificielle pour le e-commerce
                </p>
              </div>
              <div className="p-3 bg-light-gray dark:bg-muted rounded-lg">
                <div className="text-xs text-text-muted mb-0.5">{t('profile.promotionLabel')}</div>
                <div className="font-semibold text-navy dark:text-white">2025-2026</div>
              </div>
              <div className="p-3 bg-accent-blue/5 border border-accent-blue/20 rounded-lg text-xs text-text-secondary">
                <strong className="text-navy dark:text-white">{t('profile.speciality')} :</strong> Intelligence Artificielle et Systèmes Intelligents
              </div>
            </CardContent>
          </Card>
        </div>
      </div>

      {/* Quick Access */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-5">
        <Card>
          <div className="p-5 border-b border-surface-border">
            <h3 className="font-bold text-navy dark:text-white">{t('dash.quickAccess')}</h3>
          </div>
          <div className="p-5 space-y-2">
            {[
              { label: t('pub.labPublicationsLong'), href: '/dashboard/masterien/all-publications', icon: <BookOpen size={16} />, color: 'bg-teal/10 text-teal' },
              { label: t('dash.researchAxes'), href: '/dashboard/masterien/axes', icon: <Target size={16} />, color: 'bg-accent-blue/10 text-accent-blue' },
              { label: t('team.labTeam'), href: '/dashboard/masterien/team', icon: <Users size={16} />, color: 'bg-success/10 text-success' },
              { label: t('dash.upcomingEvents'), href: '/dashboard/masterien/events', icon: <Calendar size={16} />, color: 'bg-warning/10 text-warning' },
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

        <Card>
          <div className="p-5 border-b border-surface-border">
            <h3 className="font-bold text-navy dark:text-white">{t('dash.lastLabPublications')}</h3>
          </div>
          <div className="p-5 space-y-2.5">
            {[
              { title: 'Deep Learning for Medical Imaging Diagnosis', badge: 'Q1', year: 2026 },
              { title: 'Blockchain Security for Healthcare Data', badge: 'CORE A*', year: 2026 },
              { title: 'NLP for Arabic Dialectal Text', badge: 'CORE A', year: 2025 },
            ].map((p, i) => (
              <div key={i} className="flex items-center gap-3 p-3 bg-light-gray dark:bg-muted rounded-lg">
                <div className="flex-1 min-w-0">
                  <p className="text-sm font-medium text-navy dark:text-white truncate">{p.title}</p>
                  <span className="text-xs text-text-muted">{p.year}</span>
                </div>
                <Badge variant="info" className="text-xs flex-shrink-0">{p.badge}</Badge>
              </div>
            ))}
            <Link to="/dashboard/masterien/all-publications" className="flex items-center justify-center gap-1.5 pt-2 text-xs text-accent-blue hover:underline">
              {t('dash.viewAllPublications')} <ChevronRight size={13} />
            </Link>
          </div>
        </Card>
      </div>
    </div>
  );
}

function StatCard({ icon, count, label, href, color }: any) {
  const colorMap: Record<string, string> = {
    'teal': 'bg-teal/10 text-teal',
    'accent-blue': 'bg-accent-blue/10 text-accent-blue',
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
