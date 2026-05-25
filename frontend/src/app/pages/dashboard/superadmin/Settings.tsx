import { useEffect, useState } from 'react';
import { Card, CardContent, CardHeader } from '../../../components/ui/Card';
import { Button } from '../../../components/ui/Button';
import { useGetSettingsQuery, useUpdateSettingsMutation, useTestSmtpMutation } from '../../../../features/settings/api/settingsApiSlice';

export default function SuperAdminSettings() {
  const { data: settings, isLoading, error } = useGetSettingsQuery();
  const [updateSettings, { isLoading: isSaving }] = useUpdateSettingsMutation();
  const [testSmtp, { isLoading: isTesting }] = useTestSmtpMutation();

  // Identity fields
  const [labName, setLabName] = useState('');
  const [labSlogan, setLabSlogan] = useState('');
  const [contactEmail, setContactEmail] = useState('');
  const [address, setAddress] = useState('');
  const [phone, setPhone] = useState('');
  const [logoUrl, setLogoUrl] = useState<string | null>(null);

  // SMTP fields
  const [smtpHost, setSmtpHost] = useState('');
  const [smtpPort, setSmtpPort] = useState(587);
  const [smtpUsername, setSmtpUsername] = useState('');
  const [smtpPassword, setSmtpPassword] = useState('');
  const [smtpUseTls, setSmtpUseTls] = useState(false);

  // Test email
  const [testEmail, setTestEmail] = useState('');

  useEffect(() => {
    if (settings) {
      setLabName(settings.identity.labName);
      setLabSlogan(settings.identity.labSlogan);
      setContactEmail(settings.identity.contactEmail);
      setAddress(settings.identity.address);
      setPhone(settings.identity.phone);
      setLogoUrl(settings.identity.logoUrl ?? null);
      setSmtpHost(settings.smtp.host);
      setSmtpPort(settings.smtp.port);
      setSmtpUsername(settings.smtp.username);
      setSmtpUseTls(settings.smtp.useTls);
    }
  }, [settings]);

  const handleSave = async () => {
    try {
      await updateSettings({
        identity: {
          labName,
          labSlogan,
          contactEmail,
          address,
          phone,
          logoUrl,
        },
        smtp: {
          host: smtpHost,
          port: smtpPort,
          username: smtpUsername,
          password: smtpPassword || null,
          useTls: smtpUseTls,
        },
      }).unwrap();
      alert('Paramètres sauvegardés avec succès');
    } catch (err) {
      alert('Erreur lors de la sauvegarde');
      console.error(err);
    }
  };

  const handleTestSmtp = async () => {
    if (!testEmail) {
      alert('Veuillez entrer une adresse email');
      return;
    }
    try {
      await testSmtp({ testEmail }).unwrap();
      alert('Configuration SMTP valide');
    } catch (err) {
      alert('Erreur lors du test SMTP');
      console.error(err);
    }
  };

  if (isLoading) {
    return <div className="p-4">Chargement...</div>;
  }

  if (error) {
    return <div className="p-4 text-red-600">Erreur lors du chargement des paramètres</div>;
  }

  return (
    <div className="space-y-6 max-w-4xl">
      <h1 className="text-3xl font-bold text-navy">Paramètres Système</h1>

      <Card>
        <CardHeader>
          <h3 className="text-lg font-bold text-navy">Identité du laboratoire</h3>
        </CardHeader>
        <CardContent className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-2">Nom du laboratoire</label>
            <input
              type="text"
              value={labName}
              onChange={(e) => setLabName(e.target.value)}
              className="w-full px-4 py-2 border border-surface-border rounded-lg"
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-2">Slogan</label>
            <input
              type="text"
              value={labSlogan}
              onChange={(e) => setLabSlogan(e.target.value)}
              className="w-full px-4 py-2 border border-surface-border rounded-lg"
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-2">Email de contact</label>
            <input
              type="email"
              value={contactEmail}
              onChange={(e) => setContactEmail(e.target.value)}
              className="w-full px-4 py-2 border border-surface-border rounded-lg"
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-2">Adresse</label>
            <input
              type="text"
              value={address}
              onChange={(e) => setAddress(e.target.value)}
              className="w-full px-4 py-2 border border-surface-border rounded-lg"
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-2">Téléphone</label>
            <input
              type="text"
              value={phone}
              onChange={(e) => setPhone(e.target.value)}
              className="w-full px-4 py-2 border border-surface-border rounded-lg"
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-2">URL du logo</label>
            <input
              type="text"
              value={logoUrl || ''}
              onChange={(e) => setLogoUrl(e.target.value || null)}
              placeholder="https://..."
              className="w-full px-4 py-2 border border-surface-border rounded-lg"
            />
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <h3 className="text-lg font-bold text-navy">Configuration SMTP</h3>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium mb-2">Hôte SMTP</label>
              <input
                type="text"
                value={smtpHost}
                onChange={(e) => setSmtpHost(e.target.value)}
                placeholder="smtp.gmail.com"
                className="w-full px-4 py-2 border border-surface-border rounded-lg"
              />
            </div>
            <div>
              <label className="block text-sm font-medium mb-2">Port</label>
              <input
                type="number"
                value={smtpPort}
                onChange={(e) => setSmtpPort(parseInt(e.target.value) || 587)}
                className="w-full px-4 py-2 border border-surface-border rounded-lg"
              />
            </div>
          </div>
          <div>
            <label className="block text-sm font-medium mb-2">Nom d'utilisateur</label>
            <input
              type="text"
              value={smtpUsername}
              onChange={(e) => setSmtpUsername(e.target.value)}
              className="w-full px-4 py-2 border border-surface-border rounded-lg"
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-2">Mot de passe</label>
            <input
              type="password"
              value={smtpPassword}
              onChange={(e) => setSmtpPassword(e.target.value)}
              className="w-full px-4 py-2 border border-surface-border rounded-lg"
            />
          </div>
          <div className="flex items-center">
            <input
              type="checkbox"
              id="useTls"
              checked={smtpUseTls}
              onChange={(e) => setSmtpUseTls(e.target.checked)}
              className="mr-2"
            />
            <label htmlFor="useTls" className="text-sm font-medium">
              Utiliser TLS
            </label>
          </div>
          <div className="space-y-2">
            <label className="block text-sm font-medium mb-2">Tester la configuration</label>
            <div className="flex gap-2">
              <input
                type="email"
                value={testEmail}
                onChange={(e) => setTestEmail(e.target.value)}
                placeholder="test@example.com"
                className="flex-1 px-4 py-2 border border-surface-border rounded-lg"
              />
              <Button
                variant="outlined"
                onClick={handleTestSmtp}
                disabled={isTesting}
              >
                {isTesting ? 'Test en cours...' : 'Tester'}
              </Button>
            </div>
          </div>
        </CardContent>
      </Card>

      <div className="flex justify-end gap-2">
        <Button disabled={isSaving} onClick={handleSave}>
          {isSaving ? 'Sauvegarde en cours...' : 'Sauvegarder les modifications'}
        </Button>
      </div>
    </div>
  );
}
