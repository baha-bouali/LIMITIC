import { useState } from 'react';
import { Card, CardContent } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { Button } from '../../../components/ui/Button';
import { ConfirmDialog } from '../../../components/shared/ConfirmDialog';
import { PublicationForm } from '../../../components/publications/PublicationForm';
import { Plus, Pencil, Trash2, FileText, Eye, Send, AlertCircle } from 'lucide-react';
import { toast } from 'sonner';
import { clsx } from 'clsx';
import { useLanguage } from '../../../contexts/LanguageContext';

type PublicationType = 'ARTICLE_JOURNAL' | 'CONFERENCE_INT' | 'CONFERENCE_NAT' | 'CHAPITRE_OUVRAGE' | 'RAPPORT_TECHNIQUE';
type PublicationStatus = 'BROUILLON' | 'SOUMIS' | 'PUBLIE' | 'REJETE';

interface Publication {
  id: string;
  type: PublicationType;
  title: string;
  year: number;
  status: PublicationStatus;
  authors: string[];
  axe: string;
  abstract?: string;
  doi?: string;
  venue?: string;
  rejectionReason?: string;
}

function useTypeLabels() {
  const { t } = useLanguage();
  return {
    ARTICLE_JOURNAL: t('pubType.journal'),
    CONFERENCE_INT: t('pubType.confInt'),
    CONFERENCE_NAT: t('pubType.confNat'),
    CHAPITRE_OUVRAGE: t('pubType.chapter'),
    RAPPORT_TECHNIQUE: t('pubType.report'),
  };
}

function useStatusConfig() {
  const { t } = useLanguage();
  return {
    BROUILLON: { label: t('pub.draft'), variant: 'default', color: 'text-text-muted' },
    SOUMIS: { label: t('pub.underReview'), variant: 'warning', color: 'text-warning' },
    PUBLIE: { label: t('pub.published'), variant: 'success', color: 'text-success' },
    REJETE: { label: t('pub.rejected'), variant: 'default', color: 'text-error' },
  };
}

const initialPublications: Publication[] = [
  { id: '1', type: 'CONFERENCE_NAT', title: 'Système de recommandation basé sur le Deep Learning pour le e-commerce tunisien', year: 2026, status: 'SOUMIS', authors: ['Ines Hamdi', 'Dr. Ahmed Ben Salem'], axe: 'IA & Apprentissage Automatique', abstract: 'Nous proposons un système de recommandation basé sur des réseaux de neurones profonds adapté au contexte du e-commerce tunisien.', venue: 'Conférence Nationale en Informatique 2026' },
  { id: '2', type: 'RAPPORT_TECHNIQUE', title: 'État de l\'art : Systèmes de recommandation collaboratifs', year: 2025, status: 'BROUILLON', authors: ['Ines Hamdi'], axe: 'IA & Apprentissage Automatique', abstract: 'Revue de la littérature sur les approches de filtrage collaboratif.' },
];

