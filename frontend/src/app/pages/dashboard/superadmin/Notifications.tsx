import { useState } from 'react';
import type { ComponentType } from 'react';
import { Card, CardContent } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { Button } from '../../../components/ui/Button';
import { Bell, FileText, UserPlus, Calendar, AlertTriangle, CheckCircle, X, Check, Filter } from 'lucide-react';
import { toast } from 'sonner';
import { clsx } from 'clsx';

type NotifType = 'PUBLICATION_PENDING' | 'NEW_USER' | 'EVENT_UPCOMING' | 'SYSTEM' | 'PUBLICATION_REJECTED';

interface Notification {
  id: string;
  type: NotifType;
  title: string;
  message: string;
  time: string;
  read: boolean;
  actionLabel?: string;
  actionHref?: string;
}

const initialNotifs: Notification[] = [
  {
    id: '1',
    type: 'PUBLICATION_PENDING',
    title: 'Publication en attente d\'approbation',
    message: 'Mohamed Najjar a soumis "Blockchain Security Analysis for Healthcare Data" pour révision.',
    time: 'Il y a 2 heures',
    read: false,
    actionLabel: 'Réviser',
  },
  {
    id: '2',
    type: 'PUBLICATION_PENDING',
    title: 'Nouvelle publication soumise',
    message: 'Sarah Trabelsi a soumis "Deep Learning for MRI Segmentation" pour approbation.',
    time: 'Il y a 5 heures',
    read: false,
    actionLabel: 'Réviser',
  },
  {
    id: '3',
    type: 'NEW_USER',
    title: 'Nouvel utilisateur enregistré',
    message: 'Ines Hamdi (Mastérienne) a rejoint le laboratoire. Vérifiez et activez son compte.',
    time: 'Hier à 14h30',
    read: false,
    actionLabel: 'Voir profil',
  },
  {
    id: '4',
    type: 'EVENT_UPCOMING',
    title: 'Événement dans 3 jours',
    message: 'Le séminaire "IA & Santé Numérique" est prévu pour le 26 mai 2026. Préparez les communications.',
    time: 'Hier à 09h00',
    read: true,
    actionLabel: 'Voir événement',
  },
  {
    id: '5',
    type: 'SYSTEM',
    title: 'Sauvegarde système effectuée',
    message: 'La sauvegarde automatique de la base de données a été réalisée avec succès (23 mai 2026, 03h00).',
    time: 'Il y a 2 jours',
    read: true,
  },
  {
    id: '6',
    type: 'PUBLICATION_REJECTED',
    title: 'Publication rejetée — retour auteur',
    message: 'Karim Slimi a été notifié du rejet de "IoT Security in Smart Cities" avec commentaires.',
    time: 'Il y a 3 jours',
    read: true,
  },
  {
    id: '7',
    type: 'NEW_USER',
    title: 'Inscription en attente',
    message: 'Un nouvel utilisateur (role: Doctorant) attend validation de son compte depuis 48h.',
    time: 'Il y a 4 jours',
    read: true,
    actionLabel: 'Gérer',
  },
  {
    id: '8',
    type: 'SYSTEM',
    title: 'Mise à jour disponible',
    message: 'Une nouvelle version du système de gestion LIMTIC est disponible. Planifiez la maintenance.',
    time: 'Il y a 5 jours',
    read: true,
  },
];

const typeConfig: Record<NotifType, { icon: ComponentType<{ size?: number; className?: string }>; color: string; bgColor: string; label: string }> = {
  PUBLICATION_PENDING: { icon: FileText, color: 'text-accent-blue', bgColor: 'bg-accent-blue/10', label: 'Publication' },
  NEW_USER: { icon: UserPlus, color: 'text-teal', bgColor: 'bg-teal/10', label: 'Utilisateur' },
  EVENT_UPCOMING: { icon: Calendar, color: 'text-warning', bgColor: 'bg-warning/10', label: 'Événement' },
  SYSTEM: { icon: AlertTriangle, color: 'text-text-muted', bgColor: 'bg-light-gray', label: 'Système' },
  PUBLICATION_REJECTED: { icon: X, color: 'text-error', bgColor: 'bg-error/10', label: 'Rejet' },
};

