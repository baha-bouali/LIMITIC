import { useState } from 'react';
import type { ComponentType } from 'react';
import { Card, CardContent } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { Button } from '../../../components/ui/Button';
import { Bell, FileText, Calendar, CheckCircle, X, Check } from 'lucide-react';
import { toast } from 'sonner';
import { clsx } from 'clsx';

type NotifType = 'PUBLICATION_PENDING' | 'PUBLICATION_APPROVED' | 'EVENT_REMINDER' | 'SYSTEM';

interface Notification {
  id: string;
  type: NotifType;
  title: string;
  message: string;
  time: string;
  read: boolean;
}

const initialNotifs: Notification[] = [
  { id: '1', type: 'PUBLICATION_PENDING', title: 'Publication à approuver', message: 'Mohamed Najjar a soumis "Blockchain Security Analysis for Healthcare Data". En attente de votre validation.', time: 'Il y a 2 heures', read: false },
  { id: '2', type: 'PUBLICATION_PENDING', title: 'Nouvelle soumission', message: 'Sarah Trabelsi a soumis "Deep Learning for MRI Segmentation" pour révision.', time: 'Il y a 5 heures', read: false },
  { id: '3', type: 'EVENT_REMINDER', title: 'Séminaire dans 3 jours', message: 'Le séminaire "IA & Santé Numérique" est prévu pour le 26 mai 2026. Préparez les communications nécessaires.', time: 'Hier à 09h00', read: false },
  { id: '4', type: 'PUBLICATION_APPROVED', title: 'Publication approuvée', message: 'Vous avez approuvé "Deep Learning for Medical Imaging Diagnosis" de Dr. Ahmed Ben Salem.', time: 'Il y a 2 jours', read: true },
  { id: '5', type: 'SYSTEM', title: 'Rappel de tâche', message: '3 publications sont en attente de révision depuis plus de 48h. Veuillez les traiter rapidement.', time: 'Il y a 3 jours', read: true },
  { id: '6', type: 'EVENT_REMINDER', title: 'Soutenance à planifier', message: 'La soutenance de Sarah Trabelsi est prévue en juin. Confirmez les détails logistiques avec l\'équipe.', time: 'Il y a 4 jours', read: true },
];

const typeConfig: Record<NotifType, { icon: ComponentType<{ size?: number; className?: string }>; color: string; bgColor: string; label: string; variant: any }> = {
  PUBLICATION_PENDING: { icon: FileText, color: 'text-warning', bgColor: 'bg-warning/10', label: 'À valider', variant: 'warning' },
  PUBLICATION_APPROVED: { icon: CheckCircle, color: 'text-success', bgColor: 'bg-success/10', label: 'Approuvé', variant: 'success' },
  EVENT_REMINDER: { icon: Calendar, color: 'text-accent-blue', bgColor: 'bg-accent-blue/10', label: 'Événement', variant: 'info' },
  SYSTEM: { icon: Bell, color: 'text-text-muted', bgColor: 'bg-light-gray', label: 'Système', variant: 'default' },
};

export default function AdminNotifications() {
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
          <h1 className="text-3xl font-bold text-navy dark:text-white">Notifications</h1>
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
            <Card key={n.id} className={clsx('transition-all border-l-4', !n.read ? 'border-l-warning' : 'border-l-transparent')}>
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
                          {!n.read && <div className="w-2 h-2 rounded-full bg-warning" />}
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
