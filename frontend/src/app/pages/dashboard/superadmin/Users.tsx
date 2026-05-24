import { useState } from 'react';
import type { ComponentType } from 'react';
import { Card, CardContent } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { Button } from '../../../components/ui/Button';
import { SearchFilter } from '../../../components/shared/SearchFilter';
import { ConfirmDialog } from '../../../components/shared/ConfirmDialog';
import {
  Plus, Pencil, Trash2, ToggleLeft, ToggleRight,
  Users, ShieldCheck, UserCheck, GraduationCap, BookOpen, UserCircle,
  Eye, EyeOff, Loader2,
} from 'lucide-react';
import { toast } from 'sonner';
import { clsx } from 'clsx';
import { useLanguage } from '../../../contexts/LanguageContext';
import { normalizeRole } from '../../../auth/session';
import type { UserRole } from '../../../auth/session';
import {
  useGetUsersQuery,
  useAddUserMutation,
  useUpdateUserRoleMutation,
  useActivateUserMutation,
  useDeactivateUserMutation,
  useDeleteUserMutation,
} from '../../../api/usersApi';
import type { UpdateUserRoleRequest } from '../../../api/usersApi';
import {
  useGetResearchAxesQuery,
  useLazyGetResearcherProfileQuery,
  useLazyGetPhDStudentProfileQuery,
  useLazyGetMasterianProfileQuery,
  useUpdateResearcherProfileMutation,
  useUpdatePhDStudentProfileMutation,
  useUpdateMasterianProfileMutation,
} from '../../../api/profilesApi';

// ── Types ──────────────────────────────────────────────────────────────────

interface AppUser {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: UserRole;
  active: boolean;
}

// ── Constants ──────────────────────────────────────────────────────────────

const ROLE_TO_ENUM: Record<UserRole, number> = {
  SUPER_ADMIN: 1,
  ADMIN: 2,
  CHERCHEUR: 3,
  DOCTORANT: 4,
  MASTERIEN: 5,
  VISITOR: 6,
};

const roleConfig: Record<UserRole, { label: string; variant: any; icon: ComponentType<{ size?: number; className?: string }> }> = {
  SUPER_ADMIN: { label: 'SuperAdmin', variant: 'default', icon: ShieldCheck },
  ADMIN: { label: 'Admin', variant: 'info', icon: UserCheck },
  CHERCHEUR: { label: 'Chercheur', variant: 'success', icon: Users },
  DOCTORANT: { label: 'Doctorant', variant: 'warning', icon: GraduationCap },
  MASTERIEN: { label: 'Mastérien', variant: 'default', icon: BookOpen },
  VISITOR: { label: 'Visiteur', variant: 'default', icon: UserCircle },
};

const DEFAULT_STEP2_FORM = {
  role: 'VISITOR' as UserRole,
  // CHERCHEUR required
  rank: '',
  specialty: '',
  office: '',
  phoneNumber: '',
  // CHERCHEUR optional
  orcid: '',
  googleScholar: '',
  researchGate: '',
  linkedIn: '',
  // shared: CHERCHEUR + DOCTORANT (single axis selection)
  researchAxisId: '',
  // DOCTORANT required
  enrollmentYear: '',
  // DOCTORANT + MASTERIEN optional
  supervisorId: '',
  // DOCTORANT optional
  thesisSubject: '',
  // MASTERIEN required
  cohort: '',
  dissertationSubject: '',
};

// ── Component ──────────────────────────────────────────────────────────────

