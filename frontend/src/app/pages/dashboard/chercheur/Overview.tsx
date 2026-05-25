import { skipToken } from '@reduxjs/toolkit/query';
import { Link } from 'react-router-dom';
import { Card, CardContent, CardHeader } from '../../../components/ui/Card';
import { Badge } from '../../../components/ui/Badge';
import { FileText, GraduationCap, Users, ChevronRight, Target } from 'lucide-react';
import { useAuth } from '../../../contexts/AuthContext';
import { useLanguage } from '../../../contexts/LanguageContext';
import { useGetAllMastersQuery, useGetAllPhDStudentsQuery, useGetResearcherProfileQuery } from '../../../api/profilesApi';

export default function ChercheurOverview() {
  const { t } = useLanguage();
  const { user } = useAuth();
  const { data: profile } = useGetResearcherProfileQuery(user?.id ?? skipToken);
  const { data: phDStudents = [], isLoading: phdLoading } = useGetAllPhDStudentsQuery();
  const { data: masters = [], isLoading: mastersLoading } = useGetAllMastersQuery();

  const researcherName = [profile?.firstName ?? user?.firstName, profile?.lastName ?? user?.lastName].filter(Boolean).join(' ');
  const supervisedPhd = phDStudents.filter((student) => student.supervisorId === user?.id);
  const supervisedMasters = masters.filter((student) => student.supervisorId === user?.id);
  const mainAxis = profile?.researchAxes?.[0];

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-navy dark:text-white">{t('dash.overviewTitle')}</h1>
        <p className="text-text-secondary mt-1">{t('dash.welcomeDr')} {researcherName || ''}</p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-5">
        <StatCard icon={<FileText size={22} />} count={0} label={t('dash.myPublications')} href="/dashboard/chercheur/publications" color="accent-blue" />
        <StatCard icon={<GraduationCap size={22} />} count={phdLoading ? '...' : supervisedPhd.length} label={t('dash.phdSupervised')} href="/dashboard/chercheur/encadrements" color="success" />
        <StatCard icon={<Users size={22} />} count={mastersLoading ? '...' : supervisedMasters.length} label={t('dash.mastersSupervised')} href="/dashboard/chercheur/encadrements" color="teal" />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-5">
        <Card>
          <CardHeader className="flex items-center justify-between">
            <h3 className="font-bold text-navy dark:text-white">{t('dash.recentPublications')}</h3>
            <Link to="/dashboard/chercheur/publications" className="text-xs text-accent-blue hover:underline flex items-center gap-1">
              {t('dash.viewAll')} <ChevronRight size={14} />
            </Link>
          </CardHeader>
          <CardContent className="pt-0">
            <EmptyPanel label={t('pub.noPublications') || t('pubPage.noPublications') || 'No publications found'} />
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex items-center justify-between">
            <h3 className="font-bold text-navy dark:text-white">{t('dash.activeSupervisions')}</h3>
            <Link to="/dashboard/chercheur/encadrements" className="text-xs text-accent-blue hover:underline flex items-center gap-1">
              {t('dash.viewAll')} <ChevronRight size={14} />
            </Link>
          </CardHeader>
          <CardContent className="space-y-2.5 pt-0">
            {phdLoading || mastersLoading ? (
              <EmptyPanel label="Loading..." />
            ) : supervisedPhd.length + supervisedMasters.length === 0 ? (
              <EmptyPanel label={t('common.noData') || 'No data'} />
            ) : (
              <>
                {supervisedPhd.slice(0, 3).map((student) => (
                  <EncadrementItem key={student.id} name={`${student.firstName} ${student.lastName}`} type="Doctorant" thesis={student.thesisSubject} />
                ))}
                {supervisedMasters.slice(0, Math.max(0, 3 - supervisedPhd.length)).map((student) => (
                  <EncadrementItem key={student.id} name={`${student.firstName} ${student.lastName}`} type="Masterien" thesis={student.dissertationSubject} />
                ))}
              </>
            )}
          </CardContent>
        </Card>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-5">
        <Card>
          <CardHeader>
            <h3 className="font-bold text-navy dark:text-white">{t('dash.myResearchAxis')}</h3>
          </CardHeader>
          <CardContent className="pt-0">
            {mainAxis ? (
              <div className="p-4 bg-accent-blue/5 border border-accent-blue/20 rounded-xl">
                <div className="flex items-center gap-2 mb-2">
                  <Target size={18} className="text-accent-blue" />
                  <span className="font-semibold text-navy dark:text-white text-sm">{mainAxis.title}</span>
                </div>
                <p className="text-xs text-text-secondary mb-3">
                  {mainAxis.members?.length ?? 0} {t('dash.members')}, {mainAxis.publicationsCount ?? 0} {t('axes.publicationsCount')}
                </p>
                <Link to="/dashboard/chercheur/axes" className="text-xs text-accent-blue hover:underline flex items-center gap-1">
                  {t('dash.exploreAllAxes')} <ChevronRight size={13} />
                </Link>
              </div>
            ) : (
              <EmptyPanel label={t('axes.notAssigned')} />
            )}
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <h3 className="font-bold text-navy dark:text-white">{t('dash.quickAccess')}</h3>
          </CardHeader>
          <CardContent className="pt-0 space-y-2">
            {[
              { label: t('dash.createPublication'), href: '/dashboard/chercheur/publications', color: 'bg-accent-blue/10 text-accent-blue' },
              { label: t('dash.viewLabTeam'), href: '/dashboard/chercheur/team', color: 'bg-success/10 text-success' },
              { label: t('dash.consultEvents'), href: '/dashboard/chercheur/events', color: 'bg-teal/10 text-teal' },
            ].map((item) => (
              <Link
                key={item.href}
                to={item.href}
                className={`flex items-center justify-between p-3 rounded-lg hover:opacity-80 transition-opacity ${item.color}`}
              >
                <span className="text-sm font-medium">{item.label}</span>
                <ChevronRight size={16} />
              </Link>
            ))}
          </CardContent>
        </Card>
      </div>
    </div>
  );
}

