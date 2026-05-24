import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { PublicNavbar } from '../../components/layout/PublicNavbar';
import { PublicFooter } from '../../components/layout/PublicFooter';
import { Button } from '../../components/ui/Button';
import { Card, CardContent, CardHeader } from '../../components/ui/Card';
import { Badge } from '../../components/ui/Badge';
import { ArrowRight, Users, FileText, GraduationCap, Calendar, ChevronDown, MapPin } from 'lucide-react';
import { motion } from 'motion/react';
import { useLanguage } from '../../contexts/LanguageContext';

export default function HomePage() {
  const { t } = useLanguage();
  const [scrollY, setScrollY] = useState(0);

  useEffect(() => {
    const handleScroll = () => setScrollY(window.scrollY);
    window.addEventListener('scroll', handleScroll);
    return () => window.removeEventListener('scroll', handleScroll);
  }, []);

  return (
    <div className="min-h-screen bg-white dark:bg-background">
      <PublicNavbar />

      {/* Hero Section */}
      <section className="relative h-screen flex items-center justify-center overflow-hidden">
        {/* Gradient Background */}
        <div className="absolute inset-0 brand-gradient-diagonal">
          {/* Animated particle network overlay */}
          <div className="absolute inset-0 opacity-10">
            <svg className="w-full h-full">
              <defs>
                <pattern id="grid" width="50" height="50" patternUnits="userSpaceOnUse">
                  <circle cx="25" cy="25" r="1" fill="white" />
                </pattern>
              </defs>
              <rect width="100%" height="100%" fill="url(#grid)" />
            </svg>
          </div>
        </div>

        {/* Content */}
        <div className="relative z-10 max-w-5xl mx-auto px-6 text-center text-white">
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.8 }}
            className="space-y-8"
          >
            <h1 className="text-5xl md:text-6xl font-bold leading-tight">
              {t('home.hero.title')}
            </h1>
            <p className="text-xl md:text-2xl text-white/90 max-w-3xl mx-auto">
              {t('home.hero.subtitle')}
            </p>
            <div className="flex flex-wrap items-center justify-center gap-4 pt-6">
              <Link to="/publications">
                <Button size="lg" variant="outlined" className="!text-white !border-white hover:!bg-white/10">
                  {t('home.hero.publications')}
                  <ArrowRight size={20} />
                </Button>
              </Link>
              <Link to="/equipe">
                <Button size="lg" variant="outlined" className="!text-white !border-white hover:!bg-white/10">
                  {t('home.hero.team')}
                </Button>
              </Link>
              <Link to="/contact">
                <Button size="lg" variant="ghost" className="!text-white hover:!bg-white/10">
                  {t('home.hero.contact')}
                </Button>
              </Link>
            </div>
          </motion.div>
        </div>

        {/* Scroll indicator */}
        <motion.div
          className="absolute bottom-8 left-1/2 transform -translate-x-1/2"
          animate={{ y: [0, 10, 0] }}
          transition={{ duration: 1.5, repeat: Infinity }}
        >
          <ChevronDown size={32} className="text-white/80" />
        </motion.div>
      </section>

      {/* Statistics Bar */}
      <section className="bg-navy text-white py-16">
        <div className="max-w-[var(--content-max-width)] mx-auto px-6">
          <div className="grid grid-cols-1 md:grid-cols-4 gap-8">
            <StatCounter icon={<Users />} count={24} label={t('home.stats.researchers')} />
            <StatCounter icon={<FileText />} count={156} label={t('home.stats.publications')} />
            <StatCounter icon={<GraduationCap />} count={18} label={t('home.stats.phd')} />
            <StatCounter icon={<Calendar />} count={32} label={t('home.stats.events')} />
          </div>
        </div>
      </section>

      {/* Axes de Recherche Section */}
      <section className="bg-light-gray py-20">
        <div className="max-w-[var(--content-max-width)] mx-auto px-6">
          <div className="text-center mb-12">
            <h2 className="text-4xl font-bold text-navy dark:text-white mb-4">{t('home.axes.title')}</h2>
            <p className="text-text-secondary text-lg">{t('home.axes.subtitle')}</p>
          </div>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
            <AxeCard
              id="1"
              title="Intelligence Artificielle"
              description="Recherche avancée en apprentissage automatique, deep learning et systèmes intelligents"
              responsible="Dr. Ahmed Ben Salem"
              themes={['Machine Learning', 'NLP', 'Computer Vision', '+3 more']}
              color="teal"
            />
            <AxeCard
              id="2"
              title="Systèmes Distribués"
              description="Conception et optimisation de systèmes distribués à grande échelle"
              responsible="Dr. Fatma Gharbi"
              themes={['Cloud Computing', 'Blockchain', 'IoT']}
              color="accent-blue"
            />
            <AxeCard
              id="3"
              title="Sécurité Informatique"
              description="Protection des systèmes d'information et cryptographie appliquée"
              responsible="Dr. Mohamed Mezghani"
              themes={['Cryptographie', 'Sécurité Réseau', 'Audit', '+2 more']}
              color="navy"
            />
          </div>
        </div>
      </section>

      {/* Recent Publications Section */}
      <section className="bg-white dark:bg-background py-20">
        <div className="max-w-[var(--content-max-width)] mx-auto px-6">
          <div className="flex items-center justify-between mb-12">
            <h2 className="text-4xl font-bold text-navy dark:text-white">{t('home.publications.title')}</h2>
            <Link to="/publications" className="flex items-center gap-2 text-accent-blue hover:underline">
              {t('home.viewAll')} <ArrowRight size={20} />
            </Link>
          </div>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <PublicationCard
              type="q1"
              year="2026"
              title="Deep Learning Approaches for Medical Image Segmentation: A Comprehensive Survey"
              authors="A. Ben Salem, F. Gharbi, et al."
              venue="IEEE Transactions on Medical Imaging"
              doi="10.1109/TMI.2026.123456"
            />
            <PublicationCard
              type="core-a-star"
              year="2026"
              title="Blockchain-Based Secure Data Sharing in Healthcare Systems"
              authors="M. Mezghani, S. Trabelsi, et al."
              venue="ACM Conference on Computer and Communications Security (CCS 2026)"
              doi="10.1145/3576915.3623456"
            />
            <PublicationCard
              type="q2"
              year="2025"
              title="Federated Learning for Privacy-Preserving Healthcare Analytics"
              authors="F. Gharbi, A. Ben Salem"
              venue="Journal of Biomedical Informatics"
              doi="10.1016/j.jbi.2025.104321"
            />
          </div>
        </div>
      </section>

      {/* Upcoming Events */}
      <section className="bg-off-white py-20">
        <div className="max-w-[var(--content-max-width)] mx-auto px-6">
          <div className="flex items-center justify-between mb-12">
            <h2 className="text-4xl font-bold text-navy dark:text-white">{t('home.events.title')}</h2>
            <Link to="/evenements" className="flex items-center gap-2 text-accent-blue hover:underline">
              {t('home.viewAllEvents')} <ArrowRight size={20} />
            </Link>
          </div>
          <div className="space-y-4">
            <EventItem
              type="SÉMINAIRE"
              date={{ day: '15', month: 'Juin' }}
              title="Intelligence Artificielle et Santé"
              location="Amphithéâtre A, ISI"
            />
            <EventItem
              type="ATELIER"
              date={{ day: '22', month: 'Juin' }}
              title="Introduction au Deep Learning"
              location="Salle B12, ISI"
            />
            <EventItem
              type="CONFÉRENCE"
              date={{ day: '05', month: 'Juil' }}
              title="LIMTIC Research Day 2026"
              location="Campus Universitaire"
            />
          </div>
        </div>
      </section>

      <PublicFooter />
    </div>
  );
}

