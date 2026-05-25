import { useMemo, useState } from 'react';
import type { ComponentType } from 'react';
import { Card, CardContent } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { SearchFilter } from '../../../components/shared/SearchFilter';
import { Users, Mail, ExternalLink, GraduationCap, BookOpen, Target, Loader2 } from 'lucide-react';
import { clsx } from 'clsx';
import { useLanguage } from '../../../contexts/LanguageContext';
import {
  useGetAllResearchersQuery,
  useGetAllPhDStudentsQuery,
  useGetAllMastersQuery,
  useGetResearchAxesQuery,
} from '../../../api/profilesApi';

type MemberRole = 'CHERCHEUR' | 'DOCTORANT' | 'MASTERIEN';

interface TeamMember {
  id: string;
  name: string;
  role: MemberRole;
  grade?: string;
  specialization: string;
  axeIds: string[];
  axeLabel: string;
  email: string;
  thesis?: string;
  year?: string | number;
  supervisorName?: string | null;
  orcid?: string;
}

export default function ChercheurTeam() {
  const { t } = useLanguage();
  const [searchQuery, setSearchQuery] = useState('');
  const [activeFilters, setActiveFilters] = useState<Record<string, string | string[]>>({});

  const { data: researchers = [], isLoading: loadingR } = useGetAllResearchersQuery();
  const { data: phDStudents = [], isLoading: loadingP } = useGetAllPhDStudentsQuery();
  const { data: masters = [], isLoading: loadingM } = useGetAllMastersQuery();
  const { data: researchAxes = [] } = useGetResearchAxesQuery();

  const isLoading = loadingR || loadingP || loadingM;

  const members: TeamMember[] = useMemo(() => {
    const result: TeamMember[] = [];

    for (const r of researchers) {
      const axes = r.researchAxes ?? [];
      result.push({
        id: r.id,
        name: `${r.firstName} ${r.lastName}`,
        role: 'CHERCHEUR',
        grade: r.rank ?? undefined,
        specialization: r.specialty ?? '—',
        axeIds: axes.map(a => a.id),
        axeLabel: axes[0]?.title ?? '—',
        email: r.email,
        orcid: r.orcid ?? undefined,
      });
    }

    for (const p of phDStudents) {
      const axes = p.researchAxes ?? [];
      result.push({
        id: p.id,
        name: `${p.firstName} ${p.lastName}`,
        role: 'DOCTORANT',
        specialization: p.thesisSubject ?? '—',
        axeIds: axes.map(a => a.id),
        axeLabel: axes[0]?.title ?? '—',
        email: p.email,
        thesis: p.thesisSubject ?? undefined,
        year: p.enrollmentYear,
        supervisorName: p.supervisorName ?? undefined,
      });
    }

    for (const m of masters) {
      result.push({
        id: m.id,
        name: `${m.firstName} ${m.lastName}`,
        role: 'MASTERIEN',
        specialization: m.dissertationSubject ?? '—',
        axeIds: [],
        axeLabel: m.cohort ?? '—',
        email: m.email,
        thesis: m.dissertationSubject ?? undefined,
        year: m.cohort ?? undefined,
        supervisorName: m.supervisorName ?? undefined,
      });
    }

    return result;
  }, [researchers, phDStudents, masters]);

  const roleConfig: Record<MemberRole, { label: string; variant: any; icon: ComponentType<{ size?: number; className?: string }>; color: string }> = {
    CHERCHEUR: { label: t('role.researcher'), variant: 'success', icon: Users, color: 'bg-success/10 text-success' },
    DOCTORANT: { label: t('role.phd'), variant: 'warning', icon: GraduationCap, color: 'bg-warning/10 text-warning' },
    MASTERIEN: { label: t('role.master'), variant: 'info', icon: BookOpen, color: 'bg-accent-blue/10 text-accent-blue' },
  };

  const axisFilterOptions = researchAxes.map(a => ({
    id: a.id,
    label: a.title,
    value: a.id,
  }));

  const filterGroups = [
    {
      id: 'role', label: t('profile.role'), options: [
        { id: 'ch', label: t('team.researchers'), value: 'CHERCHEUR' },
        { id: 'do', label: t('team.phd'), value: 'DOCTORANT' },
        { id: 'ma', label: t('team.masters'), value: 'MASTERIEN' },
      ]
    },
    ...(axisFilterOptions.length > 0 ? [{
      id: 'axe', label: t('pub.axis'), options: axisFilterOptions.slice(0, 4),
    }] : []),
  ];

  const filtered = members.filter(m => {
    const q = searchQuery.toLowerCase();
    const matchQ = !q || m.name.toLowerCase().includes(q) || m.specialization.toLowerCase().includes(q) || m.axeLabel.toLowerCase().includes(q);
    const roleF = activeFilters.role as string;
    const axeF = activeFilters.axe as string;
    const matchAxe = !axeF || m.axeIds.includes(axeF);
    return matchQ && (!roleF || m.role === roleF) && matchAxe;
  });

  const chercheurs = filtered.filter(m => m.role === 'CHERCHEUR');
  const doctorants = filtered.filter(m => m.role === 'DOCTORANT');
  const masteriens = filtered.filter(m => m.role === 'MASTERIEN');

  if (isLoading) {
    return (
      <div className="flex items-center justify-center py-24">
        <Loader2 size={32} className="animate-spin text-accent-blue" />
      </div>
    );
  }

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

function Section({ title, icon: Icon, members, roleConfig }: {
  title: string;
  icon: ComponentType<{ size?: number; className?: string }>;
  members: TeamMember[];
  roleConfig: Record<MemberRole, { label: string; variant: any; icon: ComponentType<{ size?: number; className?: string }>; color: string }>;
}) {
  return (
    <div>
      <h2 className="text-lg font-bold text-navy dark:text-white mb-4 flex items-center gap-2">
        <Icon size={20} className="text-accent-blue" /> {title} ({members.length})
      </h2>
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {members.map(m => {
          const rc = roleConfig[m.role];
          const RoleIcon = rc.icon;
          const initials = m.name
            .split(' ')
            .filter(w => !w.startsWith('Dr.') && !w.startsWith('Prof.'))
            .map(w => w.charAt(0))
            .slice(0, 2)
            .join('');
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
                  {m.specialization !== '—' && (
                    <div className="flex items-center gap-1.5">
                      <Target size={12} className="text-teal flex-shrink-0" />
                      <span className="line-clamp-1">{m.specialization}</span>
                    </div>
                  )}
                  <div className="flex items-center gap-1.5">
                    <Mail size={12} className="text-accent-blue flex-shrink-0" />
                    <span className="truncate">{m.email}</span>
                  </div>
                  {m.axeLabel !== '—' && (
                    <div className="flex items-center gap-1.5">
                      <Target size={12} className="text-text-muted flex-shrink-0" />
                      <span className="line-clamp-1 text-text-muted">{m.axeLabel}</span>
                    </div>
                  )}
                  {m.thesis && m.role !== 'CHERCHEUR' && (
                    <div className="mt-2 p-2 bg-light-gray dark:bg-muted rounded-lg">
                      <p className="text-xs text-text-secondary italic line-clamp-2">{m.thesis}</p>
                    </div>
                  )}
                  {m.year && (
                    <div className="flex items-center gap-1.5 mt-1">
                      <GraduationCap size={12} className="text-text-muted" />
                      <span>{m.year}</span>
                    </div>
                  )}
                  {m.orcid && (
                    <div className="flex items-center gap-1.5 mt-1">
                      <ExternalLink size={12} className="text-text-muted" />
                      <span className="truncate">{m.orcid}</span>
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
