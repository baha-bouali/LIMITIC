import { useState } from 'react';
import { skipToken } from '@reduxjs/toolkit/query/react';
import { Card, CardContent } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { Button } from '../../../components/ui/Button';
import { ConfirmDialog } from '../../../components/shared/ConfirmDialog';
import { PublicationForm } from '../../../components/publications/PublicationForm';
import { Plus, Pencil, Trash2, FileText, Send, AlertCircle } from 'lucide-react';
import { toast } from 'sonner';
import { clsx } from 'clsx';
import { useLanguage } from '../../../contexts/LanguageContext';
import {
  useGetDashboardPublicationsQuery,
  useGetDashboardPublicationByIdQuery,
  useCreateDashboardPublicationMutation,
  useUpdateDashboardPublicationMutation,
  useDeleteDashboardPublicationMutation,
  useSubmitDashboardPublicationMutation,
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

const STATUS_VARIANT: Record<string, string> = {
  Draft: 'default',
  Submitted: 'warning',
  Published: 'success',
  Rejected: 'error',
};

function useStatusConfig() {
  const { t } = useLanguage();
  return {
    Draft: { label: t('pub.draft'), variant: 'default', color: 'text-text-muted' },
    Submitted: { label: t('pub.underReview'), variant: 'warning', color: 'text-warning' },
    Published: { label: t('pub.published'), variant: 'success', color: 'text-success' },
    Rejected: { label: t('pub.rejected'), variant: 'default', color: 'text-error' },
  };
}

export default function MasterienPublications() {
  const { t } = useLanguage();
  const statusConfig = useStatusConfig();

  const [showForm, setShowForm] = useState(false);
  const [editingPubId, setEditingPubId] = useState<string | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<DashboardPublicationSummaryDto | null>(null);
  const [submitTarget, setSubmitTarget] = useState<DashboardPublicationSummaryDto | null>(null);

  const { data: list, isLoading } = useGetDashboardPublicationsQuery({ scope: 'mine' });
  const { data: editingDetail } = useGetDashboardPublicationByIdQuery(editingPubId ?? skipToken);

  const [createPublication] = useCreateDashboardPublicationMutation();
  const [updatePublication] = useUpdateDashboardPublicationMutation();
  const [deletePublication] = useDeleteDashboardPublicationMutation();
  const [submitPublication] = useSubmitDashboardPublicationMutation();

  const publications = list?.items ?? [];

  const stats = {
    brouillon: publications.filter(p => p.status === 'Draft').length,
    soumis: publications.filter(p => p.status === 'Submitted').length,
    publie: publications.filter(p => p.status === 'Published').length,
    rejete: publications.filter(p => p.status === 'Rejected').length,
  };

  function openCreate() {
    setEditingPubId(null);
    setShowForm(true);
  }

  function openEdit(pub: DashboardPublicationSummaryDto) {
    setEditingPubId(pub.id);
    setShowForm(true);
  }

  async function handleSavePublication(data: CreateDashboardPublicationRequest, id?: string) {
    try {
      if (id) {
        await updatePublication({ id, body: data }).unwrap();
        toast.success(t('pub.modifiedSuccess'));
      } else {
        await createPublication(data).unwrap();
        toast.success(t('pub.submittedForReview'));
      }
    } catch {
      toast.error(id ? t('pub.modifiedError') : t('pub.createError'));
    }
    setEditingPubId(null);
  }

  async function handleDelete() {
    if (!deleteTarget) return;
    try {
      await deletePublication(deleteTarget.id).unwrap();
      toast.success(t('pub.deleted'));
    } catch {
      toast.error(t('pub.deleteError'));
    }
    setDeleteTarget(null);
  }

  async function handleSubmit() {
    if (!submitTarget) return;
    try {
      await submitPublication(submitTarget.id).unwrap();
      toast.success(t('pub.submittedForReview'));
    } catch {
      toast.error(t('pub.submitError'));
    }
    setSubmitTarget(null);
  }

  if (isLoading) {
    return (
      <div className="flex items-center justify-center py-20">
        <div className="text-text-muted">{t('common.loading')}</div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-navy dark:text-white">{t('pub.myPublications')}</h1>
          <p className="text-text-secondary mt-1">{t('pub.manageYourWork')}</p>
        </div>
        <Button onClick={openCreate} className="flex items-center gap-2">
          <Plus size={18} /> {t('pub.newPublication')}
        </Button>
      </div>

      <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
        {[
          { label: t('pub.drafts'), value: stats.brouillon, color: 'bg-light-gray text-text-secondary' },
          { label: t('pub.underReview'), value: stats.soumis, color: 'bg-warning/10 text-warning' },
          { label: t('pub.published'), value: stats.publie, color: 'bg-success/10 text-success' },
          { label: t('pub.rejected'), value: stats.rejete, color: 'bg-error/10 text-error' },
        ].map(s => (
          <Card key={s.label}>
            <CardContent className="p-4 flex items-center gap-3">
              <div className={clsx('w-10 h-10 rounded-lg flex items-center justify-center font-bold text-lg', s.color)}>{s.value}</div>
              <div className="text-sm text-text-secondary">{s.label}</div>
            </CardContent>
          </Card>
        ))}
      </div>

      <div className="flex items-start gap-3 p-4 bg-accent-blue/5 border border-accent-blue/20 rounded-xl">
        <AlertCircle size={20} className="text-accent-blue flex-shrink-0 mt-0.5" />
        <div className="text-sm text-text-secondary">
          <strong className="text-navy dark:text-white">{t('pub.workflow')}</strong> {t('pub.workflowDesc')}
        </div>
      </div>

      <div className="space-y-4">
        {publications.length === 0 && (
          <Card>
            <CardContent className="py-16 text-center text-text-muted">
              <FileText size={40} className="mx-auto mb-3 opacity-30" />
              <p>{t('pub.noPublications')} {t('pub.createFirst')}</p>
            </CardContent>
          </Card>
        )}
        {publications.map(pub => {
          const sc = statusConfig[pub.status as keyof typeof statusConfig] ?? { label: pub.status, variant: 'default', color: '' };
          const typeLabel = TYPE_LABELS[pub.type] ?? pub.type;
          return (
            <Card key={pub.id} className="hover:shadow-card-hover transition-shadow">
              <CardContent className="p-5">
                <div className="flex items-start gap-4">
                  <div className="w-10 h-10 rounded-xl bg-accent-blue/10 flex items-center justify-center flex-shrink-0">
                    <FileText size={18} className="text-accent-blue" />
                  </div>
                  <div className="flex-1 min-w-0">
                    <div className="flex items-start justify-between gap-3">
                      <div className="flex-1 min-w-0">
                        <div className="flex items-center gap-2 flex-wrap mb-1">
                          <Badge variant={sc.variant as any}>{sc.label}</Badge>
                          <Badge variant="info" className="text-xs">{typeLabel}</Badge>
                          <span className="text-xs text-text-muted">{pub.year}</span>
                        </div>
                        <h3 className="font-bold text-navy dark:text-white">{pub.title}</h3>
                        <p className="text-sm text-text-muted mt-1">{pub.authors.join(', ')}</p>
                        {pub.venue && <p className="text-xs text-text-muted mt-0.5 italic">{pub.venue}</p>}
                        {pub.rejectionReason && (
                          <div className="mt-2 p-2 bg-error/5 border border-error/20 rounded-lg text-sm text-error">
                            <strong>{t('pub.rejectionReason')}:</strong> {pub.rejectionReason}
                          </div>
                        )}
                      </div>
                      <div className="flex items-center gap-2 flex-shrink-0">
                        {pub.status === 'Draft' && (
                          <button
                            onClick={() => setSubmitTarget(pub)}
                            className="flex items-center gap-1.5 px-3 py-1.5 bg-accent-blue/10 text-accent-blue hover:bg-accent-blue/20 rounded-lg text-sm font-medium transition-colors"
                          >
                            <Send size={14} /> {t('pub.submit')}
                          </button>
                        )}
                        {['Draft', 'Rejected'].includes(pub.status) && (
                          <button onClick={() => openEdit(pub)} className="p-2 hover:bg-light-gray dark:hover:bg-muted rounded-lg text-accent-blue">
                            <Pencil size={16} />
                          </button>
                        )}
                        <button
                          onClick={() => setDeleteTarget(pub)}
                          className="p-2 hover:bg-error/10 rounded-lg text-error"
                          disabled={pub.status !== 'Draft'}
                          title={pub.status !== 'Draft' ? t('pub.cannotDeleteUnderReview') : ''}
                        >
                          <Trash2 size={16} className={pub.status !== 'Draft' ? 'opacity-30' : ''} />
                        </button>
                      </div>
                    </div>
                  </div>
                </div>
              </CardContent>
            </Card>
          );
        })}
      </div>

      {showForm && (
        <PublicationForm
          onClose={() => { setShowForm(false); setEditingPubId(null); }}
          onSubmit={handleSavePublication}
          initialData={editingPubId ? (editingDetail ?? null) : null}
          canPublishDirectly={false}
        />
      )}

      <ConfirmDialog
        isOpen={!!deleteTarget}
        onClose={() => setDeleteTarget(null)}
        onConfirm={handleDelete}
        title={t('pub.deletePublication')}
        description={`${t('pub.confirmDelete')} "${deleteTarget?.title}" ?`}
        confirmText={t('common.delete')}
        variant="danger"
      />

      <ConfirmDialog
        isOpen={!!submitTarget}
        onClose={() => setSubmitTarget(null)}
        onConfirm={handleSubmit}
        title={t('pub.submitForReview')}
        description={`${t('pub.confirmSubmit')} "${submitTarget?.title}" ${t('pub.cannotModifyDuringReview')}`}
        confirmText={t('pub.submit')}
        variant="warning"
      />
    </div>
  );
}
