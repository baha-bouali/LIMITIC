import { useState } from 'react';
import { Card, CardContent } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { Button } from '../../../components/ui/Button';
import { SearchFilter } from '../../../components/shared/SearchFilter';
import { FileText, ExternalLink, Lock, Unlock, X } from 'lucide-react';
import { useGetDashboardPublicationsQuery, type DashboardPublicationSummaryDto } from '../../../api/dashboardPublicationsApi';

const TYPE_LABELS: Record<string, string> = {
  ArticleJournal: 'Article de journal',
  ConferenceInternational: 'Conférence internationale',
  ConferenceNational: 'Conférence nationale',
  ChapterBook: "Chapitre d'ouvrage",
  TechnicalReport: 'Rapport technique',
};

const filterGroups = [
  {
    id: 'type', label: 'Type', options: [
      { id: 'aj', label: 'Article de journal', value: 'ArticleJournal' },
      { id: 'ci', label: 'Conférence internationale', value: 'ConferenceInternational' },
      { id: 'cn', label: 'Conférence nationale', value: 'ConferenceNational' },
      { id: 'co', label: "Chapitre d'ouvrage", value: 'ChapterBook' },
      { id: 'rt', label: 'Rapport technique', value: 'TechnicalReport' },
    ]
  },
  {
    id: 'visibility', label: 'Visibilité', options: [
      { id: 'pub', label: 'Publique', value: 'Public' },
      { id: 'priv', label: 'Interne', value: 'Private' },
    ]
  },
];

