import { skipToken } from '@reduxjs/toolkit/query';
import { useMemo } from 'react';

import { useGetResearcherProfileQuery } from '../../../api/profilesApi';
import { useGetUserByIdQuery } from '../../../api/usersApi';
import { ProfileForm } from '../../../components/profile/ProfileForm';
import { useAuth } from '../../../contexts/AuthContext';

export default function ChercheurProfile() {
  const { user } = useAuth();
  const { data: userRecord } = useGetUserByIdQuery(user?.id ?? skipToken);
  const { data } = useGetResearcherProfileQuery(user?.id ?? skipToken);

  const initialData = useMemo(() => ({
    firstName: data?.firstName ?? userRecord?.firstName ?? user?.firstName ?? '',
    lastName: data?.lastName ?? userRecord?.lastName ?? user?.lastName ?? '',
    email: data?.email ?? userRecord?.email ?? user?.email ?? '',
    phone: data?.phoneNumber ?? '',
    grade: data?.rank ?? 'Chercheur',
    speciality: data?.specialty ?? '',
    office: data?.office ?? '',
    bio: data?.biography ?? '',
    orcid: data?.orcid ?? '',
    googleScholar: data?.googleScholar ?? '',
    researchGate: data?.researchGate ?? '',
    linkedin: data?.linkedIn ?? '',
  }), [data, user, userRecord]);

  return (
    <ProfileForm
      role="CHERCHEUR"
      initialData={initialData}
    />
  );
}
