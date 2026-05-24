import { ProfileForm } from '../../../components/profile/ProfileForm';

export default function VisitorProfile() {
  return (
    <ProfileForm
      role="VISITOR"
      initialData={{
        firstName: 'Visiteur',
        lastName: 'Authentifié',
        email: 'visitor@limtic.tn',
        phone: '',
      }}
    />
  );
}
