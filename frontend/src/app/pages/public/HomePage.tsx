import { useEffect, useState } from 'react';
import type { ReactNode } from 'react';
import { Link } from 'react-router-dom';
import { PublicNavbar } from '../../components/layout/PublicNavbar';
import { PublicFooter } from '../../components/layout/PublicFooter';
import { Button } from '../../components/ui/Button';
import { Card, CardContent, CardHeader } from '../../components/ui/Card';
import { Badge } from '../../components/ui/Badge';
import { ArrowRight, Users, FileText, GraduationCap, Calendar, ChevronDown, MapPin, Loader2 } from 'lucide-react';
import { motion } from 'motion/react';
import { useLanguage } from '../../contexts/LanguageContext';
import { useGetAllAxesQuery } from '../../api/axesApi';
import { useGetEventsQuery } from '../../api/eventsApi';
import { useGetPublicUsersQuery } from '../../api/usersApi';

export default function HomePage() {
  const { t } = useLanguage();
  const { data: axes = [], isLoading: axesLoading } = useGetAllAxesQuery();
  const { data: events = [], isLoading: eventsLoading } = useGetEventsQuery({ limit: 1000 });
  const { data: users = [], isLoading: usersLoading } = useGetPublicUsersQuery({ status: 'active', limit: 1000 });

  const researchersCount = users.filter((user) => user.role === 3).length;
  const phdCount = users.filter((user) => user.role === 4).length;
  const publicationsCount = axes.reduce((sum, axis) => sum + (axis.publicationsCount ?? 0), 0);
  const upcomingEvents = events
    .filter((event) => event.status === 'A_VENIR')
    .sort((a, b) => new Date(a.startDate).getTime() - new Date(b.startDate).getTime());
  const featuredAxes = axes.slice(0, 3);
  const featuredPublicationAxes = [...axes]
    .filter((axis) => (axis.publicationsCount ?? 0) > 0)
    .sort((a, b) => (b.publicationsCount ?? 0) - (a.publicationsCount ?? 0))
    .slice(0, 3);
  const isStatsLoading = axesLoading || eventsLoading || usersLoading;

  return (
    <div className="min-h-screen bg-white dark:bg-background">
      <PublicNavbar />

      <section className="relative h-screen flex items-center justify-center overflow-hidden">
        <div className="absolute inset-0 brand-gradient-diagonal">
          <div className="absolute inset-0 opacity-10">
            <svg className="w-full h-full">
              <defs>
                <pattern id="grid" width="50" height="50" patternUnits="userSpaceOnUse">
                  <circle cx="25" cy="25" r="1" fill="white" />
                </pattern>
              </defs>
              <rect width="100%" height="100%" fill="url(#grid)" />
            </svg>
          </div>
        </div>

        <div className="relative z-10 max-w-5xl mx-auto px-6 text-center text-white">
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.8 }}
            className="space-y-8"
          >
            <h1 className="text-5xl md:text-6xl font-bold leading-tight">{t('home.hero.title')}</h1>
            <p className="text-xl md:text-2xl text-white/90 max-w-3xl mx-auto">{t('home.hero.subtitle')}</p>
            <div className="flex flex-wrap items-center justify-center gap-4 pt-6">
              <Link to="/publications">
                <Button size="lg" variant="outlined" className="!text-white !border-white hover:!bg-white/10">
                  {t('home.hero.publications')}
                  <ArrowRight size={20} />
                </Button>
              </Link>
              <Link to="/equipe">
                <Button size="lg" variant="outlined" className="!text-white !border-white hover:!bg-white/10">
                  {t('home.hero.team')}
                </Button>
              </Link>
              <Link to="/contact">
                <Button size="lg" variant="ghost" className="!text-white hover:!bg-white/10">
                  {t('home.hero.contact')}
                </Button>
              </Link>
            </div>
          </motion.div>
        </div>

        <motion.div
          className="absolute bottom-8 left-1/2 transform -translate-x-1/2"
          animate={{ y: [0, 10, 0] }}
          transition={{ duration: 1.5, repeat: Infinity }}
        >
          <ChevronDown size={32} className="text-white/80" />
        </motion.div>
      </section>

      <section className="bg-navy text-white py-16">
        <div className="max-w-[var(--content-max-width)] mx-auto px-6">
          <div className="grid grid-cols-1 md:grid-cols-4 gap-8">
            <StatCounter icon={<Users />} count={isStatsLoading ? 0 : researchersCount} label={t('home.stats.researchers')} />
            <StatCounter icon={<FileText />} count={isStatsLoading ? 0 : publicationsCount} label={t('home.stats.publications')} />
            <StatCounter icon={<GraduationCap />} count={isStatsLoading ? 0 : phdCount} label={t('home.stats.phd')} />
            <StatCounter icon={<Calendar />} count={isStatsLoading ? 0 : upcomingEvents.length} label={t('home.stats.events')} />
          </div>
        </div>
      </section>

      <section className="bg-light-gray py-20">
        <div className="max-w-[var(--content-max-width)] mx-auto px-6">
          <div className="text-center mb-12">
            <h2 className="text-4xl font-bold text-navy dark:text-white mb-4">{t('home.axes.title')}</h2>
            <p className="text-text-secondary text-lg">{t('home.axes.subtitle')}</p>
          </div>
          {axesLoading ? (
            <LoadingBlock label={t('common.loading') || 'Loading'} />
          ) : featuredAxes.length === 0 ? (
            <EmptyBlock label={t('axes.noAxesFoundGeneric') || 'No research areas found'} />
          ) : (
            <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
              {featuredAxes.map((axis) => (
                <AxeCard
                  key={axis.id}
                  id={axis.id}
                  title={axis.title}
                  description={axis.description}
                  responsible={axis.responsibleName || t('axes.notAssigned')}
                  themes={axis.themes || []}
                  color={axis.color || 'accent-blue'}
                />
              ))}
            </div>
          )}
        </div>
      </section>

      <section className="bg-white dark:bg-background py-20">
        <div className="max-w-[var(--content-max-width)] mx-auto px-6">
          <div className="flex items-center justify-between mb-12">
            <h2 className="text-4xl font-bold text-navy dark:text-white">{t('home.publications.title')}</h2>
            <Link to="/publications" className="flex items-center gap-2 text-accent-blue hover:underline">
              {t('home.viewAll')} <ArrowRight size={20} />
            </Link>
          </div>
          {axesLoading ? (
            <LoadingBlock label={t('common.loading') || 'Loading'} />
          ) : featuredPublicationAxes.length === 0 ? (
            <EmptyBlock label={t('pubPage.noPublications') || 'No publications found'} />
          ) : (
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
              {featuredPublicationAxes.map((axis) => (
                <PublicationAxisCard key={axis.id} axis={axis} />
              ))}
            </div>
          )}
        </div>
      </section>

      <section className="bg-off-white py-20">
        <div className="max-w-[var(--content-max-width)] mx-auto px-6">
          <div className="flex items-center justify-between mb-12">
            <h2 className="text-4xl font-bold text-navy dark:text-white">{t('home.events.title')}</h2>
            <Link to="/evenements" className="flex items-center gap-2 text-accent-blue hover:underline">
              {t('home.viewAllEvents')} <ArrowRight size={20} />
            </Link>
          </div>
          {eventsLoading ? (
            <LoadingBlock label={t('common.loading') || 'Loading'} />
          ) : upcomingEvents.length === 0 ? (
            <EmptyBlock label={t('eventPage.noEvents') || 'No events'} />
          ) : (
            <div className="space-y-4">
              {upcomingEvents.slice(0, 3).map((event) => (
                <EventItem
                  key={event.id}
                  id={event.id}
                  type={event.type}
                  date={formatEventDate(event.startDate)}
                  title={event.title}
                  location={event.location}
                />
              ))}
            </div>
          )}
        </div>
      </section>

      <PublicFooter />
    </div>
  );
}

