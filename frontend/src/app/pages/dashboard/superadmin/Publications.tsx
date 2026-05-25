import { useState } from 'react';
import { skipToken } from '@reduxjs/toolkit/query/react';
import { Card, CardContent } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { Button } from '../../../components/ui/Button';
import { SearchFilter } from '../../../components/shared/SearchFilter';
import { ConfirmDialog } from '../../../components/shared/ConfirmDialog';
import { PublicationForm } from '../../../components/publications/PublicationForm';
import { FileText, Pencil, Trash2, X } from 'lucide-react';
import { toast } from 'sonner';
import { clsx } from 'clsx';
import {
  useGetDashboardPublicationsQuery,
  useGetDashboardPublicationByIdQuery,
  useCreateDashboardPublicationMutation,
  useUpdateDashboardPublicationMutation,
  useDeleteDashboardPublicationMutation,
  useValidateDashboardPublicationMutation,
  useRejectDashboardPublicationMutation,
  type DashboardPublicationSummaryDto,
  type CreateDashboardPublicationRequest,
} from '../../../api/dashboardPublicationsApi';

const TYPE_LABELS: Record<string, string> = {
  ArticleJournal: 'Article de journal',
  ConferenceInternational: 'Conf. internationale',
  ConferenceNational: 'Conf. nationale',
  ChapterBook: "Chapitre d'ouvrage",
  TechnicalReport: 'Rapport technique',
};

const STATUS_VARIANT: Record<string, 'default' | 'warning' | 'success' | 'error'> = {
  Draft: 'default',
  Submitted: 'warning',
  Published: 'success',
  Rejected: 'error',
};

const STATUS_LABELS: Record<string, string> = {
  Draft: 'Brouillon',
  Submitted: 'Soumis',
  Published: 'Publié',
  Rejected: 'Rejeté',
};

