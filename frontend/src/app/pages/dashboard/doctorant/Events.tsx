import { useState } from 'react';
import { Card, CardContent } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { Button } from '../../../components/ui/Button';
import { SearchFilter } from '../../../components/shared/SearchFilter';
import { PhotoGallery } from '../../../components/shared/PhotoGallery';
import { Calendar, MapPin, Clock, Users, ExternalLink, X, Eye, Info, Mail, Briefcase } from 'lucide-react';
import { clsx } from 'clsx';

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

const events: LabEvent[] = [
  {
    id: '1',
    title: 'Séminaire IA & Santé Numérique',
    type: 'SEMINAIRE',
    status: 'A_VENIR',
    date: '26 Mai 2026',
    location: 'Salle de conférence A, LIMTIC',
    description: 'Présentation des avancées récentes en intelligence artificielle appliquée à la santé numérique, avec focus sur les modèles de diagnostic assisté par IA.',
    speakers: [
      { name: 'Dr. Ahmed Ben Salem', email: 'ahmed.bensalem@limtic.tn', institution: 'LIMTIC, ISI', role: 'Conférencier principal', subject: 'IA pour le diagnostic médical' },
      { name: 'Prof. Marie Dubois', email: 'marie.dubois@univ.fr', institution: 'Université Paris-Saclay', role: 'Experte invitée', subject: 'Éthique de l\'IA en santé' }
    ],
    capacity: 60,
    registered: 42
  },
  {
    id: '2',
    title: 'Workshop Blockchain & Sécurité',
    type: 'WORKSHOP',
    status: 'A_VENIR',
    date: '02 Juin 2026',
    location: 'Laboratoire informatique B',
    description: 'Atelier pratique sur l\'application des technologies blockchain pour la sécurisation des données de santé.',
    speakers: [
      { name: 'Dr. Fatma Gharbi', email: 'fatma.gharbi@limtic.tn', institution: 'LIMTIC, ISI', role: 'Formatrice', subject: 'Blockchain et sécurité' },
      { name: 'Mohamed Najjar', email: 'mohamed.najjar@limtic.tn', institution: 'LIMTIC, ISI', role: 'Co-formateur', subject: 'Applications pratiques' }
    ],
    capacity: 25,
    registered: 18
  },
  {
    id: '3',
    title: 'Soutenance de thèse — Sarah Trabelsi',
    type: 'SOUTENANCE',
    status: 'A_VENIR',
    date: '15 Juin 2026',
    location: 'Amphithéâtre principal',
    description: 'Soutenance de la thèse de doctorat de Sarah Trabelsi : "Deep Learning pour le diagnostic médical assisté par IA".',
    speakers: [
      { name: 'Sarah Trabelsi', email: 'sarah.trabelsi@limtic.tn', institution: 'LIMTIC, ISI', role: 'Doctorante', subject: 'Deep Learning pour le diagnostic médical assisté par IA' }
    ]
  },
  {
    id: '4',
    title: 'Journée Portes Ouvertes LIMTIC 2026',
    type: 'JOURNEE_PORTES_OUVERTES',
    status: 'A_VENIR',
    date: '10 Juillet 2026',
    location: 'Campus LIMTIC',
    description: 'Journée de présentation des axes de recherche et des travaux des chercheurs et doctorants du laboratoire.',
    capacity: 200,
    registered: 87
  },
  {
    id: '5',
    title: 'Conférence TALN 2026',
    type: 'CONFERENCE',
    status: 'EN_COURS',
    date: '20 Mai 2026',
    endDate: '23 Mai 2026',
    location: 'Centre de congrès, Tunis',
    description: 'Participation du laboratoire LIMTIC à la conférence internationale sur le Traitement Automatique du Langage Naturel.',
    speakers: [
      { name: 'Dr. Ahmed Ben Salem', email: 'ahmed.bensalem@limtic.tn', institution: 'LIMTIC, ISI', role: 'Présentateur', subject: 'Traitement du langage naturel en arabe' },
      { name: 'Dr. Mohamed Mezghani', email: 'mohamed.mezghani@limtic.tn', institution: 'LIMTIC, ISI', role: 'Présentateur', subject: 'Modèles de traduction automatique' }
    ]
  },
  {
    id: '6',
    title: 'Séminaire IoT & Réseaux Intelligents',
    type: 'SEMINAIRE',
    status: 'PASSE',
    date: '15 Avril 2026',
    location: 'Salle de conférence A, LIMTIC',
    description: 'Présentation des travaux de recherche sur l\'Internet des Objets et les architectures réseau intelligentes.',
    speakers: [
      { name: 'Dr. Mohamed Mezghani', email: 'mohamed.mezghani@limtic.tn', institution: 'LIMTIC, ISI', role: 'Conférencier', subject: 'IoT et réseaux intelligents' },
      { name: 'Karim Slimi', email: 'karim.slimi@limtic.tn', institution: 'LIMTIC, ISI', role: 'Présentateur', subject: 'Architectures réseau' }
    ],
    photos: [
      'https://images.unsplash.com/photo-1540575467063-178a50c2df87?w=800&auto=format&fit=crop',
      'https://images.unsplash.com/photo-1591115765373-5207764f72e7?w=800&auto=format&fit=crop',
      'https://images.unsplash.com/photo-1505373877841-8d25f7d46678?w=800&auto=format&fit=crop'
    ]
  },
  {
    id: '7',
    title: 'Workshop Sécurité Cybernétique',
    type: 'WORKSHOP',
    status: 'PASSE',
    date: '28 Mars 2026',
    location: 'Laboratoire informatique A',
    description: 'Formation sur les nouvelles menaces cybernétiques et les stratégies de défense avancées.',
    speakers: [
      { name: 'Dr. Fatma Gharbi', email: 'fatma.gharbi@limtic.tn', institution: 'LIMTIC, ISI', role: 'Formatrice', subject: 'Cybersécurité et menaces avancées' }
    ],
    capacity: 20,
    registered: 20,
    photos: [
      'https://images.unsplash.com/photo-1550751827-4bd374c3f58b?w=800&auto=format&fit=crop',
      'https://images.unsplash.com/photo-1563986768609-322da13575f3?w=800&auto=format&fit=crop',
      'https://images.unsplash.com/photo-1516321318423-f06f85e504b3?w=800&auto=format&fit=crop',
      'https://images.unsplash.com/photo-1531482615713-2afd69097998?w=800&auto=format&fit=crop'
    ]
  },
];

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

      <SearchFilter
        searchPlaceholder="Rechercher un événement..."
        filterGroups={filterGroups}
        onSearchChange={setSearchQuery}
        onFilterChange={setActiveFilters}
      />

      {upcoming.length > 0 && (
        <div>
          <h2 className="text-lg font-bold text-navy dark:text-white mb-4">Prochains événements</h2>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {upcoming.map(e => <EventCard key={e.id} event={e} onViewDetails={() => setDetailEvent(e)} />)}
          </div>
        </div>
      )}

      {past.length > 0 && (
        <div>
          <h2 className="text-lg font-bold text-navy dark:text-white mb-4">Événements passés</h2>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {past.map(e => <EventCard key={e.id} event={e} onViewDetails={() => setDetailEvent(e)} />)}
          </div>
        </div>
      )}

      {filtered.length === 0 && (
        <Card><CardContent className="py-16 text-center text-text-muted"><Calendar size={40} className="mx-auto mb-3 opacity-30" /><p>Aucun événement trouvé</p></CardContent></Card>
      )}

      {/* Event Detail Modal */}
      {detailEvent && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-[60] p-4" onClick={() => setDetailEvent(null)}>
          <div className="bg-white dark:bg-card rounded-2xl shadow-modal max-w-2xl w-full max-h-[90vh] overflow-y-auto" onClick={e => e.stopPropagation()}>
            <div className="p-6 border-b border-surface-border flex items-center justify-between">
              <h2 className="text-lg font-bold text-navy dark:text-white">Détails de l'événement</h2>
              <button onClick={() => setDetailEvent(null)} className="p-2 hover:bg-light-gray dark:hover:bg-muted rounded-lg">
                <X size={18} />
              </button>
            </div>
            <div className="p-6 space-y-5">
              <div className="flex flex-wrap gap-2">
                <Badge variant={statusConfig[detailEvent.status].variant}>{statusConfig[detailEvent.status].label}</Badge>
                <Badge variant="info">{typeLabels[detailEvent.type]}</Badge>
              </div>
              <h3 className="text-xl font-bold text-navy dark:text-white leading-snug">{detailEvent.title}</h3>

              <div className="space-y-4">
                <div className="flex items-start gap-3 p-4 bg-light-gray dark:bg-muted rounded-lg">
                  <Calendar size={18} className="text-accent-blue flex-shrink-0 mt-0.5" />
                  <div>
                    <div className="text-xs font-medium text-text-muted mb-0.5">Date</div>
                    <div className="font-medium text-navy dark:text-white">
                      {detailEvent.date}
                      {detailEvent.endDate && ` — ${detailEvent.endDate}`}
                    </div>
                  </div>
                </div>

                <div className="flex items-start gap-3 p-4 bg-light-gray dark:bg-muted rounded-lg">
                  <MapPin size={18} className="text-teal flex-shrink-0 mt-0.5" />
                  <div>
                    <div className="text-xs font-medium text-text-muted mb-0.5">Lieu</div>
                    <div className="font-medium text-navy dark:text-white">{detailEvent.location}</div>
                  </div>
                </div>

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
                              <Briefcase size={14} className="flex-shrink-0 text-teal" />
                              <span>{speaker.institution}</span>
                            </div>
                            <div className="flex items-start gap-2 mt-2">
                              <Users size={14} className="flex-shrink-0 text-warning mt-0.5" />
                              <div>
                                <div className="font-medium text-navy dark:text-white text-xs">{speaker.role}</div>
                                <div className="text-xs italic">{speaker.subject}</div>
                              </div>
                            </div>
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>
                )}

                <div>
                  <div className="text-sm font-medium text-navy dark:text-white mb-2">Description</div>
                  <p className="text-sm text-text-secondary leading-relaxed">{detailEvent.description}</p>
                </div>

                {detailEvent.status !== 'A_VENIR' && detailEvent.photos && detailEvent.photos.length > 0 && (
                  <div>
                    <div className="text-sm font-medium text-navy dark:text-white mb-3">Photos de l'événement</div>
                    <PhotoGallery photos={detailEvent.photos.map((url: string) => ({ url }))} columns={3} />
                  </div>
                )}
              </div>
            </div>
            <div className="p-6 border-t border-surface-border flex justify-end gap-3">
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
    <Card className="hover:shadow-card-hover transition-shadow">
      <CardContent className="p-5">
        <div className="flex items-start justify-between gap-3 mb-3">
          <div className="flex-1">
            <Badge variant={sc.variant} className="mb-2">{sc.label}</Badge>
            <h3 className="font-bold text-navy dark:text-white">{event.title}</h3>
          </div>
          <Badge variant="info" className="flex-shrink-0 text-xs">{typeLabels[event.type]}</Badge>
        </div>
        <p className="text-sm text-text-secondary mb-4 line-clamp-2">{event.description}</p>
        <div className="space-y-2 text-sm text-text-secondary mb-4">
          <div className="flex items-center gap-2">
            <Calendar size={14} className="text-accent-blue flex-shrink-0" />
            <span>{event.date}{event.endDate ? ` — ${event.endDate}` : ''}</span>
          </div>
          <div className="flex items-center gap-2">
            <MapPin size={14} className="text-teal flex-shrink-0" />
            <span>{event.location}</span>
          </div>
          {event.speakers && event.speakers.length > 0 && (
            <div className="flex items-center gap-2">
              <Users size={14} className="text-warning flex-shrink-0" />
              <span className="line-clamp-1">{event.speakers.map(s => s.name).join(', ')}</span>
            </div>
          )}
        </div>
        <button
          onClick={onViewDetails}
          className="flex items-center gap-2 text-sm text-accent-blue hover:underline font-medium"
        >
          <Eye size={14} /> Voir les détails
        </button>
      </CardContent>
    </Card>
  );
}
