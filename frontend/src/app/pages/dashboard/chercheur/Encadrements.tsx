import { useState } from 'react';
import { Card, CardContent } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { Button } from '../../../components/ui/Button';
import { ConfirmDialog } from '../../../components/shared/ConfirmDialog';
import { GraduationCap, BookOpen, Mail, Calendar, X, FileText, Plus, Pencil, Trash2 } from 'lucide-react';
import { clsx } from 'clsx';
import { toast } from 'sonner';
import { useLanguage } from '../../../contexts/LanguageContext';

interface Student {
  id: string;
  name: string;
  email: string;
  subject: string;
  year: string;
  publications?: number;
  type: 'DOCTORANT' | 'MASTERIEN';
}

const initialDoctorants: Student[] = [
  {
    id: '1',
    name: 'Sarah Trabelsi',
    email: 'sarah.trabelsi@isi.utm.tn',
    subject: 'Apprentissage profond pour le diagnostic médical assisté par intelligence artificielle',
    year: '2024',
    publications: 3,
    type: 'DOCTORANT',
  },
  {
    id: '2',
    name: 'Mohamed Najjar',
    email: 'med.najjar@isi.utm.tn',
    subject: 'Blockchain Security Analysis for Healthcare Data Management',
    year: '2023',
    publications: 2,
    type: 'DOCTORANT',
  },
];

const initialMasteriens: Student[] = [
  {
    id: '3',
    name: 'Ines Hamdi',
    email: 'ines.hamdi@isi.utm.tn',
    subject: 'Système de recommandation basé sur l\'intelligence artificielle pour le e-commerce',
    year: '2025-2026',
    type: 'MASTERIEN',
  },
  {
    id: '4',
    name: 'Youssef Dali',
    email: 'youssef.dali@isi.utm.tn',
    subject: 'Application mobile de santé basée sur l\'IA pour le suivi des patients chroniques',
    year: '2025-2026',
    type: 'MASTERIEN',
  },
];

// Mock available students (users that can be added as encadrements)
const availableStudents = [
  { id: 's1', name: 'Karim Slimi', email: 'karim.slimi@isi.utm.tn', type: 'MASTERIEN' as const },
  { id: 's2', name: 'Amine Fourati', email: 'amine.fourati@isi.utm.tn', type: 'DOCTORANT' as const },
  { id: 's3', name: 'Leila Jendoubi', email: 'leila.jendoubi@isi.utm.tn', type: 'MASTERIEN' as const },
];

