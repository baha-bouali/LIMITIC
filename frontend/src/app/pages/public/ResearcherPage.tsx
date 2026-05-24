import { PublicNavbar } from '../../components/layout/PublicNavbar';
import { PublicFooter } from '../../components/layout/PublicFooter';
import { Badge } from '../../components/ui/Badge';
import { Mail, Phone, ExternalLink } from 'lucide-react';
import { useState } from 'react';

export default function ResearcherPage() {
  const [activeTab, setActiveTab] = useState('bio');

  return (
    <div className="min-h-screen bg-white dark:bg-background">
      <PublicNavbar />
      <div style={{ height: 'var(--navbar-height)' }} />

      <div className="max-w-[var(--content-max-width)] mx-auto px-6 py-12">
        <div className="grid grid-cols-1 lg:grid-cols-4 gap-8">
          <div className="lg:col-span-1">
            <div className="sticky top-24 space-y-6">
              <div className="w-48 h-48 rounded-full bg-navy text-white flex items-center justify-center text-6xl font-bold mx-auto border-4 border-accent-blue">A</div>
              <div className="text-center">
                <h2 className="text-2xl font-bold text-navy dark:text-white">Dr. Ahmed Ben Salem</h2>
                <Badge variant="info" className="mt-2">Chercheur</Badge>
                <Badge variant="default" className="mt-2">Intelligence Artificielle</Badge>
              </div>
              <div className="space-y-3">
                <div className="flex items-center gap-3 text-sm">
                  <Mail size={16} className="text-accent-blue" />
                  <span>ahmed.bensalem@isi.utm.tn</span>
                </div>
                <div className="flex items-center gap-3 text-sm">
                  <Phone size={16} className="text-accent-blue" />
                  <span>+216 71 123 456</span>
                </div>
              </div>
              <div className="flex gap-2">
                <a href="#" className="flex-1 text-center px-4 py-2 border border-accent-blue text-accent-blue rounded-lg hover:bg-accent-blue hover:text-white transition-colors text-sm">ORCID</a>
                <a href="#" className="flex-1 text-center px-4 py-2 border border-accent-blue text-accent-blue rounded-lg hover:bg-accent-blue hover:text-white transition-colors text-sm">Scholar</a>
              </div>
            </div>
          </div>

          <div className="lg:col-span-3">
            <div className="border-b border-surface-border mb-6">
              <div className="flex gap-6">
                {['bio', 'publications', 'encadrements'].map((tab) => (
                  <button
                    key={tab}
                    onClick={() => setActiveTab(tab)}
                    className={`py-3 px-2 border-b-2 transition-colors capitalize ${
                      activeTab === tab ? 'border-accent-blue text-accent-blue font-medium' : 'border-transparent text-text-secondary'
                    }`}
                  >
                    {tab === 'bio' ? 'Biographie' : tab === 'publications' ? 'Publications' : 'Encadrements'}
                  </button>
                ))}
              </div>
            </div>

            {activeTab === 'bio' && (
              <div className="prose max-w-none">
                <p className="text-text-secondary leading-relaxed">
                  Dr. Ahmed Ben Salem est professeur en informatique spécialisé dans l'intelligence artificielle et l'apprentissage automatique.
                  Ses recherches portent principalement sur les applications de l'IA dans le domaine médical et la vision par ordinateur.
                  Il a publié plus de 50 articles dans des revues et conférences internationales de premier rang.
                </p>
              </div>
            )}

            {activeTab === 'publications' && (
              <div className="space-y-4">
                <div className="p-4 border border-surface-border rounded-lg">
                  <div className="flex items-start gap-4">
                    <div className="text-2xl font-bold text-navy dark:text-white">2026</div>
                    <div>
                      <Badge variant="q1" className="mb-2">Q1 ★</Badge>
                      <h4 className="font-bold text-navy dark:text-white mb-1">Deep Learning for Medical Imaging</h4>
                      <p className="text-sm text-text-secondary">A. Ben Salem, F. Gharbi, et al.</p>
                      <p className="text-sm text-text-secondary italic">IEEE Transactions on Medical Imaging</p>
                    </div>
                  </div>
                </div>
              </div>
            )}

            {activeTab === 'encadrements' && (
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div className="p-4 border border-surface-border rounded-lg">
                  <div className="flex items-center gap-3 mb-3">
                    <div className="w-12 h-12 rounded-full bg-teal text-white flex items-center justify-center font-bold">S</div>
                    <div>
                      <div className="font-semibold">Sarah Trabelsi</div>
                      <Badge variant="success" className="text-xs">En cours</Badge>
                    </div>
                  </div>
                  <p className="text-sm text-text-secondary">Apprentissage profond pour le diagnostic médical</p>
                </div>
              </div>
            )}
          </div>
        </div>
      </div>

      <PublicFooter />
    </div>
  );
}
