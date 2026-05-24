import { useState } from 'react';
import { Card, CardContent } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { Button } from '../../../components/ui/Button';
import { SearchFilter } from '../../../components/shared/SearchFilter';
import { FileText, ExternalLink, Lock, Unlock, X } from 'lucide-react';

type PublicationType = 'ARTICLE_JOURNAL' | 'CONFERENCE_INT' | 'CONFERENCE_NAT' | 'CHAPITRE_OUVRAGE' | 'RAPPORT_TECHNIQUE';

interface Publication {
  id: string;
  type: PublicationType;
  title: string;
  year: number;
  visibility: 'PUBLIQUE' | 'PRIVEE';
  quartile?: 'Q1' | 'Q2' | 'Q3' | 'Q4';
  coreRanking?: 'A*' | 'A' | 'B' | 'C';
  authors: string[];
  axe: string;
  abstract: string;
  doi?: string;
  venue?: string;
}

const allPublications: Publication[] = [
  {
    id: '1',
    type: 'ARTICLE_JOURNAL',
    title: 'Deep Learning for Medical Imaging Diagnosis',
    year: 2026,
    visibility: 'PUBLIQUE',
    quartile: 'Q1',
    authors: ['Dr. Ahmed Ben Salem', 'Sarah Trabelsi', 'Prof. Marie Dubois'],
    axe: 'Intelligence Artificielle et Apprentissage Automatique',
    abstract: 'Présentation d\'une approche deep learning pour le diagnostic automatisé d\'imagerie médicale avec 97.3% de précision.',
    doi: 'https://doi.org/10.1016/j.media.2026.01.001',
    venue: 'Medical Image Analysis, Vol. 84',
  },
  {
    id: '2',
    type: 'CONFERENCE_INT',
    title: 'Blockchain Security Analysis for Healthcare Data',
    year: 2026,
    visibility: 'PUBLIQUE',
    coreRanking: 'A*',
    authors: ['Dr. Fatma Gharbi', 'Mohamed Najjar'],
    axe: 'Sécurité Informatique et Cryptographie',
    abstract: 'Cadre d\'analyse de sécurité pour les systèmes de santé basés sur la blockchain.',
    venue: 'IEEE S&P 2026',
  },
  {
    id: '3',
    type: 'ARTICLE_JOURNAL',
    title: 'Smart City Infrastructure Security Using IoT',
    year: 2024,
    visibility: 'PUBLIQUE',
    quartile: 'Q1',
    authors: ['Dr. Fatma Gharbi', 'Dr. Karim Jebali'],
    axe: 'Sécurité Informatique et Cryptographie',
    abstract: 'Investigation des défis de sécurité dans les infrastructures de villes intelligentes avec IoT.',
    doi: 'https://doi.org/10.1109/iotcity.2024',
    venue: 'IEEE IoT Journal, Vol. 11',
  },
  {
    id: '4',
    type: 'CONFERENCE_INT',
    title: 'Natural Language Processing for Arabic Dialectal Text',
    year: 2025,
    visibility: 'PUBLIQUE',
    coreRanking: 'A',
    authors: ['Dr. Ahmed Ben Salem', 'Amira Khelil'],
    axe: 'Intelligence Artificielle et Apprentissage Automatique',
    abstract: 'Modèle NLP adapté aux dialectes arabes tunisiens avec corpus annoté de 2M tokens.',
    venue: 'ACL 2025',
  },
  {
    id: '5',
    type: 'ARTICLE_JOURNAL',
    title: 'Federated Learning for Privacy-Preserving Machine Learning',
    year: 2025,
    visibility: 'PRIVEE',
    quartile: 'Q2',
    authors: ['Dr. Ahmed Ben Salem', 'Dr. Mohamed Mezghani'],
    axe: 'Intelligence Artificielle et Apprentissage Automatique',
    abstract: 'Techniques d\'apprentissage fédéré préservant la confidentialité des données tout en maintenant la précision du modèle.',
    venue: 'ACM TIST, Vol. 16, No. 3',
  },
];

const typeLabels: Record<PublicationType, string> = {
  ARTICLE_JOURNAL: 'Article de journal',
  CONFERENCE_INT: 'Conférence internationale',
  CONFERENCE_NAT: 'Conférence nationale',
  CHAPITRE_OUVRAGE: "Chapitre d'ouvrage",
  RAPPORT_TECHNIQUE: 'Rapport technique',
};

