import { useState } from 'react';
import { Card, CardContent, CardHeader } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { Button } from '../../../components/ui/Button';
import { SearchFilter } from '../../../components/shared/SearchFilter';
import { ConfirmDialog } from '../../../components/shared/ConfirmDialog';
import { Target, Pencil, Trash2, X, Users, BookOpen, Plus, Loader2 } from 'lucide-react';
import { toast } from 'sonner';
import { clsx } from 'clsx';
import { useLanguage } from '../../../contexts/LanguageContext';
import { useGetResearchAxesQuery, useGetAllResearchersQuery, type ResearchAxisDto } from '../../../api/profilesApi';
import { useCreateAxisMutation, useUpdateAxisMutation, useDeleteAxisMutation, type AxisUpsertBody } from '../../../api/axesApi';

export default function SuperAdminAxes() {
  const { t } = useLanguage();
  const [searchQuery, setSearchQuery] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [editingAxe, setEditingAxe] = useState<ResearchAxisDto | null>(null);
  const [deleteConfirm, setDeleteConfirm] = useState<ResearchAxisDto | null>(null);

  const { data: allAxes = [], isLoading, refetch } = useGetResearchAxesQuery();
  const [deleteAxis, { isLoading: isDeleting }] = useDeleteAxisMutation();

  const filteredAxes = allAxes.filter(axe =>
    searchQuery === '' ||
    axe.title.toLowerCase().includes(searchQuery.toLowerCase()) ||
    axe.description.toLowerCase().includes(searchQuery.toLowerCase()) ||
    axe.themes.some(th => th.toLowerCase().includes(searchQuery.toLowerCase()))
  );

  const handleDelete = async (axe: ResearchAxisDto) => {
    try {
      await deleteAxis(axe.id).unwrap();
      toast.success(t('axes.axisDeleted'));
      refetch();
    } catch (err: any) {
      if (err?.status === 422) {
        toast.error(t('axes.cannotDeleteWithPublications') || 'Cannot delete: axis has linked publications');
      } else {
        toast.error(t('users.errorOccurred'));
      }
    }
    setDeleteConfirm(null);
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center py-24">
        <Loader2 size={32} className="animate-spin text-accent-blue" />
      </div>
    );
  }

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

      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <Card>
          <CardContent className="p-4">
            <div className="text-2xl font-bold text-navy dark:text-white">{allAxes.length}</div>
            <div className="text-sm text-text-secondary">{t('axes.axesCount')}</div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4">
            <div className="text-2xl font-bold text-accent-blue">
              {allAxes.reduce((sum, axe) => sum + (axe.members?.length ?? 0), 0)}
            </div>
            <div className="text-sm text-text-secondary">{t('axes.totalMembers')}</div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4">
            <div className="text-2xl font-bold text-success">
              {allAxes.reduce((sum, axe) => sum + (axe.publicationsCount ?? 0), 0)}
            </div>
            <div className="text-sm text-text-secondary">{t('axes.totalPublications')}</div>
          </CardContent>
        </Card>
      </div>

      <SearchFilter
        onSearchChange={setSearchQuery}
        filterGroups={[]}
        searchPlaceholder={t('pub.searchPlaceholder')}
      />

      {filteredAxes.length === 0 ? (
        <Card>
          <CardContent className="p-12 text-center">
            <Target size={64} className="mx-auto text-text-muted mb-4" />
            <h3 className="text-xl font-bold text-navy dark:text-white mb-2">{t('axes.noAxesFoundGeneric')}</h3>
            <p className="text-text-secondary">{t('axes.noResults')}</p>
          </CardContent>
        </Card>
      ) : (
        <div className="grid grid-cols-1 gap-6">
          {filteredAxes.map(axe => (
            <Card key={axe.id} className="hover:shadow-lg transition-shadow">
              <CardHeader className={clsx('border-l-4', `border-${axe.color ?? 'accent-blue'}`)}>
                <div className="flex items-start justify-between gap-4">
                  <div className="flex-1">
                    <h3 className="text-2xl font-bold text-navy dark:text-white mb-2">{axe.title}</h3>
                    <p className="text-text-secondary mb-4">{axe.description}</p>

                    <div className="flex items-center gap-4 text-sm text-text-secondary mb-4">
                      <div className="flex items-center gap-2">
                        <Users size={16} className="text-accent-blue" />
                        <span>
                          <strong>{t('axes.responsible')}:</strong>{' '}
                          {axe.responsibleName ?? '—'}
                        </span>
                      </div>
                      <div className="flex items-center gap-2">
                        <Users size={16} className="text-teal" />
                        <span>{axe.members?.length ?? 0} {t('axes.members').toLowerCase()}</span>
                      </div>
                      <div className="flex items-center gap-2">
                        <BookOpen size={16} className="text-success" />
                        <span>{axe.publicationsCount ?? 0} {t('axes.publicationsCount')}</span>
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
                    <Button variant="outlined" onClick={() => { setEditingAxe(axe); setShowForm(true); }}>
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

      {showForm && (
        <AxeFormModal
          axe={editingAxe}
          onClose={() => { setShowForm(false); setEditingAxe(null); }}
          onSaved={refetch}
        />
      )}

      {deleteConfirm && (
        <ConfirmDialog
          isOpen={true}
          onClose={() => setDeleteConfirm(null)}
          onConfirm={() => handleDelete(deleteConfirm)}
          title={t('axes.deleteAxis')}
          description={`${t('axes.deleteMessage')} "${deleteConfirm.title}" ? ${t('axes.deleteWarning')} ${deleteConfirm.members?.length ?? 0} ${t('axes.membersAnd')} ${deleteConfirm.publicationsCount ?? 0} ${t('axes.publicationsCount')}.`}
          confirmText={t('common.delete')}
          variant="danger"
        />
      )}
    </div>
  );
}

interface AxeFormModalProps {
  axe: ResearchAxisDto | null;
  onClose: () => void;
  onSaved: () => void;
}

function AxeFormModal({ axe, onClose, onSaved }: AxeFormModalProps) {
  const { t } = useLanguage();
  const [formData, setFormData] = useState<AxisUpsertBody>({
    title: axe?.title ?? '',
    description: axe?.description ?? '',
    responsibleId: axe?.responsibleId ?? null,
    themes: axe ? [...axe.themes] : [],
    color: axe?.color ?? 'accent-blue',
  });
  const [currentTheme, setCurrentTheme] = useState('');

  const { data: researchers = [] } = useGetAllResearchersQuery();
  const [createAxis, { isLoading: isCreating }] = useCreateAxisMutation();
  const [updateAxis, { isLoading: isUpdating }] = useUpdateAxisMutation();
  const isSaving = isCreating || isUpdating;

  const handleAddTheme = () => {
    const trimmed = currentTheme.trim();
    if (trimmed && !formData.themes?.includes(trimmed)) {
      setFormData(prev => ({ ...prev, themes: [...(prev.themes ?? []), trimmed] }));
      setCurrentTheme('');
    }
  };

  const handleRemoveTheme = (theme: string) => {
    setFormData(prev => ({ ...prev, themes: (prev.themes ?? []).filter(t => t !== theme) }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const payload: AxisUpsertBody = {
        ...formData,
        responsibleId: formData.responsibleId || null,
      };

      if (axe) {
        await updateAxis({ id: axe.id, data: payload }).unwrap();
        toast.success(t('axes.axisModified'));
      } else {
        await createAxis(payload).unwrap();
        toast.success(t('axes.axisCreated'));
      }
      onSaved();
      onClose();
    } catch {
      toast.error(t('users.errorOccurred'));
    }
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
              onChange={e => setFormData(prev => ({ ...prev, title: e.target.value }))}
              className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background"
              placeholder="Intelligence Artificielle et Apprentissage Automatique"
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-2">{t('axes.description')} *</label>
            <textarea
              required
              value={formData.description}
              onChange={e => setFormData(prev => ({ ...prev, description: e.target.value }))}
              rows={4}
              className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent resize-none dark:bg-input-background"
              placeholder={t('axes.descriptionPlaceholder')}
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-2">{t('axes.responsible')}</label>
            <select
              value={formData.responsibleId ?? ''}
              onChange={e => setFormData(prev => ({ ...prev, responsibleId: e.target.value || null }))}
              className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background"
            >
              <option value="">{t('axes.responsibleSelect')}</option>
              {researchers.map(r => (
                <option key={r.id} value={r.id}>
                  {r.firstName} {r.lastName}
                </option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium mb-2">{t('axes.thematics')}</label>
            <div className="flex gap-2 mb-3">
              <input
                type="text"
                value={currentTheme}
                onChange={e => setCurrentTheme(e.target.value)}
                onKeyDown={e => { if (e.key === 'Enter') { e.preventDefault(); handleAddTheme(); } }}
                className="flex-1 px-4 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background"
                placeholder={t('axes.thematicsPlaceholder')}
              />
              <Button onClick={handleAddTheme} type="button">
                <Plus size={16} />
                {t('axes.addTheme')}
              </Button>
            </div>
            <div className="flex flex-wrap gap-2">
              {(formData.themes ?? []).map((theme, idx) => (
                <Badge key={idx} variant="info" className="flex items-center gap-2">
                  {theme}
                  <button type="button" onClick={() => handleRemoveTheme(theme)} className="hover:text-error">
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
              value={formData.color ?? 'accent-blue'}
              onChange={e => setFormData(prev => ({ ...prev, color: e.target.value }))}
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
          <Button onClick={onClose} variant="outlined" disabled={isSaving}>
            {t('common.cancel')}
          </Button>
          <Button onClick={handleSubmit as any} disabled={isSaving}>
            {isSaving && <Loader2 size={16} className="animate-spin" />}
            {axe ? t('common.modify') : t('common.create')}
          </Button>
        </div>
      </div>
    </div>
  );
}