export default function DoctorantAllPublications() {
  const [searchQuery, setSearchQuery] = useState('');
  const [activeFilters, setActiveFilters] = useState<Record<string, string | string[]>>({});
  const [detailPub, setDetailPub] = useState<DashboardPublicationSummaryDto | null>(null);

  const { data: list, isLoading } = useGetDashboardPublicationsQuery({ scope: 'all', status: 'Published' });
  const allPublications = list?.items ?? [];

  const filtered = allPublications.filter(pub => {
    const q = searchQuery.toLowerCase();
    const matchQ = !q || pub.title.toLowerCase().includes(q) || pub.authors.some(a => a.toLowerCase().includes(q));
    const tf = activeFilters.type as string;
    const vf = activeFilters.visibility as string;
    return matchQ && (!tf || pub.type === tf) && (!vf || pub.visibility === vf);
  });

  if (isLoading) {
    return (
      <div className="flex items-center justify-center py-20">
        <div className="text-text-muted">Chargement...</div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-navy dark:text-white">Publications du Laboratoire</h1>
        <p className="text-text-secondary mt-1">Accédez à toutes les publications publiées du laboratoire LIMTIC</p>
      </div>

      <div className="grid grid-cols-3 gap-4">
        <Card>
          <CardContent className="p-4 flex items-center gap-3">
            <div className="w-10 h-10 rounded-lg bg-accent-blue/10 flex items-center justify-center">
              <FileText size={20} className="text-accent-blue" />
            </div>
            <div>
              <div className="text-xl font-bold text-navy dark:text-white">{allPublications.length}</div>
              <div className="text-xs text-text-secondary">Publications totales</div>
            </div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4 flex items-center gap-3">
            <div className="w-10 h-10 rounded-lg bg-success/10 flex items-center justify-center">
              <Unlock size={20} className="text-success" />
            </div>
            <div>
              <div className="text-xl font-bold text-success">{allPublications.filter(p => p.visibility === 'Public').length}</div>
              <div className="text-xs text-text-secondary">Publiques</div>
            </div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4 flex items-center gap-3">
            <div className="w-10 h-10 rounded-lg bg-accent-blue/10 flex items-center justify-center">
              <Lock size={20} className="text-accent-blue" />
            </div>
            <div>
              <div className="text-xl font-bold text-accent-blue">{allPublications.filter(p => p.visibility === 'Private').length}</div>
              <div className="text-xs text-text-secondary">Internes</div>
            </div>
          </CardContent>
        </Card>
      </div>

      <SearchFilter
        searchPlaceholder="Rechercher une publication, auteur..."
        filterGroups={filterGroups}
        onSearchChange={setSearchQuery}
        onFilterChange={setActiveFilters}
      />

      {filtered.length === 0 ? (
        <Card>
          <CardContent className="py-16 text-center">
            <div className="bg-accent-blue/5 dark:bg-accent-blue/10 rounded-xl p-8 inline-block">
              <FileText size={40} className="mx-auto text-accent-blue mb-3" />
              <p className="text-navy dark:text-white font-medium">Aucune publication ne correspond à vos critères</p>
            </div>
          </CardContent>
        </Card>
      ) : (
        <div className="space-y-4">
          {filtered.map(pub => (
            <Card key={pub.id} className="hover:shadow-card-hover transition-shadow">
              <CardContent className="p-5">
                <div className="flex flex-wrap items-center gap-2 mb-2.5">
                  {pub.quartile && <Badge variant={`q${pub.quartile.charAt(1)}` as any}>{pub.quartile}</Badge>}
                  {pub.coreRanking && <Badge variant={`core-${pub.coreRanking.toLowerCase().replace('*', '-star')}` as any}>CORE {pub.coreRanking}</Badge>}
                  <Badge variant="default">{pub.year}</Badge>
                  <Badge variant="info" className="text-xs">{TYPE_LABELS[pub.type] ?? pub.type}</Badge>
                  {pub.visibility === 'Private' && (
                    <span className="flex items-center gap-1 text-xs text-text-muted px-2 py-0.5 bg-light-gray dark:bg-muted rounded-full">
                      <Lock size={11} /> Interne
                    </span>
                  )}
                </div>
                <h3 className="font-bold text-navy dark:text-white mb-1.5 leading-snug">{pub.title}</h3>
                <div className="space-y-0.5 text-xs text-text-secondary mb-2">
                  <div><span className="font-medium">Auteurs :</span> {pub.authors.join(', ')}</div>
                  <div><span className="font-medium">Axe :</span> {pub.axe.title}</div>
                  {pub.venue && <div className="italic">{pub.venue}</div>}
                </div>
                <button
                  onClick={() => setDetailPub(pub)}
                  className="flex items-center gap-1.5 text-accent-blue hover:underline text-xs font-medium"
                >
                  <ExternalLink size={13} /> Voir les détails
                </button>
              </CardContent>
            </Card>
          ))}
        </div>
      )}

      {detailPub && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-[60] p-4" onClick={() => setDetailPub(null)}>
          <div className="bg-white dark:bg-card rounded-2xl shadow-modal max-w-2xl w-full max-h-[90vh] overflow-y-auto" onClick={e => e.stopPropagation()}>
            <div className="p-6 border-b border-surface-border flex items-center justify-between">
              <h2 className="text-lg font-bold text-navy dark:text-white">Détails de la publication</h2>
              <button onClick={() => setDetailPub(null)} className="p-2 hover:bg-light-gray dark:hover:bg-muted rounded-lg"><X size={18} /></button>
            </div>
            <div className="p-6 space-y-4">
              <div className="flex flex-wrap gap-2">
                <Badge variant="success">Publié</Badge>
                <Badge variant="info">{TYPE_LABELS[detailPub.type] ?? detailPub.type}</Badge>
                {detailPub.quartile && <Badge variant={`q${detailPub.quartile.charAt(1)}` as any}>{detailPub.quartile}</Badge>}
                {detailPub.coreRanking && <Badge variant={`core-${detailPub.coreRanking.toLowerCase().replace('*', '-star')}` as any}>CORE {detailPub.coreRanking}</Badge>}
                <Badge variant="default">{detailPub.year}</Badge>
                {detailPub.visibility === 'Private' && (
                  <span className="flex items-center gap-1 text-xs text-text-muted px-2 py-0.5 bg-light-gray dark:bg-muted rounded-full">
                    <Lock size={11} /> Interne
                  </span>
                )}
              </div>
              <h3 className="text-lg font-bold text-navy dark:text-white leading-snug">{detailPub.title}</h3>
              <div className="space-y-2 text-sm">
                <div><span className="font-semibold text-navy dark:text-white">Auteurs :</span> <span className="text-text-secondary">{detailPub.authors.join(', ')}</span></div>
                {detailPub.venue && <div><span className="font-semibold text-navy dark:text-white">Revue / Conférence :</span> <span className="text-text-secondary italic">{detailPub.venue}</span></div>}
                <div><span className="font-semibold text-navy dark:text-white">Axe de recherche :</span> <span className="text-text-secondary">{detailPub.axe.title}</span></div>
                {detailPub.doi && (
                  <div className="flex items-center gap-1">
                    <span className="font-semibold text-navy dark:text-white">DOI :</span>
                    <a href={detailPub.doi} target="_blank" rel="noopener noreferrer" className="text-accent-blue hover:underline flex items-center gap-1 text-sm">
                      {detailPub.doi} <ExternalLink size={12} />
                    </a>
                  </div>
                )}
              </div>
            </div>
            <div className="p-6 border-t border-surface-border flex justify-end">
              <Button variant="outlined" onClick={() => setDetailPub(null)}>Fermer</Button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
