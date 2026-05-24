import { useState } from 'react';
import { Card, CardContent } from '../../../components/ui/Card';
import { Button } from '../../../components/ui/Button';
import { Badge } from '../../../components/ui/Badge';
import { Camera, Save, Eye, EyeOff, ShieldCheck } from 'lucide-react';
import { toast } from 'sonner';
import { useChangePasswordMutation } from '../../../api/usersApi';

export default function SuperAdminProfile() {
  const [profile, setProfile] = useState({
    firstName: 'Super',
    lastName: 'Admin',
    email: 'superadmin@limtic.tn',
    phone: '+216 71 234 567',
    bio: 'Administrateur principal du système de gestion du laboratoire LIMTIC.',
  });

  const [passwords, setPasswords] = useState({ current: '', newPass: '', confirm: '' });
  const [showPass, setShowPass] = useState({ current: false, new: false, confirm: false });
  const [saving, setSaving] = useState(false);
  const [changePassword] = useChangePasswordMutation();

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
    try {
      await changePassword({
        email: profile.email,
        oldPassword: passwords.current,
        newPassword: passwords.newPass,
      }).unwrap();
      setPasswords({ current: '', newPass: '', confirm: '' });
      toast.success('Mot de passe modifié avec succès');
    } catch {
      toast.error('Impossible de modifier le mot de passe');
    } finally {
      setSaving(false);
    }
  }

  return (
    <div className="space-y-6 max-w-3xl">
      <div>
        <h1 className="text-3xl font-bold text-navy dark:text-white">Mon Profil</h1>
        <p className="text-text-secondary mt-1">Gérez vos informations personnelles et votre sécurité</p>
      </div>

      {/* Avatar & Role */}
      <Card>
        <CardContent className="p-6">
          <div className="flex items-center gap-6">
            <div className="relative">
              <div className="w-24 h-24 rounded-full bg-gradient-to-br from-navy to-accent-blue flex items-center justify-center text-white text-3xl font-bold">
                SA
              </div>
              <button className="absolute bottom-0 right-0 w-8 h-8 bg-accent-blue text-white rounded-full flex items-center justify-center hover:bg-navy transition-colors shadow-md">
                <Camera size={14} />
              </button>
            </div>
            <div>
              <h2 className="text-xl font-bold text-navy dark:text-white">{profile.firstName} {profile.lastName}</h2>
              <Badge variant="default" className="mt-1 flex items-center gap-1 w-fit">
                <ShieldCheck size={12} /> SuperAdmin
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
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-navy dark:text-white mb-1">Prénom</label>
                <input
                  value={profile.firstName}
                  onChange={e => setProfile(p => ({ ...p, firstName: e.target.value }))}
                  className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-navy dark:text-white mb-1">Nom</label>
                <input
                  value={profile.lastName}
                  onChange={e => setProfile(p => ({ ...p, lastName: e.target.value }))}
                  className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                />
              </div>
            </div>
            <div>
              <label className="block text-sm font-medium text-navy dark:text-white mb-1">Email</label>
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
            <div>
              <label className="block text-sm font-medium text-navy dark:text-white mb-1">Biographie</label>
              <textarea
                value={profile.bio}
                onChange={e => setProfile(p => ({ ...p, bio: e.target.value }))}
                rows={3}
                className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm resize-none"
              />
            </div>
            <div className="flex justify-end">
              <Button onClick={handleSaveProfile} disabled={saving} className="flex items-center gap-2">
                <Save size={16} /> {saving ? 'Enregistrement...' : 'Sauvegarder'}
              </Button>
            </div>
          </div>
        </CardContent>
      </Card>

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
