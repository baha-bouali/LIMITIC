import { useMemo, useState } from 'react';
import { PublicNavbar } from '../../components/layout/PublicNavbar';
import { PublicFooter } from '../../components/layout/PublicFooter';
import { Card, CardContent, CardHeader } from '../../components/ui/Card';
import { Badge } from '../../components/ui/Badge';
import { MapPin, Calendar, Users, Image, AlertCircle, Loader } from 'lucide-react';
import { Link } from 'react-router-dom';
import { clsx } from 'clsx';
import { useLanguage } from '../../contexts/LanguageContext';
import { useGetEventsQuery } from '@/app/api';

type EventStatus = 'A_VENIR' | 'EN_COURS' | 'PASSE';
type EventType = 'SEMINAIRE' | 'CONFERENCE' | 'WORKSHOP' | 'SOUTENANCE' | 'JOURNEE_PORTES_OUVERTES';

interface Speaker { name: string; email?: string; institution?: string; role?: string; subject?: string }
interface Event { id: string; type: EventType; title: string; date: string; endDate?: string; location: string; status: EventStatus; description?: string; speakers?: Speaker[]; photos?: string[] }
export default function EventsPage() {
  const { t } = useLanguage();
  const [activeTab, setActiveTab] = useState<EventStatus>('A_VENIR');
  const { data: eventsResponse, isLoading, error } = useGetEventsQuery({ page: 1, limit: 100 });
  const allEvents: Event[] = (eventsResponse?.items || []) as Event[];
  const filteredEvents = useMemo(() => allEvents.filter((event) => event.status === activeTab), [allEvents, activeTab]);

  const getSpeakersCount = (event: Event) => event.speakers?.length ?? 0;
  const getPhotoCount = (event: Event) => event.photos?.length ?? 0;

  const getStatusBadge = (status: EventStatus) => {
    const config = {
      'A_VENIR': { variant: 'info' as const, label: t('eventPage.upcoming'), color: '#4C1D95' },
      'EN_COURS': { variant: 'success' as const, label: t('eventPage.ongoing'), color: '#065F46' },
      'PASSE': { variant: 'default' as const, label: t('eventPage.past'), color: '#475569' }
    };
    return config[status];
  };

  const getTypeBadge = (type: EventType) => {
    const colors: Record<EventType, string> = {
      'SEMINAIRE': 'bg-[#EFF6FF] text-[#1D4ED8]',
      'WORKSHOP': 'bg-[#F0FDF4] text-[#15803D]',
      'CONFERENCE': 'bg-[#FFF7ED] text-[#C2410C]',
      'JOURNEE_PORTES_OUVERTES': 'bg-[#FAF5FF] text-[#7E22CE]',
      'SOUTENANCE': 'bg-[#FEF3C7] text-[#92400E]'
    };
    return colors[type] || 'bg-light-gray text-text-secondary';
  };

  const typeLabels: Record<EventType, string> = {
    'SEMINAIRE': 'Séminaire',
    'CONFERENCE': 'Conférence',
    'WORKSHOP': 'Workshop',
    'SOUTENANCE': 'Soutenance',
    'JOURNEE_PORTES_OUVERTES': 'Journée Portes Ouvertes',
  };

  return (
    <div className="min-h-screen bg-white dark:bg-background">
      <PublicNavbar />
      <div style={{ height: 'var(--navbar-height)' }} />

      {/* Header */}
      <div className="brand-gradient-diagonal text-white py-20">
        <div className="max-w-[var(--content-max-width)] mx-auto px-6">
          <div className="text-sm text-white/80 mb-3">{t('eventPage.breadcrumb')}</div>
          <h1 className="text-5xl font-bold mb-4">{t('eventPage.title')}</h1>
          <p className="text-xl text-white/90">{t('eventPage.subtitle')}</p>
          <div className="flex gap-8 mt-8">
            <div className="text-center">
                <div className="text-3xl font-bold">{allEvents.filter(e => e.status === 'A_VENIR').length}</div>
              <div className="text-white/80">{t('eventPage.upcoming')}</div>
            </div>
            <div className="text-center">
              <div className="text-3xl font-bold">{allEvents.filter(e => e.status === 'PASSE').length}</div>
              <div className="text-white/80">{t('eventPage.past')}</div>
            </div>
            <div className="text-center">
              <div className="text-3xl font-bold">{allEvents.filter(e => e.status === 'PASSE').reduce((sum, e) => sum + (e.photos?.length || 0), 0)}</div>
              <div className="text-white/80">{t('eventPage.photos')}</div>
            </div>
          </div>
        </div>
      </div>

      {/* Tabs */}
      <div className="sticky top-[var(--navbar-height)] z-40 bg-white dark:bg-background border-b border-surface-border shadow-sm">
        <div className="max-w-[var(--content-max-width)] mx-auto px-6">
          <div className="flex gap-8">
            {(['A_VENIR', 'EN_COURS', 'PASSE'] as EventStatus[]).map((tab) => {
              const config = getStatusBadge(tab);
              const count = allEvents.filter(e => e.status === tab).length;
              return (
                <button
                  key={tab}
                  onClick={() => setActiveTab(tab)}
                  className={clsx(
                    'py-4 px-2 border-b-2 transition-colors flex items-center gap-2',
                    activeTab === tab
                      ? 'border-accent-blue text-accent-blue font-medium'
                      : 'border-transparent text-text-secondary hover:text-text-primary'
                  )}
                >
                  {config.label}
                  <Badge variant={config.variant} className="text-xs">{count}</Badge>
                </button>
              );
            })}
          </div>
        </div>
      </div>

      {/* Content */}
      <div className="max-w-[var(--content-max-width)] mx-auto px-6 py-12">
        {isLoading && (
          <Card>
            <CardContent className="p-12 text-center">
              <Loader size={64} className="mx-auto text-accent-blue mb-4 animate-spin" />
              <h3 className="text-xl font-bold text-navy dark:text-white mb-2">{t('eventPage.loadingEvents')}</h3>
            </CardContent>
          </Card>
        )}

        {error && (
          <Card className="border-error/20 bg-error/5">
            <CardContent className="p-6 text-center">
              <AlertCircle size={64} className="mx-auto text-error mb-4" />
              <h3 className="text-xl font-bold text-error mb-2">{t('eventPage.loadingError')}</h3>
              <p className="text-text-secondary">{t('eventPage.tryAgain')}</p>
            </CardContent>
          </Card>
        )}

        {!isLoading && !error && filteredEvents.length === 0 ? (
          <Card>
            <CardContent className="p-12 text-center">
              <Calendar size={64} className="mx-auto text-text-muted mb-4" />
              <h3 className="text-xl font-bold text-navy dark:text-white mb-2">
                {t('eventPage.noEvents')} {getStatusBadge(activeTab).label.toLowerCase()}
              </h3>
              <p className="text-text-secondary">
                {t('eventPage.comeBackLater')}
              </p>
            </CardContent>
          </Card>
        ) : !isLoading && !error && (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {filteredEvents.map((event) => (
              <Card key={event.id} className="hover:shadow-lg transition-shadow overflow-hidden group">
                {/* Event Image/Placeholder - Only show for past events */}
                {event.status === 'PASSE' && (
                  <div className="h-48 relative overflow-hidden">
                    {getPhotoCount(event) > 0 ? (
                      <div className="absolute inset-0 bg-gradient-to-br from-navy to-accent-blue flex items-center justify-center">
                        <div className="text-center text-white">
                          <Image size={48} className="mx-auto mb-2" />
                          <div className="text-sm">{getPhotoCount(event)} {t('eventPage.photos').toLowerCase()}</div>
                        </div>
                      </div>
                    ) : (
                      <div className="absolute inset-0 bg-gradient-to-br from-navy to-accent-blue flex items-center justify-center text-white text-6xl font-bold">
                        {event.type.charAt(0)}
                      </div>
                    )}
                    <div className="absolute top-3 right-3">
                      <Badge
                        variant={getStatusBadge(event.status).variant}
                        style={event.status === 'PASSE' && getPhotoCount(event) > 0 ? { backgroundColor: '#059669', color: 'white' } : {}}
                      >
                        {event.status === 'PASSE' && getPhotoCount(event) > 0 ? (
                          <><Image size={12} className="mr-1" /> {t('eventPage.photosAvailable')}</>
                        ) : (
                          getStatusBadge(event.status).label
                        )}
                      </Badge>
                    </div>
                  </div>
                )}

                <CardHeader>
                  <Badge className={clsx(getTypeBadge(event.type), 'mb-3')}>{event.type}</Badge>
                  <h3 className="text-xl font-bold text-navy dark:text-white mb-2 line-clamp-2 min-h-[3.5rem]">
                    {event.title}
                  </h3>
                  <p className="text-sm text-text-secondary line-clamp-2 mb-4">{event.description}</p>
                  <div className="space-y-2 text-sm text-text-secondary">
                    <div className="flex items-center gap-2">
                      <Calendar size={16} className="flex-shrink-0 text-accent-blue" />
                      <span>{new Date(event.startDate).toLocaleDateString('fr-FR', {
                        day: '2-digit',
                        month: 'long',
                        year: 'numeric',
                      })}</span>
                    </div>
                    <div className="flex items-center gap-2">
                      <MapPin size={16} className="flex-shrink-0 text-accent-blue" />
                      <span className="line-clamp-1">{event.location}</span>
                    </div>
                    <div className="flex items-center gap-2">
                      <Users size={16} className="flex-shrink-0 text-accent-blue" />
                      <span>{getSpeakersCount(event)} {getSpeakersCount(event) > 1 ? t('eventPage.speakers_plural') : t('eventPage.speakers')}</span>
                    </div>
                  </div>
                </CardHeader>
                <CardContent className="border-t border-surface-border">
                  <Link
                    to={`/evenements/${event.id}`}
                    className="text-accent-blue hover:underline font-medium flex items-center justify-between group-hover:gap-2 transition-all"
                  >
                    <span>{t('eventPage.viewDetails')}</span>
                    <span>→</span>
                  </Link>
                </CardContent>
              </Card>
            ))}
          </div>
        )}
      </div>

      <PublicFooter />
    </div>
  );
}
