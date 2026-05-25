import { useState } from 'react';
import { Card, CardContent } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { Button } from '../../../components/ui/Button';
import { SearchFilter } from '../../../components/shared/SearchFilter';
import { PhotoGallery } from '../../../components/shared/PhotoGallery';
import { Calendar, MapPin, Clock, Users, ExternalLink, X, Eye, Info, Mail, Briefcase, AlertCircle, Loader } from 'lucide-react';
import { clsx } from 'clsx';
import { useGetEventsQuery } from '@/app/api';

type EventStatus = 'A_VENIR' | 'EN_COURS' | 'PASSE';
type EventType = 'SEMINAIRE' | 'CONFERENCE' | 'WORKSHOP' | 'SOUTENANCE' | 'JOURNEE_PORTES_OUVERTES';

interface Speaker {
  name: string;
  email: string;
  institution: string;
  role: string;
  subject: string;
}

interface LabEvent {
  id: string;
  title: string;
  type: EventType;
  status: EventStatus;
  date: string;
  endDate?: string;
  location: string;
  description: string;
  speakers?: Speaker[];
  capacity?: number;
  registered?: number;
  photos?: string[];
}

const statusConfig: Record<EventStatus, { label: string; variant: any; color: string }> = {
  A_VENIR: { label: 'À venir', variant: 'info', color: 'bg-accent-blue' },
  EN_COURS: { label: 'En cours', variant: 'success', color: 'bg-success' },
  PASSE: { label: 'Passé', variant: 'default', color: 'bg-text-muted' },
};

const typeLabels: Record<EventType, string> = {
  SEMINAIRE: 'Séminaire',
  CONFERENCE: 'Conférence',
  WORKSHOP: 'Workshop',
  SOUTENANCE: 'Soutenance',
  JOURNEE_PORTES_OUVERTES: 'Journée Portes Ouvertes',
};