function StatCard({ icon, count, label, href, color }: any) {
  const colorMap: Record<string, string> = {
    'accent-blue': 'bg-accent-blue/10 text-accent-blue',
    success: 'bg-success/10 text-success',
    teal: 'bg-teal/10 text-teal',
  };

  return (
    <Link to={href}>
      <Card className="hover:shadow-card-hover transition-shadow cursor-pointer">
        <CardContent className="p-5">
          <div className="flex items-center justify-between">
            <div>
              <div className="text-3xl font-bold text-navy dark:text-white mb-0.5">{count}</div>
              <div className="text-sm text-text-secondary">{label}</div>
            </div>
            <div className={`w-12 h-12 rounded-xl flex items-center justify-center ${colorMap[color]}`}>{icon}</div>
          </div>
        </CardContent>
      </Card>
    </Link>
  );
}

function EmptyPanel({ label }: { label: string }) {
  return <div className="py-6 text-center text-sm text-text-secondary">{label}</div>;
}

function EncadrementItem({ name, type, thesis }: any) {
  const { t } = useLanguage();

  return (
    <div className="flex items-start gap-3 p-3 bg-light-gray dark:bg-muted rounded-lg">
      <div className="w-9 h-9 rounded-full bg-gradient-to-br from-navy to-accent-blue text-white flex items-center justify-center text-sm font-bold flex-shrink-0">
        {name.charAt(0)}
      </div>
      <div className="flex-1 min-w-0">
        <div className="flex items-center gap-2">
          <span className="font-medium text-navy dark:text-white text-sm">{name}</span>
          <Badge variant={type === 'Doctorant' ? 'warning' : 'info'} className="text-xs">
            {type === 'Doctorant' ? t('role.phd') : t('role.master')}
          </Badge>
        </div>
        <p className="text-xs text-text-secondary mt-0.5 line-clamp-1">{thesis || t('common.noData')}</p>
      </div>
    </div>
  );
}
