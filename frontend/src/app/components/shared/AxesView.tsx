import { useState } from 'react';
import { Card, CardContent } from '../ui/Card';
import { Badge } from '../ui/Badge';
import { Button } from '../ui/Button';
import { Target, Users, FileText, ChevronRight, X, Lightbulb, Loader2 } from 'lucide-react';
import { clsx } from 'clsx';
import { useLanguage } from '../../contexts/LanguageContext';
import { useGetResearchAxesQuery, type ResearchAxisDto } from '../../api/profilesApi';

const colorMap: Record<string, string> = {
  'accent-blue': 'bg-accent-blue/10 text-accent-blue border-accent-blue/20',
  'error': 'bg-error/10 text-error border-error/20',
  'teal': 'bg-teal/10 text-teal border-teal/20',
  'success': 'bg-success/10 text-success border-success/20',
  'warning': 'bg-warning/10 text-warning border-warning/20',
};

const dotColorMap: Record<string, string> = {
  'accent-blue': 'bg-accent-blue',
  'error': 'bg-error',
  'teal': 'bg-teal',
  'success': 'bg-success',
  'warning': 'bg-warning',
};

function resolveColor(color: string | null | undefined): string {
  return color && colorMap[color] ? color : 'accent-blue';
}

function getInitials(name: string): string {
  return name
    .split(' ')
    .filter(w => !w.startsWith('Dr.') && !w.startsWith('Prof.'))
    .map(w => w[0])
    .filter(Boolean)
    .slice(0, 2)
    .join('');
}

interface AxesViewProps {
  myAxisIds?: string[];
  myAxisBannerLabel?: string;
  myAxisModalLabel?: string;
}

