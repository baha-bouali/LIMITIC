import { useState } from 'react';
import { Card, CardContent, CardHeader } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { Button } from '../../../components/ui/Button';
import { SearchFilter } from '../../../components/shared/SearchFilter';
import { ConfirmDialog } from '../../../components/shared/ConfirmDialog';
import { Calendar, Pencil, Trash2, X, Plus, Eye, AlertCircle, Loader } from 'lucide-react';
import { toast } from 'sonner';
import { useGetEventsQuery, useCreateEventMutation, useUpdateEventMutation, useDeleteEventMutation } from '@/app/api';

type EventStatus = 'A_VENIR' | 'EN_COURS' | 'PASSE';
type EventType = 'SEMINAIRE' | 'CONFERENCE' | 'WORKSHOP' | 'SOUTENANCE' | 'JOURNEE_PORTES_OUVERTES';

interface Speaker { id?: string; name: string; email?: string; institution?: string; role?: string; subject?: string }
interface Event { id: string; type: EventType; title: string; date: string; endDate?: string; location: string; status: EventStatus; description?: string; speakers?: Speaker[] }

function EventFormModal({ event, onClose }: { event?: Event | null; onClose: () => void }) {
  const [createEvent] = useCreateEventMutation();
  const [updateEvent] = useUpdateEventMutation();
  const [formData, setFormData] = useState(() => ({ type: (event?.type || 'SEMINAIRE') as EventType, title: event?.title || '', date: event?.date ? new Date(event.date).toISOString().split('T')[0] : '', endDate: event?.endDate ? new Date(event.endDate).toISOString().split('T')[0] : '', location: event?.location || '', description: event?.description || '', status: (event?.status || 'A_VENIR') as EventStatus, speakers: event?.speakers || ([] as Speaker[]) }));

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const speakersPayload = formData.speakers.map(s => { const [firstName, ...rest] = s.name.split(' '); return { firstName, lastName: rest.join(' '), email: s.email }; });
      const payload: any = { type: formData.type, title: formData.title, startDate: formData.date, endDate: formData.endDate || formData.date, location: formData.location, description: formData.description, speakers: speakersPayload };
      if (event?.id) { await updateEvent({ id: event.id, data: payload }).unwrap(); toast.success('Événement modifié'); } else { await createEvent(payload).unwrap(); toast.success('Événement créé'); }
      onClose();
    } catch (err: any) {
      toast.error(err?.data?.message || 'Erreur');
    }
  };

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
      <div className="bg-white dark:bg-card rounded-2xl max-w-2xl w-full p-6">
        <div className="flex items-center justify-between mb-4"><h3 className="text-lg font-bold">{event ? 'Modifier' : 'Créer'} un événement</h3><button onClick={onClose} className="p-2"><X /></button></div>
        <form onSubmit={handleSubmit} className="space-y-4">
          <input className="w-full border rounded px-3 py-2" value={formData.title} onChange={e => setFormData({ ...formData, title: e.target.value })} placeholder="Titre" required />
          <div className="grid grid-cols-2 gap-3">
            <input type="date" className="w-full border rounded px-3 py-2" value={formData.date} onChange={e => setFormData({ ...formData, date: e.target.value })} required />
            <input className="w-full border rounded px-3 py-2" value={formData.location} onChange={e => setFormData({ ...formData, location: e.target.value })} placeholder="Lieu" required />
          </div>
          <textarea className="w-full border rounded px-3 py-2" value={formData.description} onChange={e => setFormData({ ...formData, description: e.target.value })} rows={4} placeholder="Description" />
          <div className="flex justify-end gap-2"><Button variant="outlined" onClick={onClose} type="button">Annuler</Button><Button type="submit">Enregistrer</Button></div>
        </form>
      </div>
    </div>
  );
}