// Component: Stat Counter
function StatCounter({ icon, count, label }: { icon: React.ReactNode; count: number; label: string }) {
  const [displayCount, setDisplayCount] = useState(0);

  useEffect(() => {
    let start = 0;
    const end = count;
    const duration = 2000;
    const increment = end / (duration / 16);

    const timer = setInterval(() => {
      start += increment;
      if (start >= end) {
        setDisplayCount(end);
        clearInterval(timer);
      } else {
        setDisplayCount(Math.floor(start));
      }
    }, 16);

    return () => clearInterval(timer);
  }, [count]);

  return (
    <div className="text-center">
      <div className="flex items-center justify-center mb-4">
        <div className="w-16 h-16 flex items-center justify-center bg-white/10 rounded-full">
          {icon}
        </div>
      </div>
      <div className="text-5xl font-bold mb-2">{displayCount}</div>
      <div className="text-white/80">{label}</div>
    </div>
  );
}

// Component: Axe Card
function AxeCard({ id, title, description, responsible, themes, color }: any) {
  const { t } = useLanguage();
  return (
    <Card className="hover:shadow-lg transition-shadow overflow-hidden">
      <div className={`h-1 bg-${color}`} />
      <CardHeader>
        <h3 className="text-xl font-bold text-navy dark:text-white mb-2">{title}</h3>
        <p className="text-text-secondary text-sm">{description}</p>
      </CardHeader>
      <CardContent className="space-y-4">
        <div className="flex flex-wrap gap-2">
          {themes.map((theme: string, idx: number) => (
            <Badge key={idx} variant="default">{theme}</Badge>
          ))}
        </div>
        <div className="flex items-center gap-3 pt-4 border-t border-surface-border">
          <div className="w-8 h-8 rounded-full bg-navy text-white flex items-center justify-center text-sm font-bold">
            {responsible.charAt(0)}
          </div>
          <div className="text-sm text-text-secondary">{responsible}</div>
        </div>
        <Link to={`/axes-recherche/${id}`} className="flex items-center gap-2 text-accent-blue hover:underline text-sm">
          {t('home.axes.viewAxis')} <ArrowRight size={16} />
        </Link>
      </CardContent>
    </Card>
  );
}

