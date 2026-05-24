import { useState } from 'react';
import { Card, CardContent } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { Button } from '../../../components/ui/Button';
import { ConfirmDialog } from '../../../components/shared/ConfirmDialog';
import { SearchFilter } from '../../../components/shared/SearchFilter';
import { PublicationForm } from '../../../components/publications/PublicationForm';
import { Plus, Pencil, Trash2, FileText, Eye, Send, AlertCircle, X, ExternalLink, Target } from 'lucide-react';
import { toast } from 'sonner';
import { clsx } from 'clsx';
import { useLanguage } from '../../../contexts/LanguageContext';

type PubType = 'ARTICLE_JOURNAL' | 'CONFERENCE_INT' | 'CONFERENCE_NAT' | 'CHAPITRE_OUVRAGE' | 'RAPPORT_TECHNIQUE';
type PubStatus = 'BROUILLON' | 'SOUMIS' | 'PUBLIE' | 'REJETE';

interface Publication {
  id: string;
  type: PubType;
  title: string;
  year: number;
  status: PubStatus;
  authors: string[];
  axe: string;
  abstract?: string;
  doi?: string;
  venue?: string;
  quartile?: 'Q1' | 'Q2' | 'Q3' | 'Q4';
  coreRanking?: 'A*' | 'A' | 'B' | 'C';
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
    BROUILLON: { label: t('pub.draft'), variant: 'default' },
    SOUMIS: { label: t('pub.underReview'), variant: 'warning' },
    PUBLIE: { label: t('pub.published'), variant: 'success' },
    REJETE: { label: t('pub.rejected'), variant: 'error' },
  };
}

const initialPublications: Publication[] = [
  {
    id: '1',
    type: 'ARTICLE_JOURNAL',
    title: 'Medical Image Classification using Convolutional Neural Networks: A Comprehensive Study',
    year: 2026,
    status: 'SOUMIS',
    quartile: 'Q2',
    authors: ['Sarah Trabelsi', 'Dr. Ahmed Ben Salem', 'Dr. Fatma Gharbi'],
    axe: 'IA & Apprentissage Automatique',
    abstract: 'Ce travail présente une étude comparative des architectures CNN pour la classification d\'images médicales, avec des résultats sur plusieurs datasets publics.',
    venue: 'Journal of Biomedical Informatics',
    doi: 'https://doi.org/10.1016/j.jbi.2026.01.005',
  },
  {
    id: '2',
    type: 'CONFERENCE_INT',
    title: 'Deep Learning for Chest X-Ray Analysis: Automated Detection of Pulmonary Diseases',
    year: 2026,
    status: 'BROUILLON',
    coreRanking: 'A',
    authors: ['Sarah Trabelsi', 'Dr. Ahmed Ben Salem'],
    axe: 'IA & Apprentissage Automatique',
    abstract: 'Nous proposons un système automatisé de détection de maladies pulmonaires à partir de radiographies thoraciques.',
    venue: 'International Conference on Medical Imaging',
  },
  {
    id: '3',
    type: 'ARTICLE_JOURNAL',
    title: 'AI-Assisted Diagnosis System for Early Disease Detection',
    year: 2025,
    status: 'REJETE',
    quartile: 'Q1',
    authors: ['Sarah Trabelsi', 'Dr. Ahmed Ben Salem', 'Dr. Mohamed Mezghani'],
    axe: 'IA & Apprentissage Automatique',
    abstract: 'Un système d\'aide au diagnostic médical basé sur l\'IA pour la détection précoce des maladies cardiovasculaires.',
    venue: 'IEEE Transactions on Medical Imaging',
    rejectionReason: 'Le résumé doit être plus détaillé et inclure les résultats quantitatifs. Veuillez également clarifier la méthodologie expérimentale et augmenter la taille du jeu de données.',
  },
];

