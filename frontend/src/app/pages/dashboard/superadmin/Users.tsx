import { useState } from 'react';
import type { ComponentType } from 'react';
import { Card, CardContent } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { Button } from '../../../components/ui/Button';
import { SearchFilter } from '../../../components/shared/SearchFilter';
import { ConfirmDialog } from '../../../components/shared/ConfirmDialog';
import { Plus, Pencil, Trash2, ToggleLeft, ToggleRight, Users, ShieldCheck, UserCheck, GraduationCap, BookOpen, UserCircle, Eye, EyeOff } from 'lucide-react';
import { toast } from 'sonner';
import { clsx } from 'clsx';
import { useLanguage } from '../../../contexts/LanguageContext';

type UserRole = 'SUPER_ADMIN' | 'ADMIN' | 'CHERCHEUR' | 'DOCTORANT' | 'MASTERIEN' | 'VISITOR';

interface AppUser {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: UserRole;
  active: boolean;
  createdAt: string;
  lastLogin?: string;
  specialization?: string;
  researchAxis?: string;
  academicYear?: string;
  supervisor?: string;
  thesisSubject?: string;
  projectSubject?: string;
  orcid?: string;
  googleScholar?: string;
  researchGate?: string;
  linkedin?: string;
  website?: string;
}

const initialUsers: AppUser[] = [
  { id: '1', firstName: 'Super', lastName: 'Admin', email: 'superadmin@limtic.tn', role: 'SUPER_ADMIN', active: true, createdAt: '2024-01-10', lastLogin: '2026-05-23' },
  { id: '2', firstName: 'Karim', lastName: 'Mansouri', email: 'admin@limtic.tn', role: 'ADMIN', active: true, createdAt: '2024-02-15', lastLogin: '2026-05-20' },
  { id: '3', firstName: 'Ahmed', lastName: 'Ben Salem', email: 'ahmed.bensalem@limtic.tn', role: 'CHERCHEUR', active: true, createdAt: '2023-09-01', lastLogin: '2026-05-22', specialization: 'Intelligence Artificielle', researchAxis: 'IA & Apprentissage Automatique', orcid: '0000-0002-1234-5678' },
  { id: '4', firstName: 'Fatma', lastName: 'Gharbi', email: 'fatma.gharbi@limtic.tn', role: 'CHERCHEUR', active: true, createdAt: '2023-09-01', lastLogin: '2026-05-21', specialization: 'Sécurité Informatique', researchAxis: 'Cybersécurité & Cryptographie' },
  { id: '5', firstName: 'Mohamed', lastName: 'Mezghani', email: 'med.mezghani@limtic.tn', role: 'CHERCHEUR', active: true, createdAt: '2023-09-01', lastLogin: '2026-05-18', specialization: 'Réseaux et IoT', researchAxis: 'Systèmes Distribués & IoT' },
  { id: '6', firstName: 'Sarah', lastName: 'Trabelsi', email: 'sarah.trabelsi@limtic.tn', role: 'DOCTORANT', active: true, createdAt: '2024-09-15', lastLogin: '2026-05-23', specialization: 'Deep Learning Médical', researchAxis: 'IA & Apprentissage Automatique', academicYear: '2024', supervisor: 'Ahmed Ben Salem', thesisSubject: 'Apprentissage profond pour le diagnostic médical' },
  { id: '7', firstName: 'Mohamed', lastName: 'Najjar', email: 'med.najjar@limtic.tn', role: 'DOCTORANT', active: true, createdAt: '2024-09-15', lastLogin: '2026-05-19', specialization: 'Blockchain Healthcare', researchAxis: 'Cybersécurité & Cryptographie', academicYear: '2023', supervisor: 'Mohamed Mezghani', thesisSubject: 'Blockchain pour la sécurité des données médicales' },
  { id: '8', firstName: 'Ines', lastName: 'Hamdi', email: 'ines.hamdi@limtic.tn', role: 'MASTERIEN', active: true, createdAt: '2025-09-01', lastLogin: '2026-05-22', specialization: 'Système de recommandation', academicYear: '2025-2026', supervisor: 'Ahmed Ben Salem', projectSubject: 'Système de recommandation basé sur l\'IA' },
  { id: '9', firstName: 'Karim', lastName: 'Slimi', email: 'karim.slimi@limtic.tn', role: 'MASTERIEN', active: false, createdAt: '2025-09-01', lastLogin: '2026-04-10', specialization: 'IoT Sécurité', academicYear: '2025-2026', projectSubject: 'Sécurisation des réseaux IoT' },
];

