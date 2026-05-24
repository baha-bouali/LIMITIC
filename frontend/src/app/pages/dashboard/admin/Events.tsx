import { useState } from 'react';
import { Card, CardContent, CardHeader } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { Button } from '../../../components/ui/Button';
import { SearchFilter } from '../../../components/shared/SearchFilter';
import { ConfirmDialog } from '../../../components/shared/ConfirmDialog';
import { PhotoGallery } from '../../../components/shared/PhotoGallery';
import { Calendar, MapPin, Users, Image, Pencil, Trash2, X, Upload, Plus, Eye, Mail, Briefcase } from 'lucide-react';
import { toast } from 'sonner';
import { clsx } from 'clsx';

type EventStatus = 'A_VENIR' | 'EN_COURS' | 'PASSE';
type EventType = 'SÉMINAIRE' | 'ATELIER' | 'CONFÉRENCE' | 'JOURNÉE D\'ÉTUDE';

interface Speaker {
  name: string;
  email: string;
  institution: string;
  role: string;
  subject: string;
}

interface Event {
  id: string;
  type: EventType;
  title: string;
  date: string;
  endDate?: string;
  location: string;
  status: EventStatus;
  description: string;
  speakers: Speaker[];
  photoCount: number;
  hasPhotos: boolean;
  photos?: string[];
}

