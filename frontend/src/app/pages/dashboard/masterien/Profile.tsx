import { ProfileForm } from '../../../components/profile/ProfileForm';

export default function MasterienProfile() {
  return (
    <ProfileForm
      role="MASTERIEN"
      initialData={{
        firstName: 'Ines',
        lastName: 'Hamdi',
        email: 'ines.hamdi@isi.utm.tn',
        phone: '+216 58 456 789',
        bio: 'Étudiante en master informatique, spécialisée dans les systèmes de recommandation basés sur l\'IA.',
        masterSpeciality: 'Intelligence Artificielle et Systèmes Intelligents',
        projectTitle: 'Système de recommandation basé sur l\'intelligence artificielle pour le e-commerce',
        enrollmentYear: '2025',
        linkedin: 'https://www.linkedin.com/in/ines-hamdi',
      }}
    />
  );
}
