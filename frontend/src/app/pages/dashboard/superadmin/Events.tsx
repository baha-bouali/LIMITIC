import { useEffect, useState, useMemo } from 'react';
import { Card, CardContent, CardHeader } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { Button } from '../../../components/ui/Button';
import { SearchFilter } from '../../../components/shared/SearchFilter';
import { ConfirmDialog } from '../../../components/shared/ConfirmDialog';
import { PhotoGallery } from '../../../components/shared/PhotoGallery';
import {
  Calendar, MapPin, Users, Image, Pencil, Trash2, X, Upload, Plus, Eye,
  Mail, Briefcase, Loader2, AlertCircle, RefreshCw,
} from 'lucide-react';
import { toast } from 'sonner';
import { clsx } from 'clsx';
import {
  useGetEventsQuery,
  useCreateEventMutation,
  useUpdateEventMutation,
  useDeleteEventMutation,
  useAddSpeakerMutation,
  useUpdateSpeakerMutation,
  useDeleteSpeakerMutation,
  EVENT_TYPE_OPTIONS,
  toBackendEventType,
  type EventDto,
  type SpeakerDto,
  type CreateEventRequest,
  type UpdateEventRequest,
  type SpeakerRequest,
  type GetEventsParams,
} from '../../../api/eventsApi';
import { useGetResearchAxesQuery } from '../../../api/profilesApi';
import { apiBaseUrl } from '../../../api/baseApi';

// ─── Types ────────────────────────────────────────────────────────────────────

type EventStatus = 'A_VENIR' | 'EN_COURS' | 'PASSE';
type EventTypeName = 'SÉMINAIRE' | 'ATELIER' | 'CONFÉRENCE' | "JOURNÉE D'ÉTUDE" | 'SOUTENANCE' | 'AUTRE' | string;

// ─── Helpers ──────────────────────────────────────────────────────────────────

/** Build a full URL to a stored photo served from the backend */
const getPhotoUrl = (fileName: string) =>
  `${apiBaseUrl.replace(/\/api$/, '')}/uploads/events/${fileName}`;

const formatDate = (iso?: string | null) => {
  if (!iso) return 'Date invalide';

  const parsedDate = new Date(iso);
  if (Number.isNaN(parsedDate.getTime())) return 'Date invalide';

  return parsedDate.toLocaleDateString('fr-FR');
};

/** ISO date string → "YYYY-MM-DD" for <input type="date"> */
const toDateInputValue = (iso?: string | null) => {
  if (!iso) return '';

  const parsedDate = new Date(iso);
  if (Number.isNaN(parsedDate.getTime())) return '';

  return parsedDate.toISOString().split('T')[0];
};

const STATUS_CONFIG: Record<EventStatus, { variant: 'info' | 'success' | 'default'; label: string }> = {
  A_VENIR: { variant: 'info', label: 'À venir' },
  EN_COURS: { variant: 'success', label: 'En cours' },
  PASSE: { variant: 'default', label: 'Passé' },
};

const TYPE_COLORS: Record<string, string> = {
  'SÉMINAIRE':       'bg-[#EFF6FF] text-[#1D4ED8]',
  'ATELIER':         'bg-[#F0FDF4] text-[#15803D]',
  'CONFÉRENCE':      'bg-[#FFF7ED] text-[#C2410C]',
  "JOURNÉE D'ÉTUDE": 'bg-[#FAF5FF] text-[#7E22CE]',
  'SOUTENANCE':      'bg-[#FEF3C7] text-[#92400E]',
  'AUTRE':           'bg-[#E5E7EB] text-[#374151]',
  // English fallbacks in case a type reaches the UI before normalization
  'CONFERENCE':      'bg-[#FFF7ED] text-[#C2410C]',
  'SEMINAR':         'bg-[#EFF6FF] text-[#1D4ED8]',
  'WORKSHOP':        'bg-[#F0FDF4] text-[#15803D]',
  'DEFENSE':         'bg-[#FEF3C7] text-[#92400E]',
  'OTHER':           'bg-[#E5E7EB] text-[#374151]',
  'STUDY DAY':       'bg-[#FAF5FF] text-[#7E22CE]',
};

const getStatusBadge = (status: string) =>
  STATUS_CONFIG[status as EventStatus] ?? { variant: 'default' as const, label: status };

