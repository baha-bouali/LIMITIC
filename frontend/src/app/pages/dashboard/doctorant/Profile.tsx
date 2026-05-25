import { skipToken } from '@reduxjs/toolkit/query';
import { useMemo } from 'react';

import { useGetPhDStudentProfileQuery } from '../../../api/profilesApi';
import { useGetUserByIdQuery } from '../../../api/usersApi';
import { ProfileForm } from '../../../components/profile/ProfileForm';
import { useAuth } from '../../../contexts/AuthContext';

export default function DoctorantProfile() {
  const { user } = useAuth();
  const { data: userRecord } = useGetUserByIdQuery(user?.id ?? skipToken);
  const { data } = useGetPhDStudentProfileQuery(user?.id ?? skipToken);

  const initialData = useMemo(() => ({
    firstName: data?.firstName ?? userRecord?.firstName ?? user?.firstName ?? '',
    lastName: data?.lastName ?? userRecord?.lastName ?? user?.lastName ?? '',
    email: data?.email ?? userRecord?.email ?? user?.email ?? '',
    phone: '',
    bio: '',
    encadrant: data?.supervisorName ?? '',
    thesisTitle: data?.thesisSubject ?? '',
    enrollmentYear: data?.enrollmentYear ? String(data.enrollmentYear) : '',
    orcid: '',
    googleScholar: '',
    researchGate: '',
    linkedin: '',
  }), [data, user, userRecord]);

  return (
    <ProfileForm
      role="DOCTORANT"
      initialData={initialData}
    />
  );
}
