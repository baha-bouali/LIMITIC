import { useMemo, useState } from 'react';
import { PublicNavbar } from '../../components/layout/PublicNavbar';
import { PublicFooter } from '../../components/layout/PublicFooter';
import { Card, CardContent, CardHeader } from '../../components/ui/Card';
import { Badge } from '../../components/ui/Badge';
import { SearchFilter } from '../../components/shared/SearchFilter';
import { Link } from 'react-router-dom';
import { FileText } from 'lucide-react';
import { useLanguage } from '../../contexts/LanguageContext';
import { useGetPublicPublicationsQuery } from '../../api/publicationsApi';

const TYPE_LABELS: Record<string, string> = {
  ArticleJournal: 'Article de journal',
  ConferenceInternational: 'Conférence internationale',
  ConferenceNational: 'Conférence nationale',
  ChapterBook: "Chapitre d'ouvrage",
  TechnicalReport: 'Rapport technique',
};

const CONFERENCE_TYPES = new Set(['ConferenceInternational', 'ConferenceNational']);

const normalizeRanking = (value?: string | null) => value?.toLowerCase().replace(/\s+/g, '-').replace(/_/g, '-') ?? '';

const getRankingLabel = (value: string) => {
  const labels: Record<string, string> = {
    q1: 'Q1 ★',
    q2: 'Q2',
    q3: 'Q3',
    q4: 'Q4',
    'core-a-star': 'CORE A*',
    'core-a': 'CORE A',
    'core-b': 'CORE B',
    'core-c': 'CORE C',
  };

  return labels[value] || value;
};