export default function SuperAdminUsers() {
  const { t } = useLanguage();

  // ── RTK Query hooks ──────────────────────────────────────────────────────
  const { data: rawUsers, isLoading: usersLoading, isError: usersError } = useGetUsersQuery({ limit: 200 });
  const { data: researchAxes = [] } = useGetResearchAxesQuery();

  const [addUser] = useAddUserMutation();
  const [updateUserRole] = useUpdateUserRoleMutation();
  const [activateUser] = useActivateUserMutation();
  const [deactivateUser] = useDeactivateUserMutation();
  const [deleteUser] = useDeleteUserMutation();

  const [triggerResearcherProfile] = useLazyGetResearcherProfileQuery();
  const [triggerPhDProfile] = useLazyGetPhDStudentProfileQuery();
  const [triggerMasterianProfile] = useLazyGetMasterianProfileQuery();

  const [updateResearcherProfile] = useUpdateResearcherProfileMutation();
  const [updatePhDStudentProfile] = useUpdatePhDStudentProfileMutation();
  const [updateMasterianProfile] = useUpdateMasterianProfileMutation();

  // ── UI state ─────────────────────────────────────────────────────────────
  const [searchQuery, setSearchQuery] = useState('');
  const [activeFilters, setActiveFilters] = useState<Record<string, string | string[]>>({});

  const [showStep1, setShowStep1] = useState(false);
  const [showStep2, setShowStep2] = useState(false);
  const [createdUserId, setCreatedUserId] = useState<string | null>(null);
  const [editingUser, setEditingUser] = useState<AppUser | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<AppUser | null>(null);

  const [editLoadingId, setEditLoadingId] = useState<string | null>(null);
  const [step1Loading, setStep1Loading] = useState(false);
  const [step2Loading, setStep2Loading] = useState(false);

  const [step1Form, setStep1Form] = useState({ firstName: '', lastName: '', email: '', password: '' });
  const [showPass, setShowPass] = useState(false);
  const [step2Form, setStep2Form] = useState({ ...DEFAULT_STEP2_FORM });

  // ── Derived data ─────────────────────────────────────────────────────────
  const users: AppUser[] = (rawUsers ?? []).map(dto => ({
    id: dto.id,
    firstName: dto.firstName,
    lastName: dto.lastName,
    email: dto.email,
    role: normalizeRole(dto.role),
    active: dto.isActive ?? true,
  }));

  const researchers = users.filter(u => u.role === 'CHERCHEUR');

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

  // ── Helpers ───────────────────────────────────────────────────────────────

  function closeAllModals() {
    setShowStep1(false);
    setShowStep2(false);
    setCreatedUserId(null);
    setEditingUser(null);
  }

  function openCreate() {
    setEditingUser(null);
    setCreatedUserId(null);
    setStep1Form({ firstName: '', lastName: '', email: '', password: '' });
    setStep2Form({ ...DEFAULT_STEP2_FORM });
    setShowStep1(true);
  }

  async function openEdit(user: AppUser) {
    setEditLoadingId(user.id);

    const base = { ...DEFAULT_STEP2_FORM, role: user.role };

    try {
      if (user.role === 'CHERCHEUR') {
        const profile = await triggerResearcherProfile(user.id).unwrap();
        setStep2Form({
          ...base,
          rank: profile.rank || '',
          specialty: profile.specialty || '',
          office: profile.office || '',
          phoneNumber: profile.phoneNumber || '',
          orcid: profile.orcid || '',
          googleScholar: profile.googleScholar || '',
          researchGate: profile.researchGate || '',
          linkedIn: profile.linkedIn || '',
          researchAxisId: profile.researchAxes?.[0]?.id || '',
        });
      } else if (user.role === 'DOCTORANT') {
        const profile = await triggerPhDProfile(user.id).unwrap();
        setStep2Form({
          ...base,
          enrollmentYear: profile.enrollmentYear ? String(profile.enrollmentYear) : '',
          supervisorId: profile.supervisorId || '',
          thesisSubject: profile.thesisSubject || '',
          researchAxisId: profile.researchAxes?.[0]?.id || '',
        });
      } else if (user.role === 'MASTERIEN') {
        const profile = await triggerMasterianProfile(user.id).unwrap();
        setStep2Form({
          ...base,
          cohort: profile.cohort || '',
          dissertationSubject: profile.dissertationSubject || '',
          supervisorId: profile.supervisorId || '',
        });
      } else {
        setStep2Form(base);
      }
    } catch {
      // Profile may not exist yet — proceed with empty form pre-filled to current role
      setStep2Form(base);
    }

    setEditLoadingId(null);
    setEditingUser(user);
    setStep1Form({ firstName: user.firstName, lastName: user.lastName, email: user.email, password: '' });
    setShowStep1(true);
  }

  // ── Step 1 submit ─────────────────────────────────────────────────────────

  async function handleStep1Submit() {
    if (!step1Form.firstName.trim() || !step1Form.lastName.trim() || !step1Form.email.trim()) {
      toast.error(t('users.fillAllFields'));
      return;
    }
    if (!editingUser && !step1Form.password.trim()) {
      toast.error(t('users.passwordRequired'));
      return;
    }

    if (editingUser) {
      // Edit mode: step 1 is display-only, proceed straight to role/profile config
      setShowStep1(false);
      setShowStep2(true);
      return;
    }

    // New user creation
    setStep1Loading(true);
    try {
      const newUser = await addUser({
        firstName: step1Form.firstName.trim(),
        lastName: step1Form.lastName.trim(),
        email: step1Form.email.trim().toLowerCase(),
        password: step1Form.password,
        isActive: true,
      }).unwrap();

      if (!newUser?.id) throw new Error('No user data returned');

      setCreatedUserId(newUser.id);
      toast.success(t('users.userCreatedAs'));
      setShowStep1(false);
      setShowStep2(true);
    } catch (err: any) {
      const msg = err?.data?.message || err?.message || t('users.errorOccurred');
      toast.error(msg);
    } finally {
      setStep1Loading(false);
    }
  }

  // ── Step 2 submit ─────────────────────────────────────────────────────────

  async function handleStep2Submit() {
    const targetId = editingUser?.id ?? createdUserId;
    if (!targetId) return;

    // Validate role-specific required fields
    if (step2Form.role === 'CHERCHEUR') {
      if (!step2Form.rank.trim() || !step2Form.specialty.trim() || !step2Form.office.trim() || !step2Form.phoneNumber.trim()) {
        toast.error(t('users.fillRequiredFields'));
        return;
      }
    } else if (step2Form.role === 'DOCTORANT') {
      const yr = parseInt(step2Form.enrollmentYear, 10);
      if (!yr || yr < 1900 || yr > 2200) {
        toast.error(t('users.invalidYear'));
        return;
      }
    } else if (step2Form.role === 'MASTERIEN') {
      if (!step2Form.cohort.trim() || !step2Form.dissertationSubject.trim()) {
        toast.error(t('users.fillRequiredFields'));
        return;
      }
    }

    setStep2Loading(true);
    try {
      const newRoleNum = ROLE_TO_ENUM[step2Form.role];
      // isSameRole: editing an existing user whose role hasn't changed
      const isSameRole = editingUser !== null && editingUser.role === step2Form.role;

      // ── Step A: updateRole (skip only when editing with same role) ────────
      if (!isSameRole) {
        const roleData: UpdateUserRoleRequest = { role: newRoleNum };

        if (step2Form.role === 'CHERCHEUR') {
          roleData.rank = step2Form.rank.trim();
          roleData.specialty = step2Form.specialty.trim();
          roleData.office = step2Form.office.trim();
          roleData.phoneNumber = step2Form.phoneNumber.trim();
          if (step2Form.researchAxisId) {
            roleData.researchAxisIds = [step2Form.researchAxisId];
          }
        } else if (step2Form.role === 'DOCTORANT') {
          roleData.enrollmentYear = parseInt(step2Form.enrollmentYear, 10);
        } else if (step2Form.role === 'MASTERIEN') {
          roleData.cohort = step2Form.cohort.trim();
          roleData.dissertationSubject = step2Form.dissertationSubject.trim();
        }

        await updateUserRole({ userId: targetId, data: roleData }).unwrap();
      }

      // ── Step B: profile-specific updates ─────────────────────────────────

      if (step2Form.role === 'CHERCHEUR') {
        // Same-role edit: updateRole was skipped → must push all fields via profile endpoint
        // New role with optional profile data: updateRole set required fields; push optional fields
        const hasOptional = step2Form.orcid || step2Form.googleScholar || step2Form.researchGate || step2Form.linkedIn;
        if (isSameRole || hasOptional) {
          await updateResearcherProfile({
            userId: targetId,
            data: {
              rank: step2Form.rank.trim(),
              specialty: step2Form.specialty.trim(),
              office: step2Form.office.trim(),
              phoneNumber: step2Form.phoneNumber.trim(),
              orcid: step2Form.orcid.trim() || undefined,
              googleScholar: step2Form.googleScholar.trim() || undefined,
              researchGate: step2Form.researchGate.trim() || undefined,
              linkedIn: step2Form.linkedIn.trim() || undefined,
              researchAxisIds: step2Form.researchAxisId ? [step2Form.researchAxisId] : undefined,
            },
          }).unwrap();
        }
      } else if (step2Form.role === 'DOCTORANT') {
        const enrollYear = parseInt(step2Form.enrollmentYear, 10) || 0;
        const hasDoctorantExtra = step2Form.thesisSubject || step2Form.supervisorId || step2Form.researchAxisId;
        if (isSameRole || hasDoctorantExtra) {
          await updatePhDStudentProfile({
            userId: targetId,
            data: {
              thesisSubject: step2Form.thesisSubject.trim() || undefined,
              enrollmentYear: enrollYear,
              supervisorId: step2Form.supervisorId || undefined,
              researchAxisIds: step2Form.researchAxisId ? [step2Form.researchAxisId] : undefined,
            },
          }).unwrap();
        }
      } else if (step2Form.role === 'MASTERIEN') {
        // Same-role edit: updateRole skipped → must push cohort+dissertationSubject via profile
        // New role + supervisorId: updateRole set cohort+dissertationSubject, add supervisorId
        if (isSameRole || step2Form.supervisorId) {
          await updateMasterianProfile({
            userId: targetId,
            data: {
              dissertationSubject: step2Form.dissertationSubject.trim(),
              cohort: step2Form.cohort.trim(),
              supervisorId: step2Form.supervisorId || undefined,
            },
          }).unwrap();
        }
      }

      toast.success(editingUser ? t('users.userModified') : t('users.userConfigured'));
      closeAllModals();
    } catch (err: any) {
      const msg = err?.data?.message || err?.message || t('users.errorOccurred');
      toast.error(msg);
    } finally {
      setStep2Loading(false);
    }
  }

  // ── Delete ────────────────────────────────────────────────────────────────

  async function handleDelete() {
    if (!deleteTarget) return;
    const target = deleteTarget;
    setDeleteTarget(null); // close dialog immediately
    try {
      await deleteUser(target.id).unwrap();
      toast.success(`${target.firstName} ${t('users.userDeleted')}`);
    } catch (err: any) {
      toast.error(err?.data?.message || t('users.errorOccurred'));
    }
  }

  // ── Toggle active ─────────────────────────────────────────────────────────

  async function toggleActive(u: AppUser) {
    try {
      if (u.active) {
        await deactivateUser(u.id).unwrap();
        toast.success(`${u.firstName} ${t('users.userDeactivated')}`);
      } else {
        await activateUser(u.id).unwrap();
        toast.success(`${u.firstName} ${t('users.userActivated')}`);
      }
    } catch (err: any) {
      toast.error(err?.data?.message || t('users.errorOccurred'));
    }
  }

  // ── Render ────────────────────────────────────────────────────────────────

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
                {usersLoading ? '…' : s.value}
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
                {usersLoading && (
                  <tr>
                    <td colSpan={7} className="px-6 py-12 text-center text-text-muted">
                      <Loader2 size={24} className="animate-spin mx-auto" />
                    </td>
                  </tr>
                )}
                {usersError && (
                  <tr>
                    <td colSpan={7} className="px-6 py-12 text-center text-error">
                      {t('users.errorOccurred')}
                    </td>
                  </tr>
                )}
                {!usersLoading && !usersError && filtered.length === 0 && (
                  <tr>
                    <td colSpan={7} className="px-6 py-12 text-center text-text-muted">
                      {t('users.noUsers')}
                    </td>
                  </tr>
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
                          <div className="font-semibold text-navy dark:text-white">
                            {u.firstName} {u.lastName}
                          </div>
                        </div>
                      </td>
                      <td className="px-6 py-4 text-sm text-text-secondary">{u.email}</td>
                      <td className="px-6 py-4">
                        <Badge variant={rc.variant} className="flex items-center gap-1 w-fit">
                          <RoleIcon size={12} />{rc.label}
                        </Badge>
                      </td>
                      <td className="px-6 py-4 text-sm text-text-secondary">—</td>
                      <td className="px-6 py-4">
                        <Badge variant={u.active ? 'success' : 'default'}>
                          {u.active ? t('users.active') : t('users.inactive')}
                        </Badge>
                      </td>
                      <td className="px-6 py-4 text-sm text-text-muted">—</td>
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
                            disabled={editLoadingId === u.id}
                            className="p-2 hover:bg-light-gray dark:hover:bg-muted rounded-lg transition-colors text-accent-blue disabled:opacity-50"
                            title={t('common.modify')}
                          >
                            {editLoadingId === u.id
                              ? <Loader2 size={16} className="animate-spin" />
                              : <Pencil size={16} />
                            }
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

      {/* ── Step 1: Base User Info ─────────────────────────────────────────── */}
      {showStep1 && (
        <div
          className="fixed inset-0 bg-black/50 flex items-center justify-center z-[60] p-4"
          onClick={() => { setShowStep1(false); setEditingUser(null); }}
        >
          <div
            className="bg-white dark:bg-card rounded-2xl shadow-modal max-w-lg w-full max-h-[90vh] overflow-y-auto"
            onClick={e => e.stopPropagation()}
          >
            <div className="p-6 border-b border-surface-border flex items-center justify-between">
              <div>
                <h2 className="text-xl font-bold text-navy dark:text-white">
                  {editingUser ? t('users.modifyUser') : t('users.createUser')}
                </h2>
                <p className="text-sm text-text-muted mt-1">{t('users.step1')}</p>
              </div>
              <button
                onClick={() => { setShowStep1(false); setEditingUser(null); }}
                className="p-2 hover:bg-light-gray dark:hover:bg-muted rounded-lg"
              >✕</button>
            </div>
            <div className="p-6 space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-navy dark:text-white mb-1">{t('users.firstName')} *</label>
                  <input
                    value={step1Form.firstName}
                    onChange={e => setStep1Form(f => ({ ...f, firstName: e.target.value }))}
                    disabled={!!editingUser}
                    className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm disabled:opacity-60 disabled:cursor-not-allowed"
                    placeholder={t('users.firstName')}
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-navy dark:text-white mb-1">{t('users.lastName')} *</label>
                  <input
                    value={step1Form.lastName}
                    onChange={e => setStep1Form(f => ({ ...f, lastName: e.target.value }))}
                    disabled={!!editingUser}
                    className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm disabled:opacity-60 disabled:cursor-not-allowed"
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
                  disabled={!!editingUser}
                  className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm disabled:opacity-60 disabled:cursor-not-allowed"
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
              <Button variant="outlined" onClick={() => { setShowStep1(false); setEditingUser(null); }}>
                {t('common.cancel')}
              </Button>
              <Button onClick={handleStep1Submit} disabled={step1Loading} className="flex items-center gap-2">
                {step1Loading && <Loader2 size={14} className="animate-spin" />}
                {t('users.next')} →
              </Button>
            </div>
          </div>
        </div>
      )}

      {/* ── Step 2: Role & Specific Data ──────────────────────────────────── */}
      {showStep2 && (
        <div
          className="fixed inset-0 bg-black/50 flex items-center justify-center z-[60] p-4"
          onClick={() => closeAllModals()}
        >
          <div
            className="bg-white dark:bg-card rounded-2xl shadow-modal max-w-2xl w-full max-h-[90vh] overflow-y-auto"
            onClick={e => e.stopPropagation()}
          >
            <div className="p-6 border-b border-surface-border flex items-center justify-between">
              <div>
                <h2 className="text-xl font-bold text-navy dark:text-white">
                  {editingUser ? t('users.specificInfo') : t('users.configureUser')}
                </h2>
                <p className="text-sm text-text-muted mt-1">{t('users.step2')}</p>
              </div>
              <button onClick={closeAllModals} className="p-2 hover:bg-light-gray dark:hover:bg-muted rounded-lg">✕</button>
            </div>

            <div className="p-6 space-y-5">
              {/* Role selector */}
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

              {/* ── CHERCHEUR fields ─────────────────────────────────────── */}
              {step2Form.role === 'CHERCHEUR' && (
                <>
                  {/* Required professional fields */}
                  <div className="grid grid-cols-2 gap-4">
                    <div>
                      <label className="block text-sm font-medium text-navy dark:text-white mb-1">{t('users.rank')} *</label>
                      <input
                        value={step2Form.rank}
                        onChange={e => setStep2Form(f => ({ ...f, rank: e.target.value }))}
                        className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                        placeholder={t('users.rankPlaceholder')}
                      />
                    </div>
                    <div>
                      <label className="block text-sm font-medium text-navy dark:text-white mb-1">{t('profile.office')} *</label>
                      <input
                        value={step2Form.office}
                        onChange={e => setStep2Form(f => ({ ...f, office: e.target.value }))}
                        className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                        placeholder="B101"
                      />
                    </div>
                  </div>
                  <div className="grid grid-cols-2 gap-4">
                    <div>
                      <label className="block text-sm font-medium text-navy dark:text-white mb-1">{t('users.specialization')} *</label>
                      <input
                        value={step2Form.specialty}
                        onChange={e => setStep2Form(f => ({ ...f, specialty: e.target.value }))}
                        className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                        placeholder="Ex: Intelligence Artificielle..."
                      />
                    </div>
                    <div>
                      <label className="block text-sm font-medium text-navy dark:text-white mb-1">{t('profile.phone')} *</label>
                      <input
                        value={step2Form.phoneNumber}
                        onChange={e => setStep2Form(f => ({ ...f, phoneNumber: e.target.value }))}
                        className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                        placeholder="+216 XX XXX XXX"
                      />
                    </div>
                  </div>

                  {/* Research axis */}
                  <div>
                    <label className="block text-sm font-medium text-navy dark:text-white mb-2">{t('users.researchAxis')}</label>
                    <select
                      value={step2Form.researchAxisId}
                      onChange={e => setStep2Form(f => ({ ...f, researchAxisId: e.target.value }))}
                      className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                    >
                      <option value="">{t('users.selectAxis')}</option>
                      {researchAxes.map(axis => (
                        <option key={axis.id} value={axis.id}>{axis.title}</option>
                      ))}
                    </select>
                  </div>

                  {/* Optional academic profiles */}
                  <div className="border-t border-surface-border pt-5">
                    <h3 className="text-sm font-semibold text-navy dark:text-white mb-4">{t('users.academicProfiles')}</h3>
                    <div className="space-y-3">
                      {([
                        { key: 'orcid',        label: t('users.orcid'),          placeholder: '0000-0000-0000-0000' },
                        { key: 'googleScholar', label: t('users.googleScholar'),  placeholder: 'https://scholar.google.com/...' },
                        { key: 'researchGate',  label: t('users.researchGate'),   placeholder: 'https://www.researchgate.net/...' },
                        { key: 'linkedIn',      label: t('users.linkedin'),       placeholder: 'https://www.linkedin.com/in/...' },
                      ] as const).map(field => (
                        <div key={field.key}>
                          <label className="block text-xs font-medium text-text-muted mb-1">{field.label}</label>
                          <input
                            value={step2Form[field.key]}
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

              {/* ── DOCTORANT fields ─────────────────────────────────────── */}
              {step2Form.role === 'DOCTORANT' && (
                <>
                  <div>
                    <label className="block text-sm font-medium text-navy dark:text-white mb-2">{t('users.researchAxis')}</label>
                    <select
                      value={step2Form.researchAxisId}
                      onChange={e => setStep2Form(f => ({ ...f, researchAxisId: e.target.value }))}
                      className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                    >
                      <option value="">{t('users.selectAxis')}</option>
                      {researchAxes.map(axis => (
                        <option key={axis.id} value={axis.id}>{axis.title}</option>
                      ))}
                    </select>
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-navy dark:text-white mb-2">{t('users.enrollmentYear')} *</label>
                    <input
                      value={step2Form.enrollmentYear}
                      onChange={e => setStep2Form(f => ({ ...f, enrollmentYear: e.target.value }))}
                      className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                      placeholder="2024"
                      type="number"
                      min="1990"
                      max="2200"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-navy dark:text-white mb-2">{t('users.supervisor')}</label>
                    <select
                      value={step2Form.supervisorId}
                      onChange={e => setStep2Form(f => ({ ...f, supervisorId: e.target.value }))}
                      className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                    >
                      <option value="">{t('users.selectSupervisor')}</option>
                      {researchers.map(r => (
                        <option key={r.id} value={r.id}>{r.firstName} {r.lastName}</option>
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

              {/* ── MASTERIEN fields ─────────────────────────────────────── */}
              {step2Form.role === 'MASTERIEN' && (
                <>
                  <div>
                    <label className="block text-sm font-medium text-navy dark:text-white mb-2">{t('users.enrollmentYear')} *</label>
                    <input
                      value={step2Form.cohort}
                      onChange={e => setStep2Form(f => ({ ...f, cohort: e.target.value }))}
                      className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                      placeholder="2025-2026"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-navy dark:text-white mb-2">{t('users.supervisor')}</label>
                    <select
                      value={step2Form.supervisorId}
                      onChange={e => setStep2Form(f => ({ ...f, supervisorId: e.target.value }))}
                      className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                    >
                      <option value="">{t('users.selectSupervisor')}</option>
                      {researchers.map(r => (
                        <option key={r.id} value={r.id}>{r.firstName} {r.lastName}</option>
                      ))}
                    </select>
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-navy dark:text-white mb-2">{t('users.projectTitle')} *</label>
                    <textarea
                      value={step2Form.dissertationSubject}
                      onChange={e => setStep2Form(f => ({ ...f, dissertationSubject: e.target.value }))}
                      rows={3}
                      className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm resize-none"
                      placeholder="Titre du projet de mémoire..."
                    />
                  </div>
                </>
              )}
            </div>

            <div className="p-6 border-t border-surface-border flex justify-end gap-3">
              <Button variant="outlined" onClick={closeAllModals}>{t('common.cancel')}</Button>
              <Button onClick={handleStep2Submit} disabled={step2Loading} className="flex items-center gap-2">
                {step2Loading && <Loader2 size={14} className="animate-spin" />}
                {t('users.finish')}
              </Button>
            </div>
          </div>
        </div>
      )}

      {/* ── Delete confirm dialog ────────────────────────────────────────── */}
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
