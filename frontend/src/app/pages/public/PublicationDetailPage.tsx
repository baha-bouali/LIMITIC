import { PublicNavbar } from '../../components/layout/PublicNavbar';
import { PublicFooter } from '../../components/layout/PublicFooter';
import { Badge } from '../../components/ui/Badge';
import { Button } from '../../components/ui/Button';
import { FileDown, ExternalLink } from 'lucide-react';

export default function PublicationDetailPage() {
  return (
    <div className="min-h-screen bg-white dark:bg-background">
      <PublicNavbar />
      <div style={{ height: 'var(--navbar-height)' }} />

      <div className="max-w-4xl mx-auto px-6 py-12">
        <div className="mb-6">
          <Badge variant="q1" className="mb-3">Q1 ★</Badge>
          <h1 className="text-4xl font-bold text-navy dark:text-white mb-4">
            Deep Learning Approaches for Medical Image Segmentation: A Comprehensive Survey
          </h1>
          <div className="flex items-center gap-4 text-text-secondary mb-6">
            <span>Ahmed Ben Salem</span>
            <span>·</span>
            <span>Fatma Gharbi</span>
            <span>·</span>
            <span>et al.</span>
          </div>
        </div>

        <div className="grid grid-cols-2 gap-4 mb-8 p-6 bg-light-gray dark:bg-card rounded-lg">
          <div className="col-span-2">
            <strong>Axe de recherche:</strong>{' '}
            <Badge variant="info" className="ml-2">Intelligence Artificielle et Apprentissage Automatique</Badge>
          </div>
          <div><strong>Journal:</strong> IEEE Transactions on Medical Imaging</div>
          <div><strong>Année:</strong> 2026</div>
          <div><strong>Volume:</strong> 45</div>
          <div><strong>Pages:</strong> 123-145</div>
          <div><strong>DOI:</strong> <a href="#" className="text-accent-blue hover:underline">10.1109/TMI.2026.123456</a></div>
          <div><strong>Quartile:</strong> Q1</div>
          <div><strong>Impact Factor:</strong> 10.245</div>
          <div><strong>ISSN:</strong> 0278-0062</div>
        </div>

        <div className="mb-8">
          <h3 className="text-xl font-bold text-navy dark:text-white mb-4">Résumé</h3>
          <p className="text-text-secondary leading-relaxed">
            This comprehensive survey provides an in-depth analysis of deep learning approaches for medical image segmentation.
            We review state-of-the-art architectures including U-Net variants, attention mechanisms, and transformer-based models.
            The survey covers applications across multiple medical imaging modalities and discusses current challenges and future directions.
          </p>
        </div>

        <div className="mb-8">
          <h3 className="text-xl font-bold text-navy dark:text-white mb-4">Mots-clés</h3>
          <div className="flex flex-wrap gap-2">
            <Badge variant="default">Deep Learning</Badge>
            <Badge variant="default">Medical Imaging</Badge>
            <Badge variant="default">Image Segmentation</Badge>
            <Badge variant="default">Computer Vision</Badge>
          </div>
        </div>

        <div className="flex gap-4">
          <Button><FileDown size={18} /> Télécharger PDF</Button>
          <Button variant="outlined"><ExternalLink size={18} /> Voir DOI</Button>
        </div>
      </div>

      <PublicFooter />
    </div>
  );
}