export default function MasterienPublications() {
  const { t } = useLanguage();
  const typeLabels = useTypeLabels();
  const statusConfig = useStatusConfig();
  const [publications, setPublications] = useState<Publication[]>(initialPublications);
  const [showForm, setShowForm] = useState(false);
  const [editingPub, setEditingPub] = useState<Publication | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<Publication | null>(null);
  const [submitTarget, setSubmitTarget] = useState<Publication | null>(null);

  const stats = {
    total: publications.length,
    brouillon: publications.filter(p => p.status === 'BROUILLON').length,
    soumis: publications.filter(p => p.status === 'SOUMIS').length,
    publie: publications.filter(p => p.status === 'PUBLIE').length,
    rejete: publications.filter(p => p.status === 'REJETE').length,
  };

  function openCreate() {
    setEditingPub(null);
    setShowForm(true);
  }

  function openEdit(pub: Publication) {
    setEditingPub(pub);
    setShowForm(true);
  }

  function handleSavePublication(data: any) {
    if (editingPub) {
      setPublications(prev => prev.map(p => p.id === editingPub.id ? {
        ...p,
        ...data,
        id: p.id,
        status: p.status,
      } : p));
      toast.success(t('pub.modifiedSuccess'));
    } else {
      setPublications(prev => [...prev, {
        ...data,
        id: Date.now().toString(),
        status: 'BROUILLON',
      }]);
      toast.success(t('pub.createdDraft'));
    }
    setShowForm(false);
    setEditingPub(null);
  }

  function handleDelete() {
    if (!deleteTarget) return;
    setPublications(prev => prev.filter(p => p.id !== deleteTarget.id));
    toast.success(t('pub.deleted'));
    setDeleteTarget(null);
  }

  function handleSubmit() {
    if (!submitTarget) return;
    setPublications(prev => prev.map(p => p.id === submitTarget.id ? { ...p, status: 'SOUMIS' } : p));
    toast.success(t('pub.submittedForReview'));
    setSubmitTarget(null);
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

      {/* Stats */}
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

      {/* Info banner */}
      <div className="flex items-start gap-3 p-4 bg-accent-blue/5 border border-accent-blue/20 rounded-xl">
        <AlertCircle size={20} className="text-accent-blue flex-shrink-0 mt-0.5" />
        <div className="text-sm text-text-secondary">
          <strong className="text-navy dark:text-white">{t('pub.workflow')}</strong> {t('pub.workflowDesc')}
        </div>
      </div>

      {/* Publications list */}
      <div className="space-y-4">
        {publications.length === 0 && (
          <Card><CardContent className="py-16 text-center text-text-muted"><FileText size={40} className="mx-auto mb-3 opacity-30" /><p>{t('pub.noPublications')} {t('pub.createFirst')}</p></CardContent></Card>
        )}
        {publications.map(pub => {
          const sc = statusConfig[pub.status];
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
                          <Badge variant={sc.variant}>{sc.label}</Badge>
                          <Badge variant="info" className="text-xs">{typeLabels[pub.type]}</Badge>
                          <span className="text-xs text-text-muted">{pub.year}</span>
                        </div>
                        <h3 className="font-bold text-navy dark:text-white">{pub.title}</h3>
                        <p className="text-sm text-text-muted mt-1">{pub.authors.join(', ')}</p>
                        {pub.venue && <p className="text-xs text-text-muted mt-0.5 italic">{pub.venue}</p>}
                        {pub.abstract && <p className="text-sm text-text-secondary mt-2 line-clamp-2">{pub.abstract}</p>}
                        {pub.rejectionReason && (
                          <div className="mt-2 p-2 bg-error/5 border border-error/20 rounded-lg text-sm text-error">
                            <strong>{t('pub.rejectionReason')}:</strong> {pub.rejectionReason}
                          </div>
                        )}
                      </div>
                      <div className="flex items-center gap-2 flex-shrink-0">
                        {pub.status === 'BROUILLON' && (
                          <button
                            onClick={() => setSubmitTarget(pub)}
                            className="flex items-center gap-1.5 px-3 py-1.5 bg-accent-blue/10 text-accent-blue hover:bg-accent-blue/20 rounded-lg text-sm font-medium transition-colors"
                          >
                            <Send size={14} /> {t('pub.submit')}
                          </button>
                        )}
                        {['BROUILLON', 'REJETE'].includes(pub.status) && (
                          <button
                            onClick={() => openEdit(pub)}
                            className="p-2 hover:bg-light-gray dark:hover:bg-muted rounded-lg text-accent-blue"
                          >
                            <Pencil size={16} />
                          </button>
                        )}
                        <button
                          onClick={() => setDeleteTarget(pub)}
                          className="p-2 hover:bg-error/10 rounded-lg text-error"
                          disabled={pub.status === 'SOUMIS'}
                          title={pub.status === 'SOUMIS' ? t('pub.cannotDeleteUnderReview') : ''}
                        >
                          <Trash2 size={16} className={pub.status === 'SOUMIS' ? 'opacity-30' : ''} />
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

      {/* Publication Form Modal */}
      {showForm && (
        <PublicationForm
          onClose={() => { setShowForm(false); setEditingPub(null); }}
          onSubmit={handleSavePublication}
          initialData={editingPub}
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
