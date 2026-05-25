import { ProfileForm } from '../../../components/profile/ProfileForm';
import { useGetUserByIdQuery } from '../../../api/usersApi';
import { useAuth } from '../../../contexts/AuthContext';
import { skipToken } from '@reduxjs/toolkit/query';

export default function VisitorProfile() {
  const { user } = useAuth();
  const { data: userRecord } = useGetUserByIdQuery(user?.id ?? skipToken);

  return (
    <ProfileForm
      role="VISITOR"
      initialData={{
        firstName: userRecord?.firstName ?? user?.firstName ?? '',
        lastName: userRecord?.lastName ?? user?.lastName ?? '',
        email: userRecord?.email ?? user?.email ?? '',
        phone: '',
      }}
    />
  );
}