const roleConfig: Record<UserRole, { label: string; variant: any; icon: ComponentType<{ size?: number; className?: string }> }> = {
  SUPER_ADMIN: { label: 'SuperAdmin', variant: 'default', icon: ShieldCheck },
  ADMIN: { label: 'Admin', variant: 'info', icon: UserCheck },
  CHERCHEUR: { label: 'Chercheur', variant: 'success', icon: Users },
  DOCTORANT: { label: 'Doctorant', variant: 'warning', icon: GraduationCap },
  MASTERIEN: { label: 'Mastérien', variant: 'default', icon: BookOpen },
  VISITOR: { label: 'Visiteur', variant: 'default', icon: UserCircle },
};

const researchAxes = [
  'IA & Apprentissage Automatique',
  'Cybersécurité & Cryptographie',
  'Systèmes Distribués & IoT',
  'Blockchain & Technologies Décentralisées',
];

export default function SuperAdminUsers() {
  const { t } = useLanguage();
  const [users, setUsers] = useState<AppUser[]>(initialUsers);
  const [searchQuery, setSearchQuery] = useState('');
  const [activeFilters, setActiveFilters] = useState<Record<string, string | string[]>>({});
  const [showStep1, setShowStep1] = useState(false);
  const [showStep2, setShowStep2] = useState(false);
  const [createdUserId, setCreatedUserId] = useState<string | null>(null);
  const [editingUser, setEditingUser] = useState<AppUser | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<AppUser | null>(null);

  // Step 1 form (base user)
  const [step1Form, setStep1Form] = useState({ firstName: '', lastName: '', email: '', password: '' });
  const [showPass, setShowPass] = useState(false);

  // Step 2 form (role-specific data)
  const [step2Form, setStep2Form] = useState({
    role: 'VISITOR' as UserRole,
    specialization: '',
    researchAxis: '',
    academicYear: '',
    supervisor: '',
    thesisSubject: '',
    projectSubject: '',
    orcid: '',
    googleScholar: '',
    researchGate: '',
    linkedin: '',
    website: '',
  });

  const filterGroups = [
    {
      id: 'role', label: t('users.role'), options: [
        { id: 'sa', label: 'SuperAdmin', value: 'SUPER_ADMIN' },
        { id: 'ad', label: 'Admin', value: 'ADMIN' },
        { id: 'ch', label: t('role.researcher'), value: 'CHERCHEUR' },
        { id: 'do', label: t('role.phd'), value: 'DOCTORANT' },
        { id: 'ma', label: t('role.master'), value: 'MASTERIEN' },
        { id: 'vi', label: 'Visiteur', value: 'VISITOR' },
      ]
    },
    {
      id: 'status', label: t('users.status'), options: [
        { id: 'active', label: t('users.active'), value: 'active' },
        { id: 'inactive', label: t('users.inactive'), value: 'inactive' },
      ]
    },
  ];

  const filtered = users.filter(u => {
    const q = searchQuery.toLowerCase();
    const matchQ = !q || `${u.firstName} ${u.lastName}`.toLowerCase().includes(q) || u.email.toLowerCase().includes(q);
    const roleFilter = activeFilters.role as string;
    const matchRole = !roleFilter || u.role === roleFilter;
    const statusFilter = activeFilters.status as string;
    const matchStatus = !statusFilter || (statusFilter === 'active' ? u.active : !u.active);
    return matchQ && matchRole && matchStatus;
  });

  const stats = {
    total: users.length,
    chercheurs: users.filter(u => u.role === 'CHERCHEUR').length,
    doctorants: users.filter(u => u.role === 'DOCTORANT').length,
    masteriens: users.filter(u => u.role === 'MASTERIEN').length,
    visitors: users.filter(u => u.role === 'VISITOR').length,
    inactive: users.filter(u => !u.active).length,
  };

  function openCreate() {
    setStep1Form({ firstName: '', lastName: '', email: '', password: '' });
    setStep2Form({ role: 'VISITOR', specialization: '', researchAxis: '', academicYear: '', supervisor: '', thesisSubject: '', projectSubject: '', orcid: '', googleScholar: '', researchGate: '', linkedin: '', website: '' });
    setEditingUser(null);
    setCreatedUserId(null);
    setShowStep1(true);
  }

  function openEdit(user: AppUser) {
    setEditingUser(user);
    setStep1Form({ firstName: user.firstName, lastName: user.lastName, email: user.email, password: '' });
    setStep2Form({
      role: user.role,
      specialization: user.specialization || '',
      researchAxis: user.researchAxis || '',
      academicYear: user.academicYear || '',
      supervisor: user.supervisor || '',
      thesisSubject: user.thesisSubject || '',
      projectSubject: user.projectSubject || '',
      orcid: user.orcid || '',
      googleScholar: user.googleScholar || '',
      researchGate: user.researchGate || '',
      linkedin: user.linkedin || '',
      website: user.website || '',
    });
    setShowStep1(true);
  }

  function handleStep1Submit() {
    if (!step1Form.firstName.trim() || !step1Form.lastName.trim() || !step1Form.email.trim()) {
      toast.error(t('users.fillAllFields'));
      return;
    }
    if (!editingUser && !step1Form.password.trim()) {
      toast.error(t('users.passwordRequired'));
      return;
    }

    if (editingUser) {
      // Editing existing user - go to step 2
      setShowStep1(false);
      setShowStep2(true);
    } else {
      // Creating new user
      const newUser: AppUser = {
        id: Date.now().toString(),
        firstName: step1Form.firstName,
        lastName: step1Form.lastName,
        email: step1Form.email,
        role: 'VISITOR',
        active: true,
        createdAt: new Date().toISOString().split('T')[0],
      };

      setUsers(prev => [...prev, newUser]);
      setCreatedUserId(newUser.id);
      toast.success(t('users.userCreatedAs'));
      setShowStep1(false);
      setShowStep2(true);
    }
  }

  function handleStep2Submit() {
    const targetId = editingUser?.id || createdUserId;
    if (!targetId) return;

    setUsers(prev => prev.map(u => {
      if (u.id === targetId) {
        const updated: AppUser = {
          ...u,
          firstName: step1Form.firstName,
          lastName: step1Form.lastName,
          email: step1Form.email,
          role: step2Form.role,
          specialization: step2Form.specialization || undefined,
          researchAxis: step2Form.researchAxis || undefined,
          academicYear: step2Form.academicYear || undefined,
          supervisor: step2Form.supervisor || undefined,
          thesisSubject: step2Form.thesisSubject || undefined,
          projectSubject: step2Form.projectSubject || undefined,
        };

        // Only add academic profiles for CHERCHEUR
        if (step2Form.role === 'CHERCHEUR') {
          updated.orcid = step2Form.orcid || undefined;
          updated.googleScholar = step2Form.googleScholar || undefined;
          updated.researchGate = step2Form.researchGate || undefined;
          updated.linkedin = step2Form.linkedin || undefined;
          updated.website = step2Form.website || undefined;
        }

        return updated;
      }
      return u;
    }));

    toast.success(editingUser ? t('users.userModified') : t('users.userConfigured'));
    setShowStep2(false);
    setCreatedUserId(null);
    setEditingUser(null);
  }

  function handleDelete() {
    if (!deleteTarget) return;
    setUsers(prev => prev.filter(u => u.id !== deleteTarget.id));
    toast.success(t('users.userDeleted'));
    setDeleteTarget(null);
  }

  function toggleActive(u: AppUser) {
    setUsers(prev => prev.map(x => x.id === u.id ? { ...x, active: !x.active } : x));
    toast.success(u.active ? `${u.firstName} ${t('users.userDeactivated')}` : `${u.firstName} ${t('users.userActivated')}`);
  }

  const availableSupervisors = users.filter(u => u.role === 'CHERCHEUR').map(u => `${u.firstName} ${u.lastName}`);

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-navy dark:text-white">{t('users.title')}</h1>
          <p className="text-text-secondary mt-1">{users.length} {t('users.usersRegistered')}</p>
        </div>
        <Button onClick={openCreate} className="flex items-center gap-2">
          <Plus size={18} /> {t('users.createUser')}
        </Button>
      </div>

      {/* Stats */}
      <div className="grid grid-cols-2 md:grid-cols-6 gap-4">
        {[
          { label: t('users.total'), value: stats.total, color: 'bg-navy' },
          { label: t('users.researchers'), value: stats.chercheurs, color: 'bg-success' },
          { label: t('users.phdStudents'), value: stats.doctorants, color: 'bg-warning' },
          { label: t('users.masters'), value: stats.masteriens, color: 'bg-accent-blue' },
          { label: t('users.visitors'), value: stats.visitors, color: 'bg-text-muted' },
          { label: t('users.inactive'), value: stats.inactive, color: 'bg-error' },
        ].map(s => (
          <Card key={s.label}>
            <CardContent className="p-4 flex items-center gap-3">
              <div className={clsx('w-10 h-10 rounded-lg flex items-center justify-center text-white font-bold', s.color)}>
                {s.value}
              </div>
              <div className="text-sm text-text-secondary">{s.label}</div>
            </CardContent>
          </Card>
        ))}
      </div>

      {/* Search & Filter */}
      <SearchFilter
        searchPlaceholder={t('users.searchByNameEmail')}
        filterGroups={filterGroups}
        onSearchChange={setSearchQuery}
        onFilterChange={setActiveFilters}
      />

      {/* Table */}
      <Card>
        <CardContent className="p-0">
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead className="bg-light-gray dark:bg-muted border-b border-surface-border">
                <tr>
                  <th className="px-6 py-4 text-left text-sm font-semibold text-navy dark:text-white">{t('users.user')}</th>
                  <th className="px-6 py-4 text-left text-sm font-semibold text-navy dark:text-white">{t('contact.email')}</th>
                  <th className="px-6 py-4 text-left text-sm font-semibold text-navy dark:text-white">{t('users.role')}</th>
                  <th className="px-6 py-4 text-left text-sm font-semibold text-navy dark:text-white">{t('users.specialization')}</th>
                  <th className="px-6 py-4 text-left text-sm font-semibold text-navy dark:text-white">{t('users.status')}</th>
                  <th className="px-6 py-4 text-left text-sm font-semibold text-navy dark:text-white">{t('users.lastLogin')}</th>
                  <th className="px-6 py-4 text-right text-sm font-semibold text-navy dark:text-white">{t('users.actions')}</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-surface-border">
                {filtered.length === 0 && (
                  <tr><td colSpan={7} className="px-6 py-12 text-center text-text-muted">{t('users.noUsers')}</td></tr>
                )}
                {filtered.map(u => {
                  const rc = roleConfig[u.role];
                  const RoleIcon = rc.icon;
                  return (
                    <tr key={u.id} className="hover:bg-light-gray/50 dark:hover:bg-muted/50 transition-colors">
                      <td className="px-6 py-4">
                        <div className="flex items-center gap-3">
                          <div className="w-10 h-10 rounded-full bg-accent-blue flex items-center justify-center text-white font-semibold text-sm flex-shrink-0">
                            {u.firstName.charAt(0)}{u.lastName.charAt(0)}
                          </div>
                          <div>
                            <div className="font-semibold text-navy dark:text-white">{u.firstName} {u.lastName}</div>
                            <div className="text-xs text-text-muted">{t('users.since')} {u.createdAt}</div>
                          </div>
                        </div>
                      </td>
                      <td className="px-6 py-4 text-sm text-text-secondary">{u.email}</td>
                      <td className="px-6 py-4">
                        <Badge variant={rc.variant} className="flex items-center gap-1 w-fit">
                          <RoleIcon size={12} />{rc.label}
                        </Badge>
                      </td>
                      <td className="px-6 py-4 text-sm text-text-secondary">{u.specialization || '—'}</td>
                      <td className="px-6 py-4">
                        <Badge variant={u.active ? 'success' : 'default'}>
                          {u.active ? t('users.active') : t('users.inactive')}
                        </Badge>
                      </td>
                      <td className="px-6 py-4 text-sm text-text-muted">{u.lastLogin || '—'}</td>
                      <td className="px-6 py-4">
                        <div className="flex items-center justify-end gap-2">
                          <button
                            onClick={() => toggleActive(u)}
                            title={u.active ? t('users.deactivate') : t('users.activate')}
                            className="p-2 hover:bg-light-gray dark:hover:bg-muted rounded-lg transition-colors"
                          >
                            {u.active
                              ? <ToggleRight size={18} className="text-success" />
                              : <ToggleLeft size={18} className="text-text-muted" />
                            }
                          </button>
                          <button
                            onClick={() => openEdit(u)}
                            className="p-2 hover:bg-light-gray dark:hover:bg-muted rounded-lg transition-colors text-accent-blue"
                            title={t('common.modify')}
                          >
                            <Pencil size={16} />
                          </button>
                          <button
                            onClick={() => setDeleteTarget(u)}
                            className="p-2 hover:bg-error/10 rounded-lg transition-colors text-error"
                          >
                            <Trash2 size={16} />
                          </button>
                        </div>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        </CardContent>
      </Card>

      {/* Step 1: Base User Info */}
      {showStep1 && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-[60] p-4" onClick={() => { setShowStep1(false); setEditingUser(null); }}>
          <div className="bg-white dark:bg-card rounded-2xl shadow-modal max-w-lg w-full max-h-[90vh] overflow-y-auto" onClick={e => e.stopPropagation()}>
            <div className="p-6 border-b border-surface-border flex items-center justify-between">
              <div>
                <h2 className="text-xl font-bold text-navy dark:text-white">
                  {editingUser ? t('users.modifyUser') : t('users.createUser')}
                </h2>
                <p className="text-sm text-text-muted mt-1">{t('users.step1')}</p>
              </div>
              <button onClick={() => { setShowStep1(false); setEditingUser(null); }} className="p-2 hover:bg-light-gray dark:hover:bg-muted rounded-lg">✕</button>
            </div>
            <div className="p-6 space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-navy dark:text-white mb-1">{t('users.firstName')} *</label>
                  <input
                    value={step1Form.firstName}
                    onChange={e => setStep1Form(f => ({ ...f, firstName: e.target.value }))}
                    className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                    placeholder={t('users.firstName')}
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-navy dark:text-white mb-1">{t('users.lastName')} *</label>
                  <input
                    value={step1Form.lastName}
                    onChange={e => setStep1Form(f => ({ ...f, lastName: e.target.value }))}
                    className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                    placeholder={t('users.lastName')}
                  />
                </div>
              </div>
              <div>
                <label className="block text-sm font-medium text-navy dark:text-white mb-1">{t('contact.email')} *</label>
                <input
                  type="email"
                  value={step1Form.email}
                  onChange={e => setStep1Form(f => ({ ...f, email: e.target.value }))}
                  className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                  placeholder="email@limtic.tn"
                />
              </div>
              {!editingUser && (
                <>
                  <div>
                    <label className="block text-sm font-medium text-navy dark:text-white mb-1">{t('users.password')} *</label>
                    <div className="relative">
                      <input
                        type={showPass ? 'text' : 'password'}
                        value={step1Form.password}
                        onChange={e => setStep1Form(f => ({ ...f, password: e.target.value }))}
                        className="w-full px-3 py-2 pr-20 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                        placeholder={t('users.password')}
                      />
                      <button
                        type="button"
                        onClick={() => setShowPass(!showPass)}
                        className="absolute right-3 top-1/2 -translate-y-1/2 text-xs text-accent-blue"
                      >
                        {showPass ? <EyeOff size={16} /> : <Eye size={16} />}
                      </button>
                    </div>
                  </div>
                  <div className="p-3 bg-light-gray dark:bg-muted rounded-lg">
                    <p className="text-xs text-text-secondary">
                      {t('users.defaultVisitor')} <strong>Visiteur</strong>. {t('users.configureNext')}
                    </p>
                  </div>
                </>
              )}
            </div>
            <div className="p-6 border-t border-surface-border flex justify-end gap-3">
              <Button variant="outlined" onClick={() => { setShowStep1(false); setEditingUser(null); }}>{t('common.cancel')}</Button>
              <Button onClick={handleStep1Submit}>{t('users.next')} →</Button>
            </div>
          </div>
        </div>
      )}

      {/* Step 2: Role & Specific Data */}
      {showStep2 && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-[60] p-4" onClick={() => { setShowStep2(false); setCreatedUserId(null); setEditingUser(null); }}>
          <div className="bg-white dark:bg-card rounded-2xl shadow-modal max-w-2xl w-full max-h-[90vh] overflow-y-auto" onClick={e => e.stopPropagation()}>
            <div className="p-6 border-b border-surface-border flex items-center justify-between">
              <div>
                <h2 className="text-xl font-bold text-navy dark:text-white">
                  {editingUser ? t('users.specificInfo') : t('users.configureUser')}
                </h2>
                <p className="text-sm text-text-muted mt-1">{t('users.step2')}</p>
              </div>
              <button onClick={() => { setShowStep2(false); setCreatedUserId(null); setEditingUser(null); }} className="p-2 hover:bg-light-gray dark:hover:bg-muted rounded-lg">✕</button>
            </div>
            <div className="p-6 space-y-5">
              <div>
                <label className="block text-sm font-medium text-navy dark:text-white mb-2">{t('users.userRole')} *</label>
                <select
                  value={step2Form.role}
                  onChange={e => setStep2Form(f => ({ ...f, role: e.target.value as UserRole }))}
                  className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                >
                  <option value="VISITOR">{t('users.visitor')}</option>
                  <option value="MASTERIEN">{t('users.master')}</option>
                  <option value="DOCTORANT">{t('users.phd')}</option>
                  <option value="CHERCHEUR">{t('users.researcher')}</option>
                  <option value="ADMIN">{t('users.admin')}</option>
                  <option value="SUPER_ADMIN">{t('users.superAdmin')}</option>
                </select>
              </div>

              {/* Chercheur fields */}
              {step2Form.role === 'CHERCHEUR' && (
                <>
                  <div>
                    <label className="block text-sm font-medium text-navy dark:text-white mb-2">{t('users.specialization')}</label>
                    <input
                      value={step2Form.specialization}
                      onChange={e => setStep2Form(f => ({ ...f, specialization: e.target.value }))}
                      className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                      placeholder="Ex: Intelligence Artificielle, Sécurité..."
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-navy dark:text-white mb-2">{t('users.researchAxis')} *</label>
                    <select
                      value={step2Form.researchAxis}
                      onChange={e => setStep2Form(f => ({ ...f, researchAxis: e.target.value }))}
                      className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                    >
                      <option value="">{t('users.selectAxis')}</option>
                      {researchAxes.map(axe => (
                        <option key={axe} value={axe}>{axe}</option>
                      ))}
                    </select>
                  </div>
                  <div className="border-t border-surface-border pt-5">
                    <h3 className="text-sm font-semibold text-navy dark:text-white mb-4">{t('users.academicProfiles')}</h3>
                    <div className="space-y-3">
                      {[
                        { key: 'orcid', label: t('users.orcid'), placeholder: '0000-0000-0000-0000' },
                        { key: 'googleScholar', label: t('users.googleScholar'), placeholder: 'https://scholar.google.com/...' },
                        { key: 'researchGate', label: t('users.researchGate'), placeholder: 'https://www.researchgate.net/...' },
                        { key: 'linkedin', label: t('users.linkedin'), placeholder: 'https://www.linkedin.com/in/...' },
                        { key: 'website', label: t('users.personalWebsite'), placeholder: 'https://...' },
                      ].map(field => (
                        <div key={field.key}>
                          <label className="block text-xs font-medium text-text-muted mb-1">{field.label}</label>
                          <input
                            value={step2Form[field.key as keyof typeof step2Form] as string}
                            onChange={e => setStep2Form(f => ({ ...f, [field.key]: e.target.value }))}
                            className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                            placeholder={field.placeholder}
                          />
                        </div>
                      ))}
                    </div>
                  </div>
                </>
              )}

              {/* Doctorant fields */}
              {step2Form.role === 'DOCTORANT' && (
                <>
                  <div>
                    <label className="block text-sm font-medium text-navy dark:text-white mb-2">{t('users.researchAxis')} *</label>
                    <select
                      value={step2Form.researchAxis}
                      onChange={e => setStep2Form(f => ({ ...f, researchAxis: e.target.value }))}
                      className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                    >
                      <option value="">{t('users.selectAxis')}</option>
                      {researchAxes.map(axe => (
                        <option key={axe} value={axe}>{axe}</option>
                      ))}
                    </select>
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-navy dark:text-white mb-2">{t('users.enrollmentYear')} *</label>
                    <input
                      value={step2Form.academicYear}
                      onChange={e => setStep2Form(f => ({ ...f, academicYear: e.target.value }))}
                      className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                      placeholder="2024"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-navy dark:text-white mb-2">{t('users.supervisor')}</label>
                    <select
                      value={step2Form.supervisor}
                      onChange={e => setStep2Form(f => ({ ...f, supervisor: e.target.value }))}
                      className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                    >
                      <option value="">{t('users.selectSupervisor')}</option>
                      {availableSupervisors.map(sup => (
                        <option key={sup} value={sup}>{sup}</option>
                      ))}
                    </select>
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-navy dark:text-white mb-2">{t('users.thesisTitle')}</label>
                    <textarea
                      value={step2Form.thesisSubject}
                      onChange={e => setStep2Form(f => ({ ...f, thesisSubject: e.target.value }))}
                      rows={3}
                      className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm resize-none"
                      placeholder="Titre de la thèse..."
                    />
                  </div>
                </>
              )}

              {/* Masterien fields */}
              {step2Form.role === 'MASTERIEN' && (
                <>
                  <div>
                    <label className="block text-sm font-medium text-navy dark:text-white mb-2">{t('users.enrollmentYear')} *</label>
                    <input
                      value={step2Form.academicYear}
                      onChange={e => setStep2Form(f => ({ ...f, academicYear: e.target.value }))}
                      className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                      placeholder="2025-2026"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-navy dark:text-white mb-2">{t('users.supervisor')}</label>
                    <select
                      value={step2Form.supervisor}
                      onChange={e => setStep2Form(f => ({ ...f, supervisor: e.target.value }))}
                      className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                    >
                      <option value="">{t('users.selectSupervisor')}</option>
                      {availableSupervisors.map(sup => (
                        <option key={sup} value={sup}>{sup}</option>
                      ))}
                    </select>
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-navy dark:text-white mb-2">{t('users.projectTitle')}</label>
                    <textarea
                      value={step2Form.projectSubject}
                      onChange={e => setStep2Form(f => ({ ...f, projectSubject: e.target.value }))}
                      rows={3}
                      className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm resize-none"
                      placeholder="Titre du projet de mémoire..."
                    />
                  </div>
                </>
              )}
            </div>
            <div className="p-6 border-t border-surface-border flex justify-end gap-3">
              <Button variant="outlined" onClick={() => { setShowStep2(false); setCreatedUserId(null); setEditingUser(null); }}>{t('common.cancel')}</Button>
              <Button onClick={handleStep2Submit}>{t('users.finish')}</Button>
            </div>
          </div>
        </div>
      )}

      <ConfirmDialog
        isOpen={!!deleteTarget}
        onClose={() => setDeleteTarget(null)}
        onConfirm={handleDelete}
        title={t('users.deleteUserConfirm')}
        description={`${t('users.deleteUserMessage')} ${deleteTarget?.firstName} ${deleteTarget?.lastName} ? ${t('users.irreversible')}`}
        confirmText={t('common.delete')}
        variant="danger"
      />
    </div>
  );
}