const getTypeBadge = (type: string) =>
  TYPE_COLORS[type?.toUpperCase()?.trim()] ?? 'bg-light-gray text-text-secondary';

const speakerFullName = (s: SpeakerDto) => `${s.firstName} ${s.lastName}`.trim();

// ─── Main Page ────────────────────────────────────────────────────────────────

export default function AdminEvents() {
  const [searchQuery, setSearchQuery] = useState('');
  const [activeFilters, setActiveFilters] = useState<Record<string, string | string[]>>({});
  const [showForm, setShowForm] = useState(false);
  const [editingEvent, setEditingEvent] = useState<EventDto | null>(null);
  const [deleteConfirm, setDeleteConfirm] = useState<EventDto | null>(null);
  const [detailEvent, setDetailEvent] = useState<EventDto | null>(null);

  const getFilterValues = (value: string | string[] | undefined) =>
    Array.isArray(value) ? value : value ? [value] : [];

  // Build query params from active filters + search
  const queryParams = useMemo<GetEventsParams>(() => {
    const params: GetEventsParams = {};
    if (searchQuery) params.q = searchQuery;
    const statusValues = getFilterValues(activeFilters.status);
    const typeValues = getFilterValues(activeFilters.type);
    if (statusValues.length === 1) params.status = statusValues[0];
    if (typeValues.length === 1) params.type = toBackendEventType(typeValues[0]);
    return params;
  }, [searchQuery, activeFilters]);

  const { data: allEvents = [], isLoading, isError, refetch } = useGetEventsQuery(queryParams);
  const [deleteEvent, { isLoading: isDeleting }] = useDeleteEventMutation();

  // Client-side filtering on top of server results (handles multi-select + UI-only search)
  const filteredEvents = useMemo(() => {
    return allEvents.filter((event) => {
      const matchesSearch =
        !searchQuery ||
        event.title.toLowerCase().includes(searchQuery.toLowerCase()) ||
        event.location.toLowerCase().includes(searchQuery.toLowerCase());

      const matchesFilters = Object.entries(activeFilters).every(([key, values]) => {
        const selectedValues = getFilterValues(values);
        if (!selectedValues.length) return true;
        if (key === 'status') return selectedValues.includes(event.status);
        if (key === 'type') return selectedValues.some((value) => toBackendEventType(event.type) === toBackendEventType(value));
        return true;
      });

      return matchesSearch && matchesFilters;
    });
  }, [allEvents, searchQuery, activeFilters]);

  const filterGroups = [
    {
      id: 'status',
      label: 'Statut',
      options: [
        { id: 'avenir', label: 'À venir', value: 'A_VENIR' },
        { id: 'encours', label: 'En cours', value: 'EN_COURS' },
        { id: 'passe', label: 'Passé', value: 'PASSE' },
      ],
    },
    {
      id: 'type',
      label: 'Type',
      options: [
        ...EVENT_TYPE_OPTIONS.map((option) => ({
          id: option.value,
          label: option.label,
          value: option.value,
        })),
      ],
    },
  ];

  const handleConfirmDelete = async () => {
    if (!deleteConfirm) return;
    try {
      await deleteEvent(deleteConfirm.id).unwrap();
      toast.success(`"${deleteConfirm.title}" supprimé avec succès`);
    } catch {
      toast.error('Erreur lors de la suppression');
    } finally {
      setDeleteConfirm(null);
    }
  };

  // ── Render ────────────────────────────────────────────────────────────────

  return (
    <div className="space-y-6">
      {/* Header */}
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

      {/* Search & Filters */}
      <SearchFilter
        onSearchChange={setSearchQuery}
        filterGroups={filterGroups}
        onFilterChange={(filters) => setActiveFilters(filters)}
        searchPlaceholder="Rechercher un événement..."
      />

      {/* Loading state */}
      {isLoading && (
        <Card>
          <CardContent className="p-12 flex flex-col items-center gap-3">
            <Loader2 size={40} className="text-accent-blue animate-spin" />
            <p className="text-text-secondary">Chargement des événements…</p>
          </CardContent>
        </Card>
      )}

      {/* Error state */}
      {isError && !isLoading && (
        <Card>
          <CardContent className="p-12 flex flex-col items-center gap-4">
            <AlertCircle size={40} className="text-error" />
            <p className="text-text-secondary text-center">
              Impossible de charger les événements. Vérifiez votre connexion.
            </p>
            <Button variant="outlined" onClick={() => refetch()}>
              <RefreshCw size={16} />
              Réessayer
            </Button>
          </CardContent>
        </Card>
      )}

      {/* Empty state */}
      {!isLoading && !isError && filteredEvents.length === 0 && (
        <Card>
          <CardContent className="p-12 text-center">
            <Calendar size={64} className="mx-auto text-text-muted mb-4" />
            <h3 className="text-xl font-bold text-navy dark:text-white mb-2">Aucun événement trouvé</h3>
            <p className="text-text-secondary">
              {searchQuery || Object.values(activeFilters).some((v) => getFilterValues(v).length > 0)
                ? 'Aucun résultat ne correspond à vos critères'
                : 'Aucun événement enregistré'}
            </p>
          </CardContent>
        </Card>
      )}

      {/* Events Grid */}
      {!isLoading && !isError && filteredEvents.length > 0 && (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {filteredEvents.map((event) => {
            const hasPhotos = event.photoFileNames?.length > 0;
            const statusCfg = getStatusBadge(event.status);

            return (
              <Card key={event.id} className="hover:shadow-lg transition-shadow overflow-hidden">
                {/* Card banner */}
                <div className="h-32 relative overflow-hidden bg-gradient-to-br from-navy to-accent-blue flex items-center justify-center text-white text-5xl font-bold">
                  {event.type?.charAt(0) ?? '?'}
                  <div className="absolute top-3 right-3">
                    <Badge
                      variant={statusCfg.variant}
                      style={
                        event.status === 'PASSE' && hasPhotos
                          ? { backgroundColor: '#059669', color: 'white' }
                          : {}
                      }
                    >
                      {event.status === 'PASSE' && hasPhotos ? (
                        <><Image size={12} className="mr-1" />Photos disponibles</>
                      ) : (
                        statusCfg.label
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
                      <span>
                        {formatDate(event.startDate)}
                        {event.endDate && event.endDate !== event.startDate
                          ? ` — ${formatDate(event.endDate)}`
                          : ''}
                      </span>
                    </div>
                    <div className="flex items-center gap-2">
                      <MapPin size={16} className="flex-shrink-0 text-accent-blue" />
                      <span className="line-clamp-1">{event.location}</span>
                    </div>
                    <div className="flex items-center gap-2">
                      <Users size={16} className="flex-shrink-0 text-accent-blue" />
                      <span>
                        {event.speakers?.length ?? 0} intervenant
                        {(event.speakers?.length ?? 0) !== 1 ? 's' : ''}
                      </span>
                    </div>
                  </div>
                </CardHeader>

                <CardContent className="border-t border-surface-border">
                  <div className="flex gap-2">
                    <Button
                      variant="outlined"
                      className="flex-1"
                      onClick={() => {
                        if (event.status === 'PASSE' && hasPhotos) {
                          setDetailEvent(event);
                        } else {
                          setEditingEvent(event);
                          setShowForm(true);
                        }
                      }}
                    >
                      {event.status === 'PASSE' && hasPhotos ? (
                        <><Eye size={16} />Voir les détails</>
                      ) : (
                        <><Pencil size={16} />Modifier</>
                      )}
                    </Button>
                    <Button
                      variant="outlined"
                      className="text-error hover:bg-error/5 hover:border-error"
                      onClick={() => setDeleteConfirm(event)}
                      disabled={isDeleting}
                    >
                      <Trash2 size={16} />
                    </Button>
                  </div>
                </CardContent>
              </Card>
            );
          })}
        </div>
      )}

      {/* Create / Edit Modal */}
      {showForm && (
        <EventFormModal
          event={editingEvent}
          onClose={() => { setShowForm(false); setEditingEvent(null); }}
        />
      )}

      {/* Detail Modal */}
      {detailEvent && (
        <EventDetailModal
          event={detailEvent}
          onClose={() => setDetailEvent(null)}
          onEdit={() => { setDetailEvent(null); setEditingEvent(detailEvent); setShowForm(true); }}
        />
      )}

      {/* Delete Confirmation */}
      {deleteConfirm && (
        <ConfirmDialog
          isOpen
          onClose={() => setDeleteConfirm(null)}
          onConfirm={handleConfirmDelete}
          title="Supprimer cet événement"
          description={`Êtes-vous sûr de vouloir supprimer "${deleteConfirm.title}" ? Cette action est irréversible.`}
          confirmText="Supprimer"
          variant="danger"
        />
      )}
    </div>
  );
}

// ─── Detail Modal ─────────────────────────────────────────────────────────────

interface EventDetailModalProps {
  event: EventDto;
  onClose: () => void;
  onEdit: () => void;
}

function EventDetailModal({ event, onClose, onEdit }: EventDetailModalProps) {
  const hasPhotos = event.photoFileNames?.length > 0;
  const statusCfg = getStatusBadge(event.status);

  return (
    <div
      className="fixed inset-0 bg-black/50 flex items-center justify-center z-[60] p-4"
      onClick={onClose}
    >
      <div
        className="bg-white dark:bg-card rounded-2xl shadow-modal max-w-2xl w-full max-h-[90vh] overflow-y-auto"
        onClick={(e) => e.stopPropagation()}
      >
        {/* Header */}
        <div className="p-6 border-b border-surface-border flex items-center justify-between">
          <h2 className="text-lg font-bold text-navy dark:text-white">Détails de l'événement</h2>
          <button onClick={onClose} className="p-2 hover:bg-light-gray dark:hover:bg-muted rounded-lg">
            <X size={18} />
          </button>
        </div>

        <div className="p-6 space-y-5">
          {/* Badges */}
          <div className="flex flex-wrap gap-2">
            <Badge variant={statusCfg.variant}>{statusCfg.label}</Badge>
            <Badge className={clsx(getTypeBadge(event.type))}>{event.type}</Badge>
          </div>

          <h3 className="text-xl font-bold text-navy dark:text-white leading-snug">{event.title}</h3>

          {/* Date */}
          <div className="flex items-start gap-3 p-4 bg-light-gray dark:bg-muted rounded-lg">
            <Calendar size={18} className="text-accent-blue flex-shrink-0 mt-0.5" />
            <div>
              <div className="text-xs font-medium text-text-muted mb-0.5">Date</div>
              <div className="font-medium text-navy dark:text-white">
                {formatDate(event.startDate)}
                {event.endDate && event.endDate !== event.startDate
                  ? ` — ${formatDate(event.endDate)}`
                  : ''}
              </div>
            </div>
          </div>

          {/* Location */}
          <div className="flex items-start gap-3 p-4 bg-light-gray dark:bg-muted rounded-lg">
            <MapPin size={18} className="text-teal flex-shrink-0 mt-0.5" />
            <div>
              <div className="text-xs font-medium text-text-muted mb-0.5">Lieu</div>
              <div className="font-medium text-navy dark:text-white">{event.location}</div>
            </div>
          </div>

          {/* Speakers */}
          {event.speakers && event.speakers.length > 0 && (
            <div>
              <div className="text-sm font-medium text-navy dark:text-white mb-3">Intervenants</div>
              <div className="space-y-3">
                {event.speakers.map((speaker) => (
                  <div key={speaker.id} className="p-4 bg-light-gray dark:bg-muted rounded-lg">
                    <div className="font-medium text-navy dark:text-white mb-2">
                      {speakerFullName(speaker)}
                    </div>
                    <div className="space-y-1 text-sm text-text-secondary">
                      <div className="flex items-center gap-2">
                        <Mail size={14} className="flex-shrink-0 text-accent-blue" />
                        <span>{speaker.email}</span>
                      </div>
                      {speaker.institution && (
                        <div className="flex items-center gap-2">
                          <Briefcase size={14} className="flex-shrink-0 text-teal" />
                          <span>{speaker.institution}</span>
                        </div>
                      )}
                      {(speaker.role || speaker.subject) && (
                        <div className="flex items-start gap-2 mt-2">
                          <Users size={14} className="flex-shrink-0 text-warning mt-0.5" />
                          <div>
                            {speaker.role && (
                              <div className="font-medium text-navy dark:text-white text-xs">{speaker.role}</div>
                            )}
                            {speaker.subject && (
                              <div className="text-xs italic">{speaker.subject}</div>
                            )}
                          </div>
                        </div>
                      )}
                    </div>
                  </div>
                ))}
              </div>
            </div>
          )}

          {/* Description */}
          <div>
            <div className="text-sm font-medium text-navy dark:text-white mb-2">Description</div>
            <p className="text-sm text-text-secondary leading-relaxed">{event.description}</p>
          </div>

          {/* Program */}
          {event.program && (
            <div>
              <div className="text-sm font-medium text-navy dark:text-white mb-2">Programme</div>
              <p className="text-sm text-text-secondary leading-relaxed whitespace-pre-wrap">{event.program}</p>
            </div>
          )}

          {/* Photo gallery */}
          {hasPhotos && (
            <div>
              <div className="text-sm font-medium text-navy dark:text-white mb-3">Photos de l'événement</div>
              <PhotoGallery
                photos={event.photoFileNames.map((fn) => ({ url: getPhotoUrl(fn) }))}
                columns={3}
              />
            </div>
          )}
        </div>

        <div className="p-6 border-t border-surface-border flex justify-end gap-3">
          <Button variant="outlined" onClick={onClose}>Fermer</Button>
          <Button onClick={onEdit}>
            <Pencil size={16} />
            Modifier
          </Button>
        </div>
      </div>
    </div>
  );
}

// ─── Form Modal ───────────────────────────────────────────────────────────────

interface EventFormModalProps {
  event: EventDto | null;
  onClose: () => void;
}

interface SpeakerFormState {
  firstName: string;
  lastName: string;
  email: string;
  institution: string;
  role: string;
  subject: string;
}

interface EventFormState {
  type: CreateEventRequest['type'];
  researchAxisId: string;
  title: string;
  startDate: string;
  endDate: string;
  location: string;
  description: string;
  program: string;
}

const emptySpeaker = (): SpeakerFormState => ({
  firstName: '',
  lastName: '',
  email: '',
  institution: '',
  role: '',
  subject: '',
});

function EventFormModal({ event, onClose }: EventFormModalProps) {
  const isEditing = !!event;
  const { data: researchAxes = [] } = useGetResearchAxesQuery();

  // ── Form state ──────────────────────────────────────────────────────────────
  const [formData, setFormData] = useState<EventFormState>({
    type: toBackendEventType(event?.type ?? 'SÉMINAIRE'),
    researchAxisId: '',
    title: event?.title ?? '',
    startDate: toDateInputValue(event?.startDate),
    endDate: toDateInputValue(event?.endDate),
    location: event?.location ?? '',
    description: event?.description ?? '',
    program: event?.program ?? '',
  });

  useEffect(() => {
    if (!formData.researchAxisId && researchAxes.length > 0) {
      setFormData((current) => ({
        ...current,
        researchAxisId: researchAxes[0].id,
      }));
    }
  }, [formData.researchAxisId, researchAxes]);

  /** Speakers that already exist on the backend (edit mode) */
  const [existingSpeakers, setExistingSpeakers] = useState<SpeakerDto[]>(
    event?.speakers ?? [],
  );

  /** New speakers staged locally (not yet persisted when editing) */
  const [pendingSpeakers, setPendingSpeakers] = useState<SpeakerFormState[]>([]);

  const [currentSpeaker, setCurrentSpeaker] = useState<SpeakerFormState>(emptySpeaker());
  const [photos, setPhotos] = useState<File[]>([]);

  // ── Mutations ───────────────────────────────────────────────────────────────
  const [createEvent, { isLoading: isCreating }] = useCreateEventMutation();
  const [updateEvent, { isLoading: isUpdating }] = useUpdateEventMutation();
  const [addSpeaker] = useAddSpeakerMutation();
  const [deleteSpeakerMutation] = useDeleteSpeakerMutation();

  const isSaving = isCreating || isUpdating;

  // ── Speaker helpers ─────────────────────────────────────────────────────────
  const handleAddSpeaker = () => {
    const { firstName, lastName, email, institution, role } = currentSpeaker;
    if (!firstName.trim() || !lastName.trim() || !email.trim() || !institution.trim() || !role.trim()) {
      toast.error('Prénom, nom, email, institution et rôle sont requis');
      return;
    }
    setPendingSpeakers((prev) => [...prev, currentSpeaker]);
    setCurrentSpeaker(emptySpeaker());
  };

  const handleRemovePendingSpeaker = (idx: number) => {
    setPendingSpeakers((prev) => prev.filter((_, i) => i !== idx));
  };

  const handleRemoveExistingSpeaker = async (speaker: SpeakerDto) => {
    if (!event) return;
    try {
      await deleteSpeakerMutation({ eventId: event.id, speakerId: speaker.id }).unwrap();
      setExistingSpeakers((prev) => prev.filter((s) => s.id !== speaker.id));
      toast.success('Intervenant supprimé');
    } catch {
      toast.error("Impossible de supprimer l'intervenant");
    }
  };

  // ── Photo helpers ───────────────────────────────────────────────────────────
  const handlePhotoUpload = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = Array.from(e.target.files ?? []);
    setPhotos((prev) => [...prev, ...files]);
  };

  const handleRemovePhoto = (idx: number) => {
    setPhotos((prev) => prev.filter((_, i) => i !== idx));
  };

  // ── Submit ──────────────────────────────────────────────────────────────────
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    // Validate required fields
    if (!formData.title || !formData.startDate || !formData.location || !formData.description) {
      toast.error('Veuillez remplir tous les champs obligatoires');
      return;
    }

    if (!formData.researchAxisId) {
      toast.error('Veuillez sélectionner un axe de recherche');
      return;
    }

    try {
      if (isEditing && event) {
        // ── UPDATE ──────────────────────────────────────────────────────────
        const updateBody: UpdateEventRequest = {
          type: formData.type,
          title: formData.title,
          startDate: new Date(formData.startDate).toISOString(),
          endDate: new Date(formData.endDate || formData.startDate).toISOString(),
          location: formData.location,
          description: formData.description,
          program: formData.program || undefined,
          researchAxisId: formData.researchAxisId,
        };
        await updateEvent({ id: event.id, body: updateBody }).unwrap();

        // Add any pending speakers one by one
        for (const sp of pendingSpeakers) {
          const speakerBody: SpeakerRequest = {
            firstName: sp.firstName,
            lastName: sp.lastName,
            email: sp.email,
            institution: sp.institution || undefined,
            role: sp.role || undefined,
            subject: sp.subject || undefined,
          };
          await addSpeaker({ eventId: event.id, body: speakerBody }).unwrap();
        }

        toast.success('Événement modifié avec succès !');
      } else {
        // ── CREATE ──────────────────────────────────────────────────────────
        const allSpeakers = pendingSpeakers.map((sp) => ({
          firstName: sp.firstName,
          lastName: sp.lastName,
          email: sp.email,
          institution: sp.institution || undefined,
          role: sp.role || undefined,
          subject: sp.subject || undefined,
        }));

        const createBody: CreateEventRequest = {
          type: formData.type,
          title: formData.title,
          startDate: new Date(formData.startDate).toISOString(),
          endDate: new Date(formData.endDate || formData.startDate).toISOString(),
          location: formData.location,
          description: formData.description,
          program: formData.program || undefined,
          researchAxisId: formData.researchAxisId,
          speakers: allSpeakers,
        };

        await createEvent(createBody).unwrap();
        toast.success('Événement créé avec succès !');
      }

      onClose();
    } catch (err: unknown) {
      const msg =
        (err as { message?: string })?.message ??
        (isEditing ? 'Erreur lors de la modification' : 'Erreur lors de la création');
      toast.error(msg);
    }
  };

  // ── All speakers to display (existing + staged) ─────────────────────────────
  const allDisplaySpeakers = [
    ...existingSpeakers.map((s) => ({
      id: s.id,
      name: speakerFullName(s),
      email: s.email,
      institution: s.institution ?? '',
      role: s.role ?? '',
      subject: s.subject ?? '',
      isExisting: true as const,
      raw: s,
    })),
    ...pendingSpeakers.map((s, i) => ({
      id: `pending-${i}`,
      name: `${s.firstName} ${s.lastName}`.trim(),
      email: s.email,
      institution: s.institution,
      role: s.role,
      subject: s.subject,
      isExisting: false as const,
      raw: null,
    })),
  ];

  // ── Render ──────────────────────────────────────────────────────────────────
  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
      <div className="bg-white dark:bg-card rounded-2xl shadow-[0_8px_32px_rgba(15,37,87,.16)] max-w-2xl w-full max-h-[90vh] overflow-hidden flex flex-col">
        {/* Header */}
        <div className="px-6 py-4 border-b border-surface-border flex items-center justify-between">
          <h2 className="text-2xl font-bold text-navy dark:text-white">
            {isEditing ? "Modifier l'événement" : 'Créer un événement'}
          </h2>
          <button onClick={onClose} className="p-2 hover:bg-light-gray rounded-lg" disabled={isSaving}>
            <X size={24} />
          </button>
        </div>

        <form onSubmit={handleSubmit} className="flex-1 overflow-y-auto p-6 space-y-6">
          {/* Type */}
          <div>
            <label className="block text-sm font-medium mb-2">Type d'événement *</label>
            <select
              required
              value={formData.type}
              onChange={(e) => setFormData({ ...formData, type: e.target.value as CreateEventRequest['type'] })}
              className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background"
            >
              {EVENT_TYPE_OPTIONS.map((option) => (
                <option key={option.value} value={option.value}>
                  {option.label}
                </option>
              ))}
            </select>
          </div>

          {/* Research axis */}
          <div>
            <label className="block text-sm font-medium mb-2">Axe de recherche *</label>
            <select
              required
              value={formData.researchAxisId}
              onChange={(e) => setFormData({ ...formData, researchAxisId: e.target.value })}
              className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background"
            >
              <option value="">Sélectionner un axe</option>
              {researchAxes.map((axis) => (
                <option key={axis.id} value={axis.id}>
                  {axis.title}
                </option>
              ))}
            </select>
          </div>

          {/* Title */}
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

          {/* Description */}
          <div>
            <label className="block text-sm font-medium mb-2">Description *</label>
            <textarea
              required
              value={formData.description}
              onChange={(e) => setFormData({ ...formData, description: e.target.value })}
              rows={4}
              className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent resize-none dark:bg-input-background"
              placeholder="Description de l'événement…"
            />
          </div>

          {/* Program */}
          <div>
            <label className="block text-sm font-medium mb-2">Programme</label>
            <textarea
              value={formData.program}
              onChange={(e) => setFormData({ ...formData, program: e.target.value })}
              rows={3}
              className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent resize-none dark:bg-input-background"
              placeholder="Détail du programme (optionnel)…"
            />
          </div>

          {/* Dates */}
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium mb-2">Date de début *</label>
              <input
                type="date"
                required
                value={formData.startDate}
                onChange={(e) => setFormData({ ...formData, startDate: e.target.value })}
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

          {/* Location */}
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

          {/* ── Speakers section ─────────────────────────────────────────── */}
          <div>
            <label className="block text-sm font-medium mb-3">Intervenants</label>

            {/* New speaker input fields */}
            <div className="space-y-3 mb-3 p-4 border border-dashed border-surface-border rounded-lg">
              <div className="grid grid-cols-2 gap-3">
                <input
                  type="text"
                  value={currentSpeaker.firstName}
                  onChange={(e) => setCurrentSpeaker({ ...currentSpeaker, firstName: e.target.value })}
                  className="w-full px-4 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background"
                  placeholder="Prénom *"
                />
                <input
                  type="text"
                  value={currentSpeaker.lastName}
                  onChange={(e) => setCurrentSpeaker({ ...currentSpeaker, lastName: e.target.value })}
                  className="w-full px-4 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background"
                  placeholder="Nom *"
                />
              </div>
              <input
                type="email"
                value={currentSpeaker.email}
                onChange={(e) => setCurrentSpeaker({ ...currentSpeaker, email: e.target.value })}
                className="w-full px-4 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background"
                placeholder="Email *"
              />
              <input
                type="text"
                value={currentSpeaker.institution}
                onChange={(e) => setCurrentSpeaker({ ...currentSpeaker, institution: e.target.value })}
                className="w-full px-4 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background"
                placeholder="Institution *"
              />
              <div className="grid grid-cols-2 gap-3">
                <input
                  type="text"
                  value={currentSpeaker.role}
                  onChange={(e) => setCurrentSpeaker({ ...currentSpeaker, role: e.target.value })}
                  className="w-full px-4 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background"
                  placeholder="Rôle *"
                />
                <input
                  type="text"
                  value={currentSpeaker.subject}
                  onChange={(e) => setCurrentSpeaker({ ...currentSpeaker, subject: e.target.value })}
                  className="w-full px-4 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background"
                  placeholder="Sujet"
                />
              </div>
              <Button onClick={handleAddSpeaker} type="button" className="w-full">
                <Plus size={16} />
                Ajouter l'intervenant
              </Button>
            </div>

            {/* Speakers list */}
            {allDisplaySpeakers.length > 0 && (
              <div className="space-y-2">
                <div className="text-sm font-medium mb-2">
                  {allDisplaySpeakers.length} intervenant{allDisplaySpeakers.length > 1 ? 's' : ''}
                </div>
                {allDisplaySpeakers.map((sp, idx) => (
                  <div key={sp.id} className="p-3 bg-light-gray dark:bg-input-background rounded-lg">
                    <div className="flex items-start justify-between mb-2">
                      <div className="font-medium text-navy dark:text-white">{sp.name}</div>
                      <button
                        type="button"
                        onClick={() => {
                          if (sp.isExisting && sp.raw) {
                            handleRemoveExistingSpeaker(sp.raw);
                          } else {
                            handleRemovePendingSpeaker(
                              idx - existingSpeakers.length,
                            );
                          }
                        }}
                        className="p-1 hover:bg-error/10 rounded text-error"
                      >
                        <X size={16} />
                      </button>
                    </div>
                    <div className="text-sm text-text-secondary space-y-1">
                      <div className="flex items-center gap-2">
                        <Mail size={12} className="flex-shrink-0" />
                        <span>{sp.email}</span>
                      </div>
                      {sp.institution && (
                        <div className="flex items-center gap-2">
                          <Briefcase size={12} className="flex-shrink-0" />
                          <span>{sp.institution}</span>
                        </div>
                      )}
                      {sp.role && (
                        <div className="flex items-center gap-2">
                          <Users size={12} className="flex-shrink-0" />
                          <span className="font-medium">{sp.role}</span>
                        </div>
                      )}
                      {sp.subject && (
                        <div className="text-xs italic mt-1">{sp.subject}</div>
                      )}
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>

          {/* ── Photo upload (PASSE events only) ─────────────────────────── */}
          <div>
            <label className="block text-sm font-medium mb-3">
              Galerie photos{' '}
              <span className="text-text-muted font-normal">(événements passés)</span>
            </label>
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
                <p className="text-xs text-text-muted mt-1">PNG, JPG jusqu'à 5 MB chacune</p>
              </label>
            </div>

            {photos.length > 0 && (
              <div className="mt-3">
                <div className="text-sm font-medium mb-2">{photos.length} photo(s) sélectionnée(s)</div>
                <div className="grid grid-cols-3 gap-2">
                  {photos.map((file, idx) => (
                    <div key={idx} className="relative group">
                      <div className="aspect-video bg-light-gray rounded-lg flex items-center justify-center overflow-hidden">
                        <img
                          src={URL.createObjectURL(file)}
                          alt={file.name}
                          className="object-cover w-full h-full"
                        />
                      </div>
                      <button
                        type="button"
                        onClick={() => handleRemovePhoto(idx)}
                        className="absolute top-1 right-1 p-1 bg-error text-white rounded-full opacity-0 group-hover:opacity-100 transition-opacity"
                      >
                        <X size={12} />
                      </button>
                      <p className="text-xs text-text-secondary mt-1 truncate">{file.name}</p>
                    </div>
                  ))}
                </div>
              </div>
            )}
          </div>
        </form>

        {/* Footer */}
        <div className="px-6 py-4 border-t border-surface-border flex justify-end gap-3">
          <Button onClick={onClose} variant="outlined" disabled={isSaving}>
            Annuler
          </Button>
          <Button onClick={handleSubmit} disabled={isSaving}>
            {isSaving ? (
              <><Loader2 size={16} className="animate-spin" />{isEditing ? 'Modification…' : 'Création…'}</>
            ) : (
              isEditing ? 'Modifier' : 'Créer'
            )}
          </Button>
        </div>
      </div>
    </div>
  );
}