export default function SuperAdminPublications() {
  const [searchQuery, setSearchQuery] = useState('');
  const [activeFilters, setActiveFilters] = useState<Record<string, string[]>>({});
  const [showForm, setShowForm] = useState(false);
  const [editingPubId, setEditingPubId] = useState<string | null>(null);
  const [deleteConfirm, setDeleteConfirm] = useState<DashboardPublicationSummaryDto | null>(null);
  const [rejectTarget, setRejectTarget] = useState<DashboardPublicationSummaryDto | null>(null);
  const [rejectReason, setRejectReason] = useState('');

  const { data: list, isLoading } = useGetDashboardPublicationsQuery({ scope: 'all' });
  const { data: editingDetail } = useGetDashboardPublicationByIdQuery(editingPubId ?? skipToken);

  const [createPublication] = useCreateDashboardPublicationMutation();
  const [updatePublication] = useUpdateDashboardPublicationMutation();
  const [deletePublication] = useDeleteDashboardPublicationMutation();
  const [validatePublication] = useValidateDashboardPublicationMutation();
  const [rejectPublication] = useRejectDashboardPublicationMutation();

  const allPublications = list?.items ?? [];

  const filterGroups = [
    {
      id: 'status',
      label: 'Statut',
      options: [
        { id: 'brouillon', label: 'Brouillon', value: 'Draft' },
        { id: 'soumis', label: 'Soumis', value: 'Submitted' },
        { id: 'publie', label: 'Publié', value: 'Published' },
        { id: 'rejete', label: 'Rejeté', value: 'Rejected' },
      ]
    },
    {
      id: 'type',
      label: 'Type',
      options: [
        { id: 'article', label: 'Article de journal', value: 'ArticleJournal' },
        { id: 'confint', label: 'Conférence internationale', value: 'ConferenceInternational' },
        { id: 'confnat', label: 'Conférence nationale', value: 'ConferenceNational' },
        { id: 'chapitre', label: "Chapitre d'ouvrage", value: 'ChapterBook' },
        { id: 'rapport', label: 'Rapport technique', value: 'TechnicalReport' },
      ]
    },
    {
      id: 'visibility',
      label: 'Visibilité',
      options: [
        { id: 'publique', label: 'Publique', value: 'Public' },
        { id: 'privee', label: 'Privée', value: 'Private' },
      ]
    },
  ];

  const filteredPublications = allPublications.filter(pub => {
    const matchesSearch = searchQuery === '' ||
      pub.title.toLowerCase().includes(searchQuery.toLowerCase()) ||
      pub.authors.some(a => a.toLowerCase().includes(searchQuery.toLowerCase()));

    const matchesFilters = Object.entries(activeFilters).every(([key, values]) => {
      if (values.length === 0) return true;
      if (key === 'status') return values.includes(pub.status);
      if (key === 'type') return values.includes(pub.type);
      if (key === 'visibility') return values.includes(pub.visibility);
      return true;
    });

    return matchesSearch && matchesFilters;
  });

  async function handleSavePublication(data: CreateDashboardPublicationRequest, id?: string) {
    try {
      if (id) {
        await updatePublication({ id, body: data }).unwrap();
        toast.success('Publication modifiée avec succès');
      } else {
        await createPublication(data).unwrap();
        toast.success('Publication créée et publiée');
      }
    } catch {
      toast.error('Une erreur est survenue');
    }
    setEditingPubId(null);
  }

  async function handleDelete() {
    if (!deleteConfirm) return;
    try {
      await deletePublication(deleteConfirm.id).unwrap();
      toast.success(`"${deleteConfirm.title}" supprimée avec succès`);
    } catch {
      toast.error('Erreur lors de la suppression');
    }
    setDeleteConfirm(null);
  }

  async function handleApprove(pub: DashboardPublicationSummaryDto) {
    try {
      await validatePublication(pub.id).unwrap();
      toast.success(`"${pub.title}" approuvée et publiée`);
    } catch {
      toast.error('Erreur lors de la validation');
    }
  }

  async function handleRejectConfirm() {
    if (!rejectTarget) return;
    if (!rejectReason.trim()) {
      toast.error('Veuillez indiquer un motif de rejet');
      return;
    }
    try {
      await rejectPublication({ id: rejectTarget.id, body: { reason: rejectReason } }).unwrap();
      toast.success(`"${rejectTarget.title}" rejetée`);
    } catch {
      toast.error('Erreur lors du rejet');
    }
    setRejectTarget(null);
    setRejectReason('');
  }

  if (isLoading) {
    return (
      <div className="flex items-center justify-center py-20">
        <div className="text-text-muted">Chargement...</div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-navy dark:text-white">Gestion des Publications</h1>
          <p className="text-text-secondary">Gérer toutes les publications du laboratoire</p>
        </div>
        <Button onClick={() => { setEditingPubId(null); setShowForm(true); }}>
          <FileText size={18} />
          Créer une publication
        </Button>
      </div>

      {/* Stats Cards */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <Card>
          <CardContent className="p-4">
            <div className="text-2xl font-bold text-navy dark:text-white">
              {allPublications.filter(p => p.status === 'Published').length}
            </div>
            <div className="text-sm text-text-secondary">Publiées</div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4">
            <div className="text-2xl font-bold text-warning">
              {allPublications.filter(p => p.status === 'Submitted').length}
            </div>
            <div className="text-sm text-text-secondary">En attente</div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4">
            <div className="text-2xl font-bold text-text-muted">
              {allPublications.filter(p => p.status === 'Draft').length}
            </div>
            <div className="text-sm text-text-secondary">Brouillons</div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4">
            <div className="text-2xl font-bold text-error">
              {allPublications.filter(p => p.status === 'Rejected').length}
            </div>
            <div className="text-sm text-text-secondary">Rejetées</div>
          </CardContent>
        </Card>
      </div>

      <SearchFilter
        onSearchChange={setSearchQuery}
        filterGroups={filterGroups}
        onFilterChange={setActiveFilters}
        searchPlaceholder="Rechercher une publication..."
      />

      {filteredPublications.length === 0 ? (
        <Card>
          <CardContent className="p-12 text-center">
            <div className="bg-accent-blue/5 dark:bg-accent-blue/10 rounded-xl p-8 inline-block">
              <FileText size={64} className="mx-auto text-accent-blue mb-4" />
              <h3 className="text-xl font-bold text-navy dark:text-white mb-2">Aucune publication trouvée</h3>
              <p className="text-text-primary dark:text-text-primary">
                {searchQuery || Object.values(activeFilters).some(v => v.length > 0)
                  ? 'Aucun résultat ne correspond à vos critères'
                  : 'Aucune publication enregistrée'}
              </p>
            </div>
          </CardContent>
        </Card>
      ) : (
        <Card>
          <CardContent className="p-6 space-y-4">
            {filteredPublications.map((pub) => (
              <div
                key={pub.id}
                className={clsx(
                  'p-5 border rounded-lg transition-all hover:shadow-md',
                  pub.status === 'Submitted' ? 'border-warning bg-warning/5' : 'border-surface-border'
                )}
              >
                <div className="flex items-start justify-between gap-4">
                  <div className="flex-1 min-w-0">
                    <div className="flex flex-wrap items-center gap-2 mb-3">
                      {pub.quartile && <Badge variant={`q${pub.quartile.charAt(1)}` as any}>{pub.quartile}</Badge>}
                      {pub.coreRanking && <Badge variant={`core-${pub.coreRanking.toLowerCase().replace('*', '-star')}` as any}>CORE {pub.coreRanking}</Badge>}
                      <Badge variant="default">{pub.year}</Badge>
                      <Badge variant={STATUS_VARIANT[pub.status] ?? 'default'}>{STATUS_LABELS[pub.status] ?? pub.status}</Badge>
                      {pub.visibility === 'Private' && <Badge variant="default">Privée</Badge>}
                      <Badge variant="info" className="text-xs">{TYPE_LABELS[pub.type] ?? pub.type}</Badge>
                    </div>

                    <h3 className="font-bold text-navy dark:text-white text-lg mb-2">{pub.title}</h3>

                    <div className="space-y-1 text-sm text-text-secondary">
                      <div><span className="font-medium">Auteurs:</span> {pub.authors.join(', ')}</div>
                      <div><span className="font-medium">Axe de recherche:</span> {pub.axe.title}</div>
                      {pub.submittedBy && (
                        <div className="text-accent-blue"><span className="font-medium">Soumis par:</span> {pub.submittedBy}</div>
                      )}
                      {pub.status === 'Rejected' && pub.rejectionReason && (
                        <div className="p-3 bg-error/5 border border-error/20 rounded-lg text-error mt-3">
                          <span className="font-medium">Raison du rejet:</span> {pub.rejectionReason}
                        </div>
                      )}
                    </div>
                  </div>

                  <div className="flex flex-col gap-2">
                    {pub.status === 'Submitted' && (
                      <>
                        <Button
                          onClick={() => handleApprove(pub)}
                          className="whitespace-nowrap text-xs"
                        >
                          Approuver
                        </Button>
                        <Button
                          variant="outlined"
                          className="whitespace-nowrap text-xs text-error hover:bg-error/5"
                          onClick={() => { setRejectTarget(pub); setRejectReason(''); }}
                        >
                          Rejeter
                        </Button>
                      </>
                    )}
                    <Button
                      variant="outlined"
                      className="whitespace-nowrap text-xs"
                      onClick={() => { setEditingPubId(pub.id); setShowForm(true); }}
                    >
                      <Pencil size={14} />
                      Modifier
                    </Button>
                    <Button
                      variant="outlined"
                      className="whitespace-nowrap text-xs text-error hover:bg-error/5"
                      onClick={() => setDeleteConfirm(pub)}
                    >
                      <Trash2 size={14} />
                      Supprimer
                    </Button>
                  </div>
                </div>
              </div>
            ))}
          </CardContent>
        </Card>
      )}

      {showForm && (
        <PublicationForm
          onClose={() => { setShowForm(false); setEditingPubId(null); }}
          onSubmit={handleSavePublication}
          initialData={editingPubId ? (editingDetail ?? null) : null}
          canPublishDirectly={true}
        />
      )}

      {deleteConfirm && (
        <ConfirmDialog
          isOpen={true}
          onClose={() => setDeleteConfirm(null)}
          onConfirm={handleDelete}
          title="Supprimer cette publication"
          description={`Êtes-vous sûr de vouloir supprimer "${deleteConfirm.title}" ? Cette action est irréversible.`}
          confirmText="Supprimer"
          variant="danger"
        />
      )}

      {/* Reject Modal */}
      {rejectTarget && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-[60] p-4">
          <div className="bg-white dark:bg-card rounded-2xl shadow-modal max-w-lg w-full" onClick={e => e.stopPropagation()}>
            <div className="p-6 border-b border-surface-border flex items-center justify-between">
              <h2 className="text-lg font-bold text-navy dark:text-white">Rejeter la publication</h2>
              <button onClick={() => { setRejectTarget(null); setRejectReason(''); }} className="p-2 hover:bg-light-gray dark:hover:bg-muted rounded-lg">
                <X size={18} />
              </button>
            </div>
            <div className="p-6 space-y-4">
              <p className="text-sm text-text-secondary">
                Vous êtes sur le point de rejeter : <strong className="text-navy dark:text-white">"{rejectTarget.title}"</strong>
              </p>
              <div>
                <label className="block text-sm font-medium mb-2">Motif de rejet *</label>
                <textarea
                  value={rejectReason}
                  onChange={(e) => setRejectReason(e.target.value)}
                  rows={4}
                  className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-error focus:border-transparent resize-none dark:bg-input-background"
                  placeholder="Expliquez les raisons du rejet et les améliorations attendues..."
                />
              </div>
            </div>
            <div className="p-6 border-t border-surface-border flex justify-end gap-3">
              <Button variant="outlined" onClick={() => { setRejectTarget(null); setRejectReason(''); }}>
                Annuler
              </Button>
              <Button
                className="bg-error hover:bg-error/90 text-white"
                onClick={handleRejectConfirm}
              >
                Confirmer le rejet
              </Button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
