import { useParams, Link } from 'react-router-dom';
import { PublicNavbar } from '../../components/layout/PublicNavbar';
import { PublicFooter } from '../../components/layout/PublicFooter';
import { Card, CardContent } from '../../components/ui/Card';
import { Badge } from '../../components/ui/Badge';
import { Button } from '../../components/ui/Button';
import { Users, FileText, Target, Mail, ArrowLeft } from 'lucide-react';
import { useLanguage } from '../../contexts/LanguageContext';

const axesData = {
  '1': {
    id: '1',
    title: 'Intelligence Artificielle et Apprentissage Automatique',
    description: 'Recherche avancée en apprentissage profond, traitement du langage naturel, vision par ordinateur et systèmes intelligents adaptatifs.',
    responsible: { name: 'Dr. Ahmed Ben Salem', email: 'ahmed.bensalem@limtic.tn', id: '1' },
    themes: ['Machine Learning', 'Deep Learning', 'NLP', 'Computer Vision', 'Reinforcement Learning', 'Transfer Learning'],
    members: [
      { name: 'Dr. Ahmed Ben Salem', role: 'Responsable d\'axe' },
      { name: 'Sarah Trabelsi', role: 'Doctorant' },
      { name: 'Mohamed Najjar', role: 'Doctorant' },
      { name: 'Ines Hamdi', role: 'Mastérien' },
    ],
    publications: 45,
    color: 'accent-blue'
  },
  '2': {
    id: '2',
    title: 'Systèmes Distribués et Cloud Computing',
    description: 'Conception et optimisation de systèmes distribués à grande échelle, architectures cloud, blockchain et Internet des Objets.',
    responsible: { name: 'Dr. Fatma Gharbi', email: 'fatma.gharbi@limtic.tn', id: '2' },
    themes: ['Cloud Computing', 'Blockchain', 'IoT', 'Microservices', 'Edge Computing', 'Distributed Systems'],
    members: [
      { name: 'Dr. Fatma Gharbi', role: 'Responsable d\'axe' },
      { name: 'Karim Jebali', role: 'Doctorant' },
      { name: 'Youssef Dali', role: 'Mastérien' },
    ],
    publications: 32,
    color: 'teal'
  },
  '3': {
    id: '3',
    title: 'Sécurité Informatique et Cryptographie',
    description: 'Protection des systèmes d\'information, cryptographie appliquée, sécurité des réseaux et détection d\'intrusions.',
    responsible: { name: 'Dr. Mohamed Mezghani', email: 'mohamed.mezghani@limtic.tn', id: '3' },
    themes: ['Cryptographie', 'Sécurité Réseau', 'Audit de Sécurité', 'Forensics', 'Blockchain Security', 'Secure Systems'],
    members: [
      { name: 'Dr. Mohamed Mezghani', role: 'Responsable d\'axe' },
      { name: 'Karim Slimi', role: 'Mastérien' },
    ],
    publications: 28,
    color: 'navy'
  },
  '4': {
    id: '4',
    title: 'Big Data et Analyse de Données',
    description: 'Traitement et analyse de grandes masses de données, fouille de données, visualisation et systèmes de recommandation.',
    responsible: { name: 'Dr. Sarah Trabelsi', email: 'sarah.trabelsi@limtic.tn', id: '4' },
    themes: ['Data Mining', 'Data Visualization', 'Recommender Systems', 'Data Analytics', 'ETL', 'Predictive Analytics'],
    members: [
      { name: 'Dr. Sarah Trabelsi', role: 'Responsable d\'axe' },
    ],
    publications: 38,
    color: 'success'
  }
};