export function AxesView({ myAxisIds = [], myAxisBannerLabel, myAxisModalLabel }: AxesViewProps) {
  const { t } = useLanguage();
  const [selectedAxe, setSelectedAxe] = useState<ResearchAxisDto | null>(null);
  const [searchQuery, setSearchQuery] = useState('');
  const { data: axes = [], isLoading } = useGetResearchAxesQuery();

  if (isLoading) {
    return (
      <div className="flex items-center justify-center py-24">
        <Loader2 size={32} className="animate-spin text-accent-blue" />
      </div>
    );
  }

  const filtered = axes.filter(a =>
    !searchQuery ||
    a.title.toLowerCase().includes(searchQuery.toLowerCase()) ||
    a.themes.some(theme => theme.toLowerCase().includes(searchQuery.toLowerCase())) ||
    (a.responsibleName ?? '').toLowerCase().includes(searchQuery.toLowerCase())
  );

  const myAxes = axes.filter(a => myAxisIds.includes(a.id));
  const totalMembers = axes.reduce((s, a) => s + (a.members?.length ?? 0), 0);
  const totalPublications = axes.reduce((s, a) => s + (a.publicationsCount ?? 0), 0);

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-navy dark:text-white">{t('axes.title')}</h1>
        <p className="text-text-secondary mt-1">{t('axes.exploreAxes')}</p>
      </div>

      <div className="grid grid-cols-3 gap-4">
        <Card>
          <CardContent className="p-4 flex items-center gap-3">
            <div className="w-10 h-10 rounded-lg bg-accent-blue/10 flex items-center justify-center">
              <Target size={20} className="text-accent-blue" />
            </div>
            <div>
              <div className="text-xl font-bold text-navy dark:text-white">{axes.length}</div>
              <div className="text-xs text-text-secondary">{t('axes.activeAxes')}</div>
            </div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4 flex items-center gap-3">
            <div className="w-10 h-10 rounded-lg bg-success/10 flex items-center justify-center">
              <Users size={20} className="text-success" />
            </div>
            <div>
              <div className="text-xl font-bold text-navy dark:text-white">{totalMembers}</div>
              <div className="text-xs text-text-secondary">{t('axes.totalMembers')}</div>
            </div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4 flex items-center gap-3">
            <div className="w-10 h-10 rounded-lg bg-teal/10 flex items-center justify-center">
              <FileText size={20} className="text-teal" />
            </div>
            <div>
              <div className="text-xl font-bold text-navy dark:text-white">{totalPublications}</div>
              <div className="text-xs text-text-secondary">{t('pub.publications')}</div>
            </div>
          </CardContent>
        </Card>
      </div>

      <div className="relative">
        <Target className="absolute left-3 top-1/2 -translate-y-1/2 text-text-muted" size={18} />
        <input
          type="text"
          value={searchQuery}
          onChange={e => setSearchQuery(e.target.value)}
          placeholder={t('axes.searchPlaceholder')}
          className="w-full pl-10 pr-4 py-3 border border-surface-border rounded-xl focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
        />
      </div>

      {myAxisBannerLabel && myAxes.map(axe => (
        <div key={axe.id} className="p-4 bg-accent-blue/5 border border-accent-blue/20 rounded-xl flex items-center gap-3">
          <Lightbulb size={20} className="text-accent-blue flex-shrink-0" />
          <span className="text-sm text-text-secondary">
            <strong className="text-navy dark:text-white">{myAxisBannerLabel} :</strong> {axe.title}
          </span>
        </div>
      ))}

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-5">
        {filtered.map(axe => {
          const isMyAxe = myAxisIds.includes(axe.id);
          const color = resolveColor(axe.color);
          return (
            <Card
              key={axe.id}
              className={clsx(
                'hover:shadow-lg transition-all cursor-pointer border-2',
                isMyAxe ? 'border-accent-blue/30' : 'border-transparent'
              )}
              onClick={() => setSelectedAxe(axe)}
            >
              <CardContent className="p-5">
                <div className="flex items-start justify-between gap-3 mb-3">
                  <div className="flex items-start gap-3 flex-1 min-w-0">
                    <div className={clsx('w-2 h-2 rounded-full mt-2 flex-shrink-0', dotColorMap[color])} />
                    <div className="flex-1 min-w-0">
                      <h3 className="font-bold text-navy dark:text-white text-sm leading-snug">{axe.title}</h3>
                      {isMyAxe && <Badge variant="info" className="mt-1 text-xs">{t('axes.myAxis')}</Badge>}
                    </div>
                  </div>
                  <ChevronRight size={18} className="text-text-muted flex-shrink-0 mt-0.5" />
                </div>

                <p className="text-xs text-text-secondary line-clamp-2 mb-3 ml-5">{axe.description}</p>

                <div className="ml-5 space-y-2">
                  <div className="text-xs text-text-muted">
                    <span className="font-medium text-text-secondary">{t('axes.responsible')} :</span>{' '}
                    {axe.responsibleName ?? '—'}
                  </div>
                  <div className="flex flex-wrap gap-1">
                    {axe.themes.slice(0, 3).map(theme => (
                      <span key={theme} className={clsx('text-xs px-2 py-0.5 rounded-full border', colorMap[color])}>
                        {theme}
                      </span>
                    ))}
                    {axe.themes.length > 3 && (
                      <span className="text-xs px-2 py-0.5 rounded-full bg-light-gray text-text-muted border border-surface-border">
                        +{axe.themes.length - 3}
                      </span>
                    )}
                  </div>
                  <div className="flex items-center gap-4 pt-1 border-t border-surface-border">
                    <div className="flex items-center gap-1.5 text-xs text-text-secondary">
                      <Users size={13} className="text-text-muted" />
                      {axe.members?.length ?? 0} {t('axes.members').toLowerCase()}
                    </div>
                    <div className="flex items-center gap-1.5 text-xs text-text-secondary">
                      <FileText size={13} className="text-text-muted" />
                      {axe.publicationsCount ?? 0} {t('axes.publicationsCount')}
                    </div>
                  </div>
                </div>
              </CardContent>
            </Card>
          );
        })}
      </div>

      {filtered.length === 0 && (
        <Card>
          <CardContent className="py-16 text-center text-text-muted">
            <Target size={40} className="mx-auto mb-3 opacity-30" />
            <p>
              {searchQuery
                ? `${t('axes.noAxesFound')} "${searchQuery}"`
                : t('axes.noAxesFoundGeneric')}
            </p>
          </CardContent>
        </Card>
      )}

      {selectedAxe && (
        <div
          className="fixed inset-0 bg-black/50 flex items-center justify-center z-[60] p-4"
          onClick={() => setSelectedAxe(null)}
        >
          <div
            className="bg-white dark:bg-card rounded-2xl shadow-modal max-w-2xl w-full max-h-[85vh] overflow-y-auto"
            onClick={e => e.stopPropagation()}
          >
            <div className="p-6 border-b border-surface-border flex items-center justify-between">
              <div className="flex items-center gap-3">
                <div className={clsx('w-3 h-3 rounded-full', dotColorMap[resolveColor(selectedAxe.color)])} />
                <h2 className="text-lg font-bold text-navy dark:text-white leading-snug">{selectedAxe.title}</h2>
              </div>
              <button
                onClick={() => setSelectedAxe(null)}
                className="p-2 hover:bg-light-gray dark:hover:bg-muted rounded-lg transition-colors"
              >
                <X size={18} />
              </button>
            </div>

            <div className="p-6 space-y-6">
              {myAxisIds.includes(selectedAxe.id) && myAxisModalLabel && (
                <div className="flex items-center gap-2 p-3 bg-accent-blue/5 border border-accent-blue/20 rounded-lg">
                  <Lightbulb size={16} className="text-accent-blue" />
                  <span className="text-sm text-accent-blue font-medium">{myAxisModalLabel}</span>
                </div>
              )}

              <div>
                <h4 className="text-sm font-semibold text-navy dark:text-white mb-2">{t('axes.description')}</h4>
                <p className="text-sm text-text-secondary leading-relaxed">{selectedAxe.description}</p>
              </div>

              {selectedAxe.responsibleName && (
                <div>
                  <h4 className="text-sm font-semibold text-navy dark:text-white mb-2">{t('axes.responsible')}</h4>
                  <div className="flex items-center gap-3 p-3 bg-light-gray dark:bg-muted rounded-lg">
                    <div className="w-9 h-9 rounded-full bg-gradient-to-br from-navy to-accent-blue flex items-center justify-center text-white font-semibold text-sm flex-shrink-0">
                      {getInitials(selectedAxe.responsibleName)}
                    </div>
                    <span className="font-medium text-navy dark:text-white text-sm">{selectedAxe.responsibleName}</span>
                  </div>
                </div>
              )}

              <div>
                <h4 className="text-sm font-semibold text-navy dark:text-white mb-2">{t('axes.themes')}</h4>
                <div className="flex flex-wrap gap-2">
                  {selectedAxe.themes.map(theme => (
                    <span
                      key={theme}
                      className={clsx('text-sm px-3 py-1 rounded-full border font-medium', colorMap[resolveColor(selectedAxe.color)])}
                    >
                      {theme}
                    </span>
                  ))}
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div className="p-4 bg-light-gray dark:bg-muted rounded-xl text-center">
                  <div className="text-2xl font-bold text-navy dark:text-white">{selectedAxe.members?.length ?? 0}</div>
                  <div className="text-xs text-text-secondary mt-0.5">{t('axes.members')}</div>
                </div>
                <div className="p-4 bg-light-gray dark:bg-muted rounded-xl text-center">
                  <div className="text-2xl font-bold text-navy dark:text-white">{selectedAxe.publicationsCount ?? 0}</div>
                  <div className="text-xs text-text-secondary mt-0.5">{t('pub.publications')}</div>
                </div>
              </div>
            </div>

            <div className="p-6 border-t border-surface-border flex justify-end">
              <Button variant="outlined" onClick={() => setSelectedAxe(null)}>{t('common.close')}</Button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
