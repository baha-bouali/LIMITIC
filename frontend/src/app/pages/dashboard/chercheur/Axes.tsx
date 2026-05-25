import { useAuth } from '../../../contexts/AuthContext';
import { useGetResearcherProfileQuery } from '../../../api/profilesApi';
import { AxesView } from '../../../components/shared/AxesView';
import { useLanguage } from '../../../contexts/LanguageContext';

export default function ChercheurAxes() {
  const { t } = useLanguage();
  const { user } = useAuth();
  const { data: profile } = useGetResearcherProfileQuery(user?.id ?? '', { skip: !user?.id });
  const myAxisIds = profile?.researchAxes?.map(a => a.id) ?? [];

  return (
    <AxesView
      myAxisIds={myAxisIds}
      myAxisBannerLabel={t('axes.myAxisAttachment')}
      myAxisModalLabel={t('axes.youAreAttached')}
    />
  );
}
