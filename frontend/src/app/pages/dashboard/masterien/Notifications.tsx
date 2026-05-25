import { useState } from 'react';
import type { ComponentType } from 'react';
import { Card, CardContent } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { Button } from '../../../components/ui/Button';
import { Bell, FileText, CheckCircle, XCircle, BookOpen, X, Check } from 'lucide-react';
import { toast } from 'sonner';
import { clsx } from 'clsx';

type NotifType = 'PUB_SUBMITTED' | 'PUB_APPROVED' | 'PUB_REJECTED' | 'MEMOIR_UPDATE' | 'GENERAL';

interface Notification {
  id: string;
  type: NotifType;
  title: string;
  message: string;
  time: string;
  read: boolean;
}

const initialNotifs: Notification[] = [
  { id: '1', type: 'PUB_SUBMITTED', title: 'Publication soumise avec succès', message: '"Système de recommandation basé sur le Deep Learning" a été soumise pour révision. Vous serez notifié dès qu\'une décision sera prise.', time: 'Il y a 3 heures', read: false },
  { id: '2', type: 'MEMOIR_UPDATE', title: 'Réunion avec votre encadrant', message: 'Dr. Ahmed Ben Salem a planifié une réunion de suivi de mémoire pour le 30 mai 2026 à 14h00. Préparez votre avancement.', time: 'Hier à 10h00', read: false },
  { id: '3', type: 'GENERAL', title: 'Formulaire de rapport à compléter', message: 'Le formulaire de rapport de mémoire de fin d\'études est disponible. Date limite : 30 juin 2026.', time: 'Il y a 2 jours', read: false },
  { id: '4', type: 'PUB_APPROVED', title: 'Félicitations — Publication approuvée', message: '"État de l\'art : Filtrage collaboratif" a été approuvée. Elle est maintenant visible sur le portail du laboratoire.', time: 'Il y a 5 jours', read: true },
  { id: '5', type: 'GENERAL', title: 'Bienvenue sur le portail LIMTIC', message: 'Votre compte Mastérien a été activé. Découvrez les publications du laboratoire, l\'équipe de recherche et les événements scientifiques.', time: 'Il y a 2 mois', read: true },
];

const typeConfig: Record<NotifType, { icon: ComponentType<{ size?: number; className?: string }>; color: string; bgColor: string; label: string; variant: any }> = {
  PUB_SUBMITTED: { icon: FileText, color: 'text-accent-blue', bgColor: 'bg-accent-blue/10', label: 'Publication', variant: 'info' },
  PUB_APPROVED: { icon: CheckCircle, color: 'text-success', bgColor: 'bg-success/10', label: 'Approuvée', variant: 'success' },
  PUB_REJECTED: { icon: XCircle, color: 'text-error', bgColor: 'bg-error/10', label: 'Rejetée', variant: 'default' },
  MEMOIR_UPDATE: { icon: BookOpen, color: 'text-warning', bgColor: 'bg-warning/10', label: 'Mémoire', variant: 'warning' },
  GENERAL: { icon: Bell, color: 'text-text-muted', bgColor: 'bg-light-gray', label: 'Général', variant: 'default' },
};

export default function MasterienNotifications() {
  const [notifications, setNotifications] = useState<Notification[]>(initialNotifs);
  const [filter, setFilter] = useState<'all' | 'unread'>('all');

  const unreadCount = notifications.filter(n => !n.read).length;
  const filtered = filter === 'unread' ? notifications.filter(n => !n.read) : notifications;

  function markRead(id: string) {
    setNotifications(prev => prev.map(n => n.id === id ? { ...n, read: true } : n));
  }

  function markAllRead() {
    setNotifications(prev => prev.map(n => ({ ...n, read: true })));
    toast.success('Toutes les notifications marquées comme lues');
  }

  function deleteNotif(id: string) {
    setNotifications(prev => prev.filter(n => n.id !== id));
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <h1 className="text-3xl font-bold text-navy dark:text-white">Mes notifications</h1>
          {unreadCount > 0 && <Badge variant="default" className="!bg-error !text-white">{unreadCount}</Badge>}
        </div>
        {unreadCount > 0 && (
          <Button variant="outlined" onClick={markAllRead} className="flex items-center gap-2">
            <Check size={16} /> Tout marquer comme lu
          </Button>
        )}
      </div>

      <div className="flex gap-2">
        {[{ key: 'all', label: `Toutes (${notifications.length})` }, { key: 'unread', label: `Non lues (${unreadCount})` }].map(tab => (
          <button
            key={tab.key}
            onClick={() => setFilter(tab.key as 'all' | 'unread')}
            className={clsx(
              'px-4 py-2 rounded-full text-sm font-medium transition-colors',
              filter === tab.key ? 'bg-navy text-white' : 'bg-light-gray dark:bg-muted text-text-secondary hover:bg-surface-border'
            )}
          >
            {tab.label}
          </button>
        ))}
      </div>

      <div className="space-y-3">
        {filtered.length === 0 && (
          <Card><CardContent className="py-16 flex flex-col items-center gap-3 text-text-muted"><Bell size={40} className="opacity-30" /><p>Aucune notification</p></CardContent></Card>
        )}
        {filtered.map(n => {
          const cfg = typeConfig[n.type];
          const Icon = cfg.icon;
          return (
            <Card key={n.id} className={clsx('transition-all border-l-4', !n.read ? 'border-l-accent-blue' : 'border-l-transparent')}>
              <CardContent className="p-5">
                <div className="flex items-start gap-4">
                  <div className={clsx('w-10 h-10 rounded-xl flex items-center justify-center flex-shrink-0', cfg.bgColor)}>
                    <Icon className={cfg.color} size={18} />
                  </div>
                  <div className="flex-1 min-w-0">
                    <div className="flex items-start justify-between gap-2">
                      <div>
                        <div className="flex items-center gap-2 mb-1">
                          <Badge variant={cfg.variant} className="text-xs">{cfg.label}</Badge>
                          {!n.read && <div className="w-2 h-2 rounded-full bg-accent-blue" />}
                        </div>
                        <h3 className="font-semibold text-navy dark:text-white text-sm">{n.title}</h3>
                        <p className="text-text-secondary text-sm mt-1">{n.message}</p>
                      </div>
                      <button onClick={() => deleteNotif(n.id)} className="p-1 hover:bg-light-gray dark:hover:bg-muted rounded text-text-muted flex-shrink-0">
                        <X size={16} />
                      </button>
                    </div>
                    <div className="flex items-center justify-between mt-3">
                      <span className="text-xs text-text-muted">{n.time}</span>
                      {!n.read && (
                        <button onClick={() => markRead(n.id)} className="flex items-center gap-1 text-xs text-text-muted hover:text-navy dark:hover:text-white">
                          <CheckCircle size={14} /> Marquer lu
                        </button>
                      )}
                    </div>
                  </div>
                </div>
              </CardContent>
            </Card>
          );
        })}
      </div>
    </div>
  );
}
