import { useState } from 'react';
import { Card, CardContent } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { Button } from '../../../components/ui/Button';
import { SearchFilter } from '../../../components/shared/SearchFilter';
import { ConfirmDialog } from '../../../components/shared/ConfirmDialog';
import { PublicationForm } from '../../../components/publications/PublicationForm';
import { FileText, Pencil, Trash2, ExternalLink, Eye } from 'lucide-react';
import { toast } from 'sonner';
import { clsx } from 'clsx';

type PublicationType = 'ARTICLE_JOURNAL' | 'CONFERENCE_INT' | 'CONFERENCE_NAT' | 'CHAPITRE_OUVRAGE' | 'RAPPORT_TECHNIQUE';
type PublicationStatus = 'BROUILLON' | 'SOUMIS' | 'PUBLIE' | 'REJETE';

interface Publication {
  id: string;
  type: PublicationType;
  title: string;
  year: number;
  status: PublicationStatus;
  visibility: 'PUBLIQUE' | 'PRIVEE';
  quartile?: 'Q1' | 'Q2' | 'Q3' | 'Q4';
  coreRanking?: 'A*' | 'A' | 'B' | 'C';
  authors: string[];
  axe: string;
  submittedBy?: string;
  rejectionReason?: string;
}

export default function SuperAdminPublications() {
  const [searchQuery, setSearchQuery] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [editingPublication, setEditingPublication] = useState<Publication | null>(null);
  const [deleteConfirm, setDeleteConfirm] = useState<Publication | null>(null);
  const [activeFilters, setActiveFilters] = useState<Record<string, string[]>>({});

  const allPublications: Publication[] = [
    {
      id: '1',
      type: 'ARTICLE_JOURNAL',
      title: 'Deep Learning for Medical Imaging Diagnosis',
      year: 2026,
      status: 'PUBLIE',
      visibility: 'PUBLIQUE',
      quartile: 'Q1',
      authors: ['Dr. Ahmed Ben Salem', 'Sarah Trabelsi', 'Prof. Marie Dubois'],
      axe: 'Intelligence Artificielle et Apprentissage Automatique'
    },
    {
      id: '2',
      type: 'CONFERENCE_INT',
      title: 'Blockchain Security Analysis for Healthcare Data',
      year: 2026,
      status: 'SOUMIS',
      visibility: 'PUBLIQUE',
      coreRanking: 'A*',
      authors: ['Dr. Fatma Gharbi', 'Mohamed Najjar'],
      axe: 'Sécurité Informatique et Cryptographie',
      submittedBy: 'Mohamed Najjar (Doctorant)'
    },
    {
      id: '3',
      type: 'ARTICLE_JOURNAL',
      title: 'Federated Learning Approaches for Privacy-Preserving ML',
      year: 2025,
      status: 'BROUILLON',
      visibility: 'PRIVEE',
      quartile: 'Q2',
      authors: ['Dr. Ahmed Ben Salem', 'Dr. Mohamed Mezghani'],
      axe: 'Intelligence Artificielle et Apprentissage Automatique'
    },
    {
      id: '4',
      type: 'CONFERENCE_NAT',
      title: 'IoT Security in Smart Cities',
      year: 2025,
      status: 'REJETE',
      visibility: 'PRIVEE',
      authors: ['Karim Slimi'],
      axe: 'Sécurité Informatique et Cryptographie',
      submittedBy: 'Karim Slimi (Mastérien)',
      rejectionReason: 'Méthodologie insuffisante et absence de résultats expérimentaux significatifs'
    },
  ];

  const filterGroups = [
    {
      id: 'status',
      label: 'Statut',
      options: [
        { id: 'brouillon', label: 'Brouillon', value: 'BROUILLON' },
        { id: 'soumis', label: 'Soumis', value: 'SOUMIS' },
        { id: 'publie', label: 'Publié', value: 'PUBLIE' },
        { id: 'rejete', label: 'Rejeté', value: 'REJETE' },
      ]
    },
    {
      id: 'type',
      label: 'Type',
      options: [
        { id: 'article', label: 'Article de journal', value: 'ARTICLE_JOURNAL' },
        { id: 'confint', label: 'Conférence internationale', value: 'CONFERENCE_INT' },
        { id: 'confnat', label: 'Conférence nationale', value: 'CONFERENCE_NAT' },
        { id: 'chapitre', label: 'Chapitre d\'ouvrage', value: 'CHAPITRE_OUVRAGE' },
        { id: 'rapport', label: 'Rapport technique', value: 'RAPPORT_TECHNIQUE' },
      ]
    },
    {
      id: 'visibility',
      label: 'Visibilité',
      options: [
        { id: 'publique', label: 'Publique', value: 'PUBLIQUE' },
        { id: 'privee', label: 'Privée', value: 'PRIVEE' },
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

  const handleDelete = (pub: Publication) => {
    toast.success(`"${pub.title}" supprimée avec succès`);
    setDeleteConfirm(null);
  };

  const handleApprove = (pub: Publication) => {
    toast.success(`"${pub.title}" approuvée et publiée`);
  };

  const handleReject = (pub: Publication) => {
    toast.success(`"${pub.title}" rejetée`);
  };

  const getStatusVariant = (status: PublicationStatus) => {
    const config = {
      'PUBLIE': 'success' as const,
      'SOUMIS': 'warning' as const,
      'BROUILLON': 'default' as const,
      'REJETE': 'error' as const,
    };
    return config[status];
  };

  const getTypeLabel = (type: PublicationType) => {
    const labels = {
      'ARTICLE_JOURNAL': 'Article de journal',
      'CONFERENCE_INT': 'Conf. internationale',
      'CONFERENCE_NAT': 'Conf. nationale',
      'CHAPITRE_OUVRAGE': 'Chapitre d\'ouvrage',
      'RAPPORT_TECHNIQUE': 'Rapport technique',
    };
    return labels[type];
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-navy dark:text-white mb-2">Gestion des Publications</h1>
          <p className="text-text-secondary">Gérer toutes les publications du laboratoire</p>
        </div>
        <Button onClick={() => { setEditingPublication(null); setShowForm(true); }}>
          <FileText size={18} />
          Créer une publication
        </Button>
      </div>

      {/* Stats Cards */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <Card>
          <CardContent className="p-4">
            <div className="text-2xl font-bold text-navy dark:text-white">
              {allPublications.filter(p => p.status === 'PUBLIE').length}
            </div>
            <div className="text-sm text-text-secondary">Publiées</div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4">
            <div className="text-2xl font-bold text-warning">
              {allPublications.filter(p => p.status === 'SOUMIS').length}
            </div>
            <div className="text-sm text-text-secondary">En attente</div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4">
            <div className="text-2xl font-bold text-text-muted">
              {allPublications.filter(p => p.status === 'BROUILLON').length}
            </div>
            <div className="text-sm text-text-secondary">Brouillons</div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4">
            <div className="text-2xl font-bold text-error">
              {allPublications.filter(p => p.status === 'REJETE').length}
            </div>
            <div className="text-sm text-text-secondary">Rejetées</div>
          </CardContent>
        </Card>
      </div>

      {/* Search and Filters */}
      <SearchFilter
        onSearchChange={setSearchQuery}
        filterGroups={filterGroups}
        onFilterChange={setActiveFilters}
        searchPlaceholder="Rechercher une publication..."
      />

      {/* Publications List */}
      {filteredPublications.length === 0 ? (
        <Card>
          <CardContent className="p-12 text-center">
            <div className="bg-accent-blue/5 dark:bg-accent-blue/10 rounded-xl p-8 inline-block">
              <FileText size={64} className="mx-auto text-accent-blue mb-4" />
              <h3 className="text-xl font-bold text-navy dark:text-white mb-2">
                Aucune publication trouvée
              </h3>
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
                  pub.status === 'SOUMIS' ? 'border-warning bg-warning/5' : 'border-surface-border'
                )}
              >
                <div className="flex items-start justify-between gap-4">
                  <div className="flex-1 min-w-0">
                    {/* Header */}
                    <div className="flex flex-wrap items-center gap-2 mb-3">
                      {pub.quartile && <Badge variant={`q${pub.quartile.charAt(1)}` as any}>{pub.quartile}</Badge>}
                      {pub.coreRanking && <Badge variant={`core-${pub.coreRanking.toLowerCase().replace('*', '-star')}` as any}>CORE {pub.coreRanking}</Badge>}
                      <Badge variant="default">{pub.year}</Badge>
                      <Badge variant={getStatusVariant(pub.status)}>{pub.status}</Badge>
                      {pub.visibility === 'PRIVEE' && <Badge variant="default">Privée</Badge>}
                      <Badge variant="info" className="text-xs">{getTypeLabel(pub.type)}</Badge>
                    </div>

                    {/* Title */}
                    <h3 className="font-bold text-navy dark:text-white text-lg mb-2">{pub.title}</h3>

                    {/* Meta Info */}
                    <div className="space-y-1 text-sm text-text-secondary">
                      <div><span className="font-medium">Auteurs:</span> {pub.authors.join(', ')}</div>
                      <div><span className="font-medium">Axe de recherche:</span> {pub.axe}</div>
                      {pub.submittedBy && (
                        <div className="text-accent-blue"><span className="font-medium">Soumis par:</span> {pub.submittedBy}</div>
                      )}
                      {pub.status === 'REJETE' && pub.rejectionReason && (
                        <div className="p-3 bg-error/5 border border-error/20 rounded-lg text-error mt-3">
                          <span className="font-medium">Raison du rejet:</span> {pub.rejectionReason}
                        </div>
                      )}
                    </div>
                  </div>

                  {/* Actions */}
                  <div className="flex flex-col gap-2">
                    {pub.status === 'SOUMIS' && (
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
                          onClick={() => handleReject(pub)}
                        >
                          Rejeter
                        </Button>
                      </>
                    )}
                    <Button
                      variant="outlined"
                      className="whitespace-nowrap text-xs"
                      onClick={() => { setEditingPublication(pub); setShowForm(true); }}
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

      {/* Publication Form Modal */}
      {showForm && (
        <PublicationForm
          onClose={() => { setShowForm(false); setEditingPublication(null); }}
          onSubmit={(data) => console.log(data)}
          initialData={editingPublication}
          canPublishDirectly={true}
        />
      )}

      {/* Delete Confirmation */}
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
    </div>
  );
}