function LoadingBlock({ label }: { label: string }) {
  return (
    <Card>
      <CardContent className="p-10 flex items-center justify-center gap-3 text-text-secondary">
        <Loader2 className="animate-spin text-accent-blue" size={22} />
        <span>{label}</span>
      </CardContent>
    </Card>
  );
}

function EmptyBlock({ label }: { label: string }) {
  return (
    <Card>
      <CardContent className="p-10 text-center text-text-secondary">{label}</CardContent>
    </Card>
  );
}

function formatEventDate(date: string) {
  const parsed = new Date(date);

  if (Number.isNaN(parsed.getTime())) {
    return { day: '--', month: '' };
  }

  return {
    day: parsed.toLocaleDateString('fr-FR', { day: '2-digit' }),
    month: parsed.toLocaleDateString('fr-FR', { month: 'short' }),
  };
}

function StatCounter({ icon, count, label }: { icon: ReactNode; count: number; label: string }) {
  const [displayCount, setDisplayCount] = useState(0);

  useEffect(() => {
    let start = 0;
    const end = count;
    const duration = 2000;
    const increment = end / (duration / 16);

    const timer = setInterval(() => {
      start += increment;
      if (start >= end) {
        setDisplayCount(end);
        clearInterval(timer);
      } else {
        setDisplayCount(Math.floor(start));
      }
    }, 16);

    return () => clearInterval(timer);
  }, [count]);

  return (
    <div className="text-center">
      <div className="flex items-center justify-center mb-4">
        <div className="w-16 h-16 flex items-center justify-center bg-white/10 rounded-full">{icon}</div>
      </div>
      <div className="text-5xl font-bold mb-2">{displayCount}</div>
      <div className="text-white/80">{label}</div>
    </div>
  );
}

