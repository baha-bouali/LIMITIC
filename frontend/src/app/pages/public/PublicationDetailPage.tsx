import { useParams, Link } from 'react-router-dom';
import { PublicNavbar } from '../../components/layout/PublicNavbar';
import { PublicFooter } from '../../components/layout/PublicFooter';
import { Badge } from '../../components/ui/Badge';
import { Button } from '../../components/ui/Button';
import { FileDown, ExternalLink } from 'lucide-react';
import { useGetPublicPublicationByIdQuery } from '../../api/publicationsApi';

export default function PublicationDetailPage() {
  const { id } = useParams();
  const { data: publication, isLoading, isError } = useGetPublicPublicationByIdQuery(id ?? '', {
    skip: !id,
  });

  if (isLoading) {
    return (
      <div className="min-h-screen bg-white dark:bg-background">
        <PublicNavbar />
        <div style={{ height: 'var(--navbar-height)' }} />
        <div className="max-w-4xl mx-auto px-6 py-24 text-center text-text-secondary">
          Chargement de la publication...
        </div>
        <PublicFooter />
      </div>
    );
  }

  if (isError || !publication) {
    return (
      <div className="min-h-screen bg-white dark:bg-background">
        <PublicNavbar />
        <div style={{ height: 'var(--navbar-height)' }} />
        <div className="max-w-4xl mx-auto px-6 py-24 text-center text-text-secondary space-y-4">
          <p>Publication introuvable.</p>
          <Link to="/publications" className="text-accent-blue hover:underline">
            Retour à la liste des publications
          </Link>
        </div>
        <PublicFooter />
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-white dark:bg-background">
      <PublicNavbar />
      <div style={{ height: 'var(--navbar-height)' }} />

      <div className="max-w-4xl mx-auto px-6 py-12">
        <div className="mb-6">
          <Badge variant={(publication.ranking?.toLowerCase() as any) || (publication.coreRanking?.toLowerCase() as any) || 'default'} className="mb-3">
            {publication.ranking || publication.coreRanking || publication.type}
          </Badge>
          <h1 className="text-4xl font-bold text-navy dark:text-white mb-4">
            {publication.title}
          </h1>
          <div className="flex items-center gap-4 text-text-secondary mb-6">
            <span>{publication.authors.join(', ')}</span>
          </div>
        </div>

        <div className="grid grid-cols-2 gap-4 mb-8 p-6 bg-light-gray dark:bg-card rounded-lg">
          <div className="col-span-2">
            <strong>Axe de recherche:</strong>{' '}
            <Badge variant="info" className="ml-2">{publication.axe.title}</Badge>
          </div>
          <div><strong>Journal:</strong> {publication.journalName || publication.venue || 'N/A'}</div>
          <div><strong>Année:</strong> {publication.year}</div>
          <div><strong>Volume:</strong> {publication.volume || 'N/A'}</div>
          <div><strong>Pages:</strong> {publication.pages || 'N/A'}</div>
          <div><strong>DOI:</strong> {publication.doi ? <a href={`https://doi.org/${publication.doi}`} className="text-accent-blue hover:underline" target="_blank" rel="noopener noreferrer">{publication.doi}</a> : 'N/A'}</div>
          <div><strong>Quartile:</strong> {publication.ranking || publication.coreRanking || 'N/A'}</div>
          <div><strong>Status:</strong> {publication.status || 'N/A'}</div>
          <div><strong>ISSN:</strong> {publication.isbn || 'N/A'}</div>
        </div>

        <div className="mb-8">
          <h3 className="text-xl font-bold text-navy dark:text-white mb-4">Résumé</h3>
          <p className="text-text-secondary leading-relaxed">
            {publication.abstract_ || 'Aucun résumé disponible pour cette publication.'}
          </p>
        </div>

        <div className="mb-8">
          <h3 className="text-xl font-bold text-navy dark:text-white mb-4">Mots-clés</h3>
          <div className="flex flex-wrap gap-2">
            {publication.keywords.length > 0 ? publication.keywords.map((keyword) => (
              <Badge key={keyword} variant="default">{keyword}</Badge>
            )) : <Badge variant="default">Aucun mot-clé</Badge>}
          </div>
        </div>

        <div className="flex gap-4">
          <Button
            disabled={!publication.pdfUrl}
            onClick={() => {
              if (publication.pdfUrl) {
                window.open(publication.pdfUrl, '_blank', 'noopener,noreferrer');
              }
            }}
          >
            <FileDown size={18} /> Télécharger PDF
          </Button>
          <Button
            variant="outlined"
            disabled={!publication.doi}
            onClick={() => {
              if (publication.doi) {
                window.open(`https://doi.org/${publication.doi}`, '_blank', 'noopener,noreferrer');
              }
            }}
          >
            <ExternalLink size={18} /> Voir DOI
          </Button>
        </div>
      </div>

      <PublicFooter />
    </div>
  );
}