// Component: Publication Card
function PublicationCard({ type, year, title, authors, venue, doi }: any) {
  const { t } = useLanguage();
  // Create a simple ID from DOI
  const pubId = doi.replace(/\//g, '-').replace(/\./g, '-');

  return (
    <Card className="hover:shadow-lg transition-shadow">
      <CardHeader>
        <div className="flex items-center gap-2 mb-3">
          <Badge variant={type}>{type === 'q1' ? 'Q1 ★' : type === 'core-a-star' ? 'CORE A*' : 'Q2'}</Badge>
          <Badge variant="default">{year}</Badge>
        </div>
        <h4 className="text-base font-bold text-navy dark:text-white line-clamp-2 mb-2">{title}</h4>
        <p className="text-sm text-text-secondary mb-1">{authors}</p>
        <p className="text-sm text-text-secondary italic">{venue}</p>
      </CardHeader>
      <CardContent>
        <div className="flex items-center justify-between">
          <a href={`https://doi.org/${doi}`} target="_blank" rel="noopener noreferrer" className="text-xs text-accent-blue hover:underline">
            DOI: {doi}
          </a>
          <Link to={`/publications/${pubId}`} className="text-accent-blue hover:underline text-sm">
            {t('home.publications.read')} →
          </Link>
        </div>
      </CardContent>
    </Card>
  );
}

// Component: Event Item
function EventItem({ type, date, title, location, id = '1' }: any) {
  const { t } = useLanguage();
  return (
    <Card className="hover:shadow-md transition-shadow">
      <div className="p-6 flex items-center gap-6">
        <div className="flex-shrink-0 w-20 text-center">
          <div className="text-3xl font-bold text-navy dark:text-white">{date.day}</div>
          <div className="text-sm text-text-secondary uppercase">{date.month}</div>
        </div>
        <div className="w-1 h-16 bg-accent-blue rounded-full" />
        <div className="flex-1">
          <Badge variant="info" className="mb-2">{type}</Badge>
          <h4 className="text-lg font-bold text-navy dark:text-white mb-1">{title}</h4>
          <div className="flex items-center gap-2 text-sm text-text-secondary">
            <MapPin size={14} />
            <span>{location}</span>
          </div>
        </div>
        <Link to={`/evenements/${id}`} className="text-accent-blue hover:underline">
          {t('home.events.view')} →
        </Link>
      </div>
    </Card>
  );
}