export default function AdminEvents() {
  const [searchQuery, setSearchQuery] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [editingEvent, setEditingEvent] = useState<Event | null>(null);
  const [deleteConfirm, setDeleteConfirm] = useState<Event | null>(null);
  const [activeFilters, setActiveFilters] = useState<Record<string, string[]>>({});

  const { data: eventsResponse, isLoading, error } = useGetEventsQuery({ page: 1, limit: 100, status: activeFilters.status?.[0], type: activeFilters.type?.[0], q: searchQuery });
  const [deleteEvent] = useDeleteEventMutation();
  const events = eventsResponse?.items || [];

  const filterGroups = [ { id: 'status', label: 'Statut', options: [ { id: 'avenir', label: 'À venir', value: 'A_VENIR' }, { id: 'encours', label: 'En cours', value: 'EN_COURS' }, { id: 'passe', label: 'Passé', value: 'PASSE' } ] }, { id: 'type', label: 'Type', options: [ { id: 'seminaire', label: 'Séminaire', value: 'SEMINAIRE' }, { id: 'atelier', label: 'Workshop', value: 'WORKSHOP' }, { id: 'conference', label: 'Conférence', value: 'CONFERENCE' } ] } ];

  const filtered = events.filter(ev => { const q = searchQuery.trim().toLowerCase(); const matchQ = !q || ev.title.toLowerCase().includes(q) || ev.location.toLowerCase().includes(q); const statusF = activeFilters.status?.[0]; const typeF = activeFilters.type?.[0]; return matchQ && (!statusF || ev.status === statusF) && (!typeF || ev.type === typeF); });

  const handleDelete = async (e: Event) => { try { await deleteEvent(e.id).unwrap(); toast.success('Événement supprimé'); setDeleteConfirm(null); } catch (err: any) { toast.error(err?.data?.message || 'Erreur'); } };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold">Gestion des Événements</h1>
          <p className="text-text-secondary">Créer, modifier et supprimer les événements</p>
        </div>
        <Button onClick={() => { setEditingEvent(null); setShowForm(true); }}><Plus /> Créer</Button>
      </div>

      <SearchFilter onSearchChange={setSearchQuery} filterGroups={filterGroups} onFilterChange={setActiveFilters} searchPlaceholder="Rechercher un événement..." />

      {isLoading ? (<Card><CardContent className="p-12 text-center"><Loader size={48} className="mx-auto" /></CardContent></Card>) : error ? (<Card><CardContent className="p-6 text-center"><AlertCircle size={48} className="mx-auto" /><p>Erreur lors du chargement</p></CardContent></Card>) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {filtered.map(ev => (
            <Card key={ev.id}>
              <CardHeader>
                <div className="flex items-center justify-between">
                  <div>
                    <h3 className="font-bold text-lg">{ev.title}</h3>
                    <div className="text-sm text-text-secondary">{ev.location} • {new Date(ev.date).toLocaleDateString('fr-FR')}</div>
                  </div>
                  <div className="space-y-1 text-right"><Badge>{ev.status}</Badge><div className="text-xs text-text-secondary">{ev.type}</div></div>
                </div>
              </CardHeader>
              <CardContent>
                <p className="text-sm text-text-secondary line-clamp-3">{ev.description}</p>
                <div className="flex gap-2 mt-4"><Button variant="outlined" onClick={() => { setEditingEvent(ev); setShowForm(true); }}><Pencil /> Modifier</Button><Button variant="outlined" onClick={() => setDeleteConfirm(ev)} className="text-error"><Trash2 /></Button></div>
              </CardContent>
            </Card>
          ))}
        </div>
      )}

      {showForm && <EventFormModal event={editingEvent} onClose={() => { setShowForm(false); setEditingEvent(null); }} />}
      {deleteConfirm && <ConfirmDialog isOpen onClose={() => setDeleteConfirm(null)} onConfirm={() => handleDelete(deleteConfirm)} title="Supprimer" description={`Supprimer "${deleteConfirm.title}" ?`} confirmText="Supprimer" />}
    </div>
  );
}
