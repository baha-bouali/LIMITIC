import { Card, CardContent } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';

export default function SuperAdminAudit() {
  return (
    <div className="space-y-6">
      <h1 className="text-3xl font-bold text-navy">Journal d'Audit</h1>
      <Card>
        <CardContent className="p-0">
          <table className="w-full">
            <thead className="bg-light-gray">
              <tr>
                <th className="px-6 py-3 text-left text-sm font-medium">Date/Heure</th>
                <th className="px-6 py-3 text-left text-sm font-medium">Utilisateur</th>
                <th className="px-6 py-3 text-left text-sm font-medium">Action</th>
                <th className="px-6 py-3 text-left text-sm font-medium">Ressource</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-surface-border">
              <AuditRow time="2026-05-22 14:30" user="admin@limtic.tn" action="CREATE" resource="Publication #123" />
              <AuditRow time="2026-05-22 12:15" user="chercheur@limtic.tn" action="UPDATE" resource="Event #45" />
              <AuditRow time="2026-05-21 16:45" user="admin@limtic.tn" action="DELETE" resource="User #67" />
            </tbody>
          </table>
        </CardContent>
      </Card>
    </div>
  );
}

function AuditRow({ time, user, action, resource }: any) {
  const getActionVariant = (a: string) => {
    if (a === 'CREATE') return 'success';
    if (a === 'UPDATE') return 'info';
    if (a === 'DELETE') return 'error';
    return 'default';
  };

  return (
    <tr>
      <td className="px-6 py-4 text-sm">{time}</td>
      <td className="px-6 py-4 text-sm">{user}</td>
      <td className="px-6 py-4"><Badge variant={getActionVariant(action)}>{action}</Badge></td>
      <td className="px-6 py-4 text-sm">{resource}</td>
    </tr>
  );
}
