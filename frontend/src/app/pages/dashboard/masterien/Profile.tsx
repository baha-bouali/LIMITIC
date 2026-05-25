import { skipToken } from '@reduxjs/toolkit/query';
import { useMemo } from 'react';

import { useGetMasterianProfileQuery } from '../../../api/profilesApi';
import { useGetUserByIdQuery } from '../../../api/usersApi';
import { ProfileForm } from '../../../components/profile/ProfileForm';
import { useAuth } from '../../../contexts/AuthContext';

export default function MasterienProfile() {
  const { user } = useAuth();
  const { data: userRecord } = useGetUserByIdQuery(user?.id ?? skipToken);
  const { data } = useGetMasterianProfileQuery(user?.id ?? skipToken);

  const initialData = useMemo(() => ({
    firstName: data?.firstName ?? userRecord?.firstName ?? user?.firstName ?? '',
    lastName: data?.lastName ?? userRecord?.lastName ?? user?.lastName ?? '',
    email: data?.email ?? userRecord?.email ?? user?.email ?? '',
    phone: '',
    bio: '',
    masterSpeciality: '',
    projectTitle: data?.dissertationSubject ?? '',
    enrollmentYear: data?.cohort ?? '',
    linkedin: '',
  }), [data, user, userRecord]);

  return (
    <ProfileForm
      role="MASTERIEN"
      initialData={initialData}
    />
  );
}