export default function SuperAdminNotifications() {
  const [notifications, setNotifications] = useState<Notification[]>(initialNotifs);
  const [filter, setFilter] = useState<'all' | 'unread' | NotifType>('all');

  const unreadCount = notifications.filter(n => !n.read).length;

  const filtered = notifications.filter(n => {
    if (filter === 'all') return true;
    if (filter === 'unread') return !n.read;
    return n.type === filter;
  });

  function markRead(id: string) {
    setNotifications(prev => prev.map(n => n.id === id ? { ...n, read: true } : n));
  }

  function markAllRead() {
    setNotifications(prev => prev.map(n => ({ ...n, read: true })));
    toast.success('Toutes les notifications marquées comme lues');
  }

  function deleteNotif(id: string) {
    setNotifications(prev => prev.filter(n => n.id !== id));
    toast.success('Notification supprimée');
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <h1 className="text-3xl font-bold text-navy dark:text-white">Notifications</h1>
          {unreadCount > 0 && (
            <Badge variant="default" className="!bg-error !text-white">{unreadCount} nouvelles</Badge>
          )}
        </div>
        {unreadCount > 0 && (
          <Button variant="outlined" onClick={markAllRead} className="flex items-center gap-2">
            <Check size={16} /> Tout marquer comme lu
          </Button>
        )}
      </div>

      {/* Filter tabs */}
      <div className="flex flex-wrap gap-2">
        {[
          { key: 'all', label: 'Toutes', count: notifications.length },
          { key: 'unread', label: 'Non lues', count: unreadCount },
          { key: 'PUBLICATION_PENDING', label: 'Publications', count: notifications.filter(n => n.type === 'PUBLICATION_PENDING').length },
          { key: 'NEW_USER', label: 'Utilisateurs', count: notifications.filter(n => n.type === 'NEW_USER').length },
          { key: 'EVENT_UPCOMING', label: 'Événements', count: notifications.filter(n => n.type === 'EVENT_UPCOMING').length },
          { key: 'SYSTEM', label: 'Système', count: notifications.filter(n => n.type === 'SYSTEM').length },
        ].map(tab => (
          <button
            key={tab.key}
            onClick={() => setFilter(tab.key as any)}
            className={clsx(
              'flex items-center gap-2 px-4 py-2 rounded-full text-sm font-medium transition-colors',
              filter === tab.key
                ? 'bg-navy text-white'
                : 'bg-light-gray dark:bg-muted text-text-secondary hover:bg-surface-border'
            )}
          >
            {tab.label}
            {tab.count > 0 && (
              <span className={clsx(
                'text-xs rounded-full px-1.5 py-0.5',
                filter === tab.key ? 'bg-white/20 text-white' : 'bg-surface-border text-text-muted'
              )}>
                {tab.count}
              </span>
            )}
          </button>
        ))}
      </div>

      {/* Notifications list */}
      <div className="space-y-3">
        {filtered.length === 0 && (
          <Card>
            <CardContent className="py-16 flex flex-col items-center gap-3 text-text-muted">
              <Bell size={40} className="opacity-30" />
              <p>Aucune notification</p>
            </CardContent>
          </Card>
        )}
        {filtered.map(n => {
          const cfg = typeConfig[n.type];
          const Icon = cfg.icon;
          return (
            <Card
              key={n.id}
              className={clsx(
                'transition-all border-l-4',
                !n.read ? 'border-l-accent-blue' : 'border-l-transparent'
              )}
            >
              <CardContent className="p-5">
                <div className="flex items-start gap-4">
                  <div className={clsx('w-11 h-11 rounded-xl flex items-center justify-center flex-shrink-0', cfg.bgColor)}>
                    <Icon className={cfg.color} size={20} />
                  </div>
                  <div className="flex-1 min-w-0">
                    <div className="flex items-start justify-between gap-3">
                      <div>
                        <div className="flex items-center gap-2 mb-1">
                          <Badge variant="info" className="text-xs">{cfg.label}</Badge>
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
                      <div className="flex items-center gap-2">
                        {n.actionLabel && (
                          <button className="text-xs text-accent-blue hover:underline font-medium">{n.actionLabel}</button>
                        )}
                        {!n.read && (
                          <button
                            onClick={() => markRead(n.id)}
                            className="flex items-center gap-1 text-xs text-text-muted hover:text-navy dark:hover:text-white"
                          >
                            <CheckCircle size={14} /> Marquer lu
                          </button>
                        )}
                      </div>
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