export default function DoctorantPublications() {
  const { t } = useLanguage();
  const typeLabels = useTypeLabels();
  const statusConfig = useStatusConfig();
  const [publications, setPublications] = useState<Publication[]>(initialPublications);
  const [showForm, setShowForm] = useState(false);
  const [editingPub, setEditingPub] = useState<Publication | null>(null);
  const [detailPub, setDetailPub] = useState<Publication | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<Publication | null>(null);
  const [submitTarget, setSubmitTarget] = useState<Publication | null>(null);
  const [searchQuery, setSearchQuery] = useState('');
  const [activeFilters, setActiveFilters] = useState<Record<string, string | string[]>>({});

  const filterGroups = [
    {
      id: 'status', label: t('common.status'), options: [
        { id: 'b', label: t('pub.draft'), value: 'BROUILLON' },
        { id: 's', label: t('pub.underReview'), value: 'SOUMIS' },
        { id: 'p', label: t('pub.published'), value: 'PUBLIE' },
        { id: 'r', label: t('pub.rejected'), value: 'REJETE' },
      ]
    },
    {
      id: 'type', label: t('common.type'), options: [
        { id: 'aj', label: t('pubType.journal'), value: 'ARTICLE_JOURNAL' },
        { id: 'ci', label: t('pubType.confInt'), value: 'CONFERENCE_INT' },
        { id: 'cn', label: t('pubType.confNat'), value: 'CONFERENCE_NAT' },
      ]
    },
    {
      id: 'year', label: t('common.year'), options: [
        { id: '2026', label: '2026', value: '2026' },
        { id: '2025', label: '2025', value: '2025' },
      ]
    },
  ];

  const filtered = publications.filter(pub => {
    const q = searchQuery.toLowerCase();
    const matchQ = !q || pub.title.toLowerCase().includes(q) || pub.authors.some(a => a.toLowerCase().includes(q));
    const sf = activeFilters.status as string;
    const tf = activeFilters.type as string;
    const yf = activeFilters.year as string;
    return matchQ && (!sf || pub.status === sf) && (!tf || pub.type === tf) && (!yf || pub.year.toString() === yf);
  });

  const stats = {
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
      <div className="flex items-start justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-navy dark:text-white">{t('pub.myPublications')}</h1>
          <p className="text-text-secondary mt-1">{t('pub.manageYourWork')}</p>
        </div>
        <Button onClick={openCreate} className="flex items-center gap-2 flex-shrink-0">
          <Plus size={18} /> {t('pub.newPublication')}
        </Button>
      </div>

      {/* Stats */}
      <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
        {[
          { label: t('pub.drafts'), value: stats.brouillon, cls: 'bg-light-gray dark:bg-muted text-text-secondary' },
          { label: t('pub.underReview'), value: stats.soumis, cls: 'bg-warning/10 text-warning' },
          { label: t('pub.publishedCount'), value: stats.publie, cls: 'bg-success/10 text-success' },
          { label: t('pub.rejectedCount'), value: stats.rejete, cls: 'bg-error/10 text-error' },
        ].map(s => (
          <Card key={s.label}>
            <CardContent className="p-4 flex items-center gap-3">
              <div className={clsx('w-10 h-10 rounded-lg flex items-center justify-center font-bold text-lg', s.cls)}>{s.value}</div>
              <div className="text-sm text-text-secondary">{s.label}</div>
            </CardContent>
          </Card>
        ))}
      </div>

      <div className="flex items-start gap-3 p-4 bg-accent-blue/5 border border-accent-blue/20 rounded-xl">
        <AlertCircle size={18} className="text-accent-blue flex-shrink-0 mt-0.5" />
        <p className="text-sm text-text-secondary">
          <strong className="text-navy dark:text-white">{t('pub.workflow')}:</strong> {t('pub.workflowDesc')}
        </p>
      </div>

      <SearchFilter
        searchPlaceholder={t('pub.searchPlaceholder')}
        filterGroups={filterGroups}
        onSearchChange={setSearchQuery}
        onFilterChange={setActiveFilters}
      />

      <div className="space-y-4">
        {filtered.length === 0 && (
          <Card>
            <CardContent className="py-16 text-center">
              <FileText size={40} className="mx-auto mb-3 text-text-muted opacity-30" />
              <p className="text-text-muted">
                {searchQuery || Object.keys(activeFilters).length > 0
                  ? t('pub.noResults')
                  : t('pub.createFirst')}
              </p>
            </CardContent>
          </Card>
        )}

        {filtered.map(pub => {
          const sc = statusConfig[pub.status];
          return (
            <Card
              key={pub.id}
              className={clsx(
                'transition-shadow hover:shadow-card-hover',
                pub.status === 'REJETE' && 'border-l-4 border-l-error',
                pub.status === 'PUBLIE' && 'border-l-4 border-l-success',
              )}
            >
              <CardContent className="p-5">
                <div className="flex items-start gap-4">
                  <div className="w-10 h-10 rounded-xl bg-accent-blue/10 flex items-center justify-center flex-shrink-0">
                    <FileText size={18} className="text-accent-blue" />
                  </div>
                  <div className="flex-1 min-w-0">
                    <div className="flex items-start justify-between gap-3">
                      <div className="flex-1 min-w-0">
                        <div className="flex items-center gap-2 flex-wrap mb-1.5">
                          <Badge variant={sc.variant}>{sc.label}</Badge>
                          <Badge variant="info" className="text-xs">{typeLabels[pub.type]}</Badge>
                          {pub.quartile && <Badge variant={`q${pub.quartile.charAt(1)}` as any}>{pub.quartile}</Badge>}
                          {pub.coreRanking && <Badge variant={`core-${pub.coreRanking.toLowerCase().replace('*', '-star')}` as any}>CORE {pub.coreRanking}</Badge>}
                          <span className="text-xs text-text-muted">{pub.year}</span>
                        </div>
                        <h3 className="font-bold text-navy dark:text-white text-sm leading-snug">{pub.title}</h3>
                        <p className="text-xs text-text-muted mt-1">{pub.authors.join(', ')}</p>
                        {pub.venue && <p className="text-xs text-text-muted italic mt-0.5">{pub.venue}</p>}
                        {pub.abstract && <p className="text-xs text-text-secondary mt-2 line-clamp-2">{pub.abstract}</p>}
                        {pub.rejectionReason && (
                          <div className="mt-3 p-3 bg-error/5 border border-error/20 rounded-lg">
                            <p className="text-xs font-semibold text-error mb-1">{t('pub.rejectionReason')}:</p>
                            <p className="text-xs text-text-secondary">{pub.rejectionReason}</p>
                          </div>
                        )}
                      </div>
                      <div className="flex items-center gap-1.5 flex-shrink-0">
                        <button
                          onClick={() => setDetailPub(pub)}
                          className="p-2 hover:bg-light-gray dark:hover:bg-muted rounded-lg text-text-muted hover:text-navy dark:hover:text-white transition-colors"
                          title={t('common.view')}
                        >
                          <Eye size={16} />
                        </button>
                        {pub.status === 'BROUILLON' && (
                          <button
                            onClick={() => setSubmitTarget(pub)}
                            className="flex items-center gap-1.5 px-3 py-1.5 bg-accent-blue/10 text-accent-blue hover:bg-accent-blue/20 rounded-lg text-xs font-medium transition-colors"
                          >
                            <Send size={13} /> {t('pub.submit')}
                          </button>
                        )}
                        {['BROUILLON', 'REJETE'].includes(pub.status) && (
                          <button
                            onClick={() => openEdit(pub)}
                            className="p-2 hover:bg-light-gray dark:hover:bg-muted rounded-lg text-accent-blue transition-colors"
                          >
                            <Pencil size={16} />
                          </button>
                        )}
                        {pub.status !== 'SOUMIS' && (
                          <button
                            onClick={() => setDeleteTarget(pub)}
                            className="p-2 hover:bg-error/10 rounded-lg text-error transition-colors"
                          >
                            <Trash2 size={16} />
                          </button>
                        )}
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

      {/* Detail Modal */}
      {detailPub && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-[60] p-4" onClick={() => setDetailPub(null)}>
          <div className="bg-white dark:bg-card rounded-2xl shadow-modal max-w-2xl w-full max-h-[90vh] overflow-y-auto" onClick={e => e.stopPropagation()}>
            <div className="p-6 border-b border-surface-border flex items-center justify-between">
              <h2 className="text-lg font-bold text-navy dark:text-white">{t('pub.publicationDetails')}</h2>
              <button onClick={() => setDetailPub(null)} className="p-2 hover:bg-light-gray dark:hover:bg-muted rounded-lg"><X size={18} /></button>
            </div>
            <div className="p-6 space-y-4">
              <div className="flex flex-wrap gap-2">
                <Badge variant={statusConfig[detailPub.status].variant}>{statusConfig[detailPub.status].label}</Badge>
                <Badge variant="info">{typeLabels[detailPub.type]}</Badge>
                {detailPub.quartile && <Badge variant={`q${detailPub.quartile.charAt(1)}` as any}>{detailPub.quartile}</Badge>}
                {detailPub.coreRanking && <Badge variant={`core-${detailPub.coreRanking.toLowerCase().replace('*', '-star')}` as any}>CORE {detailPub.coreRanking}</Badge>}
                <Badge variant="default">{detailPub.year}</Badge>
              </div>
              <h3 className="text-lg font-bold text-navy dark:text-white leading-snug">{detailPub.title}</h3>
              <div className="space-y-2 text-sm">
                <div><span className="font-semibold text-navy dark:text-white">{t('pub.authors')}:</span> <span className="text-text-secondary">{detailPub.authors.join(', ')}</span></div>
                {detailPub.venue && <div><span className="font-semibold text-navy dark:text-white">Revue :</span> <span className="text-text-secondary italic">{detailPub.venue}</span></div>}
                <div>
                  <span className="font-semibold text-navy dark:text-white">{t('pub.axis')}:</span>{' '}
                  <span className="inline-flex items-center gap-1.5 px-2.5 py-1 bg-accent-blue/10 text-accent-blue rounded-full text-xs font-medium">
                    <Target size={12} /> {detailPub.axe}
                  </span>
                </div>
                {detailPub.doi && (
                  <div className="flex items-center gap-1.5 flex-wrap">
                    <span className="font-semibold text-navy dark:text-white">DOI :</span>
                    <a href={detailPub.doi} target="_blank" rel="noopener noreferrer" className="text-accent-blue hover:underline flex items-center gap-1 text-sm break-all">
                      {detailPub.doi} <ExternalLink size={12} />
                    </a>
                  </div>
                )}
                {detailPub.abstract && (
                  <div>
                    <p className="font-semibold text-navy dark:text-white mb-1.5">{t('pub.abstract')}:</p>
                    <p className="text-text-secondary leading-relaxed">{detailPub.abstract}</p>
                  </div>
                )}
                {detailPub.rejectionReason && (
                  <div className="p-3 bg-error/5 border border-error/20 rounded-lg">
                    <p className="font-semibold text-error mb-1">Motif de rejet :</p>
                    <p className="text-text-secondary">{detailPub.rejectionReason}</p>
                  </div>
                )}
              </div>
            </div>
            <div className="p-6 border-t border-surface-border flex justify-end gap-3">
              {['BROUILLON', 'REJETE'].includes(detailPub.status) && (
                <Button variant="outlined" onClick={() => { openEdit(detailPub); setDetailPub(null); }}>
                  <Pencil size={16} /> Modifier
                </Button>
              )}
              <Button variant="outlined" onClick={() => setDetailPub(null)}>{t('common.close')}</Button>
            </div>
          </div>
        </div>
      )}

      <ConfirmDialog
        isOpen={!!deleteTarget}
        onClose={() => setDeleteTarget(null)}
        onConfirm={handleDelete}
        title={t('pub.deletePublication')}
        description={`${t('pub.confirmDelete')} "${deleteTarget?.title}" ? ${t('pub.deleteWarning')}`}
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
