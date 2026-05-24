import { useState } from 'react';
import { Card, CardContent, CardHeader } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { Button } from '../../../components/ui/Button';
import { SearchFilter } from '../../../components/shared/SearchFilter';
import { ConfirmDialog } from '../../../components/shared/ConfirmDialog';
import { Target, Pencil, Trash2, X, Users, BookOpen, Plus } from 'lucide-react';
import { toast } from 'sonner';
import { clsx } from 'clsx';
import { useLanguage } from '../../../contexts/LanguageContext';

interface Axe {
  id: string;
  title: string;
  description: string;
  responsible: {
    id: string;
    name: string;
  };
  themes: string[];
  members: number;
  publications: number;
  color: string;
}

export default function SuperAdminAxes() {
  const { t } = useLanguage();
  const [searchQuery, setSearchQuery] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [editingAxe, setEditingAxe] = useState<Axe | null>(null);
  const [deleteConfirm, setDeleteConfirm] = useState<Axe | null>(null);

  const allAxes: Axe[] = [
    {
      id: '1',
      title: 'Intelligence Artificielle et Apprentissage Automatique',
      description: 'Recherche avancée en IA, machine learning, deep learning et leurs applications dans divers domaines.',
      responsible: { id: '1', name: 'Dr. Ahmed Ben Salem' },
      themes: ['Machine Learning', 'Deep Learning', 'NLP', 'Computer Vision', 'Reinforcement Learning'],
      members: 8,
      publications: 45,
      color: 'accent-blue'
    },
    {
      id: '2',
      title: 'Sécurité Informatique et Cryptographie',
      description: 'Étude des mécanismes de sécurité, cryptographie, blockchain et protection des systèmes d\'information.',
      responsible: { id: '2', name: 'Dr. Fatma Gharbi' },
      themes: ['Cryptographie', 'Blockchain', 'Sécurité Réseau', 'Cybersécurité', 'Protection des Données'],
      members: 6,
      publications: 32,
      color: 'error'
    },
    {
      id: '3',
      title: 'Systèmes Distribués et Cloud Computing',
      description: 'Développement et optimisation de systèmes distribués, cloud computing et architectures scalables.',
      responsible: { id: '3', name: 'Dr. Mohamed Mezghani' },
      themes: ['Cloud Computing', 'Microservices', 'Container Orchestration', 'Edge Computing'],
      members: 5,
      publications: 28,
      color: 'teal'
    },
    {
      id: '4',
      title: 'Traitement d\'Images et Vision par Ordinateur',
      description: 'Traitement et analyse d\'images, reconnaissance de formes et vision artificielle.',
      responsible: { id: '4', name: 'Dr. Leila Ammar' },
      themes: ['Traitement d\'Image', 'Vision par Ordinateur', 'Reconnaissance de Formes', 'Imagerie Médicale'],
      members: 7,
      publications: 38,
      color: 'success'
    },
    {
      id: '5',
      title: 'Big Data et Science des Données',
      description: 'Analyse de données massives, data mining, visualisation et aide à la décision.',
      responsible: { id: '5', name: 'Dr. Karim Jebali' },
      themes: ['Big Data', 'Data Mining', 'Data Visualization', 'Business Intelligence', 'Data Analytics'],
      members: 6,
      publications: 35,
      color: 'warning'
    },
  ];

  const filteredAxes = allAxes.filter(axe => {
    return searchQuery === '' ||
      axe.title.toLowerCase().includes(searchQuery.toLowerCase()) ||
      axe.description.toLowerCase().includes(searchQuery.toLowerCase()) ||
      axe.themes.some(t => t.toLowerCase().includes(searchQuery.toLowerCase()));
  });

  const handleDelete = (axe: Axe) => {
    toast.success(t('axes.axisDeleted'));
    setDeleteConfirm(null);
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-navy dark:text-white mb-2">{t('axes.manageAxes')}</h1>
          <p className="text-text-secondary">{t('axes.manageLabAxes')}</p>
        </div>
        <Button onClick={() => { setEditingAxe(null); setShowForm(true); }}>
          <Target size={18} />
          {t('axes.createAxis')}
        </Button>
      </div>

      {/* Stats */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <Card>
          <CardContent className="p-4">
            <div className="text-2xl font-bold text-navy dark:text-white">
              {allAxes.length}
            </div>
            <div className="text-sm text-text-secondary">{t('axes.axesCount')}</div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4">
            <div className="text-2xl font-bold text-accent-blue">
              {allAxes.reduce((sum, axe) => sum + axe.members, 0)}
            </div>
            <div className="text-sm text-text-secondary">{t('axes.totalMembers')}</div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4">
            <div className="text-2xl font-bold text-success">
              {allAxes.reduce((sum, axe) => sum + axe.publications, 0)}
            </div>
            <div className="text-sm text-text-secondary">{t('axes.totalPublications')}</div>
          </CardContent>
        </Card>
      </div>

      {/* Search */}
      <SearchFilter
        onSearchChange={setSearchQuery}
        filterGroups={[]}
        searchPlaceholder={t('pub.searchPlaceholder')}
      />

      {/* Axes List */}
      {filteredAxes.length === 0 ? (
        <Card>
          <CardContent className="p-12 text-center">
            <Target size={64} className="mx-auto text-text-muted mb-4" />
            <h3 className="text-xl font-bold text-navy dark:text-white mb-2">
              {t('axes.noAxesFoundGeneric')}
            </h3>
            <p className="text-text-secondary">
              {t('axes.noResults')}
            </p>
          </CardContent>
        </Card>
      ) : (
        <div className="grid grid-cols-1 gap-6">
          {filteredAxes.map((axe) => (
            <Card key={axe.id} className="hover:shadow-lg transition-shadow">
              <CardHeader className={clsx('border-l-4', `border-${axe.color}`)}>
                <div className="flex items-start justify-between gap-4">
                  <div className="flex-1">
                    <h3 className="text-2xl font-bold text-navy dark:text-white mb-2">{axe.title}</h3>
                    <p className="text-text-secondary mb-4">{axe.description}</p>

                    <div className="flex items-center gap-4 text-sm text-text-secondary mb-4">
                      <div className="flex items-center gap-2">
                        <Users size={16} className="text-accent-blue" />
                        <span><strong>{t('axes.responsible')}:</strong> {axe.responsible.name}</span>
                      </div>
                      <div className="flex items-center gap-2">
                        <Users size={16} className="text-teal" />
                        <span>{axe.members} {t('axes.members').toLowerCase()}</span>
                      </div>
                      <div className="flex items-center gap-2">
                        <BookOpen size={16} className="text-success" />
                        <span>{axe.publications} {t('axes.publicationsCount')}</span>
                      </div>
                    </div>

                    <div>
                      <div className="text-sm font-medium mb-2">{t('axes.thematics')}:</div>
                      <div className="flex flex-wrap gap-2">
                        {axe.themes.map((theme, idx) => (
                          <Badge key={idx} variant="info" className="text-xs">{theme}</Badge>
                        ))}
                      </div>
                    </div>
                  </div>

                  <div className="flex flex-col gap-2">
                    <Button
                      variant="outlined"
                      onClick={() => { setEditingAxe(axe); setShowForm(true); }}
                    >
                      <Pencil size={16} />
                      {t('common.modify')}
                    </Button>
                    <Button
                      variant="outlined"
                      className="text-error hover:bg-error/5 hover:border-error"
                      onClick={() => setDeleteConfirm(axe)}
                    >
                      <Trash2 size={16} />
                      {t('common.delete')}
                    </Button>
                  </div>
                </div>
              </CardHeader>
            </Card>
          ))}
        </div>
      )}

      {/* Axe Form Modal */}
      {showForm && (
        <AxeFormModal
          axe={editingAxe}
          onClose={() => { setShowForm(false); setEditingAxe(null); }}
        />
      )}

      {/* Delete Confirmation */}
      {deleteConfirm && (
        <ConfirmDialog
          isOpen={true}
          onClose={() => setDeleteConfirm(null)}
          onConfirm={() => handleDelete(deleteConfirm)}
          title={t('axes.deleteAxis')}
          description={`${t('axes.deleteMessage')} "${deleteConfirm.title}" ? ${t('axes.deleteWarning')} ${deleteConfirm.members} ${t('axes.membersAnd')} ${deleteConfirm.publications} ${t('axes.publicationsCount')}.`}
          confirmText={t('common.delete')}
          variant="danger"
        />
      )}
    </div>
  );
}

interface AxeFormModalProps {
  axe: Axe | null;
  onClose: () => void;
}

function AxeFormModal({ axe, onClose }: AxeFormModalProps) {
  const { t } = useLanguage();
  const [formData, setFormData] = useState({
    title: axe?.title || '',
    description: axe?.description || '',
    responsibleId: axe?.responsible.id || '',
    themes: axe?.themes || [],
    color: axe?.color || 'accent-blue',
  });

  const [currentTheme, setCurrentTheme] = useState('');

  const chercheurs = [
    { id: '1', name: 'Dr. Ahmed Ben Salem' },
    { id: '2', name: 'Dr. Fatma Gharbi' },
    { id: '3', name: 'Dr. Mohamed Mezghani' },
    { id: '4', name: 'Dr. Leila Ammar' },
    { id: '5', name: 'Dr. Karim Jebali' },
  ];

  const handleAddTheme = () => {
    if (currentTheme.trim() && !formData.themes.includes(currentTheme.trim())) {
      setFormData({ ...formData, themes: [...formData.themes, currentTheme.trim()] });
      setCurrentTheme('');
    }
  };

  const handleRemoveTheme = (theme: string) => {
    setFormData({ ...formData, themes: formData.themes.filter(t => t !== theme) });
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    toast.success(axe ? t('axes.axisModified') : t('axes.axisCreated'));
    onClose();
  };

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
      <div className="bg-white dark:bg-card rounded-2xl shadow-[0_8px_32px_rgba(15,37,87,.16)] max-w-2xl w-full max-h-[90vh] overflow-hidden flex flex-col">
        <div className="px-6 py-4 border-b border-surface-border flex items-center justify-between">
          <h2 className="text-2xl font-bold text-navy dark:text-white">
            {axe ? t('axes.editAxisTitle') : t('axes.createAxisTitle')}
          </h2>
          <button onClick={onClose} className="p-2 hover:bg-light-gray rounded-lg">
            <X size={24} />
          </button>
        </div>

        <form onSubmit={handleSubmit} className="flex-1 overflow-y-auto p-6 space-y-6">
          <div>
            <label className="block text-sm font-medium mb-2">{t('axes.axisTitle')} *</label>
            <input
              type="text"
              required
              value={formData.title}
              onChange={(e) => setFormData({ ...formData, title: e.target.value })}
              className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background"
              placeholder="Intelligence Artificielle et Apprentissage Automatique"
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-2">{t('axes.description')} *</label>
            <textarea
              required
              value={formData.description}
              onChange={(e) => setFormData({ ...formData, description: e.target.value })}
              rows={4}
              className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent resize-none dark:bg-input-background"
              placeholder={t('axes.descriptionPlaceholder')}
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-2">{t('axes.responsible')} *</label>
            <select
              required
              value={formData.responsibleId}
              onChange={(e) => setFormData({ ...formData, responsibleId: e.target.value })}
              className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background"
            >
              <option value="">{t('axes.responsibleSelect')}</option>
              {chercheurs.map(c => (
                <option key={c.id} value={c.id}>{c.name}</option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium mb-2">{t('axes.thematics')}</label>
            <div className="flex gap-2 mb-3">
              <input
                type="text"
                value={currentTheme}
                onChange={(e) => setCurrentTheme(e.target.value)}
                onKeyPress={(e) => e.key === 'Enter' && (e.preventDefault(), handleAddTheme())}
                className="flex-1 px-4 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background"
                placeholder={t('axes.thematicsPlaceholder')}
              />
              <Button onClick={handleAddTheme} type="button">
                <Plus size={16} />
                {t('axes.addTheme')}
              </Button>
            </div>
            <div className="flex flex-wrap gap-2">
              {formData.themes.map((theme, idx) => (
                <Badge key={idx} variant="info" className="flex items-center gap-2">
                  {theme}
                  <button onClick={() => handleRemoveTheme(theme)} className="hover:text-error">
                    <X size={14} />
                  </button>
                </Badge>
              ))}
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium mb-2">{t('axes.color')} *</label>
            <select
              required
              value={formData.color}
              onChange={(e) => setFormData({ ...formData, color: e.target.value })}
              className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background"
            >
              <option value="accent-blue">{t('axes.colorBlue')}</option>
              <option value="error">{t('axes.colorRed')}</option>
              <option value="teal">{t('axes.colorTeal')}</option>
              <option value="success">{t('axes.colorGreen')}</option>
              <option value="warning">{t('axes.colorOrange')}</option>
            </select>
          </div>
        </form>

        <div className="px-6 py-4 border-t border-surface-border flex justify-end gap-3">
          <Button onClick={onClose} variant="outlined">{t('common.cancel')}</Button>
          <Button onClick={handleSubmit}>{axe ? t('common.modify') : t('common.create')}</Button>
        </div>
      </div>
    </div>
  );
}
