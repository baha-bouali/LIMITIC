import { useState } from 'react';
import { Button } from '../ui/Button';
import { Badge } from '../ui/Badge';
import { X, FileText, Globe, BookOpen, File, ClipboardList, Plus, Trash2, User } from 'lucide-react';
import { toast } from 'sonner';
import { clsx } from 'clsx';
import { useGetAllAxesQuery } from '../../api/axesApi';
import type { CreateDashboardPublicationRequest, DashboardPublicationDetailDto } from '../../api/dashboardPublicationsApi';

type ApiPublicationType = 'ArticleJournal' | 'ConferenceInternational' | 'ConferenceNational' | 'ChapterBook' | 'TechnicalReport';
type ApiVisibility = 'Public' | 'Private';

interface PublicationFormProps {
  onClose: () => void;
  onSubmit: (data: CreateDashboardPublicationRequest, id?: string) => void;
  initialData?: DashboardPublicationDetailDto | null;
  canPublishDirectly?: boolean;
}

export function PublicationForm({ onClose, onSubmit, initialData, canPublishDirectly = false }: PublicationFormProps) {
  const { data: axes = [] } = useGetAllAxesQuery();

  const [step, setStep] = useState(1);
  const [formData, setFormData] = useState({
    type: (initialData?.type as ApiPublicationType) || 'ArticleJournal' as ApiPublicationType,
    title: initialData?.title || '',
    year: initialData?.year || new Date().getFullYear(),
    abstract: initialData?.abstract_ || '',
    keywords: initialData?.keywords || [] as string[],
    visibility: (initialData?.visibility as ApiVisibility) || 'Public' as ApiVisibility,
    researchAxisId: initialData?.axe?.id || '',
    doi: initialData?.doi || '',
    venue: initialData?.venue || '',
    authors: initialData?.authors || [] as string[],
    // Journal article fields
    journalName: initialData?.journalName || '',
    volume: initialData?.volume || '',
    number: initialData?.number || '',
    pages: initialData?.pages || '',
    ranking: initialData?.quartile || '',
    // Conference fields
    conferenceName: '',
    location: initialData?.location || '',
    coreRanking: initialData?.coreRanking || '',
    // Book chapter fields
    bookTitle: initialData?.bookTitle || '',
    publisher: initialData?.publisher || '',
    isbn: initialData?.isbn || '',
    // Technical report fields
    reportNumber: initialData?.reportNumber ? String(initialData.reportNumber) : '',
    institution: initialData?.institution || '',
  });

  const [currentKeyword, setCurrentKeyword] = useState('');
  const [showAddAuthor, setShowAddAuthor] = useState(false);
  const [newAuthor, setNewAuthor] = useState({ firstName: '', lastName: '', affiliation: '', email: '' });
  const [authorSearch, setAuthorSearch] = useState('');
  const [showAuthorDropdown, setShowAuthorDropdown] = useState(false);

  const labMembers = [
    { id: '1', name: 'Dr. Ahmed Ben Salem', role: 'Chercheur', email: 'ahmed.bensalem@limtic.tn' },
    { id: '2', name: 'Dr. Fatma Gharbi', role: 'Chercheur', email: 'fatma.gharbi@limtic.tn' },
    { id: '3', name: 'Dr. Mohamed Mezghani', role: 'Chercheur', email: 'mohamed.mezghani@limtic.tn' },
    { id: '4', name: 'Sarah Trabelsi', role: 'Doctorant', email: 'sarah.trabelsi@limtic.tn' },
    { id: '5', name: 'Mohamed Najjar', role: 'Doctorant', email: 'mohamed.najjar@limtic.tn' },
    { id: '6', name: 'Karim Slimi', role: 'Mastérien', email: 'karim.slimi@limtic.tn' },
  ];

  const publicationTypes = [
    { value: 'ArticleJournal' as ApiPublicationType, label: 'Article de Journal', icon: FileText },
    { value: 'ConferenceInternational' as ApiPublicationType, label: 'Conférence Internationale', icon: Globe },
    { value: 'ConferenceNational' as ApiPublicationType, label: 'Conférence Nationale', icon: BookOpen },
    { value: 'ChapterBook' as ApiPublicationType, label: "Chapitre d'ouvrage", icon: File },
    { value: 'TechnicalReport' as ApiPublicationType, label: 'Rapport Technique', icon: ClipboardList },
  ];

  const handleAddKeyword = () => {
    if (currentKeyword.trim() && !formData.keywords.includes(currentKeyword.trim())) {
      setFormData({ ...formData, keywords: [...formData.keywords, currentKeyword.trim()] });
      setCurrentKeyword('');
    }
  };

  const handleRemoveKeyword = (keyword: string) => {
    setFormData({ ...formData, keywords: formData.keywords.filter(k => k !== keyword) });
  };

  const handleAddExistingAuthor = (memberName: string) => {
    if (!formData.authors.includes(memberName)) {
      setFormData({ ...formData, authors: [...formData.authors, memberName] });
      toast.success(`${memberName} ajouté comme auteur`);
    }
  };

  const handleAddNewAuthor = () => {
    if (!newAuthor.firstName || !newAuthor.lastName) {
      toast.error('Le prénom et le nom sont requis');
      return;
    }
    const fullName = `${newAuthor.firstName} ${newAuthor.lastName}`;
    if (!formData.authors.includes(fullName)) {
      setFormData({ ...formData, authors: [...formData.authors, fullName] });
      setNewAuthor({ firstName: '', lastName: '', affiliation: '', email: '' });
      setShowAddAuthor(false);
      toast.success(`${fullName} ajouté comme auteur`);
    }
  };

  const handleRemoveAuthor = (author: string) => {
    setFormData({ ...formData, authors: formData.authors.filter(a => a !== author) });
  };

  const moveAuthorUp = (index: number) => {
    if (index === 0) return;
    const newAuthors = [...formData.authors];
    [newAuthors[index - 1], newAuthors[index]] = [newAuthors[index], newAuthors[index - 1]];
    setFormData({ ...formData, authors: newAuthors });
  };

  const moveAuthorDown = (index: number) => {
    if (index === formData.authors.length - 1) return;
    const newAuthors = [...formData.authors];
    [newAuthors[index], newAuthors[index + 1]] = [newAuthors[index + 1], newAuthors[index]];
    setFormData({ ...formData, authors: newAuthors });
  };

  const buildApiRequest = (): CreateDashboardPublicationRequest => {
    const base: CreateDashboardPublicationRequest = {
      researchAxisId: formData.researchAxisId,
      title: formData.title,
      abstract: formData.abstract || null,
      keywords: formData.keywords,
      doi: formData.doi || null,
      venue: formData.venue || null,
      type: formData.type,
      visibility: formData.visibility,
      year: formData.year,
      authors: formData.authors,
      journalArticle: null,
      technicalReport: null,
      bookChapter: null,
      nationalConference: null,
      internationalConference: null,
    };

    if (formData.type === 'ArticleJournal') {
      base.journalArticle = {
        journalName: formData.journalName,
        volume: formData.volume || undefined,
        number: formData.number || undefined,
        pages: formData.pages || undefined,
        ranking: formData.ranking || undefined,
      };
    } else if (formData.type === 'ConferenceInternational') {
      base.internationalConference = {
        conferenceName: formData.conferenceName,
        location: formData.location || undefined,
        pages: formData.pages || undefined,
        ranking: formData.coreRanking || undefined,
      };
    } else if (formData.type === 'ConferenceNational') {
      base.nationalConference = {
        conferenceName: formData.conferenceName,
        location: formData.location || undefined,
        pages: formData.pages || undefined,
      };
    } else if (formData.type === 'ChapterBook') {
      base.bookChapter = {
        bookTitle: formData.bookTitle,
        publisher: formData.publisher || undefined,
        isbn: formData.isbn || undefined,
        pages: formData.pages || undefined,
      };
    } else if (formData.type === 'TechnicalReport') {
      base.technicalReport = {
        reportNumber: formData.reportNumber || undefined,
        institution: formData.institution || undefined,
      };
    }

    return base;
  };

  const handleSubmit = () => {
    if (!formData.title.trim()) {
      toast.error('Le titre est requis');
      return;
    }
    if (!formData.researchAxisId) {
      toast.error("Veuillez sélectionner un axe de recherche");
      return;
    }
    if (formData.authors.length === 0) {
      toast.error('Au moins un auteur est requis');
      return;
    }

    const request = buildApiRequest();
    onSubmit(request, initialData?.id);
    onClose();
  };

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
      <div className="bg-white dark:bg-card rounded-2xl shadow-[0_8px_32px_rgba(15,37,87,.16)] max-w-4xl w-full max-h-[90vh] overflow-hidden flex flex-col">
        {/* Header */}
        <div className="px-6 py-4 border-b border-surface-border flex items-center justify-between">
          <div>
            <h2 className="text-2xl font-bold text-navy dark:text-white">
              {initialData ? 'Modifier la publication' : 'Créer une publication'}
            </h2>
            <div className="flex items-center gap-2 mt-2">
              {[1, 2, 3, 4, 5].map((s) => (
                <div key={s} className={clsx('h-1 w-12 rounded-full transition-colors', step >= s ? 'bg-accent-blue' : 'bg-light-gray')} />
              ))}
            </div>
          </div>
          <button onClick={onClose} className="p-2 hover:bg-light-gray rounded-lg">
            <X size={24} />
          </button>
        </div>

        {/* Content */}
        <div className="flex-1 overflow-y-auto p-6">
          {/* Step 1: Type & Info générale */}
          {step === 1 && (
            <div className="space-y-6">
              <div>
                <label className="block text-sm font-medium mb-3">Type de publication *</label>
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-3">
                  {publicationTypes.map((type) => {
                    const Icon = type.icon;
                    return (
                      <button
                        key={type.value}
                        onClick={() => setFormData({ ...formData, type: type.value })}
                        className={clsx(
                          'p-4 border-2 rounded-lg transition-all text-left',
                          formData.type === type.value
                            ? 'border-accent-blue bg-accent-blue/5'
                            : 'border-surface-border hover:border-accent-blue/50'
                        )}
                      >
                        <Icon size={24} className="text-accent-blue mb-2" />
                        <div className="font-medium text-sm">{type.label}</div>
                      </button>
                    );
                  })}
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium mb-2">Titre *</label>
                <input
                  type="text"
                  required
                  value={formData.title}
                  onChange={(e) => setFormData({ ...formData, title: e.target.value })}
                  className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background dark:text-white"
                  placeholder="Titre complet de la publication"
                />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium mb-2">Année *</label>
                  <input
                    type="number"
                    required
                    value={formData.year}
                    onChange={(e) => setFormData({ ...formData, year: parseInt(e.target.value) })}
                    className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background dark:text-white"
                    min="1900"
                    max={new Date().getFullYear() + 1}
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium mb-2">Visibilité</label>
                  <select
                    value={formData.visibility}
                    onChange={(e) => setFormData({ ...formData, visibility: e.target.value as ApiVisibility })}
                    className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background dark:text-white"
                  >
                    <option value="Public">Publique</option>
                    <option value="Private">Privée</option>
                  </select>
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium mb-2">Résumé / Abstract</label>
                <textarea
                  value={formData.abstract}
                  onChange={(e) => setFormData({ ...formData, abstract: e.target.value })}
                  rows={6}
                  className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent resize-none dark:bg-input-background"
                  placeholder="Résumé complet de la publication..."
                />
              </div>

              <div>
                <label className="block text-sm font-medium mb-2">Mots-clés</label>
                <div className="flex gap-2 mb-3">
                  <input
                    type="text"
                    value={currentKeyword}
                    onChange={(e) => setCurrentKeyword(e.target.value)}
                    onKeyPress={(e) => e.key === 'Enter' && (e.preventDefault(), handleAddKeyword())}
                    className="flex-1 px-4 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background"
                    placeholder="Tapez un mot-clé et appuyez sur Entrée"
                  />
                  <Button onClick={handleAddKeyword} type="button">Ajouter</Button>
                </div>
                <div className="flex flex-wrap gap-2">
                  {formData.keywords.map((keyword, idx) => (
                    <Badge key={idx} variant="default" className="flex items-center gap-2">
                      {keyword}
                      <button onClick={() => handleRemoveKeyword(keyword)} className="hover:text-error">
                        <X size={14} />
                      </button>
                    </Badge>
                  ))}
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium mb-2">Axe de recherche *</label>
                <select
                  required
                  value={formData.researchAxisId}
                  onChange={(e) => setFormData({ ...formData, researchAxisId: e.target.value })}
                  className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background dark:text-white"
                >
                  <option value="">Sélectionner un axe de recherche</option>
                  {axes.map((axis) => (
                    <option key={axis.id} value={axis.id}>{axis.title}</option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium mb-2">DOI</label>
                <input
                  type="text"
                  value={formData.doi}
                  onChange={(e) => setFormData({ ...formData, doi: e.target.value })}
                  className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background dark:text-white"
                  placeholder="10.1234/example"
                />
              </div>
            </div>
          )}

          {/* Step 2: Type-specific details */}
          {step === 2 && formData.type === 'ArticleJournal' && (
            <div className="space-y-6">
              <h3 className="text-lg font-bold text-navy dark:text-white">Détails de l'article</h3>

              <div>
                <label className="block text-sm font-medium mb-2">Nom du journal *</label>
                <input
                  type="text"
                  value={formData.journalName}
                  onChange={(e) => setFormData({ ...formData, journalName: e.target.value })}
                  className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background dark:text-white"
                />
              </div>

              <div>
                <label className="block text-sm font-medium mb-2">Revue / Venue</label>
                <input
                  type="text"
                  value={formData.venue}
                  onChange={(e) => setFormData({ ...formData, venue: e.target.value })}
                  className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background dark:text-white"
                  placeholder="Medical Image Analysis, Vol. 84"
                />
              </div>

              <div className="grid grid-cols-3 gap-4">
                <div>
                  <label className="block text-sm font-medium mb-2">Volume</label>
                  <input
                    type="text"
                    value={formData.volume}
                    onChange={(e) => setFormData({ ...formData, volume: e.target.value })}
                    className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background dark:text-white"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium mb-2">Numéro</label>
                  <input
                    type="text"
                    value={formData.number}
                    onChange={(e) => setFormData({ ...formData, number: e.target.value })}
                    className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background dark:text-white"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium mb-2">Pages</label>
                  <input
                    type="text"
                    value={formData.pages}
                    onChange={(e) => setFormData({ ...formData, pages: e.target.value })}
                    className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background dark:text-white"
                    placeholder="123-145"
                  />
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium mb-2">Quartile Scimago</label>
                <select
                  value={formData.ranking}
                  onChange={(e) => setFormData({ ...formData, ranking: e.target.value })}
                  className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background dark:text-white"
                >
                  <option value="">Non renseigné</option>
                  <option value="Q1">Q1</option>
                  <option value="Q2">Q2</option>
                  <option value="Q3">Q3</option>
                  <option value="Q4">Q4</option>
                </select>
              </div>
            </div>
          )}

          {step === 2 && (formData.type === 'ConferenceInternational' || formData.type === 'ConferenceNational') && (
            <div className="space-y-6">
              <h3 className="text-lg font-bold text-navy dark:text-white">Détails de la conférence</h3>

              <div>
                <label className="block text-sm font-medium mb-2">Nom de la conférence *</label>
                <input
                  type="text"
                  value={formData.conferenceName}
                  onChange={(e) => setFormData({ ...formData, conferenceName: e.target.value })}
                  className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background dark:text-white"
                />
              </div>

              <div>
                <label className="block text-sm font-medium mb-2">Venue / Nom court</label>
                <input
                  type="text"
                  value={formData.venue}
                  onChange={(e) => setFormData({ ...formData, venue: e.target.value })}
                  className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background dark:text-white"
                  placeholder="CVPR 2026"
                />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium mb-2">Lieu</label>
                  <input
                    type="text"
                    value={formData.location}
                    onChange={(e) => setFormData({ ...formData, location: e.target.value })}
                    className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background dark:text-white"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium mb-2">Pages</label>
                  <input
                    type="text"
                    value={formData.pages}
                    onChange={(e) => setFormData({ ...formData, pages: e.target.value })}
                    className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background dark:text-white"
                  />
                </div>
              </div>

              {formData.type === 'ConferenceInternational' && (
                <div>
                  <label className="block text-sm font-medium mb-2">Classement CORE</label>
                  <select
                    value={formData.coreRanking}
                    onChange={(e) => setFormData({ ...formData, coreRanking: e.target.value })}
                    className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background dark:text-white"
                  >
                    <option value="">Non renseigné</option>
                    <option value="A*">CORE A*</option>
                    <option value="A">CORE A</option>
                    <option value="B">CORE B</option>
                    <option value="C">CORE C</option>
                  </select>
                </div>
              )}
            </div>
          )}

          {step === 2 && formData.type === 'ChapterBook' && (
            <div className="space-y-6">
              <h3 className="text-lg font-bold text-navy dark:text-white">Détails du chapitre</h3>

              <div>
                <label className="block text-sm font-medium mb-2">Titre du livre *</label>
                <input
                  type="text"
                  value={formData.bookTitle}
                  onChange={(e) => setFormData({ ...formData, bookTitle: e.target.value })}
                  className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background dark:text-white"
                />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium mb-2">Éditeur</label>
                  <input
                    type="text"
                    value={formData.publisher}
                    onChange={(e) => setFormData({ ...formData, publisher: e.target.value })}
                    className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background dark:text-white"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium mb-2">ISBN</label>
                  <input
                    type="text"
                    value={formData.isbn}
                    onChange={(e) => setFormData({ ...formData, isbn: e.target.value })}
                    className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background dark:text-white"
                  />
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium mb-2">Pages</label>
                <input
                  type="text"
                  value={formData.pages}
                  onChange={(e) => setFormData({ ...formData, pages: e.target.value })}
                  className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background dark:text-white"
                  placeholder="123-145"
                />
              </div>
            </div>
          )}

          {step === 2 && formData.type === 'TechnicalReport' && (
            <div className="space-y-6">
              <h3 className="text-lg font-bold text-navy dark:text-white">Détails du rapport</h3>

              <div>
                <label className="block text-sm font-medium mb-2">Numéro du rapport</label>
                <input
                  type="text"
                  value={formData.reportNumber}
                  onChange={(e) => setFormData({ ...formData, reportNumber: e.target.value })}
                  className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background dark:text-white"
                />
              </div>

              <div>
                <label className="block text-sm font-medium mb-2">Institution</label>
                <input
                  type="text"
                  value={formData.institution}
                  onChange={(e) => setFormData({ ...formData, institution: e.target.value })}
                  className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background dark:text-white"
                />
              </div>
            </div>
          )}

          {/* Step 3: Authors */}
          {step === 3 && (
            <div className="space-y-6">
              <div className="flex items-center justify-between">
                <h3 className="text-lg font-bold text-navy dark:text-white">Auteurs</h3>
                <Button onClick={() => setShowAddAuthor(!showAddAuthor)} variant="outlined" className="text-sm">
                  <Plus size={16} /> Ajouter auteur externe
                </Button>
              </div>

              {showAddAuthor && (
                <div className="p-4 bg-light-gray dark:bg-input-background rounded-lg border border-surface-border space-y-4">
                  <h4 className="font-medium text-navy dark:text-white">Nouvel auteur externe</h4>
                  <div className="grid grid-cols-2 gap-4">
                    <div>
                      <label className="block text-sm font-medium mb-1">Prénom *</label>
                      <input
                        type="text"
                        value={newAuthor.firstName}
                        onChange={(e) => setNewAuthor({ ...newAuthor, firstName: e.target.value })}
                        className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                      />
                    </div>
                    <div>
                      <label className="block text-sm font-medium mb-1">Nom *</label>
                      <input
                        type="text"
                        value={newAuthor.lastName}
                        onChange={(e) => setNewAuthor({ ...newAuthor, lastName: e.target.value })}
                        className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                      />
                    </div>
                  </div>
                  <div>
                    <label className="block text-sm font-medium mb-1">Affiliation</label>
                    <input
                      type="text"
                      value={newAuthor.affiliation}
                      onChange={(e) => setNewAuthor({ ...newAuthor, affiliation: e.target.value })}
                      className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                      placeholder="Université, Laboratoire..."
                    />
                  </div>
                  <div className="flex gap-2 justify-end">
                    <Button onClick={() => { setShowAddAuthor(false); setNewAuthor({ firstName: '', lastName: '', affiliation: '', email: '' }); }} variant="outlined" className="text-sm">
                      Annuler
                    </Button>
                    <Button onClick={handleAddNewAuthor} className="text-sm">
                      Ajouter
                    </Button>
                  </div>
                </div>
              )}

              {formData.authors.length > 0 && (
                <div className="space-y-3">
                  <h4 className="font-medium text-navy dark:text-white">Auteurs sélectionnés ({formData.authors.length})</h4>
                  <div className="space-y-2">
                    {formData.authors.map((author, index) => (
                      <div key={index} className="flex items-center gap-3 p-3 bg-white dark:bg-card border border-surface-border rounded-lg">
                        <span className="w-6 h-6 rounded-full bg-accent-blue text-white text-xs flex items-center justify-center font-medium">
                          {index + 1}
                        </span>
                        <div className="flex-1">
                          <div className="font-medium text-sm">{author}</div>
                        </div>
                        <div className="flex gap-1">
                          <button onClick={() => moveAuthorUp(index)} disabled={index === 0} className="p-1 hover:bg-light-gray rounded disabled:opacity-30" title="Monter">
                            <span className="text-lg">↑</span>
                          </button>
                          <button onClick={() => moveAuthorDown(index)} disabled={index === formData.authors.length - 1} className="p-1 hover:bg-light-gray rounded disabled:opacity-30" title="Descendre">
                            <span className="text-lg">↓</span>
                          </button>
                          <button onClick={() => handleRemoveAuthor(author)} className="p-1.5 hover:bg-error/10 text-error rounded" title="Retirer">
                            <Trash2 size={14} />
                          </button>
                        </div>
                      </div>
                    ))}
                  </div>
                </div>
              )}

              <div className="space-y-3">
                <h4 className="font-medium text-navy dark:text-white">Membres du laboratoire</h4>
                <div className="relative">
                  <input
                    type="text"
                    value={authorSearch}
                    onChange={(e) => { setAuthorSearch(e.target.value); setShowAuthorDropdown(true); }}
                    onFocus={() => setShowAuthorDropdown(true)}
                    placeholder="Tapez le nom d'un membre du labo..."
                    className="w-full px-4 py-2.5 pl-10 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                  />
                  <User size={16} className="absolute left-3 top-1/2 -translate-y-1/2 text-text-muted" />

                  {showAuthorDropdown && authorSearch && (
                    <div className="absolute z-10 w-full mt-1 bg-white dark:bg-card border border-surface-border rounded-lg shadow-lg max-h-60 overflow-y-auto">
                      {labMembers
                        .filter(m => m.name.toLowerCase().includes(authorSearch.toLowerCase()) && !formData.authors.includes(m.name))
                        .map(member => (
                          <button
                            key={member.id}
                            onClick={() => { handleAddExistingAuthor(member.name); setAuthorSearch(''); setShowAuthorDropdown(false); }}
                            className="w-full p-3 text-left hover:bg-light-gray dark:hover:bg-muted transition-colors border-b border-surface-border last:border-b-0"
                          >
                            <div className="flex items-center gap-2">
                              <User size={14} className="text-accent-blue" />
                              <div>
                                <div className="font-medium text-sm">{member.name}</div>
                                <div className="text-xs text-text-muted">{member.role} • {member.email}</div>
                              </div>
                            </div>
                          </button>
                        ))}
                      {labMembers.filter(m => m.name.toLowerCase().includes(authorSearch.toLowerCase()) && !formData.authors.includes(m.name)).length === 0 && (
                        <div className="p-3 text-sm text-text-muted text-center">Aucun membre trouvé</div>
                      )}
                    </div>
                  )}
                </div>
              </div>
            </div>
          )}

          {/* Step 4: Files */}
          {step === 4 && (
            <div className="space-y-6">
              <h3 className="text-lg font-bold text-navy dark:text-white">Fichiers</h3>
              <div>
                <label className="block text-sm font-medium mb-2">PDF de la publication</label>
                <div className="border-2 border-dashed border-surface-border rounded-lg p-8 text-center">
                  <FileText size={48} className="mx-auto text-accent-blue mb-3" />
                  <p className="text-sm font-medium">Le PDF peut être ajouté après création via la page de détail</p>
                  <p className="text-xs text-text-muted mt-1">Formats acceptés: PDF (max 20MB)</p>
                </div>
              </div>
            </div>
          )}

          {/* Step 5: Summary */}
          {step === 5 && (
            <div className="space-y-6">
              <h3 className="text-lg font-bold text-navy dark:text-white">Résumé</h3>
              <div className="p-6 bg-light-gray dark:bg-input-background rounded-lg space-y-4">
                <div>
                  <div className="text-sm text-text-secondary">Type</div>
                  <div className="font-medium">{publicationTypes.find(t => t.value === formData.type)?.label}</div>
                </div>
                <div>
                  <div className="text-sm text-text-secondary">Titre</div>
                  <div className="font-medium">{formData.title || '—'}</div>
                </div>
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <div className="text-sm text-text-secondary">Année</div>
                    <div className="font-medium">{formData.year}</div>
                  </div>
                  <div>
                    <div className="text-sm text-text-secondary">Visibilité</div>
                    <div className="font-medium">{formData.visibility === 'Public' ? 'Publique' : 'Privée'}</div>
                  </div>
                </div>
                <div>
                  <div className="text-sm text-text-secondary">Axe de recherche</div>
                  <div className="font-medium">{axes.find(a => a.id === formData.researchAxisId)?.title || '—'}</div>
                </div>
                <div>
                  <div className="text-sm text-text-secondary">Auteurs</div>
                  <div className="font-medium">{formData.authors.join(', ') || '—'}</div>
                </div>
                {formData.keywords.length > 0 && (
                  <div>
                    <div className="text-sm text-text-secondary mb-2">Mots-clés</div>
                    <div className="flex flex-wrap gap-2">
                      {formData.keywords.map((kw, idx) => (
                        <Badge key={idx} variant="default">{kw}</Badge>
                      ))}
                    </div>
                  </div>
                )}
              </div>
            </div>
          )}
        </div>

        {/* Footer */}
        <div className="px-6 py-4 border-t border-surface-border flex items-center justify-between">
          <div>
            {step > 1 && (
              <Button onClick={() => setStep(step - 1)} variant="outlined">Précédent</Button>
            )}
          </div>
          <div className="flex gap-3">
            <Button onClick={onClose} variant="outlined">Annuler</Button>
            {step < 5 ? (
              <Button onClick={() => setStep(step + 1)}>Suivant</Button>
            ) : (
              <Button onClick={handleSubmit}>
                {initialData ? 'Enregistrer' : (canPublishDirectly ? 'Publier' : 'Soumettre')}
              </Button>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
