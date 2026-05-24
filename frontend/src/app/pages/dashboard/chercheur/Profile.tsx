import { ProfileForm } from '../../../components/profile/ProfileForm';

export default function ChercheurProfile() {
  return (
    <ProfileForm
      role="CHERCHEUR"
      initialData={{
        firstName: 'Ahmed',
        lastName: 'Ben Salem',
        email: 'ahmed.bensalem@limtic.tn',
        phone: '+216 71 123 456',
        grade: 'Chercheur',
        speciality: 'Intelligence Artificielle',
        office: 'B205',
        bio: 'Chercheur en informatique spécialisé en intelligence artificielle et apprentissage automatique.',
        orcid: '0000-0002-1234-5678',
        googleScholar: 'https://scholar.google.com/citations?user=example',
      }}
    />
  );
}