export default function VisitorEvents() {
  const [searchQuery, setSearchQuery] = useState('');
  const [activeFilters, setActiveFilters] = useState<Record<string, string | string[]>>({});
  const [detailEvent, setDetailEvent] = useState<LabEvent | null>(null);

  // RTK Query hook
  const { data: eventsResponse, isLoading, error } = useGetEventsQuery({
    page: 1,
    limit: 100,
    status: typeof activeFilters.status === 'string' ? activeFilters.status : undefined,
    type: typeof activeFilters.type === 'string' ? activeFilters.type : undefined,
    q: searchQuery,
  });

  const events: LabEvent[] = (eventsResponse?.items || []) as LabEvent[];

  const filterGroups = [
    {
      id: 'status', label: 'Statut', options: [
        { id: 's1', label: 'À venir', value: 'A_VENIR' },
        { id: 's2', label: 'En cours', value: 'EN_COURS' },
        { id: 's3', label: 'Passé', value: 'PASSE' },
      ]
    },
    {
      id: 'type', label: 'Type', options: [
        { id: 't1', label: 'Séminaire', value: 'SEMINAIRE' },
        { id: 't2', label: 'Conférence', value: 'CONFERENCE' },
        { id: 't3', label: 'Workshop', value: 'WORKSHOP' },
        { id: 't4', label: 'Soutenance', value: 'SOUTENANCE' },
        { id: 't5', label: 'Journée Portes Ouvertes', value: 'JOURNEE_PORTES_OUVERTES' },
      ]
    },
  ];

  const filtered = events.filter(e => {
    const q = searchQuery.toLowerCase();
    const matchQ = !q || e.title.toLowerCase().includes(q) || e.location.toLowerCase().includes(q);
    const statusF = activeFilters.status as string;
    const typeF = activeFilters.type as string;
    return matchQ && (!statusF || e.status === statusF) && (!typeF || e.type === typeF);
  });

  const upcoming = filtered.filter(e => e.status === 'A_VENIR' || e.status === 'EN_COURS');
  const past = filtered.filter(e => e.status === 'PASSE');

  return (
    <div className="space-y-8">
      <div>
        <h1 className="text-4xl font-bold text-navy dark:text-white mb-2">Événements</h1>
        <p className="text-text-secondary max-w-2xl">
          Découvrez les séminaires, conférences et ateliers organisés par le laboratoire LIMTIC
        </p>
      </div>

      {/* Search and Filters */}
      <SearchFilter
        onSearchChange={setSearchQuery}
        filterGroups={filterGroups}
        onFilterChange={setActiveFilters}
        searchPlaceholder="Rechercher un événement..."
      />

      {/* Loading State */}
      {isLoading && (
        <Card>
          <CardContent className="p-12 text-center">
            <Loader size={64} className="mx-auto text-accent-blue mb-4 animate-spin" />
            <h3 className="text-xl font-bold text-navy dark:text-white mb-2">
              Chargement des événements...
            </h3>
          </CardContent>
        </Card>
      )}

      {/* Error State */}
      {error && (
        <Card className="border-error/20 bg-error/5">
          <CardContent className="p-6 text-center">
            <AlertCircle size={64} className="mx-auto text-error mb-4" />
            <h3 className="text-xl font-bold text-error mb-2">
              Erreur lors du chargement
            </h3>
            <p className="text-text-secondary">
              Une erreur est survenue. Veuillez réessayer.
            </p>
          </CardContent>
        </Card>
      )}

      {/* Upcoming Events Section */}
      {!isLoading && !error && upcoming.length > 0 && (
        <section className="space-y-4">
          <h2 className="text-2xl font-bold text-navy dark:text-white">Événements à venir</h2>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {upcoming.map((event) => (
              <EventCard key={event.id} event={event} onViewDetails={() => setDetailEvent(event)} />
            ))}
          </div>
        </section>
      )}

      {/* Past Events Section */}
      {!isLoading && !error && past.length > 0 && (
        <section className="space-y-4">
          <h2 className="text-2xl font-bold text-navy dark:text-white">Événements passés</h2>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {past.map((event) => (
              <EventCard key={event.id} event={event} onViewDetails={() => setDetailEvent(event)} />
            ))}
          </div>
        </section>
      )}

      {/* No Events State */}
      {!isLoading && !error && filtered.length === 0 && (
        <Card>
          <CardContent className="p-12 text-center">
            <Calendar size={64} className="mx-auto text-text-muted mb-4" />
            <h3 className="text-xl font-bold text-navy dark:text-white mb-2">
              Aucun événement trouvé
            </h3>
            <p className="text-text-secondary">
              {searchQuery || Object.values(activeFilters).some(v => v.length > 0)
                ? 'Aucun résultat ne correspond à vos critères'
                : 'Aucun événement disponible'}
            </p>
          </CardContent>
        </Card>
      )}

      {/* Event Detail Modal */}
      {detailEvent && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4" onClick={() => setDetailEvent(null)}>
          <div className="bg-white dark:bg-card rounded-2xl shadow-modal max-w-2xl w-full max-h-[90vh] overflow-y-auto" onClick={e => e.stopPropagation()}>
            <div className="p-6 border-b border-surface-border flex items-center justify-between sticky top-0 bg-white dark:bg-card">
              <h2 className="text-lg font-bold text-navy dark:text-white">Détails de l'événement</h2>
              <button onClick={() => setDetailEvent(null)} className="p-2 hover:bg-light-gray dark:hover:bg-muted rounded-lg">
                <X size={18} />
              </button>
            </div>

            <div className="p-6 space-y-5">
              <div className="flex items-center gap-2 mb-4">
                <Badge variant={statusConfig[detailEvent.status].variant}>
                  {statusConfig[detailEvent.status].label}
                </Badge>
              </div>

              <div>
                <h3 className="text-2xl font-bold text-navy dark:text-white mb-4">{detailEvent.title}</h3>
                <p className="text-sm text-text-secondary mb-4">{detailEvent.description}</p>
              </div>

              <div className="space-y-4">
                <div className="flex items-start gap-3 p-4 bg-light-gray dark:bg-muted rounded-lg">
                  <Calendar size={18} className="text-accent-blue flex-shrink-0 mt-0.5" />
                  <div>
                    <div className="text-xs font-medium text-text-muted mb-0.5">Date</div>
                    <div className="font-medium text-navy dark:text-white">
                      {new Date(detailEvent.date).toLocaleDateString('fr-FR', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' })}
                      {detailEvent.endDate && ` — ${new Date(detailEvent.endDate).toLocaleDateString('fr-FR', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' })}`}
                    </div>
                  </div>
                </div>

                <div className="flex items-start gap-3 p-4 bg-light-gray dark:bg-muted rounded-lg">
                  <MapPin size={18} className="text-accent-blue flex-shrink-0 mt-0.5" />
                  <div>
                    <div className="text-xs font-medium text-text-muted mb-0.5">Lieu</div>
                    <div className="font-medium text-navy dark:text-white">{detailEvent.location}</div>
                  </div>
                </div>

                {detailEvent.capacity && (
                  <div className="flex items-start gap-3 p-4 bg-light-gray dark:bg-muted rounded-lg">
                    <Users size={18} className="text-accent-blue flex-shrink-0 mt-0.5" />
                    <div>
                      <div className="text-xs font-medium text-text-muted mb-0.5">Capacité</div>
                      <div className="font-medium text-navy dark:text-white">
                        {detailEvent.registered || 0} / {detailEvent.capacity} inscrits
                      </div>
                    </div>
                  </div>
                )}

                {detailEvent.speakers && detailEvent.speakers.length > 0 && (
                  <div>
                    <div className="text-sm font-medium text-navy dark:text-white mb-3">Intervenants</div>
                    <div className="space-y-3">
                      {detailEvent.speakers.map((speaker, idx) => (
                        <div key={idx} className="p-4 bg-light-gray dark:bg-muted rounded-lg">
                          <div className="font-medium text-navy dark:text-white mb-2">{speaker.name}</div>
                          <div className="space-y-1 text-sm text-text-secondary">
                            <div className="flex items-center gap-2">
                              <Mail size={14} className="flex-shrink-0 text-accent-blue" />
                              <span>{speaker.email}</span>
                            </div>
                            <div className="flex items-center gap-2">
                              <Briefcase size={14} className="flex-shrink-0 text-accent-blue" />
                              <span>{speaker.institution}</span>
                            </div>
                            {speaker.role && (
                              <div className="flex items-start gap-2">
                                <Info size={14} className="flex-shrink-0 text-accent-blue mt-0.5" />
                                <div>
                                  <div className="font-medium text-navy dark:text-white text-xs">{speaker.role}</div>
                                  {speaker.subject && <div className="text-xs italic">{speaker.subject}</div>}
                                </div>
                              </div>
                            )}
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>
                )}

                {detailEvent.status === 'PASSE' && detailEvent.photos && detailEvent.photos.length > 0 && (
                  <div>
                    <div className="text-sm font-medium text-navy dark:text-white mb-3">Photos de l'événement</div>
                    <PhotoGallery photos={detailEvent.photos.map((url: string) => ({ url }))} columns={3} />
                  </div>
                )}
              </div>
            </div>

            <div className="p-6 border-t border-surface-border flex justify-end gap-3 sticky bottom-0 bg-white dark:bg-card">
              <Button variant="outlined" onClick={() => setDetailEvent(null)}>Fermer</Button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

function EventCard({ event, onViewDetails }: { event: LabEvent; onViewDetails: () => void }) {
  const statusConfig = {
    'A_VENIR': { variant: 'info' as const, label: 'À venir', color: 'bg-accent-blue' },
    'EN_COURS': { variant: 'success' as const, label: 'En cours', color: 'bg-success' },
    'PASSE': { variant: 'default' as const, label: 'Passé', color: 'bg-text-muted' }
  };

  const typeLabels: Record<EventType, string> = {
    SEMINAIRE: 'Séminaire',
    CONFERENCE: 'Conférence',
    WORKSHOP: 'Workshop',
    SOUTENANCE: 'Soutenance',
    JOURNEE_PORTES_OUVERTES: 'Journée Portes Ouvertes',
  };

  const sc = statusConfig[event.status];

  return (
    <Card className="hover:shadow-lg transition-shadow overflow-hidden group cursor-pointer">
      <CardContent className="p-0">
        <div className="h-32 bg-gradient-to-br from-navy to-accent-blue flex items-center justify-center relative overflow-hidden">
          <div className="absolute inset-0 opacity-10 group-hover:opacity-20 transition-opacity"></div>
          <div className="text-white text-6xl font-bold opacity-20 group-hover:opacity-30 transition-opacity">
            {event.type.charAt(0)}
          </div>
          <div className="absolute top-3 right-3">
            <Badge variant={sc.variant}>{sc.label}</Badge>
          </div>
        </div>

        <div className="p-4">
          <Badge className="mb-2 bg-accent-blue/10 text-accent-blue border-accent-blue/20">
            {typeLabels[event.type]}
          </Badge>
          <h3 className="font-bold text-navy dark:text-white mb-2 line-clamp-2 text-lg">
            {event.title}
          </h3>
          <p className="text-sm text-text-secondary mb-3 line-clamp-2">{event.description}</p>

          <div className="space-y-2 mb-4">
            <div className="flex items-center gap-2 text-xs text-text-secondary">
              <Calendar size={14} />
              <span>{new Date(event.date).toLocaleDateString('fr-FR')}</span>
            </div>
            <div className="flex items-center gap-2 text-xs text-text-secondary">
              <MapPin size={14} />
              <span className="line-clamp-1">{event.location}</span>
            </div>
            {event.capacity && (
              <div className="flex items-center gap-2 text-xs text-text-secondary">
                <Users size={14} />
                <span>{event.registered || 0} / {event.capacity} inscrits</span>
              </div>
            )}
          </div>

          <Button className="w-full" onClick={onViewDetails}>
            <Eye size={16} />
            Voir les détails
          </Button>
        </div>
      </CardContent>
    </Card>
  );
}
