import { useState } from 'react';
import { Card, CardContent } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { Button } from '../../../components/ui/Button';
import { SearchFilter } from '../../../components/shared/SearchFilter';
import { PhotoGallery } from '../../../components/shared/PhotoGallery';
import { Calendar, MapPin, Users, X, Eye, Mail, Briefcase, AlertCircle, Loader } from 'lucide-react';
import { clsx } from 'clsx';
import { formatEventDateRange, useGetEventsQuery, EVENT_TYPE_OPTIONS, toBackendEventType } from '@/app/api';

type EventStatus = 'A_VENIR' | 'EN_COURS' | 'PASSE';
type EventType = 'SEMINAIRE' | 'CONFERENCE' | 'WORKSHOP' | 'SOUTENANCE' | 'JOURNEE_PORTES_OUVERTES';

interface Speaker {
  name: string;
  email?: string;
  institution?: string;
  role?: string;
  subject?: string;
}

interface LabEvent {
  id: string;
  title: string;
  type: EventType;
  status: EventStatus;
  startDate: string;
  endDate?: string;
  location: string;
  description?: string;
  speakers?: Speaker[];
  capacity?: number;
  registered?: number;
  photos?: string[];
}

const getFilterValue = (value: string | string[] | undefined) => (Array.isArray(value) ? value[0] : value);
const getTypeLabel = (type: string) => EVENT_TYPE_OPTIONS.find((option: { value: string; label: string }) => option.value === toBackendEventType(type))?.label ?? type;

export default function ChercheurEvents() {
  const [searchQuery, setSearchQuery] = useState('');
  const [activeFilters, setActiveFilters] = useState<Record<string, string | string[]>>({});
  const [detailEvent, setDetailEvent] = useState<LabEvent | null>(null);

  const { data: eventsResponse, isLoading, error } = useGetEventsQuery({
    page: 1,
    limit: 100,
    status: getFilterValue(activeFilters.status) as EventStatus | undefined,
    type: getFilterValue(activeFilters.type) ? toBackendEventType(getFilterValue(activeFilters.type) as string) : undefined,
    q: searchQuery,
  });
  const events: LabEvent[] = (eventsResponse ?? []) as LabEvent[];

  const statusConfig: Record<EventStatus, { label: string; variant: any }> = {
    A_VENIR: { label: 'À venir', variant: 'info' },
    EN_COURS: { label: 'En cours', variant: 'success' },
    PASSE: { label: 'Passé', variant: 'default' },
  };

  const typeLabels: Record<EventType, string> = {
    SEMINAIRE: 'Séminaire',
    CONFERENCE: 'Conférence',
    WORKSHOP: 'Workshop',
    SOUTENANCE: 'Soutenance',
    JOURNEE_PORTES_OUVERTES: 'Journée Portes Ouvertes',
  };

  const filterGroups = [
    { id: 'status', label: 'Statut', options: [ { id: 's1', label: 'À venir', value: 'A_VENIR' }, { id: 's2', label: 'En cours', value: 'EN_COURS' }, { id: 's3', label: 'Passé', value: 'PASSE' } ] },
    { id: 'type', label: 'Type', options: EVENT_TYPE_OPTIONS.map((option: { value: string; label: string }) => ({ id: option.value, label: option.label, value: option.value })) },
  ];

  const filtered = events.filter(e => {
    const q = searchQuery.toLowerCase();
    const matchQ = !q || e.title.toLowerCase().includes(q) || e.location.toLowerCase().includes(q);
    const statusF = getFilterValue(activeFilters.status) as EventStatus | undefined;
    const typeF = getFilterValue(activeFilters.type) ? toBackendEventType(getFilterValue(activeFilters.type) as string) : undefined;
    return matchQ && (!statusF || e.status === statusF) && (!typeF || toBackendEventType(e.type) === typeF);
  });

  const upcoming = filtered.filter(e => e.status === 'A_VENIR' || e.status === 'EN_COURS');
  const past = filtered.filter(e => e.status === 'PASSE');

  return (
    <div className="space-y-8">
      <div>
        <h1 className="text-4xl font-bold text-navy dark:text-white mb-2">Événements</h1>
        <p className="text-text-secondary max-w-2xl">Découvrez les séminaires, conférences et ateliers organisés par le laboratoire LIMTIC</p>
      </div>

      <SearchFilter onSearchChange={setSearchQuery} filterGroups={filterGroups} onFilterChange={setActiveFilters} searchPlaceholder="Rechercher un événement..." />

      {isLoading ? (
        <Card><CardContent className="p-12 text-center"><Loader size={48} className="mx-auto" /></CardContent></Card>
      ) : error ? (
        <Card><CardContent className="p-6 text-center"><AlertCircle size={48} className="mx-auto" /><p>Erreur lors du chargement des événements</p></CardContent></Card>
      ) : (
        <>
          {upcoming.length > 0 && (
            <div>
              <h2 className="text-lg font-bold mb-4">Prochains événements</h2>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">{upcoming.map(e => <EventCard key={e.id} event={e} onViewDetails={() => setDetailEvent(e)} />)}</div>
            </div>
          )}

          {past.length > 0 && (
            <div>
              <h2 className="text-lg font-bold mb-4">Événements passés</h2>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">{past.map(e => <EventCard key={e.id} event={e} onViewDetails={() => setDetailEvent(e)} />)}</div>
            </div>
          )}

          {filtered.length === 0 && (<Card><CardContent className="p-12 text-center"><p>Aucun événement trouvé</p></CardContent></Card>)}

          {detailEvent && (
            <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4" onClick={() => setDetailEvent(null)}>
              <div className="bg-white dark:bg-card rounded-2xl max-w-2xl w-full max-h-[90vh] overflow-y-auto" onClick={e => e.stopPropagation()}>
                <div className="p-6 border-b flex items-center justify-between">
                  <h3 className="text-lg font-bold">Détails de l'événement</h3>
                  <button onClick={() => setDetailEvent(null)} className="p-2"><X /></button>
                </div>
                <div className="p-6 space-y-4">
                  <div className="flex items-center justify-between">
                    <div className="space-y-1">
                      <div className="text-sm font-medium text-text-secondary">{statusConfig[detailEvent.status].label}</div>
                      <h4 className="text-xl font-bold">{detailEvent.title}</h4>
                      <div className="text-sm text-text-secondary leading-snug break-words">{formatEventDateRange(detailEvent.startDate, detailEvent.endDate)}</div>
                      <div className="text-sm text-text-secondary leading-snug break-words">{detailEvent.location}</div>
                    </div>
                  </div>

                  {detailEvent.speakers && detailEvent.speakers.length > 0 && (
                    <div>
                      <div className="text-sm font-medium mb-2">Intervenants</div>
                      <div className="space-y-2">{detailEvent.speakers.map((s, i) => (
                        <div key={i} className="p-3 bg-light-gray rounded">
                          <div className="font-medium">{s.name}</div>
                          <div className="text-xs text-text-secondary">{s.institution} • {s.email}</div>
                        </div>
                      ))}</div>
                    </div>
                  )}

                  {detailEvent.photos && detailEvent.photos.length > 0 && (
                    <PhotoGallery photos={detailEvent.photos.map(url => ({ url }))} columns={3} />
                  )}
                </div>

                <div className="p-6 border-t flex justify-end">
                  <Button variant="outlined" onClick={() => setDetailEvent(null)}>Fermer</Button>
                </div>
              </div>
            </div>
          )}
        </>
      )}
    </div>
  );
}