export default function AxisDetailPage() {
  const { id } = useParams<{ id: string }>();
  const { t } = useLanguage();
  const axis = id && axesData[id as keyof typeof axesData];

  if (!axis) {
    return (
      <div className="min-h-screen bg-white dark:bg-background">
        <PublicNavbar />
        <div style={{ height: 'var(--navbar-height)' }} />
        <div className="max-w-[var(--content-max-width)] mx-auto px-6 py-20 text-center">
          <h1 className="text-4xl font-bold text-navy dark:text-white mb-4">{t('axes.axisNotFound')}</h1>
          <Link to="/axes-recherche">
            <Button>{t('axes.backToAxes')}</Button>
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

      {/* Header */}
      <div className="brand-gradient-diagonal text-white py-20">
        <div className="max-w-[var(--content-max-width)] mx-auto px-6">
          <Link to="/axes-recherche" className="inline-flex items-center gap-2 text-white/80 hover:text-white mb-4 text-sm">
            <ArrowLeft size={16} /> {t('axes.backToAxes')}
          </Link>
          <h1 className="text-5xl font-bold mb-4">{axis.title}</h1>
          <p className="text-xl text-white/90 max-w-3xl">{axis.description}</p>

          <div className="flex gap-8 mt-8">
            <div className="text-center">
              <div className="text-3xl font-bold">{axis.members.length}</div>
              <div className="text-white/80">{t('axes.members')}</div>
            </div>
            <div className="text-center">
              <div className="text-3xl font-bold">{axis.publications}</div>
              <div className="text-white/80">{t('pub.publications')}</div>
            </div>
          </div>
        </div>
      </div>

      {/* Content */}
      <div className="max-w-[var(--content-max-width)] mx-auto px-6 py-12 space-y-8">
        {/* Responsable */}
        <Card>
          <CardContent className="p-6">
            <h2 className="text-2xl font-bold text-navy dark:text-white mb-6 flex items-center gap-2">
              <Target size={24} className="text-accent-blue" />
              {t('axes.axisHead')}
            </h2>
            <div className="flex items-center gap-4">
              <div className="w-16 h-16 rounded-full bg-accent-blue text-white flex items-center justify-center text-2xl font-bold">
                {axis.responsible.name.charAt(0)}
              </div>
              <div>
                <h3 className="text-lg font-bold text-navy dark:text-white">{axis.responsible.name}</h3>
                <a href={`mailto:${axis.responsible.email}`} className="text-sm text-accent-blue hover:underline flex items-center gap-1">
                  <Mail size={14} /> {axis.responsible.email}
                </a>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Thématiques */}
        <Card>
          <CardContent className="p-6">
            <h2 className="text-2xl font-bold text-navy dark:text-white mb-4">{t('axes.themesTitle')}</h2>
            <div className="flex flex-wrap gap-3">
              {axis.themes.map((theme, idx) => (
                <Badge key={idx} variant="default" className="text-sm">{theme}</Badge>
              ))}
            </div>
          </CardContent>
        </Card>

        {/* Membres */}
        <Card>
          <CardContent className="p-6">
            <h2 className="text-2xl font-bold text-navy dark:text-white mb-6 flex items-center gap-2">
              <Users size={24} className="text-accent-blue" />
              {t('axes.axisMembers')}
            </h2>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              {axis.members.map((member, idx) => (
                <div key={idx} className="p-4 border border-surface-border rounded-lg hover:shadow-md transition-shadow">
                  <div className="flex items-center gap-3">
                    <div className="w-12 h-12 rounded-full bg-gradient-to-br from-navy to-accent-blue text-white flex items-center justify-center font-bold">
                      {member.name.charAt(0)}
                    </div>
                    <div>
                      <h4 className="font-medium text-navy dark:text-white">{member.name}</h4>
                      <p className="text-sm text-text-secondary">{member.role}</p>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>

        {/* Publications */}
        <Card>
          <CardContent className="p-6">
            <div className="flex items-center justify-between mb-6">
              <h2 className="text-2xl font-bold text-navy dark:text-white flex items-center gap-2">
                <FileText size={24} className="text-accent-blue" />
                {t('axes.axisPublications')}
              </h2>
              <Link to="/publications" className="text-accent-blue hover:underline text-sm">
                {t('axes.viewAllPublications')} →
              </Link>
            </div>
            <div className="text-center py-8">
              <div className="text-5xl font-bold text-accent-blue mb-2">{axis.publications}</div>
              <div className="text-text-secondary">{t('axes.scientificPublications')}</div>
              <Link to="/publications" className="mt-4 inline-block">
                <Button>{t('axes.explorePublications')}</Button>
              </Link>
            </div>
          </CardContent>
        </Card>
      </div>

      <PublicFooter />
    </div>
  );
}