export default function ChercheurEncadrements() {
  const { t } = useLanguage();
  const [activeTab, setActiveTab] = useState<'doctorants' | 'masteriens'>('doctorants');
  const [doctorants, setDoctorants] = useState<Student[]>(initialDoctorants);
  const [masteriens, setMasteriens] = useState<Student[]>(initialMasteriens);
  const [detailStudent, setDetailStudent] = useState<Student | null>(null);
  const [editStudent, setEditStudent] = useState<Student | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<Student | null>(null);
  const [showAddTypeSelector, setShowAddTypeSelector] = useState(false);
  const [showAddForm, setShowAddForm] = useState(false);
  const [addFormType, setAddFormType] = useState<'DOCTORANT' | 'MASTERIEN'>('DOCTORANT');
  const [addForm, setAddForm] = useState({
    userId: '',
    subject: '',
    year: '',
  });

  const students = activeTab === 'doctorants' ? doctorants : masteriens;
  const setStudents = activeTab === 'doctorants' ? setDoctorants : setMasteriens;

  function openAddForm(type: 'DOCTORANT' | 'MASTERIEN') {
    setAddFormType(type);
    setAddForm({ userId: '', subject: '', year: '' });
    setShowAddTypeSelector(false);
    setShowAddForm(true);
  }

  function handleAddStudent() {
    if (!addForm.userId || !addForm.subject || !addForm.year) {
      toast.error(t('users.fillAllFields'));
      return;
    }

    const selectedUser = availableStudents.find(s => s.id === addForm.userId);
    if (!selectedUser) return;

    const newStudent: Student = {
      id: Date.now().toString(),
      name: selectedUser.name,
      email: selectedUser.email,
      subject: addForm.subject,
      year: addForm.year,
      type: addFormType,
      publications: 0,
    };

    if (addFormType === 'DOCTORANT') {
      setDoctorants(prev => [...prev, newStudent]);
    } else {
      setMasteriens(prev => [...prev, newStudent]);
    }

    toast.success(`${addFormType === 'DOCTORANT' ? t('role.phd') : t('role.master')} ${t('supervision.supervisionAdded')}`);
    setShowAddForm(false);
  }

  function openEditStudent(student: Student) {
    setEditStudent(student);
  }

  function handleSaveEdit() {
    if (!editStudent) return;

    const updater = (prev: Student[]) =>
      prev.map(s => (s.id === editStudent.id ? editStudent : s));

    if (editStudent.type === 'DOCTORANT') {
      setDoctorants(updater);
    } else {
      setMasteriens(updater);
    }

    toast.success(t('supervision.supervisionModified'));
    setEditStudent(null);
  }

  function handleDelete() {
    if (!deleteTarget) return;

    if (deleteTarget.type === 'DOCTORANT') {
      setDoctorants(prev => prev.filter(s => s.id !== deleteTarget.id));
    } else {
      setMasteriens(prev => prev.filter(s => s.id !== deleteTarget.id));
    }

    toast.success(t('supervision.supervisionDeleted'));
    setDeleteTarget(null);
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between flex-wrap gap-3">
        <div>
          <h1 className="text-2xl font-bold text-navy dark:text-white">{t('supervision.title')}</h1>
          <p className="text-text-secondary mt-1">
            {doctorants.length} {doctorants.length > 1 ? t('supervision.phd').toLowerCase() : t('role.phd').toLowerCase()} · {masteriens.length} {masteriens.length > 1 ? t('supervision.master').toLowerCase() : t('role.master').toLowerCase()}
          </p>
        </div>
        <Button onClick={() => setShowAddTypeSelector(true)} className="flex items-center gap-2">
          <Plus size={16} /> {t('supervision.add')}
        </Button>
      </div>

      {/* Tabs */}
      <div className="flex gap-1 bg-light-gray dark:bg-muted p-1 rounded-xl w-fit">
        {[
          { key: 'doctorants', label: t('supervision.phd'), icon: GraduationCap, count: doctorants.length },
          { key: 'masteriens', label: t('supervision.master'), icon: BookOpen, count: masteriens.length },
        ].map(tab => (
          <button
            key={tab.key}
            onClick={() => setActiveTab(tab.key as any)}
            className={clsx(
              'flex items-center gap-2 px-4 py-2 rounded-lg text-sm font-medium transition-all',
              activeTab === tab.key
                ? 'bg-white dark:bg-card shadow-sm text-navy dark:text-white'
                : 'text-text-secondary hover:text-navy dark:hover:text-white'
            )}
          >
            <tab.icon size={16} />
            {tab.label}
            <span className={clsx(
              'text-xs px-1.5 py-0.5 rounded-full',
              activeTab === tab.key ? 'bg-accent-blue text-white' : 'bg-surface-border text-text-muted'
            )}>
              {tab.count}
            </span>
          </button>
        ))}
      </div>

      {/* Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-5">
        {students.map(student => (
          <Card key={student.id} className="hover:shadow-card-hover transition-shadow">
            <CardContent className="p-5 space-y-4">
              <div className="flex items-start gap-3">
                <div className="w-12 h-12 rounded-full bg-gradient-to-br from-navy to-accent-blue text-white flex items-center justify-center font-bold text-lg flex-shrink-0">
                  {student.name.charAt(0)}
                </div>
                <div className="flex-1 min-w-0">
                  <h3 className="font-bold text-navy dark:text-white text-sm">{student.name}</h3>
                  <Badge variant={student.type === 'DOCTORANT' ? 'warning' : 'info'} className="mt-1 text-xs">
                    {student.type === 'DOCTORANT' ? t('role.phd') : t('role.master')}
                  </Badge>
                </div>
              </div>

              <div className="p-3 bg-light-gray dark:bg-muted rounded-lg">
                <p className="text-xs text-text-secondary line-clamp-2 leading-relaxed">{student.subject}</p>
              </div>

              <div className="space-y-1.5 text-xs text-text-secondary">
                <div className="flex items-center gap-2">
                  <Mail size={12} className="text-text-muted" />
                  <span className="truncate">{student.email}</span>
                </div>
                <div className="flex items-center gap-2">
                  <Calendar size={12} className="text-text-muted" />
                  <span>{student.type === 'DOCTORANT' ? `${t('supervision.enrolled')} ${student.year}` : `${t('supervision.promotion')} ${student.year}`}</span>
                </div>
                {student.publications !== undefined && (
                  <div className="flex items-center gap-2">
                    <FileText size={12} className="text-text-muted" />
                    <span>{student.publications} {student.publications !== 1 ? t('supervision.publications_plural') : t('supervision.publications')}</span>
                  </div>
                )}
              </div>

              <div className="flex gap-2">
                <button
                  onClick={() => setDetailStudent(student)}
                  className="flex-1 py-2 text-xs font-medium text-accent-blue hover:bg-accent-blue/5 rounded-lg transition-colors border border-accent-blue/20"
                >
                  {t('supervision.details')}
                </button>
                <button
                  onClick={() => openEditStudent(student)}
                  className="p-2 text-navy dark:text-white hover:bg-light-gray dark:hover:bg-muted rounded-lg transition-colors"
                  title={t('common.modify')}
                >
                  <Pencil size={16} />
                </button>
                <button
                  onClick={() => setDeleteTarget(student)}
                  className="p-2 text-error hover:bg-error/10 rounded-lg transition-colors"
                  title={t('common.delete')}
                >
                  <Trash2 size={16} />
                </button>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      {students.length === 0 && (
        <Card>
          <CardContent className="py-16 text-center text-text-muted">
            <GraduationCap size={40} className="mx-auto mb-3 opacity-30" />
            <p>{t('supervision.none')}</p>
            <Button
              onClick={() => setShowAddTypeSelector(true)}
              variant="outlined"
              className="mt-4"
            >
              <Plus size={16} className="mr-2" />
              {t('supervision.addSupervision')}
            </Button>
          </CardContent>
        </Card>
      )}

      {/* Type Selector Modal */}
      {showAddTypeSelector && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-[60] p-4" onClick={() => setShowAddTypeSelector(false)}>
          <div className="bg-white dark:bg-card rounded-2xl shadow-modal max-w-md w-full" onClick={e => e.stopPropagation()}>
            <div className="p-6 border-b border-surface-border">
              <h2 className="text-lg font-bold text-navy dark:text-white">{t('supervision.addSupervision')}</h2>
              <p className="text-sm text-text-muted mt-1">{t('supervision.selectType')}</p>
            </div>
            <div className="p-6 space-y-3">
              <button
                onClick={() => openAddForm('DOCTORANT')}
                className="w-full p-4 border-2 border-surface-border hover:border-warning hover:bg-warning/5 rounded-lg transition-all text-left group"
              >
                <div className="flex items-center gap-3">
                  <div className="w-12 h-12 rounded-lg bg-warning/10 group-hover:bg-warning/20 flex items-center justify-center">
                    <GraduationCap size={24} className="text-warning" />
                  </div>
                  <div>
                    <h3 className="font-semibold text-navy dark:text-white">{t('supervision.phdStudent')}</h3>
                    <p className="text-sm text-text-muted">{t('supervision.phdDescription')}</p>
                  </div>
                </div>
              </button>
              <button
                onClick={() => openAddForm('MASTERIEN')}
                className="w-full p-4 border-2 border-surface-border hover:border-accent-blue hover:bg-accent-blue/5 rounded-lg transition-all text-left group"
              >
                <div className="flex items-center gap-3">
                  <div className="w-12 h-12 rounded-lg bg-accent-blue/10 group-hover:bg-accent-blue/20 flex items-center justify-center">
                    <BookOpen size={24} className="text-accent-blue" />
                  </div>
                  <div>
                    <h3 className="font-semibold text-navy dark:text-white">{t('supervision.masterStudent')}</h3>
                    <p className="text-sm text-text-muted">{t('supervision.masterDescription')}</p>
                  </div>
                </div>
              </button>
            </div>
            <div className="p-6 border-t border-surface-border flex justify-end">
              <Button variant="outlined" onClick={() => setShowAddTypeSelector(false)}>{t('common.cancel')}</Button>
            </div>
          </div>
        </div>
      )}

      {/* Add Encadrement Form */}
      {showAddForm && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-[60] p-4" onClick={() => setShowAddForm(false)}>
          <div className="bg-white dark:bg-card rounded-2xl shadow-modal max-w-lg w-full max-h-[90vh] overflow-y-auto" onClick={e => e.stopPropagation()}>
            <div className="p-6 border-b border-surface-border flex items-center justify-between">
              <h2 className="text-lg font-bold text-navy dark:text-white">
                {t('supervision.addSupervision')} {addFormType === 'DOCTORANT' ? t('role.phd') : t('role.master')}
              </h2>
              <button onClick={() => setShowAddForm(false)} className="p-2 hover:bg-light-gray dark:hover:bg-muted rounded-lg"><X size={18} /></button>
            </div>
            <div className="p-6 space-y-4">
              <div>
                <label className="block text-sm font-medium text-navy dark:text-white mb-2">
                  {t('supervision.selectStudent')} *
                </label>
                <select
                  value={addForm.userId}
                  onChange={e => setAddForm(f => ({ ...f, userId: e.target.value }))}
                  className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                >
                  <option value="">{t('supervision.chooseStudent')}</option>
                  {availableStudents
                    .filter(s => s.type === addFormType)
                    .map(student => (
                      <option key={student.id} value={student.id}>
                        {student.name} ({student.email})
                      </option>
                    ))}
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium text-navy dark:text-white mb-2">
                  {addFormType === 'DOCTORANT' ? t('supervision.thesisSubject') : t('supervision.projectSubject')} *
                </label>
                <textarea
                  value={addForm.subject}
                  onChange={e => setAddForm(f => ({ ...f, subject: e.target.value }))}
                  rows={3}
                  className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm resize-none"
                  placeholder={t('supervision.researchSubject')}
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-navy dark:text-white mb-2">
                  {addFormType === 'DOCTORANT' ? t('supervision.enrollmentYear') : t('supervision.academicYear')} *
                </label>
                <input
                  value={addForm.year}
                  onChange={e => setAddForm(f => ({ ...f, year: e.target.value }))}
                  className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                  placeholder={addFormType === 'DOCTORANT' ? '2024' : '2025-2026'}
                />
              </div>
            </div>
            <div className="p-6 border-t border-surface-border flex justify-end gap-3">
              <Button variant="outlined" onClick={() => setShowAddForm(false)}>{t('common.cancel')}</Button>
              <Button onClick={handleAddStudent}>{t('common.add')}</Button>
            </div>
          </div>
        </div>
      )}

      {/* Edit Modal */}
      {editStudent && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-[60] p-4" onClick={() => setEditStudent(null)}>
          <div className="bg-white dark:bg-card rounded-2xl shadow-modal max-w-lg w-full max-h-[90vh] overflow-y-auto" onClick={e => e.stopPropagation()}>
            <div className="p-6 border-b border-surface-border flex items-center justify-between">
              <h2 className="text-lg font-bold text-navy dark:text-white">{t('supervision.modifySupervision')}</h2>
              <button onClick={() => setEditStudent(null)} className="p-2 hover:bg-light-gray dark:hover:bg-muted rounded-lg"><X size={18} /></button>
            </div>
            <div className="p-6 space-y-4">
              <div>
                <label className="block text-sm font-medium text-navy dark:text-white mb-2">{t('contact.name')}</label>
                <input
                  value={editStudent.name}
                  disabled
                  className="w-full px-3 py-2 border border-surface-border rounded-lg dark:bg-input-background text-sm bg-light-gray dark:bg-muted cursor-not-allowed"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-navy dark:text-white mb-2">{editStudent.type === 'DOCTORANT' ? t('supervision.thesisSubject') : t('supervision.projectSubject')} *</label>
                <textarea
                  value={editStudent.subject}
                  onChange={e => setEditStudent({ ...editStudent, subject: e.target.value })}
                  rows={3}
                  className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm resize-none"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-navy dark:text-white mb-2">
                  {editStudent.type === 'DOCTORANT' ? t('supervision.enrollmentYear') : t('supervision.academicYear')} *
                </label>
                <input
                  value={editStudent.year}
                  onChange={e => setEditStudent({ ...editStudent, year: e.target.value })}
                  className="w-full px-3 py-2 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background text-sm"
                />
              </div>
            </div>
            <div className="p-6 border-t border-surface-border flex justify-end gap-3">
              <Button variant="outlined" onClick={() => setEditStudent(null)}>{t('common.cancel')}</Button>
              <Button onClick={handleSaveEdit}>{t('common.save')}</Button>
            </div>
          </div>
        </div>
      )}

      {/* Detail Modal */}
      {detailStudent && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-[60] p-4" onClick={() => setDetailStudent(null)}>
          <div className="bg-white dark:bg-card rounded-2xl shadow-modal max-w-lg w-full max-h-[85vh] overflow-y-auto" onClick={e => e.stopPropagation()}>
            <div className="p-6 border-b border-surface-border flex items-center justify-between">
              <h2 className="text-lg font-bold text-navy dark:text-white">{t('supervision.studentProfile')}</h2>
              <button onClick={() => setDetailStudent(null)} className="p-2 hover:bg-light-gray dark:hover:bg-muted rounded-lg"><X size={18} /></button>
            </div>
            <div className="p-6 space-y-5">
              <div className="flex items-center gap-4">
                <div className="w-16 h-16 rounded-full bg-gradient-to-br from-navy to-accent-blue text-white flex items-center justify-center text-2xl font-bold">
                  {detailStudent.name.charAt(0)}
                </div>
                <div>
                  <h3 className="text-lg font-bold text-navy dark:text-white">{detailStudent.name}</h3>
                  <Badge variant={detailStudent.type === 'DOCTORANT' ? 'warning' : 'info'}>
                    {detailStudent.type === 'DOCTORANT' ? t('role.phd') : t('role.master')}
                  </Badge>
                </div>
              </div>
              <div className="space-y-3 text-sm">
                <div className="flex items-center gap-2 text-text-secondary">
                  <Mail size={15} className="text-text-muted flex-shrink-0" />
                  {detailStudent.email}
                </div>
                <div className="flex items-center gap-2 text-text-secondary">
                  <Calendar size={15} className="text-text-muted flex-shrink-0" />
                  {detailStudent.type === 'DOCTORANT' ? `${t('supervision.enrolled')} ${detailStudent.year}` : `${t('supervision.promotion')} ${detailStudent.year}`}
                </div>
                {detailStudent.publications !== undefined && (
                  <div className="flex items-center gap-2 text-text-secondary">
                    <FileText size={15} className="text-text-muted flex-shrink-0" />
                    {detailStudent.publications} {detailStudent.publications !== 1 ? t('supervision.publications_plural') : t('supervision.publications')}
                  </div>
                )}
              </div>
              <div>
                <h4 className="text-sm font-semibold text-navy dark:text-white mb-2">
                  {detailStudent.type === 'DOCTORANT' ? t('supervision.thesisTitle') : t('supervision.projectTitle')}
                </h4>
                <p className="text-sm text-text-secondary leading-relaxed">{detailStudent.subject}</p>
              </div>
            </div>
            <div className="p-6 border-t border-surface-border flex justify-end">
              <Button variant="outlined" onClick={() => setDetailStudent(null)}>{t('supervision.close')}</Button>
            </div>
          </div>
        </div>
      )}

      <ConfirmDialog
        isOpen={!!deleteTarget}
        onClose={() => setDeleteTarget(null)}
        onConfirm={handleDelete}
        title={t('supervision.deleteSupervision')}
        description={`${t('supervision.deleteMessage')} ${deleteTarget?.name} ${t('supervision.fromSupervisions')}`}
        confirmText={t('common.delete')}
        variant="danger"
      />
    </div>
  );
}
