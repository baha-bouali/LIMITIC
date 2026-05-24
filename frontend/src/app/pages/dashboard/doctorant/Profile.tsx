import { ProfileForm } from '../../../components/profile/ProfileForm';

export default function DoctorantProfile() {
  return (
    <ProfileForm
      role="DOCTORANT"
      initialData={{
        firstName: 'Sarah',
        lastName: 'Trabelsi',
        email: 'sarah.trabelsi@isi.utm.tn',
        phone: '+216 55 234 567',
        bio: 'Doctorante en informatique spécialisée dans l\'application du deep learning pour le diagnostic médical.',
        encadrant: 'Dr. Ahmed Ben Salem',
        thesisTitle: 'Apprentissage profond pour le diagnostic médical assisté par intelligence artificielle',
        enrollmentYear: '2024',
        orcid: '0000-0003-8765-4321',
        googleScholar: 'https://scholar.google.com/citations?user=sarah-trabelsi',
        researchGate: 'https://www.researchgate.net/profile/Sarah-Trabelsi',
        linkedin: 'https://www.linkedin.com/in/sarah-trabelsi',
      }}
    />
  );
}
