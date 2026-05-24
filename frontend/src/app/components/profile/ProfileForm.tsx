import { useState } from 'react';
import { Card, CardContent } from '../ui/Card';
import { Button } from '../ui/Button';
import { Badge } from '../ui/Badge';
import { Camera, Save, Eye, EyeOff, User } from 'lucide-react';
import { toast } from 'sonner';

interface ProfileFormProps {
  role: 'SUPER_ADMIN' | 'ADMIN' | 'CHERCHEUR' | 'DOCTORANT' | 'MASTERIEN' | 'VISITOR';
  initialData?: any;
}

export function ProfileForm({ role, initialData }: ProfileFormProps) {
  const [profile, setProfile] = useState({
    firstName: initialData?.firstName || '',
    lastName: initialData?.lastName || '',
    email: initialData?.email || '',
    phone: initialData?.phone || '',
    bio: initialData?.bio || '',
    // Role-specific fields
    grade: initialData?.grade || '',
    speciality: initialData?.speciality || '',
    office: initialData?.office || '',
    orcid: initialData?.orcid || '',
    googleScholar: initialData?.googleScholar || '',
    researchGate: initialData?.researchGate || '',
    linkedin: initialData?.linkedin || '',
    website: initialData?.website || '',
    encadrant: initialData?.encadrant || '',
    thesisTitle: initialData?.thesisTitle || '',
    enrollmentYear: initialData?.enrollmentYear || '',
    masterSpeciality: initialData?.masterSpeciality || '',
    projectTitle: initialData?.projectTitle || '',
  });

  const [profilePhoto, setProfilePhoto] = useState<File | null>(null);
  const [photoPreview, setPhotoPreview] = useState<string | null>(null);
  const [passwords, setPasswords] = useState({ current: '', newPass: '', confirm: '' });
  const [showPass, setShowPass] = useState({ current: false, new: false, confirm: false });
  const [saving, setSaving] = useState(false);

  const handlePhotoChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      if (file.size > 5 * 1024 * 1024) {
        toast.error('La photo ne doit pas dépasser 5MB');
        return;
      }
      setProfilePhoto(file);
      const reader = new FileReader();
      reader.onloadend = () => {
        setPhotoPreview(reader.result as string);
      };
      reader.readAsDataURL(file);
      toast.success('Photo sélectionnée');
    }
  };

  async function handleSaveProfile() {
    setSaving(true);
    await new Promise(r => setTimeout(r, 600));
    setSaving(false);
    toast.success('Profil mis à jour avec succès');
  }

  async function handleChangePassword() {
    if (!passwords.current || !passwords.newPass || !passwords.confirm) {
      toast.error('Veuillez remplir tous les champs de mot de passe');
      return;
    }
    if (passwords.newPass !== passwords.confirm) {
      toast.error('Les mots de passe ne correspondent pas');
      return;
    }
    if (passwords.newPass.length < 8) {
      toast.error('Le mot de passe doit contenir au moins 8 caractères');
      return;
    }
    setSaving(true);
    await new Promise(r => setTimeout(r, 600));
    setSaving(false);
    setPasswords({ current: '', newPass: '', confirm: '' });
    toast.success('Mot de passe modifié avec succès');
  }

  const getRoleBadge = () => {
    const badges = {
      'SUPER_ADMIN': { label: 'SuperAdmin', variant: 'default' as const },
      'ADMIN': { label: 'Admin', variant: 'info' as const },
      'CHERCHEUR': { label: 'Chercheur', variant: 'success' as const },
      'DOCTORANT': { label: 'Doctorant', variant: 'warning' as const },
      'MASTERIEN': { label: 'Mastérien', variant: 'default' as const },
      'VISITOR': { label: 'Visiteur', variant: 'default' as const },
    };
    return badges[role];
  };

  const initials = `${profile.firstName.charAt(0)}${profile.lastName.charAt(0)}`.toUpperCase() || 'U';

  return (
    <div className="space-y-6 max-w-4xl">
      <div>
        <h1 className="text-3xl font-bold text-navy dark:text-white">Mon Profil</h1>
        <p className="text-text-secondary mt-1">Gérez vos informations personnelles et votre sécurité</p>
      </div>

      {/* Avatar & Role */}
      <Card>
        <CardContent className="p-6">
          <div className="flex flex-col md:flex-row items-center gap-6">
            <div className="relative">
              {photoPreview ? (
                <img src={photoPreview} alt="Profile" className="w-24 h-24 rounded-full object-cover" />
              ) : (
                <div className="w-24 h-24 rounded-full bg-gradient-to-br from-navy to-accent-blue flex items-center justify-center text-white text-3xl font-bold">
                  {initials}
                </div>
              )}
              <label htmlFor="photo-upload" className="absolute bottom-0 right-0 w-8 h-8 bg-accent-blue text-white rounded-full flex items-center justify-center hover:bg-navy transition-colors shadow-md cursor-pointer">
                <Camera size={14} />
              </label>
              <input
                id="photo-upload"
                type="file"
                accept="image/*"
                onChange={handlePhotoChange}
                className="hidden"
              />
            </div>
            <div>
              <h2 className="text-xl font-bold text-navy dark:text-white">
                {profile.firstName || profile.lastName ? `${profile.firstName} ${profile.lastName}` : 'Utilisateur'}
              </h2>
              <Badge variant={getRoleBadge().variant} className="mt-1 flex items-center gap-1 w-fit">
                <User size={12} /> {getRoleBadge().label}
              </Badge>
              <p className="text-sm text-text-muted mt-2">{profile.email}</p>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Personal Info */}
      <Card>
        <CardContent className="p-6">
          <h3 className="text-lg font-bold text-navy dark:text-white mb-5">Informations personnelles</h3>
          <div className="space-y-4">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-navy dark:text-white mb-1">Prénom *</label>
                <input
                  value={profile.firstName}
                  onChange={e => setProfile(p => ({ ...p, firstName: e.target.value }))}
                  className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-navy dark:text-white mb-1">Nom *</label>
                <input
                  value={profile.lastName}
                  onChange={e => setProfile(p => ({ ...p, lastName: e.target.value }))}
                  className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                />
              </div>
            </div>
            <div>
              <label className="block text-sm font-medium text-navy dark:text-white mb-1">Email *</label>
              <input
                type="email"
                value={profile.email}
                onChange={e => setProfile(p => ({ ...p, email: e.target.value }))}
                className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-navy dark:text-white mb-1">Téléphone</label>
              <input
                value={profile.phone}
                onChange={e => setProfile(p => ({ ...p, phone: e.target.value }))}
                className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                placeholder="+216 XX XXX XXX"
              />
            </div>

            {/* Role-specific fields */}
            {role === 'CHERCHEUR' && (
              <>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-navy dark:text-white mb-1">Grade</label>
                    <input
                      value={profile.grade}
                      onChange={e => setProfile(p => ({ ...p, grade: e.target.value }))}
                      className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                      placeholder="Chercheur, Chercheur associé..."
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-navy dark:text-white mb-1">Spécialité</label>
                    <input
                      value={profile.speciality}
                      onChange={e => setProfile(p => ({ ...p, speciality: e.target.value }))}
                      className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                    />
                  </div>
                </div>
                <div>
                  <label className="block text-sm font-medium text-navy dark:text-white mb-1">Bureau</label>
                  <input
                    value={profile.office}
                    onChange={e => setProfile(p => ({ ...p, office: e.target.value }))}
                    className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                    placeholder="B205"
                  />
                </div>
              </>
            )}

            {role === 'DOCTORANT' && (
              <>
                <div>
                  <label className="block text-sm font-medium text-navy dark:text-white mb-1">Encadrant</label>
                  <input
                    value={profile.encadrant}
                    readOnly
                    disabled
                    className="w-full px-3 py-2 border border-surface-border rounded-lg dark:bg-input-background text-sm bg-light-gray dark:bg-muted cursor-not-allowed opacity-70"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-navy dark:text-white mb-1">Titre de la thèse</label>
                  <input
                    value={profile.thesisTitle}
                    readOnly
                    disabled
                    className="w-full px-3 py-2 border border-surface-border rounded-lg dark:bg-input-background text-sm bg-light-gray dark:bg-muted cursor-not-allowed opacity-70"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-navy dark:text-white mb-1">Année d'inscription</label>
                  <input
                    type="number"
                    value={profile.enrollmentYear}
                    readOnly
                    disabled
                    className="w-full px-3 py-2 border border-surface-border rounded-lg dark:bg-input-background text-sm bg-light-gray dark:bg-muted cursor-not-allowed opacity-70"
                    min="2000"
                    max={new Date().getFullYear()}
                  />
                </div>
              </>
            )}

            {role === 'MASTERIEN' && (
              <>
                <div>
                  <label className="block text-sm font-medium text-navy dark:text-white mb-1">Spécialité Master</label>
                  <input
                    value={profile.masterSpeciality}
                    readOnly
                    disabled
                    className="w-full px-3 py-2 border border-surface-border rounded-lg dark:bg-input-background text-sm bg-light-gray dark:bg-muted cursor-not-allowed opacity-70"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-navy dark:text-white mb-1">Titre du projet</label>
                  <input
                    value={profile.projectTitle}
                    readOnly
                    disabled
                    className="w-full px-3 py-2 border border-surface-border rounded-lg dark:bg-input-background text-sm bg-light-gray dark:bg-muted cursor-not-allowed opacity-70"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-navy dark:text-white mb-1">Année académique</label>
                  <input
                    type="number"
                    value={profile.enrollmentYear}
                    readOnly
                    disabled
                    className="w-full px-3 py-2 border border-surface-border rounded-lg dark:bg-input-background text-sm bg-light-gray dark:bg-muted cursor-not-allowed opacity-70"
                    min="2000"
                    max={new Date().getFullYear()}
                  />
                </div>
              </>
            )}

            {/* Biography only for SUPER_ADMIN, ADMIN */}
            {['SUPER_ADMIN', 'ADMIN'].includes(role) && (
              <div>
                <label className="block text-sm font-medium text-navy dark:text-white mb-1">Biographie</label>
                <textarea
                  value={profile.bio}
                  onChange={e => setProfile(p => ({ ...p, bio: e.target.value }))}
                  rows={3}
                  className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm resize-none"
                  placeholder="Présentez-vous en quelques lignes..."
                />
              </div>
            )}
            <div className="flex justify-end">
              <Button onClick={handleSaveProfile} disabled={saving} className="flex items-center gap-2">
                <Save size={16} /> {saving ? 'Enregistrement...' : 'Sauvegarder'}
              </Button>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Academic Profiles - Only for CHERCHEUR */}
      {role === 'CHERCHEUR' && (
        <Card>
          <CardContent className="p-6">
            <h3 className="text-lg font-bold text-navy dark:text-white mb-5">Profils académiques</h3>
            <div className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-navy dark:text-white mb-1">ORCID</label>
                <input
                  value={profile.orcid}
                  onChange={e => setProfile(p => ({ ...p, orcid: e.target.value }))}
                  className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                  placeholder="0000-0000-0000-0000"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-navy dark:text-white mb-1">Google Scholar</label>
                <input
                  value={profile.googleScholar}
                  onChange={e => setProfile(p => ({ ...p, googleScholar: e.target.value }))}
                  className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                  placeholder="https://scholar.google.com/..."
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-navy dark:text-white mb-1">ResearchGate</label>
                <input
                  value={profile.researchGate}
                  onChange={e => setProfile(p => ({ ...p, researchGate: e.target.value }))}
                  className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                  placeholder="https://www.researchgate.net/..."
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-navy dark:text-white mb-1">LinkedIn</label>
                <input
                  value={profile.linkedin}
                  onChange={e => setProfile(p => ({ ...p, linkedin: e.target.value }))}
                  className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                  placeholder="https://www.linkedin.com/in/..."
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-navy dark:text-white mb-1">Site web personnel</label>
                <input
                  value={profile.website}
                  onChange={e => setProfile(p => ({ ...p, website: e.target.value }))}
                  className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                  placeholder="https://..."
                />
              </div>
            </div>
          </CardContent>
        </Card>
      )}

      {/* Change Password */}
      <Card>
        <CardContent className="p-6">
          <h3 className="text-lg font-bold text-navy dark:text-white mb-5">Changer le mot de passe</h3>
          <div className="space-y-4">
            {[
              { key: 'current', label: 'Mot de passe actuel', show: showPass.current, toggle: () => setShowPass(s => ({ ...s, current: !s.current })), value: passwords.current, onChange: (v: string) => setPasswords(p => ({ ...p, current: v })) },
              { key: 'new', label: 'Nouveau mot de passe', show: showPass.new, toggle: () => setShowPass(s => ({ ...s, new: !s.new })), value: passwords.newPass, onChange: (v: string) => setPasswords(p => ({ ...p, newPass: v })) },
              { key: 'confirm', label: 'Confirmer le nouveau mot de passe', show: showPass.confirm, toggle: () => setShowPass(s => ({ ...s, confirm: !s.confirm })), value: passwords.confirm, onChange: (v: string) => setPasswords(p => ({ ...p, confirm: v })) },
            ].map(field => (
              <div key={field.key}>
                <label className="block text-sm font-medium text-navy dark:text-white mb-1">{field.label}</label>
                <div className="relative">
                  <input
                    type={field.show ? 'text' : 'password'}
                    value={field.value}
                    onChange={e => field.onChange(e.target.value)}
                    className="w-full px-3 py-2 pr-10 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                    placeholder="••••••••"
                  />
                  <button
                    type="button"
                    onClick={field.toggle}
                    className="absolute right-3 top-1/2 -translate-y-1/2 text-text-muted hover:text-navy dark:hover:text-white"
                  >
                    {field.show ? <EyeOff size={16} /> : <Eye size={16} />}
                  </button>
                </div>
              </div>
            ))}
            <div className="flex justify-end">
              <Button onClick={handleChangePassword} disabled={saving} className="flex items-center gap-2">
                <Save size={16} /> {saving ? 'Modification...' : 'Changer le mot de passe'}
              </Button>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