const filterGroups = [
  {
    id: 'type', label: 'Type', options: [
      { id: 'aj', label: 'Article de journal', value: 'ARTICLE_JOURNAL' },
      { id: 'ci', label: 'Conférence internationale', value: 'CONFERENCE_INT' },
      { id: 'cn', label: 'Conférence nationale', value: 'CONFERENCE_NAT' },
    ]
  },
  {
    id: 'visibility', label: 'Visibilité', options: [
      { id: 'pub', label: 'Publique', value: 'PUBLIQUE' },
      { id: 'priv', label: 'Interne', value: 'PRIVEE' },
    ]
  },
  {
    id: 'year', label: 'Année', options: [
      { id: '2026', label: '2026', value: '2026' },
      { id: '2025', label: '2025', value: '2025' },
      { id: '2024', label: '2024', value: '2024' },
    ]
  },
];

export default function MasterienAllPublications() {
  const [searchQuery, setSearchQuery] = useState('');
  const [activeFilters, setActiveFilters] = useState<Record<string, string | string[]>>({});
  const [detailPub, setDetailPub] = useState<Publication | null>(null);

  const filtered = allPublications.filter(pub => {
    const q = searchQuery.toLowerCase();
    const matchQ = !q || pub.title.toLowerCase().includes(q) || pub.authors.some(a => a.toLowerCase().includes(q));
    const tf = activeFilters.type as string;
    const vf = activeFilters.visibility as string;
    const yf = activeFilters.year as string;
    return matchQ && (!tf || pub.type === tf) && (!vf || pub.visibility === vf) && (!yf || pub.year.toString() === yf);
  });

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-navy dark:text-white">Publications du Laboratoire</h1>
        <p className="text-text-secondary mt-1">Consultez les publications scientifiques du laboratoire LIMTIC</p>
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
              <div className="text-xl font-bold text-success">{allPublications.filter(p => p.visibility === 'PUBLIQUE').length}</div>
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
              <div className="text-xl font-bold text-accent-blue">{allPublications.filter(p => p.visibility === 'PRIVEE').length}</div>
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
                  <Badge variant="info" className="text-xs">{typeLabels[pub.type]}</Badge>
                  {pub.visibility === 'PRIVEE' && (
                    <span className="flex items-center gap-1 text-xs text-text-muted px-2 py-0.5 bg-light-gray dark:bg-muted rounded-full">
                      <Lock size={11} /> Interne
                    </span>
                  )}
                </div>
                <h3 className="font-bold text-navy dark:text-white mb-1.5 leading-snug">{pub.title}</h3>
                <div className="space-y-0.5 text-xs text-text-secondary mb-2">
                  <div><span className="font-medium">Auteurs :</span> {pub.authors.join(', ')}</div>
                  {pub.venue && <div className="italic">{pub.venue}</div>}
                </div>
                <p className="text-xs text-text-secondary line-clamp-2 mb-3">{pub.abstract}</p>
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
                <Badge variant="info">{typeLabels[detailPub.type]}</Badge>
                {detailPub.quartile && <Badge variant={`q${detailPub.quartile.charAt(1)}` as any}>{detailPub.quartile}</Badge>}
                {detailPub.coreRanking && <Badge variant={`core-${detailPub.coreRanking.toLowerCase().replace('*', '-star')}` as any}>CORE {detailPub.coreRanking}</Badge>}
                <Badge variant="default">{detailPub.year}</Badge>
              </div>
              <h3 className="text-lg font-bold text-navy dark:text-white leading-snug">{detailPub.title}</h3>
              <div className="space-y-2 text-sm">
                <div><span className="font-semibold text-navy dark:text-white">Auteurs :</span> <span className="text-text-secondary">{detailPub.authors.join(', ')}</span></div>
                {detailPub.venue && <div><span className="font-semibold text-navy dark:text-white">Revue / Conférence :</span> <span className="text-text-secondary italic">{detailPub.venue}</span></div>}
                <div><span className="font-semibold text-navy dark:text-white">Axe :</span> <span className="text-text-secondary">{detailPub.axe}</span></div>
                {detailPub.doi && (
                  <div className="flex items-center gap-1.5 flex-wrap">
                    <span className="font-semibold text-navy dark:text-white">DOI :</span>
                    <a href={detailPub.doi} target="_blank" rel="noopener noreferrer" className="text-accent-blue hover:underline flex items-center gap-1 text-sm break-all">
                      {detailPub.doi} <ExternalLink size={12} />
                    </a>
                  </div>
                )}
                <div>
                  <p className="font-semibold text-navy dark:text-white mb-1.5">Résumé :</p>
                  <p className="text-text-secondary leading-relaxed">{detailPub.abstract}</p>
                </div>
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