function AxeCard({ id, title, description, responsible, themes, color }: any) {
  const { t } = useLanguage();

  return (
    <Card className="hover:shadow-lg transition-shadow overflow-hidden">
      <div className={`h-1 bg-${color}`} />
      <CardHeader>
        <h3 className="text-xl font-bold text-navy dark:text-white mb-2">{title}</h3>
        <p className="text-text-secondary text-sm">{description}</p>
      </CardHeader>
      <CardContent className="space-y-4">
        <div className="flex flex-wrap gap-2">
          {themes.slice(0, 4).map((theme: string, idx: number) => (
            <Badge key={idx} variant="default">{theme}</Badge>
          ))}
        </div>
        <div className="flex items-center gap-3 pt-4 border-t border-surface-border">
          <div className="w-8 h-8 rounded-full bg-navy text-white flex items-center justify-center text-sm font-bold">
            {responsible.charAt(0)}
          </div>
          <div className="text-sm text-text-secondary">{responsible}</div>
        </div>
        <Link to={`/axes-recherche/${id}`} className="flex items-center gap-2 text-accent-blue hover:underline text-sm">
          {t('home.axes.viewAxis')} <ArrowRight size={16} />
        </Link>
      </CardContent>
    </Card>
  );
}

function PublicationAxisCard({ axis }: any) {
  const { t } = useLanguage();

  return (
    <Card className="hover:shadow-lg transition-shadow">
      <CardHeader>
        <div className="flex items-center gap-2 mb-3">
          <Badge variant="info">{axis.publicationsCount ?? 0} {t('axes.publicationsCount')}</Badge>
        </div>
        <h4 className="text-base font-bold text-navy dark:text-white line-clamp-2 mb-2">{axis.title}</h4>
        <p className="text-sm text-text-secondary line-clamp-3">{axis.description}</p>
      </CardHeader>
      <CardContent>
        <div className="flex items-center justify-between">
          <Link to={`/axes-recherche/${axis.id}`} className="text-accent-blue hover:underline text-sm">
            {t('home.publications.read')} <ArrowRight size={14} className="inline" />
          </Link>
          <Link to={`/publications?axe=${axis.id}`} className="text-accent-blue hover:underline text-sm">
            {t('home.viewAll')} <ArrowRight size={14} className="inline" />
          </Link>
        </div>
      </CardContent>
    </Card>
  );
}

function EventItem({ type, date, title, location, id }: any) {
  const { t } = useLanguage();

  return (
    <Card className="hover:shadow-md transition-shadow">
      <div className="p-6 flex items-center gap-6">
        <div className="flex-shrink-0 w-20 text-center">
          <div className="text-3xl font-bold text-navy dark:text-white">{date.day}</div>
          <div className="text-sm text-text-secondary uppercase">{date.month}</div>
        </div>
        <div className="w-1 h-16 bg-accent-blue rounded-full" />
        <div className="flex-1">
          <Badge variant="info" className="mb-2">{type}</Badge>
          <h4 className="text-lg font-bold text-navy dark:text-white mb-1">{title}</h4>
          <div className="flex items-center gap-2 text-sm text-text-secondary">
            <MapPin size={14} />
            <span>{location}</span>
          </div>
        </div>
        <Link to={`/evenements/${id}`} className="text-accent-blue hover:underline">
          {t('home.events.view')} <ArrowRight size={14} className="inline" />
        </Link>
      </div>
    </Card>
  );
}
