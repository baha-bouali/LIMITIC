import { useState } from 'react';
import { PublicNavbar } from '../../components/layout/PublicNavbar';
import { PublicFooter } from '../../components/layout/PublicFooter';
import { Card, CardContent, CardHeader } from '../../components/ui/Card';
import { Badge } from '../../components/ui/Badge';
import { SearchFilter } from '../../components/shared/SearchFilter';
import { Link } from 'react-router-dom';
import { FileDown, ExternalLink, FileText } from 'lucide-react';
import { useLanguage } from '../../contexts/LanguageContext';

export default function PublicationsPage() {
  const { t } = useLanguage();
  const [searchQuery, setSearchQuery] = useState('');
  const [activeFilters, setActiveFilters] = useState<Record<string, string[]>>({});

  const allPublications = [
    {
      id: '1',
      type: 'q1',
      year: 2026,
      title: 'Deep Learning for Medical Imaging Diagnosis',
      authors: ['A. Ben Salem', 'F. Gharbi', 'S. Trabelsi'],
      venue: 'IEEE Trans. Medical Imaging',
      doi: '10.1109/TMI.2026.123456',
      axe: 'Intelligence Artificielle et Apprentissage Automatique',
      publicationType: 'ARTICLE_JOURNAL'
    },
    {
      id: '2',
      type: 'core-a-star',
      year: 2026,
      title: 'Blockchain Security Analysis in Healthcare Systems',
      authors: ['M. Mezghani', 'F. Gharbi'],
      venue: 'ACM CCS 2026',
      doi: '10.1145/3576915.3623456',
      axe: 'Sécurité Informatique et Cryptographie',
      publicationType: 'CONFERENCE_INT'
    },
    {
      id: '3',
      type: 'q2',
      year: 2025,
      title: 'Federated Learning Approaches for Privacy-Preserving ML',
      authors: ['F. Gharbi', 'A. Ben Salem'],
      venue: 'J. Biomedical Informatics',
      doi: '10.1016/j.jbi.2025.104321',
      axe: 'Intelligence Artificielle et Apprentissage Automatique',
      publicationType: 'ARTICLE_JOURNAL'
    },
    {
      id: '4',
      type: 'core-a',
      year: 2025,
      title: 'Distributed Systems Optimization for Cloud Computing',
      authors: ['M. Mezghani', 'K. Jebali'],
      venue: 'IEEE ICDCS 2025',
      doi: '10.1109/ICDCS.2025.00012',
      axe: 'Systèmes Distribués et Cloud Computing',
      publicationType: 'CONFERENCE_INT'
    },
    {
      id: '5',
      type: 'q1',
      year: 2024,
      title: 'Computer Vision Applications in Medical Image Analysis',
      authors: ['L. Ammar', 'A. Ben Salem'],
      venue: 'Computer Vision and Image Understanding',
      doi: '10.1016/j.cviu.2024.103892',
      axe: 'Traitement d\'Images et Vision par Ordinateur',
      publicationType: 'ARTICLE_JOURNAL'
    },
  ];

  const filterGroups = [
    {
      id: 'type',
      label: t('pubPage.typePublication'),
      options: [
        { id: 'journal', label: t('pubPage.journalArticles'), value: 'ARTICLE_JOURNAL' },
        { id: 'conf', label: t('pubPage.conferences'), value: 'CONFERENCE_INT' },
      ]
    },
    {
      id: 'ranking',
      label: t('pubPage.ranking'),
      options: [
        { id: 'q1', label: t('pubPage.q1'), value: 'q1' },
        { id: 'q2', label: t('pubPage.q2'), value: 'q2' },
        { id: 'coreastar', label: t('pubPage.coreAStar'), value: 'core-a-star' },
        { id: 'corea', label: t('pubPage.coreA'), value: 'core-a' },
      ]
    },
    {
      id: 'year',
      label: t('pubPage.year'),
      options: [
        { id: '2026', label: '2026', value: '2026' },
        { id: '2025', label: '2025', value: '2025' },
        { id: '2024', label: '2024', value: '2024' },
      ]
    },
    {
      id: 'axe',
      label: t('pubPage.axis'),
      options: [
        { id: 'ia', label: t('pubPage.aiAxis'), value: 'Intelligence Artificielle et Apprentissage Automatique' },
        { id: 'secu', label: t('pubPage.securityAxis'), value: 'Sécurité Informatique et Cryptographie' },
        { id: 'sys', label: t('pubPage.systemsAxis'), value: 'Systèmes Distribués et Cloud Computing' },
        { id: 'vision', label: t('pubPage.visionAxis'), value: 'Traitement d\'Images et Vision par Ordinateur' },
      ]
    }
  ];

  const filteredPublications = allPublications.filter(pub => {
    const matchesSearch = searchQuery === '' ||
      pub.title.toLowerCase().includes(searchQuery.toLowerCase()) ||
      pub.authors.some(a => a.toLowerCase().includes(searchQuery.toLowerCase())) ||
      pub.venue.toLowerCase().includes(searchQuery.toLowerCase());

    const matchesFilters = Object.entries(activeFilters).every(([key, values]) => {
      if (values.length === 0) return true;
      if (key === 'type') return values.includes(pub.publicationType);
      if (key === 'ranking') return values.includes(pub.type);
      if (key === 'year') return values.includes(pub.year.toString());
      if (key === 'axe') return values.includes(pub.axe);
      return true;
    });

    return matchesSearch && matchesFilters;
  });

  const getTypeLabel = (type: string) => {
    const labels: Record<string, string> = {
      'q1': 'Q1 ★',
      'q2': 'Q2',
      'q3': 'Q3',
      'q4': 'Q4',
      'core-a-star': 'CORE A*',
      'core-a': 'CORE A',
      'core-b': 'CORE B',
      'core-c': 'CORE C',
    };
    return labels[type] || type;
  };

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
            <div className="text-center"><div className="text-3xl font-bold">{allPublications.length}</div><div className="text-white/80">{t('pubPage.total')}</div></div>
            <div className="text-center"><div className="text-3xl font-bold">{allPublications.filter(p => p.publicationType === 'ARTICLE_JOURNAL').length}</div><div className="text-white/80">{t('pubPage.journals')}</div></div>
            <div className="text-center"><div className="text-3xl font-bold">{allPublications.filter(p => p.publicationType === 'CONFERENCE_INT').length}</div><div className="text-white/80">{t('pubPage.conferences')}</div></div>
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
                        <Badge variant={pub.type}>{getTypeLabel(pub.type)}</Badge>
                        <Badge variant="default">{pub.publicationType === 'ARTICLE_JOURNAL' ? t('pubPage.journalArticle') : t('pubPage.conference')}</Badge>
                        <Badge variant="info" className="text-xs">{pub.axe}</Badge>
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
                      DOI: <a href={`https://doi.org/${pub.doi}`} className="text-accent-blue hover:underline" target="_blank" rel="noopener noreferrer">{pub.doi}</a>
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
