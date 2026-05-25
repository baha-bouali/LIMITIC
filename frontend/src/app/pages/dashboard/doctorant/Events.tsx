import { useState } from 'react';
import { Card, CardContent } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { Button } from '../../../components/ui/Button';
import { SearchFilter } from '../../../components/shared/SearchFilter';
import { PhotoGallery } from '../../../components/shared/PhotoGallery';
import { Calendar, MapPin, Users, X, Eye, Mail, Briefcase, AlertCircle, Loader } from 'lucide-react';
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

export default function DoctorantEvents() {
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
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-navy dark:text-white">Événements du laboratoire</h1>
        <p className="text-text-secondary mt-1">Séminaires, conférences, workshops et soutenances</p>
      </div>

      {/* Summary */}
      {!isLoading && !error && (
        <div className="grid grid-cols-3 gap-4">
          {[
            { label: 'À venir', count: events.filter(e => e.status === 'A_VENIR').length, color: 'bg-accent-blue/10 text-accent-blue' },
            { label: 'En cours', count: events.filter(e => e.status === 'EN_COURS').length, color: 'bg-success/10 text-success' },
            { label: 'Passés', count: events.filter(e => e.status === 'PASSE').length, color: 'bg-light-gray text-text-secondary' },
          ].map(s => (
            <Card key={s.label}>
              <CardContent className="p-4 flex items-center gap-3">
                <div className={clsx('w-10 h-10 rounded-lg flex items-center justify-center font-bold text-lg', s.color)}>{s.count}</div>
                <div className="text-sm text-text-secondary">{s.label}</div>
              </CardContent>
            </Card>
          ))}
        </div>
      )}

      <SearchFilter
        searchPlaceholder="Rechercher un événement..."
        filterGroups={filterGroups}
        onSearchChange={setSearchQuery}
        onFilterChange={setActiveFilters}
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

      {!isLoading && !error && upcoming.length > 0 && (
        <div>
          <h2 className="text-lg font-bold text-navy dark:text-white mb-4">Prochains événements</h2>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {upcoming.map(e => <EventCard key={e.id} event={e} onViewDetails={() => setDetailEvent(e)} />)}
          </div>
        </div>
      )}

      {!isLoading && !error && past.length > 0 && (
        <div>
          <h2 className="text-lg font-bold text-navy dark:text-white mb-4">Événements passés</h2>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {past.map(e => <EventCard key={e.id} event={e} onViewDetails={() => setDetailEvent(e)} />)}
          </div>
        </div>
      )}

      {!isLoading && !error && filtered.length === 0 && (
        <Card><CardContent className="py-16 text-center text-text-muted"><Calendar size={40} className="mx-auto mb-3 opacity-30" /><p>Aucun événement trouvé</p></CardContent></Card>
      )}

      {/* Event Detail Modal */}
      {detailEvent && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-[60] p-4" onClick={() => setDetailEvent(null)}>
          <div className="bg-white dark:bg-card rounded-2xl shadow-modal max-w-2xl w-full max-h-[90vh] overflow-y-auto" onClick={e => e.stopPropagation()}>
            <div className="p-6 border-b border-surface-border flex items-center justify-between sticky top-0 bg-white dark:bg-card">
              <h2 className="text-lg font-bold text-navy dark:text-white">Détails de l'événement</h2>
              <button onClick={() => setDetailEvent(null)} className="p-2 hover:bg-light-gray dark:hover:bg-muted rounded-lg">
                <X size={18} />
              </button>
            </div>

            <div className="p-6 space-y-5">
              <div className="flex flex-wrap gap-2">
                <Badge variant={statusConfig[detailEvent.status].variant}>{statusConfig[detailEvent.status].label}</Badge>
              </div>
              <h3 className="text-2xl font-bold text-navy dark:text-white leading-snug">{detailEvent.title}</h3>
              <p className="text-sm text-text-secondary">{detailEvent.description}</p>

              <div className="space-y-4">
                <div className="flex items-start gap-3 p-4 bg-light-gray dark:bg-muted rounded-lg">
                  <Calendar size={18} className="text-accent-blue flex-shrink-0 mt-0.5" />
                  <div>
                    <div className="text-xs font-medium text-text-muted mb-0.5">Date</div>
                    <div className="font-medium text-navy dark:text-white">
                      {new Date(detailEvent.date).toLocaleDateString('fr-FR')}
                      {detailEvent.endDate && ` — ${new Date(detailEvent.endDate).toLocaleDateString('fr-FR')}`}
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
                                <Users size={14} className="flex-shrink-0 text-accent-blue mt-0.5" />
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
  const sc = statusConfig[event.status];

  return (
    <Card className="hover:shadow-lg transition-shadow overflow-hidden">
      <div className="p-4 h-24 bg-gradient-to-br from-navy to-accent-blue relative flex items-center justify-center">
        <div className="absolute inset-0 opacity-10"></div>
        <div className="text-white text-4xl font-bold opacity-20">{event.type.charAt(0)}</div>
        <div className="absolute top-3 right-3">
          <Badge variant={sc.variant}>{sc.label}</Badge>
        </div>
      </div>

      <CardContent className="p-4">
        <h3 className="font-bold text-navy dark:text-white mb-2 line-clamp-2">{event.title}</h3>
        <p className="text-sm text-text-secondary mb-3 line-clamp-2">{event.description}</p>

        <div className="space-y-2 mb-4 text-xs text-text-secondary">
          <div className="flex items-center gap-2">
            <Calendar size={14} />
            <span>{new Date(event.date).toLocaleDateString('fr-FR')}</span>
          </div>
          <div className="flex items-center gap-2">
            <MapPin size={14} />
            <span className="line-clamp-1">{event.location}</span>
          </div>
          {event.capacity && (
            <div className="flex items-center gap-2">
              <Users size={14} />
              <span>{event.registered || 0} / {event.capacity} inscrits</span>
            </div>
          )}
        </div>

        <Button className="w-full" onClick={onViewDetails}>
          <Eye size={16} />
          Voir les détails
        </Button>
      </CardContent>
    </Card>
  );
}
