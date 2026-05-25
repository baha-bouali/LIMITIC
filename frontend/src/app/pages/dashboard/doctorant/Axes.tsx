import { useAuth } from '../../../contexts/AuthContext';
import { useGetPhDStudentProfileQuery } from '../../../api/profilesApi';
import { AxesView } from '../../../components/shared/AxesView';
import { useLanguage } from '../../../contexts/LanguageContext';

export default function DoctorantAxes() {
  const { t } = useLanguage();
  const { user } = useAuth();
  const { data: profile } = useGetPhDStudentProfileQuery(user?.id ?? '', { skip: !user?.id });
  const myAxisIds = profile?.researchAxes?.map(a => a.id) ?? [];

  return (
    <AxesView
      myAxisIds={myAxisIds}
      myAxisBannerLabel={t('axes.myThesisAxis')}
      myAxisModalLabel={t('axes.youAreAttachedThesis')}
    />
  );
}
