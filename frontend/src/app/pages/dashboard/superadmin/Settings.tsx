import { Card, CardContent, CardHeader } from '../../../components/ui/Card';
import { Button } from '../../../components/ui/Button';

export default function SuperAdminSettings() {
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
            <input type="text" defaultValue="LIMTIC" className="w-full px-4 py-2 border border-surface-border rounded-lg" />
          </div>
          <div>
            <label className="block text-sm font-medium mb-2">Slogan</label>
            <input type="text" defaultValue="Laboratoire d'Informatique, Modélisation..." className="w-full px-4 py-2 border border-surface-border rounded-lg" />
          </div>
          <div>
            <label className="block text-sm font-medium mb-2">Email de contact</label>
            <input type="email" defaultValue="contact@limtic.tn" className="w-full px-4 py-2 border border-surface-border rounded-lg" />
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
              <input type="text" placeholder="smtp.gmail.com" className="w-full px-4 py-2 border border-surface-border rounded-lg" />
            </div>
            <div>
              <label className="block text-sm font-medium mb-2">Port</label>
              <input type="number" defaultValue="587" className="w-full px-4 py-2 border border-surface-border rounded-lg" />
            </div>
          </div>
          <Button variant="outlined">Tester la configuration</Button>
        </CardContent>
      </Card>

      <div className="flex justify-end">
        <Button>Sauvegarder les modifications</Button>
      </div>
    </div>
  );
}