export default function PublicationsPage() {
  const { t } = useLanguage();
  const [searchQuery, setSearchQuery] = useState('');
  const [activeFilters, setActiveFilters] = useState<Record<string, string[]>>({});
  const { data, isLoading, isError } = useGetPublicPublicationsQuery({
    page: 1,
    limit: 100,
    search: searchQuery || undefined,
  });

  const allPublications = data?.items ?? [];

  const filterGroups = useMemo(() => {
    const rankingValues = Array.from(new Set(allPublications.map((pub) => normalizeRanking(pub.ranking || pub.coreRanking)).filter(Boolean)));
    const yearValues = Array.from(new Set(allPublications.map((pub) => pub.year))).sort((a, b) => b - a);
    const axes = Array.from(new Map(allPublications.map((pub) => [pub.axe.id, pub.axe.title])).entries());

    return [
      {
        id: 'type',
        label: t('pubPage.typePublication'),
        options: [
          { id: 'journal', label: t('pubPage.journalArticles'), value: 'ArticleJournal' },
          { id: 'conf-int', label: 'Conférence internationale', value: 'ConferenceInternational' },
          { id: 'conf-nat', label: 'Conférence nationale', value: 'ConferenceNational' },
          { id: 'chapter', label: "Chapitre d'ouvrage", value: 'ChapterBook' },
          { id: 'report', label: 'Rapport technique', value: 'TechnicalReport' },
        ],
      },
      {
        id: 'ranking',
        label: t('pubPage.ranking'),
        options: rankingValues.map((value) => ({ id: value, label: getRankingLabel(value), value })),
      },
      {
        id: 'year',
        label: t('pubPage.year'),
        options: yearValues.map((year) => ({ id: String(year), label: String(year), value: String(year) })),
      },
      {
        id: 'axe',
        label: t('pubPage.axis'),
        options: axes.map(([id, label]) => ({ id, label, value: id })),
      },
    ];
  }, [allPublications, t]);

  const filteredPublications = allPublications.filter(pub => {
    const matchesSearch = searchQuery === '' ||
      pub.title.toLowerCase().includes(searchQuery.toLowerCase()) ||
      pub.authors.some(a => a.toLowerCase().includes(searchQuery.toLowerCase())) ||
      (pub.venue ?? '').toLowerCase().includes(searchQuery.toLowerCase());

    const matchesFilters = Object.entries(activeFilters).every(([key, values]) => {
      if (values.length === 0) return true;
      if (key === 'type') return values.includes(pub.type);
      if (key === 'ranking') return values.includes(normalizeRanking(pub.ranking || pub.coreRanking));
      if (key === 'year') return values.includes(pub.year.toString());
      if (key === 'axe') return values.includes(pub.axe.id);
      return true;
    });

    return matchesSearch && matchesFilters;
  });

  if (isLoading) {
    return (
      <div className="min-h-screen bg-white dark:bg-background">
        <PublicNavbar />
        <div style={{ height: 'var(--navbar-height)' }} />
        <div className="max-w-[var(--content-max-width)] mx-auto px-6 py-24 text-center text-text-secondary">
          Chargement des publications...
        </div>
        <PublicFooter />
      </div>
    );
  }

  if (isError) {
    return (
      <div className="min-h-screen bg-white dark:bg-background">
        <PublicNavbar />
        <div style={{ height: 'var(--navbar-height)' }} />
        <div className="max-w-[var(--content-max-width)] mx-auto px-6 py-24 text-center text-text-secondary">
          Impossible de charger les publications.
        </div>
        <PublicFooter />
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-white dark:bg-background">
      <PublicNavbar />
      <div style={{ height: 'var(--navbar-height)' }} />

      <div className="brand-gradient-diagonal text-white py-20">
        <div className="max-w-[var(--content-max-width)] mx-auto px-6">
          <div className="text-sm text-white/80 mb-3">{t('pubPage.breadcrumb')}</div>
          <h1 className="text-5xl font-bold mb-4">{t('pubPage.title')}</h1>
          <p className="text-xl text-white/90 mb-6">{t('pubPage.subtitle')}</p>
          <div className="flex gap-8 mt-8">
            <div className="text-center"><div className="text-3xl font-bold">{data?.stats.total ?? allPublications.length}</div><div className="text-white/80">{t('pubPage.total')}</div></div>
            <div className="text-center"><div className="text-3xl font-bold">{data?.stats.journals ?? allPublications.filter(p => p.type === 'ArticleJournal').length}</div><div className="text-white/80">{t('pubPage.journals')}</div></div>
            <div className="text-center"><div className="text-3xl font-bold">{data?.stats.conferences ?? allPublications.filter(p => CONFERENCE_TYPES.has(p.type)).length}</div><div className="text-white/80">{t('pubPage.conferences')}</div></div>
          </div>
        </div>
      </div>

      <div className="max-w-[var(--content-max-width)] mx-auto px-6 py-12">
        {/* Search and Filters */}
        <SearchFilter
          onSearchChange={setSearchQuery}
          filterGroups={filterGroups}
          onFilterChange={setActiveFilters}
          searchPlaceholder={t('pubPage.searchPlaceholder')}
        />

        {/* Publications List */}
        {filteredPublications.length === 0 ? (
          <Card>
            <CardContent className="p-12 text-center">
              <div className="bg-accent-blue/5 dark:bg-accent-blue/10 rounded-xl p-8 inline-block">
                <FileText size={64} className="mx-auto text-accent-blue mb-4" />
                <h3 className="text-xl font-bold text-navy dark:text-white mb-2">
                  {t('pubPage.noPublications')}
                </h3>
                <p className="text-text-primary dark:text-text-primary">
                  {t('pubPage.noResults')}
                </p>
              </div>
            </CardContent>
          </Card>
        ) : (
          <div className="space-y-6">
            {filteredPublications.map((pub) => (
              <Card key={pub.id} className="hover:shadow-lg transition-shadow">
                <CardHeader>
                  <div className="flex items-start gap-4">
                    <div className="flex-shrink-0 w-20 text-center">
                      <div className="text-3xl font-bold text-navy dark:text-white">{pub.year}</div>
                    </div>
                    <div className="flex-1">
                      <div className="flex items-center gap-2 mb-3 flex-wrap">
                        <Badge variant={(normalizeRanking(pub.ranking || pub.coreRanking) || 'default') as any}>{getRankingLabel(normalizeRanking(pub.ranking || pub.coreRanking) || pub.type)}</Badge>
                        <Badge variant="default">{TYPE_LABELS[pub.type] ?? pub.type}</Badge>
                        <Badge variant="info" className="text-xs">{pub.axe.title}</Badge>
                      </div>
                      <Link to={`/publications/${pub.id}`}>
                        <h3 className="text-xl font-bold text-navy dark:text-white mb-2 hover:text-accent-blue transition-colors">{pub.title}</h3>
                      </Link>
                      <p className="text-sm text-text-secondary mb-1">{pub.authors.join(', ')}</p>
                      <p className="text-sm text-text-secondary italic">{pub.venue}</p>
                    </div>
                  </div>
                </CardHeader>
                <CardContent className="border-t border-surface-border">
                  <div className="flex items-center justify-between">
                    <div className="text-sm text-text-muted">
                      DOI: {pub.doi ? <a href={`https://doi.org/${pub.doi}`} className="text-accent-blue hover:underline" target="_blank" rel="noopener noreferrer">{pub.doi}</a> : <span>N/A</span>}
                    </div>
                    <div className="flex gap-2">
                      <Link
                        to={`/publications/${pub.id}`}
                        className="text-accent-blue hover:underline text-sm flex items-center gap-1"
                      >
                        {t('pubPage.viewDetails')} →
                      </Link>
                    </div>
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>
        )}
      </div>

      <PublicFooter />
    </div>
  );
}
