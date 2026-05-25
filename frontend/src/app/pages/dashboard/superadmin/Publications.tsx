import { useMemo, useState } from 'react';
import { FileText, Loader2, CheckCircle2, XCircle, Trash2, Pencil } from 'lucide-react';
import { toast } from 'sonner';
import { clsx } from 'clsx';

import { Card, CardContent } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { Button } from '../../../components/ui/Button';
import { SearchFilter } from '../../../components/shared/SearchFilter';
import { ConfirmDialog } from '../../../components/shared/ConfirmDialog';
import { PublicationForm } from '../../../components/publications/PublicationForm';
import { useGetPublicUsersQuery } from '../../../api/usersApi';
import { useGetResearchAxesQuery } from '../../../api/profilesApi';
import {
  useCreateDashboardPublicationMutation,
  useDeleteDashboardPublicationMutation,
  useGetDashboardPublicationsQuery,
  useLazyGetDashboardPublicationByIdQuery,
  useRejectDashboardPublicationMutation,
  useUpdateDashboardPublicationMutation,
  useValidateDashboardPublicationMutation,
  type CreateDashboardPublicationRequest,
  type DashboardPublicationDetailDto,
  type DashboardPublicationSummaryDto,
  type DashboardPublicationsQueryParams,
} from '../../../api/dashboardPublicationsApi';

type PublicationType = 'ARTICLE_JOURNAL' | 'CONFERENCE_INT' | 'CONFERENCE_NAT' | 'CHAPITRE_OUVRAGE' | 'RAPPORT_TECHNIQUE';
type PublicationStatus = 'Draft' | 'Submitted' | 'Published' | 'Rejected';
type PublicationVisibility = 'PUBLIQUE' | 'PRIVEE';

type PublicationFormData = {
  type: PublicationType;
  title: string;
  year: number;
  abstract: string;
  keywords: string[];
  visibility: PublicationVisibility;
  axes: string[];
  journalName: string;
  volume: string;
  issue: string;
  pages: string;
  doi: string;
  quartile: 'Q1' | 'Q2' | 'Q3' | 'Q4';
  conferenceName: string;
  location: string;
  coreRanking: 'A*' | 'A' | 'B' | 'C';
  bookTitle: string;
  editor: string;
  isbn: string;
  reportNumber: string;
  institution: string;
  authors: string[];
};

const statusConfig: Record<PublicationStatus, { label: string; variant: 'default' | 'warning' | 'success' | 'error' }> = {
  Draft: { label: 'Brouillon', variant: 'default' },
  Submitted: { label: 'Soumis', variant: 'warning' },
  Published: { label: 'Publié', variant: 'success' },
  Rejected: { label: 'Rejeté', variant: 'error' },
};

const typeLabels: Record<string, string> = {
  ARTICLE_JOURNAL: 'Article de journal',
  CONFERENCE_INT: 'Conférence internationale',
  CONFERENCE_NAT: 'Conférence nationale',
  CHAPITRE_OUVRAGE: 'Chapitre d\'ouvrage',
  RAPPORT_TECHNIQUE: 'Rapport technique',
  ArticleJournal: 'Article de journal',
  ConferenceInternational: 'Conférence internationale',
  ConferenceNational: 'Conférence nationale',
  ChapterBook: 'Chapitre d\'ouvrage',
  TechnicalReport: 'Rapport technique',
};

const normalizeStatus = (status?: string): PublicationStatus => {
  const normalized = status?.toLowerCase();
  if (normalized === 'draft' || normalized === 'brouillon') return 'Draft';
  if (normalized === 'submitted' || normalized === 'soumis') return 'Submitted';
  if (normalized === 'published' || normalized === 'publie' || normalized === 'publié') return 'Published';
  if (normalized === 'rejected' || normalized === 'rejete' || normalized === 'rejeté') return 'Rejected';
  return 'Draft';
};

const toFormType = (type?: string): PublicationType => {
  const map: Record<string, PublicationType> = {
    ArticleJournal: 'ARTICLE_JOURNAL',
    ConferenceInternational: 'CONFERENCE_INT',
    ConferenceNational: 'CONFERENCE_NAT',
    ChapterBook: 'CHAPITRE_OUVRAGE',
    TechnicalReport: 'RAPPORT_TECHNIQUE',
  };

  return map[type ?? ''] ?? (type as PublicationType) ?? 'ARTICLE_JOURNAL';
};