function EventCard({ event, onViewDetails }: { event: LabEvent; onViewDetails: () => void }) {
  const statusConfig = { A_VENIR: { variant: 'info' as const, label: 'À venir' }, EN_COURS: { variant: 'success' as const, label: 'En cours' }, PASSE: { variant: 'default' as const, label: 'Passé' } };
  const typeLabels: Record<EventType, string> = { SEMINAIRE: 'Séminaire', CONFERENCE: 'Conférence', WORKSHOP: 'Workshop', SOUTENANCE: 'Soutenance', JOURNEE_PORTES_OUVERTES: 'Journée Portes Ouvertes' };
  const sc = statusConfig[event.status];

  return (
    <Card className="hover:shadow-lg">
      <CardContent className="p-0">
        <div className="h-28 bg-gradient-to-br from-navy to-accent-blue flex items-center justify-center text-white text-5xl font-bold">{event.type.charAt(0)}</div>
        <div className="p-4">
          <Badge className="mb-2">{getTypeLabel(event.type)}</Badge>
          <h3 className="font-bold text-lg mb-1">{event.title}</h3>
          <p className="text-sm text-text-secondary mb-3 line-clamp-2">{event.description}</p>
          <div className="space-y-3">
            <div className="flex items-start gap-2 text-sm text-text-secondary leading-snug min-w-0">
              <Calendar size={14} className="mt-0.5 flex-shrink-0" />
              <span className="whitespace-normal break-words">{formatEventDateRange(event.startDate, event.endDate)}</span>
            </div>
            <Button variant="ghost" onClick={onViewDetails}><Eye size={14} /></Button>
          </div>
        </div>
      </CardContent>
    </Card>
  );
}

