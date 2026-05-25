import { Card, CardContent } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { CheckCircle, XCircle, Info } from 'lucide-react';

export default function ChercheurNotifications() {
  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-3xl font-bold text-navy">Notifications</h1>
        <button className="text-accent-blue hover:underline text-sm">Tout marquer comme lu</button>
      </div>

      <div className="space-y-3">
        <NotificationCard
          icon={<CheckCircle className="text-success" />}
          title="Publication validée"
          message="Votre publication 'Deep Learning for Medical Imaging' a été validée et publiée."
          time="Il y a 2h"
          unread={true}
        />
        <NotificationCard
          icon={<XCircle className="text-error" />}
          title="Publication rejetée"
          message="Votre publication 'AI Survey' nécessite des révisions. Motif: Résumé insuffisant."
          time="Il y a 1j"
          unread={true}
        />
        <NotificationCard
          icon={<Info className="text-info" />}
          title="Nouveau doctorant ajouté"
          message="Un nouveau doctorant a été ajouté à vos encadrements: Karim Jebali"
          time="Il y a 3j"
          unread={false}
        />
      </div>
    </div>
  );
}

function NotificationCard({ icon, title, message, time, unread }: any) {
  return (
    <Card className={unread ? 'border-l-4 border-l-accent-blue' : ''}>
      <CardContent className="p-6">
        <div className="flex items-start gap-4">
          <div className="flex-shrink-0 mt-1">{icon}</div>
          <div className="flex-1">
            <div className="flex items-start justify-between mb-2">
              <h4 className="font-bold text-navy">{title}</h4>
              {unread && <Badge variant="info">Nouveau</Badge>}
            </div>
            <p className="text-sm text-text-secondary mb-2">{message}</p>
            <div className="text-xs text-text-muted">{time}</div>
          </div>
        </div>
      </CardContent>
    </Card>
  );
}