const toCoreRanking = (ranking?: string | null): 'A*' | 'A' | 'B' | 'C' => {
  if (ranking === 'APlus' || ranking === 'A*') return 'A*';
  if (ranking === 'A' || ranking === 'B' || ranking === 'C') return ranking;
  return 'A';
};

const toVisibility = (visibility?: string | null): PublicationVisibility =>
  visibility === 'Private' || visibility === 'PRIVEE' ? 'PRIVEE' : 'PUBLIQUE';

const roleLabel = (role?: number) => {
  if (role === 1) return 'Super Admin';
  if (role === 2) return 'Admin';
  if (role === 3) return 'Chercheur';
  if (role === 4) return 'Doctorant';
  if (role === 5) return 'Mastérien';
  return 'Membre';
};

export default function SuperAdminPublications() {
  const [searchQuery, setSearchQuery] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [editingPublication, setEditingPublication] = useState<DashboardPublicationDetailDto | null>(null);
  const [deleteConfirm, setDeleteConfirm] = useState<DashboardPublicationSummaryDto | null>(null);
  const [activeFilters, setActiveFilters] = useState<Record<string, string | string[]>>({});

  const { data: researchAxes = [] } = useGetResearchAxesQuery();
  const { data: labUsers = [] } = useGetPublicUsersQuery({ status: 'active', limit: 1000 });

  const getFilterValues = (value: string | string[] | undefined) => {
    if (!value) return [] as string[];
    return Array.isArray(value) ? value : [value];
  };

  const queryParams = useMemo<DashboardPublicationsQueryParams>(() => {
    const params: DashboardPublicationsQueryParams = { scope: 'all', limit: 1000 };

    if (searchQuery.trim()) {
      params.search = searchQuery.trim();
    }

    const statusValues = getFilterValues(activeFilters.status);
    const typeValues = getFilterValues(activeFilters.type);
    const visibilityValues = getFilterValues(activeFilters.visibility);

    if (statusValues.length === 1) params.status = statusValues[0];
    if (typeValues.length === 1) params.type = typeValues[0];
    if (visibilityValues.length === 1) params.visibility = visibilityValues[0];

    return params;
  }, [searchQuery, activeFilters]);

  const { data: publicationPage, isLoading, isFetching, isError, refetch } = useGetDashboardPublicationsQuery(queryParams);

  const [createPublication, { isLoading: isCreating }] = useCreateDashboardPublicationMutation();
  const [updatePublication, { isLoading: isUpdating }] = useUpdateDashboardPublicationMutation();
  const [loadPublicationById, { isFetching: isLoadingPublicationDetail }] = useLazyGetDashboardPublicationByIdQuery();
  const [validatePublication, { isLoading: isValidating }] = useValidateDashboardPublicationMutation();
  const [rejectPublication, { isLoading: isRejecting }] = useRejectDashboardPublicationMutation();
  const [deletePublication, { isLoading: isDeleting }] = useDeleteDashboardPublicationMutation();

  const publications = publicationPage?.items ?? [];
  const labMembersOptions = labUsers.map((user) => ({
    id: user.id,
    name: [user.firstName, user.lastName].filter(Boolean).join(' ') || user.email,
    role: roleLabel(user.role),
    email: user.email,
  }));

  const filterGroups = [
    {
      id: 'status',
      label: 'Statut',
      options: [
        { id: 'draft', label: 'Brouillon', value: 'Draft' },
        { id: 'submitted', label: 'Soumis', value: 'Submitted' },
        { id: 'published', label: 'Publié', value: 'Published' },
        { id: 'rejected', label: 'Rejeté', value: 'Rejected' },
      ],
    },
    {
      id: 'type',
      label: 'Type',
      options: [
        { id: 'article', label: 'Article de journal', value: 'ARTICLE_JOURNAL' },
        { id: 'confint', label: 'Conférence internationale', value: 'CONFERENCE_INT' },
        { id: 'confnat', label: 'Conférence nationale', value: 'CONFERENCE_NAT' },
        { id: 'chapter', label: 'Chapitre d\'ouvrage', value: 'CHAPITRE_OUVRAGE' },
        { id: 'report', label: 'Rapport technique', value: 'RAPPORT_TECHNIQUE' },
      ],
    },
    {
      id: 'visibility',
      label: 'Visibilité',
      options: [
        { id: 'public', label: 'Publique', value: 'Public' },
        { id: 'private', label: 'Privée', value: 'Private' },
      ],
    },
  ];

  const stats = {
    published: publications.filter((publication) => normalizeStatus(publication.status) === 'Published').length,
    submitted: publications.filter((publication) => normalizeStatus(publication.status) === 'Submitted').length,
    draft: publications.filter((publication) => normalizeStatus(publication.status) === 'Draft').length,
    rejected: publications.filter((publication) => normalizeStatus(publication.status) === 'Rejected').length,
  };

  const toBackendRequest = (data: PublicationFormData): CreateDashboardPublicationRequest | null => {
    const axis = researchAxes.find((item) => item.id === data.axes[0] || item.title === data.axes[0]);

    if (!axis) {
      toast.error('Sélectionnez un axe de recherche valide.');
      return null;
    }

    const request: CreateDashboardPublicationRequest = {
      researchAxisId: axis.id,
      title: data.title.trim(),
      abstract: data.abstract.trim() || null,
      keywords: data.keywords.map((keyword) => keyword.trim()).filter(Boolean),
      doi: data.doi.trim() || null,
      venue: null,
      type: data.type,
      visibility: data.visibility === 'PUBLIQUE' ? 'Public' : 'Private',
      year: Number(data.year),
      authors: data.authors.map((author) => author.trim()).filter(Boolean),
      journalArticle: null,
      technicalReport: null,
      bookChapter: null,
      nationalConference: null,
      internationalConference: null,
    };

    if (data.type === 'ARTICLE_JOURNAL') {
      request.venue = data.journalName.trim() || null;
      request.journalArticle = {
        journalName: data.journalName.trim(),
        volume: data.volume.trim(),
        number: data.issue.trim(),
        pages: data.pages.trim() || undefined,
        ranking: data.quartile,
      };
    }

    if (data.type === 'CONFERENCE_INT') {
      request.venue = data.conferenceName.trim() || null;
      request.internationalConference = {
        conferenceName: data.conferenceName.trim(),
        location: data.location.trim(),
        pages: data.pages.trim() || undefined,
        ranking: data.coreRanking === 'A*' ? 'APlus' : data.coreRanking,
      };
    }

    if (data.type === 'CONFERENCE_NAT') {
      request.venue = data.conferenceName.trim() || null;
      request.nationalConference = {
        conferenceName: data.conferenceName.trim(),
        location: data.location.trim(),
        pages: data.pages.trim() || undefined,
      };
    }

    if (data.type === 'CHAPITRE_OUVRAGE') {
      request.venue = data.bookTitle.trim() || null;
      request.bookChapter = {
        bookTitle: data.bookTitle.trim(),
        publisher: data.editor.trim() || data.institution.trim(),
        isbn: data.isbn.trim() || undefined,
        pages: data.pages.trim() || undefined,
      };
    }

    if (data.type === 'RAPPORT_TECHNIQUE') {
      request.venue = data.institution.trim() || null;
      request.technicalReport = {
        reportNumber: data.reportNumber.trim(),
        institution: data.institution.trim(),
      };
    }

    return request;
  };

  const toFormInitialData = (publication: DashboardPublicationDetailDto) => ({
    type: toFormType(publication.type || publication.publicationType),
    title: publication.title,
    year: publication.year,
    abstract: publication.abstract_ ?? '',
    keywords: publication.keywords ?? [],
    visibility: toVisibility(publication.visibility),
    axes: publication.axe?.id ? [publication.axe.id] : [],
    journalName: publication.journalName ?? publication.venue ?? '',
    volume: publication.volume ?? '',
    issue: publication.number ?? '',
    pages: publication.pages ?? '',
    doi: publication.doi ?? '',
    quartile: (publication.quartile ?? 'Q1') as 'Q1' | 'Q2' | 'Q3' | 'Q4',
    conferenceName: publication.venue ?? '',
    location: publication.location ?? '',
    coreRanking: toCoreRanking(publication.coreRanking),
    bookTitle: publication.bookTitle ?? '',
    editor: publication.publisher ?? '',
    isbn: publication.isbn ?? '',
    reportNumber: publication.reportNumber ?? '',
    institution: publication.institution ?? '',
    authors: publication.authors ?? [],
    status: normalizeStatus(publication.status) === 'Published' ? 'PUBLIE' : normalizeStatus(publication.status) === 'Submitted' ? 'SOUMIS' : 'BROUILLON',
  });

  const handleOpenCreate = () => {
    setEditingPublication(null);
    setShowForm(true);
  };

  const handleOpenEdit = async (publication: DashboardPublicationSummaryDto) => {
    try {
      const detail = await loadPublicationById(publication.id).unwrap();
      if (!detail) {
        toast.error('Publication introuvable.');
        return;
      }

      setEditingPublication(detail);
      setShowForm(true);
    } catch {
      toast.error('Impossible de charger la publication.');
    }
  };

  const handleSavePublication = async (data: PublicationFormData) => {
    const request = toBackendRequest(data);
    if (!request) return;

    if (editingPublication) {
      await updatePublication({ id: editingPublication.id, body: request }).unwrap();
      toast.success('Publication modifiée avec succès');
    } else {
      const result = await createPublication(request).unwrap();
      toast.success(result?.status === 'Published' ? 'Publication publiée avec succès' : 'Publication soumise pour validation');
    }

    setEditingPublication(null);
    setShowForm(false);
  };

  const handleCreatePublication = async (data: PublicationFormData) => {
    const request = toBackendRequest(data);
    if (!request) return;

    const result = await createPublication(request).unwrap();
    toast.success(result?.status === 'Published' ? 'Publication publiée avec succès' : 'Publication soumise pour validation');
    setShowForm(false);
  };

  const handleApprove = async (publication: DashboardPublicationSummaryDto) => {
    await validatePublication(publication.id).unwrap();
    toast.success(`"${publication.title}" approuvée et publiée`);
  };

  const handleReject = async (publication: DashboardPublicationSummaryDto) => {
    const reason = window.prompt(`Raison du rejet pour "${publication.title}"`);
    if (!reason || !reason.trim()) return;

    await rejectPublication({ id: publication.id, body: { reason: reason.trim() } }).unwrap();
    toast.success(`"${publication.title}" rejetée`);
  };

  const handleDelete = async (publication: DashboardPublicationSummaryDto) => {
    await deletePublication(publication.id).unwrap();
    toast.success(`"${publication.title}" supprimée avec succès`);
    setDeleteConfirm(null);
  };

  const busy = isCreating || isUpdating || isValidating || isRejecting || isDeleting || isLoadingPublicationDetail;

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between gap-4">
        <div>
          <h1 className="text-3xl font-bold text-navy dark:text-white mb-2">Gestion des Publications</h1>
          <p className="text-text-secondary">Gérer toutes les publications du laboratoire</p>
        </div>

        <Button onClick={handleOpenCreate} disabled={busy}>
          <FileText size={18} />
          Créer une publication
        </Button>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <Card><CardContent className="p-4"><div className="text-2xl font-bold text-navy dark:text-white">{stats.published}</div><div className="text-sm text-text-secondary">Publiées</div></CardContent></Card>
        <Card><CardContent className="p-4"><div className="text-2xl font-bold text-warning">{stats.submitted}</div><div className="text-sm text-text-secondary">En attente</div></CardContent></Card>
        <Card><CardContent className="p-4"><div className="text-2xl font-bold text-text-muted">{stats.draft}</div><div className="text-sm text-text-secondary">Brouillons</div></CardContent></Card>
        <Card><CardContent className="p-4"><div className="text-2xl font-bold text-error">{stats.rejected}</div><div className="text-sm text-text-secondary">Rejetées</div></CardContent></Card>
      </div>

      <SearchFilter
        onSearchChange={setSearchQuery}
        filterGroups={filterGroups}
        onFilterChange={setActiveFilters}
        searchPlaceholder="Rechercher une publication..."
      />

      {isLoading ? (
        <Card><CardContent className="p-12 text-center text-text-secondary flex flex-col items-center gap-3"><Loader2 className="h-8 w-8 animate-spin text-accent-blue" />Chargement des publications...</CardContent></Card>
      ) : isError ? (
        <Card>
          <CardContent className="p-12 text-center space-y-4">
            <div className="mx-auto w-fit rounded-full bg-error/10 p-4 text-error"><XCircle size={40} /></div>
            <div><h3 className="text-xl font-bold text-navy dark:text-white mb-2">Impossible de charger les publications</h3><p className="text-text-secondary">Vérifiez l’API puis réessayez.</p></div>
            <Button onClick={() => refetch()}>Réessayer</Button>
          </CardContent>
        </Card>
      ) : publications.length === 0 ? (
        <Card>
          <CardContent className="p-12 text-center">
            <div className="bg-accent-blue/5 dark:bg-accent-blue/10 rounded-xl p-8 inline-block">
              <FileText size={64} className="mx-auto text-accent-blue mb-4" />
              <h3 className="text-xl font-bold text-navy dark:text-white mb-2">Aucune publication trouvée</h3>
              <p className="text-text-primary dark:text-text-primary">{searchQuery || Object.values(activeFilters).some((value) => getFilterValues(value).length > 0) ? 'Aucun résultat ne correspond à vos critères' : 'Aucune publication enregistrée'}</p>
            </div>
          </CardContent>
        </Card>
      ) : (
        <Card>
          <CardContent className="p-6 space-y-4">
            {publications.map((publication) => (
              <div key={publication.id} className={clsx('p-5 border rounded-lg transition-all hover:shadow-md', normalizeStatus(publication.status) === 'Submitted' ? 'border-warning bg-warning/5' : 'border-surface-border')}>
                {(() => {
                  const publicationStatus = normalizeStatus(publication.status);
                  const publicationType = publication.type;

                  return (
                    <div className="flex items-start justify-between gap-4">
                  <div className="flex-1 min-w-0">
                    <div className="flex flex-wrap items-center gap-2 mb-3">
                      <Badge variant="default">{publication.year}</Badge>
                      <Badge variant={statusConfig[publicationStatus]?.variant ?? 'default'}>{statusConfig[publicationStatus]?.label ?? publication.status}</Badge>
                      <Badge variant="info" className="text-xs">{typeLabels[publicationType] ?? publication.type}</Badge>
                      <Badge variant={publication.visibility === 'Public' ? 'success' : 'default'}>{publication.visibility === 'Public' ? 'Publique' : 'Privée'}</Badge>
                    </div>

                    <h3 className="font-bold text-navy dark:text-white text-lg mb-2">{publication.title}</h3>

                    <div className="space-y-1 text-sm text-text-secondary">
                      <div><span className="font-medium">Auteurs:</span> {publication.authors.join(', ')}</div>
                      <div><span className="font-medium">Axe de recherche:</span> {publication.axe?.title ?? 'N/A'}</div>
                      {publication.submittedBy && <div className="text-accent-blue"><span className="font-medium">Soumis par:</span> {publication.submittedBy}</div>}
                      {publicationStatus === 'Rejected' && publication.rejectionReason && <div className="p-3 bg-error/5 border border-error/20 rounded-lg text-error mt-3"><span className="font-medium">Raison du rejet:</span> {publication.rejectionReason}</div>}
                    </div>
                  </div>

                  <div className="flex flex-col gap-2">
                    {publicationStatus === 'Submitted' && (
                      <>
                        <Button onClick={() => handleApprove(publication)} disabled={busy} className="whitespace-nowrap text-xs"><CheckCircle2 size={14} />Approuver</Button>
                        <Button variant="outlined" className="whitespace-nowrap text-xs text-error hover:bg-error/5" onClick={() => handleReject(publication)} disabled={busy}><XCircle size={14} />Rejeter</Button>
                      </>
                    )}
                    <Button variant="outlined" className="whitespace-nowrap text-xs" onClick={() => handleOpenEdit(publication)} disabled={busy}><Pencil size={14} />Modifier</Button>
                    <Button variant="outlined" className="whitespace-nowrap text-xs text-error hover:bg-error/5" onClick={() => setDeleteConfirm(publication)} disabled={busy}><Trash2 size={14} />Supprimer</Button>
                  </div>
                </div>
                  );
                })()}
              </div>
            ))}
          </CardContent>
        </Card>
      )}

      {showForm && (
        <PublicationForm
          onClose={() => {
            setEditingPublication(null);
            setShowForm(false);
          }}
          onSubmit={handleSavePublication}
          initialData={editingPublication ? toFormInitialData(editingPublication) : undefined}
          canPublishDirectly={true}
          submitMode="manual"
          axesOptions={researchAxes.map((axis) => ({ id: axis.id, title: axis.title }))}
          labMembersOptions={labMembersOptions}
        />
      )}

      {deleteConfirm && (
        <ConfirmDialog
          isOpen={true}
          onClose={() => setDeleteConfirm(null)}
          onConfirm={() => handleDelete(deleteConfirm)}
          title="Supprimer cette publication"
          description={`Êtes-vous sûr de vouloir supprimer "${deleteConfirm.title}" ? Cette action est irréversible.`}
          confirmText="Supprimer"
          variant="danger"
        />
      )}

      {isFetching && !isLoading && (
        <div className="fixed bottom-6 right-6 rounded-full bg-navy text-white px-4 py-2 shadow-lg text-sm flex items-center gap-2">
          <Loader2 size={14} className="animate-spin" />
          Mise à jour...
        </div>
      )}
    </div>
  );
}
