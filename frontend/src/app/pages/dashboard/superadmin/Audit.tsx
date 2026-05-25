import { useMemo } from 'react';
import { Card, CardContent } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { Loader2 } from 'lucide-react';
import { useGetAuditLogsQuery, type AuditLogDto } from '../../../api/auditLogsApi';

const DAYS_BACK = 15;

function getFromUtc(): string {
  const d = new Date();
  d.setUTCDate(d.getUTCDate() - DAYS_BACK);
  // Date-only format avoids colon URL-encoding issues with .NET DateTime model binder
  return d.toISOString().slice(0, 10); // "YYYY-MM-DD"
}

type BadgeVariant = 'success' | 'info' | 'error' | 'default';

function getActionVariant(action: string): BadgeVariant {
  switch (action) {
    case 'CREATE':
    case 'VALIDATE':
      return 'success';
    case 'UPDATE':
    case 'ROLE_CHANGE':
      return 'info';
    case 'DELETE':
    case 'REJECT':
      return 'error';
    default:
      return 'default';
  }
}

function formatTimestamp(iso: string): string {
  return new Date(iso).toLocaleString('fr-FR', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
  });
}

export default function SuperAdminAudit() {
  const fromUtc = useMemo(() => getFromUtc(), []);
  const { data: logs = [], isLoading, isError } = useGetAuditLogsQuery(
    { fromUtc },
    { refetchOnMountOrArgChange: true },
  );

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-navy dark:text-white mb-1">Journal d'Audit</h1>
        <p className="text-sm text-text-secondary">Activité des {DAYS_BACK} derniers jours</p>
      </div>

      <Card>
        <CardContent className="p-0">
          {isLoading ? (
            <div className="flex items-center justify-center py-16">
              <Loader2 size={28} className="animate-spin text-accent-blue" />
            </div>
          ) : isError ? (
            <div className="py-12 text-center text-error text-sm">
              Impossible de charger le journal d'audit.
            </div>
          ) : logs.length === 0 ? (
            <div className="py-12 text-center text-text-secondary text-sm">
              Aucune activité sur les {DAYS_BACK} derniers jours.
            </div>
          ) : (
            <table className="w-full">
              <thead className="bg-light-gray dark:bg-input-background">
                <tr>
                  <th className="px-6 py-3 text-left text-sm font-medium">Date/Heure</th>
                  <th className="px-6 py-3 text-left text-sm font-medium">Utilisateur</th>
                  <th className="px-6 py-3 text-left text-sm font-medium">Action</th>
                  <th className="px-6 py-3 text-left text-sm font-medium">Ressource</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-surface-border">
                {logs.map((log) => (
                  <AuditRow key={log.id} log={log} />
                ))}
              </tbody>
            </table>
          )}
        </CardContent>
      </Card>
    </div>
  );
}

function AuditRow({ log }: { log: AuditLogDto }) {
  return (
    <tr className="hover:bg-light-gray/50 dark:hover:bg-input-background/30 transition-colors">
      <td className="px-6 py-4 text-sm text-text-secondary whitespace-nowrap">
        {formatTimestamp(log.timestamp)}
      </td>
      <td className="px-6 py-4 text-sm font-medium text-navy dark:text-white">
        {log.actorName ?? log.actorId}
      </td>
      <td className="px-6 py-4">
        <Badge variant={getActionVariant(log.action)}>{log.action}</Badge>
      </td>
      <td className="px-6 py-4 text-sm text-text-secondary">{log.resource}</td>
    </tr>
  );
}
