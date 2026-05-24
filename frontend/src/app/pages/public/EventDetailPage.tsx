import { PublicNavbar } from '../../components/layout/PublicNavbar';
import { PublicFooter } from '../../components/layout/PublicFooter';
import { Badge } from '../../components/ui/Badge';
import { Card, CardContent } from '../../components/ui/Card';
import { PhotoGallery } from '../../components/shared/PhotoGallery';
import { Calendar, MapPin, Users, Download } from 'lucide-react';

export default function EventDetailPage() {
  // Mock event data - would come from route params/API in real app
  const eventStatus = 'PASSE'; // Could be 'A_VENIR', 'EN_COURS', 'PASSE'
  const hasPhotos = eventStatus === 'PASSE';

  // Mock photos (only shown if event is past and has photos)
  const eventPhotos = [
    { url: 'https://images.unsplash.com/photo-1540575467063-178a50c2df87?w=800', caption: 'Conférence inaugurale par Dr. Ahmed Ben Salem' },
    { url: 'https://images.unsplash.com/photo-1591115765373-5207764f72e7?w=800', caption: 'Session interactive avec les participants' },
    { url: 'https://images.unsplash.com/photo-1475721027785-f74eccf877e2?w=800', caption: 'Pause networking et discussions' },
    { url: 'https://images.unsplash.com/photo-1560439513-74b037a25d84?w=800', caption: 'Présentation des cas cliniques' },
    { url: 'https://images.unsplash.com/photo-1587825140708-dfaf72ae4b04?w=800', caption: 'Démonstration technologique' },
    { url: 'https://images.unsplash.com/photo-1515187029135-18ee286d815b?w=800', caption: 'Photo de groupe des participants' },
  ];

  const speakers = [
    { name: 'Dr. Ahmed Ben Salem', affiliation: 'LIMTIC, ISI', role: 'Conférencier principal', talk: 'Deep Learning pour le diagnostic médical' },
    { name: 'Prof. Marie Dubois', affiliation: 'Hôpital Charles Nicolle', role: 'Conférencière', talk: 'Applications cliniques de l\'IA' },
    { name: 'Dr. Karim Jebali', affiliation: 'LIMTIC, ISI', role: 'Modérateur', talk: 'Table ronde: Défis et perspectives' },
  ];

  return (
    <div className="min-h-screen bg-white dark:bg-background">
      <PublicNavbar />
      <div style={{ height: 'var(--navbar-height)' }} />

      <div className="h-96 bg-gradient-to-br from-navy to-accent-blue flex items-center justify-center relative overflow-hidden">
        <div className="absolute inset-0 opacity-10">
          <svg className="w-full h-full">
            <defs>
              <pattern id="event-grid" width="50" height="50" patternUnits="userSpaceOnUse">
                <circle cx="25" cy="25" r="1" fill="white" />
              </pattern>
            </defs>
            <rect width="100%" height="100%" fill="url(#event-grid)" />
          </svg>
        </div>
        <div className="text-white text-center relative z-10 px-6">
          <Badge variant="info" className="mb-4 !bg-white !text-accent-blue">SÉMINAIRE</Badge>
          <h1 className="text-4xl md:text-5xl font-bold mb-2">Intelligence Artificielle et Santé</h1>
          <p className="text-white/80 text-lg">15 Juin 2026</p>
        </div>
      </div>

      <div className="max-w-7xl mx-auto px-6 py-12">
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          {/* Main Content */}
          <div className="lg:col-span-2 space-y-12">
            {/* Breadcrumb */}
            <div className="text-sm text-text-secondary">
              Événements → Intelligence Artificielle et Santé
            </div>

            {/* Description */}
            <div>
              <h2 className="text-2xl font-bold text-navy dark:text-white mb-4">Description</h2>
              <div className="prose dark:prose-invert max-w-none text-text-secondary dark:text-text-secondary leading-relaxed">
                <p>
                  Ce séminaire explore les applications de l'intelligence artificielle dans le domaine de la santé,
                  avec un focus sur le diagnostic médical assisté par IA, l'imagerie médicale et la médecine personnalisée.
                  Des experts nationaux et internationaux partageront leurs recherches et expériences.
                </p>
                <p className="mt-4">
                  Les participants auront l'opportunité d'assister à des présentations de pointe, de participer à des
                  discussions interactives et de découvrir les dernières innovations en matière d'IA appliquée à la santé.
                </p>
              </div>
            </div>

            {/* Programme */}
            <div>
              <h3 className="text-xl font-bold text-navy dark:text-white mb-4">Programme scientifique</h3>
              <div className="space-y-3">
                <div className="p-4 bg-light-gray dark:bg-card rounded-lg border-l-4 border-accent-blue">
                  <div className="font-semibold text-navy dark:text-white">09h00 - 10h00: Accueil et inscription</div>
                  <div className="text-sm text-text-secondary mt-1">Café de bienvenue et distribution des badges</div>
                </div>
                <div className="p-4 bg-light-gray dark:bg-card rounded-lg border-l-4 border-teal">
                  <div className="font-semibold text-navy dark:text-white">10h00 - 11h30: Conférence inaugurale</div>
                  <div className="text-sm text-text-secondary mt-1">Deep Learning pour le diagnostic médical - Dr. Ahmed Ben Salem</div>
                </div>
                <div className="p-4 bg-light-gray dark:bg-card rounded-lg border-l-4 border-accent-blue">
                  <div className="font-semibold text-navy dark:text-white">11h30 - 13h00: Session 1</div>
                  <div className="text-sm text-text-secondary mt-1">Applications cliniques de l'IA</div>
                </div>
                <div className="p-4 bg-light-gray dark:bg-card rounded-lg border-l-4 border-success">
                  <div className="font-semibold text-navy dark:text-white">14h00 - 15h30: Session 2</div>
                  <div className="text-sm text-text-secondary mt-1">Imagerie médicale et vision par ordinateur</div>
                </div>
                <div className="p-4 bg-light-gray dark:bg-card rounded-lg border-l-4 border-warning">
                  <div className="font-semibold text-navy dark:text-white">15h45 - 17h00: Table ronde</div>
                  <div className="text-sm text-text-secondary mt-1">Défis et perspectives de l'IA en santé</div>
                </div>
              </div>
            </div>

            {/* Speakers */}
            <div>
              <h3 className="text-xl font-bold text-navy dark:text-white mb-4">Intervenants</h3>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                {speakers.map((speaker, idx) => (
                  <Card key={idx}>
                    <CardContent className="p-4">
                      <div className="flex items-start gap-4">
                        <div className="w-16 h-16 rounded-full bg-navy dark:bg-accent-blue text-white flex items-center justify-center text-xl font-bold flex-shrink-0">
                          {speaker.name.charAt(0)}
                        </div>
                        <div className="flex-1 min-w-0">
                          <h4 className="font-bold text-navy dark:text-white">{speaker.name}</h4>
                          <p className="text-sm text-text-secondary">{speaker.affiliation}</p>
                          <Badge variant="info" className="mt-2 text-xs">{speaker.role}</Badge>
                          <p className="text-sm text-text-secondary mt-2 italic">{speaker.talk}</p>
                        </div>
                      </div>
                    </CardContent>
                  </Card>
                ))}
              </div>
            </div>

            {/* Photo Gallery - Only show if event is past and has photos */}
            {hasPhotos && eventPhotos.length > 0 && (
              <div>
                <div className="flex items-center justify-between mb-4">
                  <h3 className="text-xl font-bold text-navy dark:text-white">Galerie photos</h3>
                  <span className="text-sm text-text-secondary">{eventPhotos.length} photos</span>
                </div>
                <PhotoGallery photos={eventPhotos} columns={3} />
              </div>
            )}
          </div>

          {/* Sidebar */}
          <div className="space-y-6">
            <Card>
              <CardContent className="p-6 space-y-6">
                <div>
                  <div className="flex items-start gap-3">
                    <Calendar size={20} className="text-accent-blue mt-1 flex-shrink-0" />
                    <div>
                      <div className="font-semibold text-navy dark:text-white">Date</div>
                      <div className="text-sm text-text-secondary mt-1">15 Juin 2026</div>
                      <div className="text-sm text-text-secondary">09h00 - 17h00</div>
                    </div>
                  </div>
                </div>

                <div className="h-px bg-surface-border" />

                <div>
                  <div className="flex items-start gap-3">
                    <MapPin size={20} className="text-accent-blue mt-1 flex-shrink-0" />
                    <div>
                      <div className="font-semibold text-navy dark:text-white">Lieu</div>
                      <div className="text-sm text-text-secondary mt-1">Amphithéâtre A</div>
                      <div className="text-sm text-text-secondary">Institut Supérieur d'Informatique</div>
                      <div className="text-sm text-text-secondary">2010 Tunis, Tunisie</div>
                    </div>
                  </div>
                </div>

                <div className="h-px bg-surface-border" />

                <div>
                  <div className="flex items-start gap-3">
                    <Users size={20} className="text-accent-blue mt-1 flex-shrink-0" />
                    <div>
                      <div className="font-semibold text-navy dark:text-white">Intervenants</div>
                      <div className="text-sm text-text-secondary mt-1">{speakers.length} conférenciers</div>
                    </div>
                  </div>
                </div>

                <div className="h-px bg-surface-border" />

                <button className="flex items-center justify-center gap-2 w-full px-4 py-3 bg-accent-blue text-white rounded-lg hover:bg-accent-blue/90 transition-colors">
                  <Download size={18} />
                  Télécharger le programme
                </button>
              </CardContent>
            </Card>

            {/* Map */}
            <Card>
              <CardContent className="p-0">
                <div className="aspect-video bg-light-gray dark:bg-card rounded-lg flex items-center justify-center text-text-muted">
                  <MapPin size={48} />
                </div>
                <div className="p-4">
                  <p className="text-sm text-text-secondary text-center">Google Maps embed placeholder</p>
                </div>
              </CardContent>
            </Card>
          </div>
        </div>
      </div>

      <PublicFooter />
    </div>
  );
}
