import { useState } from 'react';
import type { ComponentType } from 'react';
import { Card, CardContent } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { SearchFilter } from '../../../components/shared/SearchFilter';
import { Users, Mail, ExternalLink, GraduationCap, BookOpen, Target } from 'lucide-react';
import { clsx } from 'clsx';
import { useLanguage } from '../../../contexts/LanguageContext';

type MemberRole = 'CHERCHEUR' | 'DOCTORANT' | 'MASTERIEN';

interface TeamMember {
  id: string;
  name: string;
  role: MemberRole;
  grade?: string;
  specialization: string;
  axe: string;
  email: string;
  thesis?: string;
  year?: string | number;
  publications?: number;
  orcid?: string;
}

const members: TeamMember[] = [
  { id: '1', name: 'Dr. Ahmed Ben Salem', role: 'CHERCHEUR', grade: 'Chercheur', specialization: 'Intelligence Artificielle et ML', axe: 'IA & Apprentissage Automatique', email: 'ahmed.bensalem@limtic.tn', publications: 28, orcid: '0000-0001-2345-6789' },
  { id: '2', name: 'Dr. Fatma Gharbi', role: 'CHERCHEUR', grade: 'Maître-Assistante', specialization: 'Sécurité Informatique', axe: 'Sécurité & Cryptographie', email: 'fatma.gharbi@limtic.tn', publications: 19 },
  { id: '3', name: 'Dr. Mohamed Mezghani', role: 'CHERCHEUR', grade: 'Chercheur', specialization: 'Réseaux & IoT', axe: 'Réseaux & Systèmes Distribués', email: 'med.mezghani@limtic.tn', publications: 45 },
  { id: '4', name: 'Dr. Leila Ouertani', role: 'CHERCHEUR', grade: 'Chercheur', specialization: 'Vision par ordinateur', axe: 'IA & Apprentissage Automatique', email: 'leila.ouertani@limtic.tn', publications: 12 },
  { id: '5', name: 'Dr. Sami Belhadj', role: 'CHERCHEUR', grade: 'Maître-Assistant', specialization: 'Data Science & Big Data', axe: 'Data Science & Visualisation', email: 'sami.belhadj@limtic.tn', publications: 8 },
  { id: '6', name: 'Sarah Trabelsi', role: 'DOCTORANT', specialization: 'Deep Learning Médical', axe: 'IA & Apprentissage Automatique', email: 'sarah.trabelsi@limtic.tn', thesis: 'Deep Learning pour le diagnostic médical assisté par IA', year: 2024, publications: 3 },
  { id: '7', name: 'Mohamed Najjar', role: 'DOCTORANT', specialization: 'Blockchain Healthcare', axe: 'Sécurité & Cryptographie', email: 'med.najjar@limtic.tn', thesis: 'Blockchain Security Analysis for Healthcare Data', year: 2023, publications: 2 },
  { id: '8', name: 'Amira Khelil', role: 'DOCTORANT', specialization: 'NLP & Traitement de texte', axe: 'IA & Apprentissage Automatique', email: 'amira.khelil@limtic.tn', thesis: 'Modèles de langage pour l\'arabe dialectal', year: 2025, publications: 1 },
  { id: '9', name: 'Ines Hamdi', role: 'MASTERIEN', specialization: 'Système de recommandation', axe: 'Data Science & Visualisation', email: 'ines.hamdi@limtic.tn', thesis: 'Système de recommandation basé sur l\'intelligence artificielle', year: '2025-2026' },
  { id: '10', name: 'Karim Slimi', role: 'MASTERIEN', specialization: 'IoT Sécurité', axe: 'Réseaux & Systèmes Distribués', email: 'karim.slimi@limtic.tn', thesis: 'Sécurisation des architectures IoT pour les villes intelligentes', year: '2024-2025' },
];