export default function AdminEvents() {
  const [searchQuery, setSearchQuery] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [editingEvent, setEditingEvent] = useState<Event | null>(null);
  const [deleteConfirm, setDeleteConfirm] = useState<Event | null>(null);
  const [activeFilters, setActiveFilters] = useState<Record<string, string[]>>({});
  const [detailEvent, setDetailEvent] = useState<Event | null>(null);

  const allEvents: Event[] = [
    {
      id: '1',
      type: 'SÉMINAIRE',
      title: 'Intelligence Artificielle et Santé',
      date: '2026-06-15',
      endDate: '2026-06-15',
      location: 'Amphithéâtre A, ISI',
      status: 'A_VENIR',
      description: 'Exploration des applications de l\'IA dans le domaine de la santé avec des experts internationaux.',
      speakers: [
        { name: 'Dr. Ahmed Ben Salem', email: 'ahmed.bensalem@limtic.tn', institution: 'LIMTIC, ISI', role: 'Conférencier principal', subject: 'IA pour le diagnostic médical' },
        { name: 'Prof. Marie Dubois', email: 'marie.dubois@univ.fr', institution: 'Université Paris-Saclay', role: 'Experte invitée', subject: 'Éthique de l\'IA en santé' }
      ],
      photoCount: 0,
      hasPhotos: false
    },
    {
      id: '2',
      type: 'ATELIER',
      title: 'Introduction au Deep Learning',
      date: '2026-06-22',
      endDate: '2026-06-22',
      location: 'Salle B12, ISI',
      status: 'A_VENIR',
      description: 'Atelier pratique pour les débutants en apprentissage profond.',
      speakers: [
        { name: 'Dr. Fatma Gharbi', email: 'fatma.gharbi@limtic.tn', institution: 'LIMTIC, ISI', role: 'Formatrice', subject: 'Réseaux de neurones profonds' }
      ],
      photoCount: 0,
      hasPhotos: false
    },
    {
      id: '3',
      type: 'CONFÉRENCE',
      title: 'LIMTIC Research Day 2026',
      date: '2026-07-05',
      location: 'Campus Universitaire',
      status: 'A_VENIR',
      description: 'Journée de présentation des travaux de recherche du laboratoire.',
      speakers: [
        { name: 'Dr. Mohamed Mezghani', email: 'mohamed.mezghani@limtic.tn', institution: 'LIMTIC, ISI', role: 'Modérateur', subject: 'Table ronde: Défis et perspectives' },
        { name: 'Dr. Karim Jebali', email: 'karim.jebali@limtic.tn', institution: 'LIMTIC, ISI', role: 'Présentateur', subject: 'IoT et réseaux intelligents' }
      ],
      photoCount: 0,
      hasPhotos: false
    },
    {
      id: '4',
      type: 'SÉMINAIRE',
      title: 'Blockchain et Sécurité',
      date: '2026-05-20',
      endDate: '2026-05-20',
      location: 'Amphithéâtre B, ISI',
      status: 'PASSE',
      description: 'Séminaire sur les applications de la blockchain en sécurité informatique.',
      speakers: [
        { name: 'Dr. Mohamed Mezghani', email: 'mohamed.mezghani@limtic.tn', institution: 'LIMTIC, ISI', role: 'Conférencier', subject: 'Blockchain et cybersécurité' },
        { name: 'Prof. Sarah Trabelsi', email: 'sarah.trabelsi@limtic.tn', institution: 'LIMTIC, ISI', role: 'Panéliste', subject: 'Applications décentralisées' }
      ],
      photoCount: 25,
      hasPhotos: true,
      photos: [
        'https://images.unsplash.com/photo-1540575467063-178a50c2df87?w=800&auto=format&fit=crop',
        'https://images.unsplash.com/photo-1591115765373-5207764f72e7?w=800&auto=format&fit=crop',
        'https://images.unsplash.com/photo-1505373877841-8d25f7d46678?w=800&auto=format&fit=crop',
        'https://images.unsplash.com/photo-1550751827-4bd374c3f58b?w=800&auto=format&fit=crop'
      ]
    },
    {
      id: '5',
      type: 'ATELIER',
      title: 'Big Data et Analytics',
      date: '2026-05-10',
      location: 'Salle C5, ISI',
      status: 'PASSE',
      description: 'Formation pratique sur les outils de Big Data.',
      speakers: [
        { name: 'Dr. Fatma Gharbi', email: 'fatma.gharbi@limtic.tn', institution: 'LIMTIC, ISI', role: 'Formatrice', subject: 'Hadoop et Spark' },
        { name: 'Mohamed Najjar', email: 'mohamed.najjar@limtic.tn', institution: 'LIMTIC, ISI', role: 'Assistant formateur', subject: 'Analytics en temps réel' }
      ],
      photoCount: 18,
      hasPhotos: true,
      photos: [
        'https://images.unsplash.com/photo-1563986768609-322da13575f3?w=800&auto=format&fit=crop',
        'https://images.unsplash.com/photo-1516321318423-f06f85e504b3?w=800&auto=format&fit=crop',
        'https://images.unsplash.com/photo-1531482615713-2afd69097998?w=800&auto=format&fit=crop'
      ]
    },
  ];

  const filterGroups = [
    {
      id: 'status',
      label: 'Statut',
      options: [
        { id: 'avenir', label: 'À venir', value: 'A_VENIR' },
        { id: 'encours', label: 'En cours', value: 'EN_COURS' },
        { id: 'passe', label: 'Passé', value: 'PASSE' },
      ]
    },
    {
      id: 'type',
      label: 'Type',
      options: [
        { id: 'seminaire', label: 'Séminaire', value: 'SÉMINAIRE' },
        { id: 'atelier', label: 'Atelier', value: 'ATELIER' },
        { id: 'conference', label: 'Conférence', value: 'CONFÉRENCE' },
        { id: 'journee', label: 'Journée d\'étude', value: 'JOURNÉE D\'ÉTUDE' },
      ]
    },
  ];

  const filteredEvents = allEvents.filter(event => {
    const matchesSearch = searchQuery === '' ||
      event.title.toLowerCase().includes(searchQuery.toLowerCase()) ||
      event.location.toLowerCase().includes(searchQuery.toLowerCase());

    const matchesFilters = Object.entries(activeFilters).every(([key, values]) => {
      if (values.length === 0) return true;
      if (key === 'status') return values.includes(event.status);
      if (key === 'type') return values.includes(event.type);
      return true;
    });

    return matchesSearch && matchesFilters;
  });

  const handleDelete = (event: Event) => {
    toast.success(`${event.title} supprimé avec succès`);
    setDeleteConfirm(null);
  };

  const getStatusBadge = (status: EventStatus) => {
    const config = {
      'A_VENIR': { variant: 'info' as const, label: 'À venir' },
      'EN_COURS': { variant: 'success' as const, label: 'En cours' },
      'PASSE': { variant: 'default' as const, label: 'Passé' }
    };
    return config[status];
  };

  const getTypeBadge = (type: EventType) => {
    const colors: Record<EventType, string> = {
      'SÉMINAIRE': 'bg-[#EFF6FF] text-[#1D4ED8]',
      'ATELIER': 'bg-[#F0FDF4] text-[#15803D]',
      'CONFÉRENCE': 'bg-[#FFF7ED] text-[#C2410C]',
      'JOURNÉE D\'ÉTUDE': 'bg-[#FAF5FF] text-[#7E22CE]'
    };
    return colors[type] || 'bg-light-gray text-text-secondary';
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-navy dark:text-white mb-2">Gestion des Événements</h1>
          <p className="text-text-secondary">Gérer les séminaires, ateliers et conférences du laboratoire</p>
        </div>
        <Button onClick={() => { setEditingEvent(null); setShowForm(true); }}>
          <Calendar size={18} />
          Créer un événement
        </Button>
      </div>

      {/* Search and Filters */}
      <SearchFilter
        onSearchChange={setSearchQuery}
        filterGroups={filterGroups}
        onFilterChange={setActiveFilters}
        searchPlaceholder="Rechercher un événement..."
      />

      {/* Events Grid */}
      {filteredEvents.length === 0 ? (
        <Card>
          <CardContent className="p-12 text-center">
            <Calendar size={64} className="mx-auto text-text-muted mb-4" />
            <h3 className="text-xl font-bold text-navy dark:text-white mb-2">
              Aucun événement trouvé
            </h3>
            <p className="text-text-secondary">
              {searchQuery || Object.values(activeFilters).some(v => v.length > 0)
                ? 'Aucun résultat ne correspond à vos critères'
                : 'Aucun événement enregistré'}
            </p>
          </CardContent>
        </Card>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {filteredEvents.map((event) => (
            <Card key={event.id} className="hover:shadow-lg transition-shadow overflow-hidden group">
              <div className="h-32 relative overflow-hidden bg-gradient-to-br from-navy to-accent-blue flex items-center justify-center text-white text-5xl font-bold">
                {event.type.charAt(0)}
                <div className="absolute top-3 right-3">
                  <Badge
                    variant={getStatusBadge(event.status).variant}
                    style={event.status === 'PASSE' && event.hasPhotos ? { backgroundColor: '#059669', color: 'white' } : {}}
                  >
                    {event.status === 'PASSE' && event.hasPhotos ? (
                      <><Image size={12} className="mr-1" /> Photos disponibles</>
                    ) : (
                      getStatusBadge(event.status).label
                    )}
                  </Badge>
                </div>
              </div>

              <CardHeader>
                <Badge className={clsx(getTypeBadge(event.type), 'mb-3')}>{event.type}</Badge>
                <h3 className="text-xl font-bold text-navy dark:text-white mb-2 line-clamp-2 min-h-[3.5rem]">
                  {event.title}
                </h3>
                <p className="text-sm text-text-secondary line-clamp-2 mb-4">{event.description}</p>
                <div className="space-y-2 text-sm text-text-secondary">
                  <div className="flex items-center gap-2">
                    <Calendar size={16} className="flex-shrink-0 text-accent-blue" />
                    <span>{new Date(event.date).toLocaleDateString('fr-FR')}</span>
                  </div>
                  <div className="flex items-center gap-2">
                    <MapPin size={16} className="flex-shrink-0 text-accent-blue" />
                    <span className="line-clamp-1">{event.location}</span>
                  </div>
                  <div className="flex items-center gap-2">
                    <Users size={16} className="flex-shrink-0 text-accent-blue" />
                    <span>{event.speakers.length} intervenant{event.speakers.length > 1 ? 's' : ''}</span>
                  </div>
                </div>
              </CardHeader>

              <CardContent className="border-t border-surface-border">
                <div className="flex gap-2">
                  {event.status === 'PASSE' && event.hasPhotos ? (
                    <Button
                      variant="outlined"
                      className="flex-1"
                      onClick={() => setDetailEvent(event)}
                    >
                      <Eye size={16} />
                      Voir les détails
                    </Button>
                  ) : (
                    <Button
                      variant="outlined"
                      className="flex-1"
                      onClick={() => { setEditingEvent(event); setShowForm(true); }}
                    >
                      <Pencil size={16} />
                      Modifier
                    </Button>
                  )}
                  <Button
                    variant="outlined"
                    className="text-error hover:bg-error/5 hover:border-error"
                    onClick={() => setDeleteConfirm(event)}
                  >
                    <Trash2 size={16} />
                  </Button>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      )}

      {/* Event Form Modal */}
      {showForm && (
        <EventFormModal
          event={editingEvent}
          onClose={() => { setShowForm(false); setEditingEvent(null); }}
        />
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
                <Badge variant={getStatusBadge(detailEvent.status).variant}>{getStatusBadge(detailEvent.status).label}</Badge>
                <Badge className={clsx(getTypeBadge(detailEvent.type))}>{detailEvent.type}</Badge>
              </div>
              <h3 className="text-xl font-bold text-navy dark:text-white leading-snug">{detailEvent.title}</h3>

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

                {detailEvent.status === 'PASSE' && detailEvent.photos && detailEvent.photos.length > 0 && (
                  <div>
                    <div className="text-sm font-medium text-navy dark:text-white mb-3">Photos de l'événement</div>
                    <PhotoGallery photos={detailEvent.photos.map((url: string) => ({ url }))} columns={3} />
                  </div>
                )}
              </div>
            </div>
            <div className="p-6 border-t border-surface-border flex justify-end gap-3">
              <Button variant="outlined" onClick={() => setDetailEvent(null)}>Fermer</Button>
              <Button onClick={() => { setDetailEvent(null); setEditingEvent(detailEvent); setShowForm(true); }}>
                <Pencil size={16} />
                Modifier
              </Button>
            </div>
          </div>
        </div>
      )}

      {/* Delete Confirmation */}
      {deleteConfirm && (
        <ConfirmDialog
          isOpen={true}
          onClose={() => setDeleteConfirm(null)}
          onConfirm={() => handleDelete(deleteConfirm)}
          title="Supprimer cet événement"
          description={`Êtes-vous sûr de vouloir supprimer "${deleteConfirm.title}" ? Cette action est irréversible.`}
          confirmText="Supprimer"
          variant="danger"
        />
      )}
    </div>
  );
}

interface EventFormModalProps {
  event: Event | null;
  onClose: () => void;
}

function EventFormModal({ event, onClose }: EventFormModalProps) {
  const [formData, setFormData] = useState({
    type: event?.type || 'SÉMINAIRE' as EventType,
    title: event?.title || '',
    date: event?.date || '',
    endDate: event?.endDate || '',
    location: event?.location || '',
    description: event?.description || '',
    status: event?.status || 'A_VENIR' as EventStatus,
    speakers: event?.speakers || [] as Speaker[],
    photos: [] as File[],
  });

  const [currentSpeaker, setCurrentSpeaker] = useState<Speaker>({
    name: '',
    email: '',
    institution: '',
    role: '',
    subject: ''
  });

  const handleAddSpeaker = () => {
    if (currentSpeaker.name.trim() && currentSpeaker.email.trim() && currentSpeaker.institution.trim()) {
      setFormData({ ...formData, speakers: [...formData.speakers, currentSpeaker] });
      setCurrentSpeaker({ name: '', email: '', institution: '', role: '', subject: '' });
    }
  };

  const handleRemoveSpeaker = (index: number) => {
    setFormData({ ...formData, speakers: formData.speakers.filter((_, i) => i !== index) });
  };

  const handlePhotoUpload = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = Array.from(e.target.files || []);
    setFormData({ ...formData, photos: [...formData.photos, ...files] });
  };

  const handleRemovePhoto = (index: number) => {
    setFormData({ ...formData, photos: formData.photos.filter((_, i) => i !== index) });
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    toast.success(event ? 'Événement modifié avec succès!' : 'Événement créé avec succès!');
    onClose();
  };

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
      <div className="bg-white dark:bg-card rounded-2xl shadow-[0_8px_32px_rgba(15,37,87,.16)] max-w-2xl w-full max-h-[90vh] overflow-hidden flex flex-col">
        <div className="px-6 py-4 border-b border-surface-border flex items-center justify-between">
          <h2 className="text-2xl font-bold text-navy dark:text-white">
            {event ? 'Modifier l\'événement' : 'Créer un événement'}
          </h2>
          <button onClick={onClose} className="p-2 hover:bg-light-gray rounded-lg">
            <X size={24} />
          </button>
        </div>

        <form onSubmit={handleSubmit} className="flex-1 overflow-y-auto p-6 space-y-6">
          <div>
            <label className="block text-sm font-medium mb-2">Type d'événement *</label>
            <select
              required
              value={formData.type}
              onChange={(e) => setFormData({ ...formData, type: e.target.value as EventType })}
              className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background"
            >
              <option value="SÉMINAIRE">Séminaire</option>
              <option value="ATELIER">Atelier</option>
              <option value="CONFÉRENCE">Conférence</option>
              <option value="JOURNÉE D'ÉTUDE">Journée d'étude</option>
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium mb-2">Titre *</label>
            <input
              type="text"
              required
              value={formData.title}
              onChange={(e) => setFormData({ ...formData, title: e.target.value })}
              className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background"
              placeholder="Titre de l'événement"
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-2">Description *</label>
            <textarea
              required
              value={formData.description}
              onChange={(e) => setFormData({ ...formData, description: e.target.value })}
              rows={4}
              className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent resize-none dark:bg-input-background"
              placeholder="Description de l'événement..."
            />
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium mb-2">Date de début *</label>
              <input
                type="date"
                required
                value={formData.date}
                onChange={(e) => setFormData({ ...formData, date: e.target.value })}
                className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background"
              />
            </div>
            <div>
              <label className="block text-sm font-medium mb-2">Date de fin</label>
              <input
                type="date"
                value={formData.endDate}
                onChange={(e) => setFormData({ ...formData, endDate: e.target.value })}
                className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background"
              />
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium mb-2">Lieu *</label>
            <input
              type="text"
              required
              value={formData.location}
              onChange={(e) => setFormData({ ...formData, location: e.target.value })}
              className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background"
              placeholder="Amphithéâtre A, ISI"
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-2">Statut *</label>
            <select
              required
              value={formData.status}
              onChange={(e) => setFormData({ ...formData, status: e.target.value as EventStatus })}
              className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background"
            >
              <option value="A_VENIR">À venir</option>
              <option value="EN_COURS">En cours</option>
              <option value="PASSE">Passé</option>
            </select>
          </div>

          {/* Intervenants Section */}
          <div>
            <label className="block text-sm font-medium mb-3">Intervenants</label>
            <div className="space-y-3 mb-3">
              <input
                type="text"
                value={currentSpeaker.name}
                onChange={(e) => setCurrentSpeaker({ ...currentSpeaker, name: e.target.value })}
                className="w-full px-4 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background"
                placeholder="Nom complet (ex: Dr. Karim Jebali)"
              />
              <input
                type="email"
                value={currentSpeaker.email}
                onChange={(e) => setCurrentSpeaker({ ...currentSpeaker, email: e.target.value })}
                className="w-full px-4 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background"
                placeholder="Email (ex: karim.jebali@limtic.tn)"
              />
              <input
                type="text"
                value={currentSpeaker.institution}
                onChange={(e) => setCurrentSpeaker({ ...currentSpeaker, institution: e.target.value })}
                className="w-full px-4 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background"
                placeholder="Institution (ex: LIMTIC, ISI)"
              />
              <input
                type="text"
                value={currentSpeaker.role}
                onChange={(e) => setCurrentSpeaker({ ...currentSpeaker, role: e.target.value })}
                className="w-full px-4 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background"
                placeholder="Rôle (ex: Modérateur, Conférencier)"
              />
              <input
                type="text"
                value={currentSpeaker.subject}
                onChange={(e) => setCurrentSpeaker({ ...currentSpeaker, subject: e.target.value })}
                className="w-full px-4 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background"
                placeholder="Sujet (ex: Table ronde: Défis et perspectives)"
              />
              <Button onClick={handleAddSpeaker} type="button" className="w-full">
                <Plus size={16} />
                Ajouter l'intervenant
              </Button>
            </div>

            {formData.speakers.length > 0 && (
              <div className="space-y-2">
                <div className="text-sm font-medium mb-2">{formData.speakers.length} intervenant(s)</div>
                {formData.speakers.map((speaker, idx) => (
                  <div key={idx} className="p-3 bg-light-gray dark:bg-input-background rounded-lg">
                    <div className="flex items-start justify-between mb-2">
                      <div className="font-medium text-navy dark:text-white">{speaker.name}</div>
                      <button
                        type="button"
                        onClick={() => handleRemoveSpeaker(idx)}
                        className="p-1 hover:bg-error/10 rounded text-error"
                      >
                        <X size={16} />
                      </button>
                    </div>
                    <div className="text-sm text-text-secondary space-y-1">
                      <div className="flex items-center gap-2">
                        <Mail size={12} className="flex-shrink-0" />
                        <span>{speaker.email}</span>
                      </div>
                      <div className="flex items-center gap-2">
                        <Briefcase size={12} className="flex-shrink-0" />
                        <span>{speaker.institution}</span>
                      </div>
                      {speaker.role && (
                        <div className="flex items-center gap-2">
                          <Users size={12} className="flex-shrink-0" />
                          <span className="font-medium">{speaker.role}</span>
                        </div>
                      )}
                      {speaker.subject && (
                        <div className="text-xs italic mt-1">{speaker.subject}</div>
                      )}
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>

          {/* Photos Gallery Section - Only for past events */}
          {formData.status !== 'A_VENIR' && formData.status === 'PASSE' && (
            <div>
              <label className="block text-sm font-medium mb-3">Galerie photos</label>
              <div className="border-2 border-dashed border-surface-border rounded-lg p-6 text-center">
                <input
                  type="file"
                  id="photo-upload"
                  accept="image/*"
                  multiple
                  onChange={handlePhotoUpload}
                  className="hidden"
                />
                <label htmlFor="photo-upload" className="cursor-pointer">
                  <Upload size={32} className="mx-auto text-accent-blue mb-2" />
                  <p className="text-sm font-medium">Cliquer pour ajouter des photos</p>
                  <p className="text-xs text-text-muted mt-1">PNG, JPG jusqu'à 5MB chacune</p>
                </label>
              </div>

              {formData.photos.length > 0 && (
                <div className="mt-3">
                  <div className="text-sm font-medium mb-2">{formData.photos.length} photo(s) sélectionnée(s)</div>
                  <div className="grid grid-cols-3 gap-2">
                    {formData.photos.map((photo, idx) => (
                      <div key={idx} className="relative group">
                        <div className="aspect-video bg-light-gray rounded-lg flex items-center justify-center">
                          <Image size={24} className="text-text-muted" />
                        </div>
                        <button
                          type="button"
                          onClick={() => handleRemovePhoto(idx)}
                          className="absolute top-1 right-1 p-1 bg-error text-white rounded-full opacity-0 group-hover:opacity-100 transition-opacity"
                        >
                          <X size={12} />
                        </button>
                        <p className="text-xs text-text-secondary mt-1 truncate">{photo.name}</p>
                      </div>
                    ))}
                  </div>
                </div>
              )}
            </div>
          )}
        </form>

        <div className="px-6 py-4 border-t border-surface-border flex justify-end gap-3">
          <Button onClick={onClose} variant="outlined">Annuler</Button>
          <Button onClick={handleSubmit}>{event ? 'Modifier' : 'Créer'}</Button>
        </div>
      </div>
    </div>
  );
}