export default function ChercheurTeam() {
  const { t } = useLanguage();
  const [searchQuery, setSearchQuery] = useState('');
  const [activeFilters, setActiveFilters] = useState<Record<string, string | string[]>>({});

  const roleConfig: Record<MemberRole, { label: string; variant: any; icon: ComponentType<{ size?: number; className?: string }>; color: string }> = {
    CHERCHEUR: { label: t('role.researcher'), variant: 'success', icon: Users, color: 'bg-success/10 text-success' },
    DOCTORANT: { label: t('role.phd'), variant: 'warning', icon: GraduationCap, color: 'bg-warning/10 text-warning' },
    MASTERIEN: { label: t('role.master'), variant: 'info', icon: BookOpen, color: 'bg-accent-blue/10 text-accent-blue' },
  };

  const filterGroups = [
    {
      id: 'role', label: t('profile.role'), options: [
        { id: 'ch', label: t('team.researchers'), value: 'CHERCHEUR' },
        { id: 'do', label: t('team.phd'), value: 'DOCTORANT' },
        { id: 'ma', label: t('team.masters'), value: 'MASTERIEN' },
      ]
    },
    {
      id: 'axe', label: t('pub.axis'), options: [
        { id: 'a1', label: 'IA & Apprentissage', value: 'IA & Apprentissage Automatique' },
        { id: 'a2', label: 'Sécurité & Crypto', value: 'Sécurité & Cryptographie' },
        { id: 'a3', label: 'Réseaux & Systèmes', value: 'Réseaux & Systèmes Distribués' },
        { id: 'a4', label: 'Data Science', value: 'Data Science & Visualisation' },
      ]
    },
  ];

  const filtered = members.filter(m => {
    const q = searchQuery.toLowerCase();
    const matchQ = !q || m.name.toLowerCase().includes(q) || m.specialization.toLowerCase().includes(q) || m.axe.toLowerCase().includes(q);
    const roleF = activeFilters.role as string;
    const axeF = activeFilters.axe as string;
    return matchQ && (!roleF || m.role === roleF) && (!axeF || m.axe === axeF);
  });

  const chercheurs = filtered.filter(m => m.role === 'CHERCHEUR');
  const doctorants = filtered.filter(m => m.role === 'DOCTORANT');
  const masteriens = filtered.filter(m => m.role === 'MASTERIEN');

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-navy dark:text-white">{t('team.labTeam')}</h1>
        <p className="text-text-secondary mt-1">{members.length} {t('team.membersCount')}</p>
      </div>

      <div className="grid grid-cols-3 gap-4">
        {[
          { label: t('team.researchers'), count: members.filter(m => m.role === 'CHERCHEUR').length, color: 'bg-success/10 text-success' },
          { label: t('team.phd'), count: members.filter(m => m.role === 'DOCTORANT').length, color: 'bg-warning/10 text-warning' },
          { label: t('team.masters'), count: members.filter(m => m.role === 'MASTERIEN').length, color: 'bg-accent-blue/10 text-accent-blue' },
        ].map(s => (
          <Card key={s.label}>
            <CardContent className="p-4 flex items-center gap-3">
              <div className={clsx('w-10 h-10 rounded-lg flex items-center justify-center font-bold text-lg', s.color)}>{s.count}</div>
              <div className="text-sm text-text-secondary">{s.label}</div>
            </CardContent>
          </Card>
        ))}
      </div>

      <SearchFilter
        searchPlaceholder={t('team.searchMember')}
        filterGroups={filterGroups}
        onSearchChange={setSearchQuery}
        onFilterChange={setActiveFilters}
      />

      {chercheurs.length > 0 && (
        <Section title={t('team.researchers')} icon={Users} members={chercheurs} roleConfig={roleConfig} />
      )}
      {doctorants.length > 0 && (
        <Section title={t('team.phd')} icon={GraduationCap} members={doctorants} roleConfig={roleConfig} />
      )}
      {masteriens.length > 0 && (
        <Section title={t('team.masters')} icon={BookOpen} members={masteriens} roleConfig={roleConfig} />
      )}

      {filtered.length === 0 && (
        <Card><CardContent className="py-16 text-center text-text-muted"><Users size={40} className="mx-auto mb-3 opacity-30" /><p>{t('team.noMembers')}</p></CardContent></Card>
      )}
    </div>
  );
}

function Section({ title, icon: Icon, members, roleConfig }: { title: string; icon: ComponentType<{ size?: number; className?: string }>; members: TeamMember[]; roleConfig: Record<MemberRole, { label: string; variant: any; icon: ComponentType<{ size?: number; className?: string }>; color: string }> }) {
  return (
    <div>
      <h2 className="text-lg font-bold text-navy dark:text-white mb-4 flex items-center gap-2">
        <Icon size={20} className="text-accent-blue" /> {title} ({members.length})
      </h2>
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {members.map(m => {
          const rc = roleConfig[m.role];
          const RoleIcon = rc.icon;
          const initials = m.name.split(' ').filter(w => !w.startsWith('Dr.') && !w.startsWith('Prof.')).map(w => w.charAt(0)).slice(0, 2).join('');
          return (
            <Card key={m.id} className="hover:shadow-card-hover transition-shadow">
              <CardContent className="p-5">
                <div className="flex items-start gap-3 mb-3">
                  <div className="w-12 h-12 rounded-full bg-gradient-to-br from-navy to-accent-blue flex items-center justify-center text-white font-semibold flex-shrink-0">
                    {initials}
                  </div>
                  <div className="flex-1 min-w-0">
                    <h3 className="font-bold text-navy dark:text-white text-sm">{m.name}</h3>
                    {m.grade && <p className="text-xs text-text-muted">{m.grade}</p>}
                    <Badge variant={rc.variant} className="mt-1 text-xs flex items-center gap-1 w-fit">
                      <RoleIcon size={10} /> {rc.label}
                    </Badge>
                  </div>
                </div>
                <div className="space-y-1.5 text-xs text-text-secondary">
                  <div className="flex items-center gap-1.5">
                    <Target size={12} className="text-teal flex-shrink-0" />
                    <span className="line-clamp-1">{m.specialization}</span>
                  </div>
                  <div className="flex items-center gap-1.5">
                    <Mail size={12} className="text-accent-blue flex-shrink-0" />
                    <span className="truncate">{m.email}</span>
                  </div>
                  {m.thesis && (
                    <div className="mt-2 p-2 bg-light-gray dark:bg-muted rounded-lg">
                      <p className="text-xs text-text-secondary italic line-clamp-2">{m.thesis}</p>
                    </div>
                  )}
                  {m.publications !== undefined && (
                    <div className="flex items-center gap-1.5 mt-1">
                      <ExternalLink size={12} className="text-text-muted" />
                      <span>{m.publications} publication{m.publications !== 1 ? 's' : ''}</span>
                    </div>
                  )}
                </div>
              </CardContent>
            </Card>
          );
        })}
      </div>
    </div>
  );
}
