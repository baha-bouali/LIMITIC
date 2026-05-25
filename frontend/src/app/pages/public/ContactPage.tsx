import { useState } from 'react';
import { PublicNavbar } from '../../components/layout/PublicNavbar';
import { PublicFooter } from '../../components/layout/PublicFooter';
import { Button } from '../../components/ui/Button';
import { Card, CardContent } from '../../components/ui/Card';
import { Mail, Phone, MapPin, Linkedin, Twitter } from 'lucide-react';
import { toast } from 'sonner';
import { useLanguage } from '../../contexts/LanguageContext';
import { useSendContactMessageMutation } from '../../api/contactsApi';

export default function ContactPage() {
  const { t } = useLanguage();
  const [sendContactMessage, { isLoading }] = useSendContactMessageMutation();
  const [formData, setFormData] = useState({ name: '', email: '', subject: '', message: '' });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      const response = await sendContactMessage({
        fullName: formData.name,
        email: formData.email,
        subject: formData.subject,
        message: formData.message,
      }).unwrap();

      toast.success(response.message ?? t('contact.messageSent'));
      setFormData({ name: '', email: '', subject: '', message: '' });
    } catch (error) {
      const errorMessage =
        typeof error === 'object' && error !== null && 'data' in error
          ? ((error as { data?: { message?: string } }).data?.message ?? t('common.error'))
          : t('common.error');

      toast.error(errorMessage);
    }
  };

  return (
    <div className="min-h-screen bg-white dark:bg-background">
      <PublicNavbar />
      <div style={{ height: 'var(--navbar-height)' }} />

      <div className="brand-gradient-diagonal text-white py-20">
        <div className="max-w-[var(--content-max-width)] mx-auto px-6">
          <div className="text-sm text-white/80 mb-3">{t('nav.home')} / {t('nav.contact')}</div>
          <h1 className="text-5xl font-bold">{t('contact.title')}</h1>
        </div>
      </div>

      <div className="max-w-[var(--content-max-width)] mx-auto px-6 py-12">
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-12">
          <div>
            <form onSubmit={handleSubmit} className="space-y-6">
              <div>
                <label className="block text-sm font-medium mb-2">{t('contact.name')} *</label>
                <input type="text" required value={formData.name} onChange={(e) => setFormData({...formData, name: e.target.value})} className="w-full px-4 py-3 border border-surface-border rounded-[var(--radius-input)] focus:ring-2 focus:ring-accent-blue focus:border-transparent" />
              </div>
              <div>
                <label className="block text-sm font-medium mb-2">{t('contact.email')} *</label>
                <input type="email" required value={formData.email} onChange={(e) => setFormData({...formData, email: e.target.value})} className="w-full px-4 py-3 border border-surface-border rounded-[var(--radius-input)] focus:ring-2 focus:ring-accent-blue focus:border-transparent" />
              </div>
              <div>
                <label className="block text-sm font-medium mb-2">{t('contact.subject')} *</label>
                <input type="text" required value={formData.subject} onChange={(e) => setFormData({...formData, subject: e.target.value})} className="w-full px-4 py-3 border border-surface-border rounded-[var(--radius-input)] focus:ring-2 focus:ring-accent-blue focus:border-transparent" />
              </div>
              <div>
                <label className="block text-sm font-medium mb-2">{t('contact.message')} *</label>
                <textarea required value={formData.message} onChange={(e) => setFormData({...formData, message: e.target.value})} rows={6} className="w-full px-4 py-3 border border-surface-border rounded-[var(--radius-input)] focus:ring-2 focus:ring-accent-blue focus:border-transparent resize-none" />
              </div>
              <Button type="submit" className="w-full" disabled={isLoading}>
                {isLoading ? t('common.loading') || 'Sending...' : t('contact.send')}
              </Button>
            </form>
          </div>

          <div className="space-y-6">
            {/* Google Maps */}
            <Card>
              <CardContent className="p-0 overflow-hidden">
                <iframe
                  src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3193.3837484841705!2d10.188876313495362!3d36.861576424348456!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x0%3A0x0!2zMzbCsDUxJzQxLjciTiAxMMKwMTEnMjAuMCJF!5e0!3m2!1sen!2stn!4v1234567890"
                  width="100%"
                  height="300"
                  style={{ border: 0 }}
                  allowFullScreen
                  loading="lazy"
                  referrerPolicy="no-referrer-when-downgrade"
                  className="w-full"
                />
              </CardContent>
            </Card>

            <Card>
              <CardContent className="p-6 flex items-start gap-4">
                <div className="w-12 h-12 bg-accent-blue/10 rounded-lg flex items-center justify-center flex-shrink-0">
                  <MapPin className="text-accent-blue" size={24} />
                </div>
                <div>
                  <h3 className="font-semibold mb-1">{t('contact.address')}</h3>
                  <p className="text-sm text-text-secondary whitespace-pre-line">{t('lab.addressFull')}</p>
                </div>
              </CardContent>
            </Card>

            <Card>
              <CardContent className="p-6 flex items-start gap-4">
                <div className="w-12 h-12 bg-accent-blue/10 rounded-lg flex items-center justify-center flex-shrink-0">
                  <Phone className="text-accent-blue" size={24} />
                </div>
                <div>
                  <h3 className="font-semibold mb-1">{t('contact.phone')}</h3>
                  <p className="text-sm text-text-secondary">+216 71 123 456</p>
                </div>
              </CardContent>
            </Card>

            <Card>
              <CardContent className="p-6 flex items-start gap-4">
                <div className="w-12 h-12 bg-accent-blue/10 rounded-lg flex items-center justify-center flex-shrink-0">
                  <Mail className="text-accent-blue" size={24} />
                </div>
                <div>
                  <h3 className="font-semibold mb-1">{t('contact.email')}</h3>
                  <p className="text-sm text-text-secondary">contact@limtic.tn</p>
                </div>
              </CardContent>
            </Card>

            <Card>
              <CardContent className="p-6">
                <h3 className="font-semibold mb-4">{t('contact.followUs')}</h3>
                <div className="flex gap-3">
                  <a href="#" className="w-12 h-12 bg-accent-blue text-white rounded-lg flex items-center justify-center hover:bg-navy transition-colors">
                    <Linkedin size={20} />
                  </a>
                  <a href="#" className="w-12 h-12 bg-accent-blue text-white rounded-lg flex items-center justify-center hover:bg-navy transition-colors">
                    <Twitter size={20} />
                  </a>
                </div>
              </CardContent>
            </Card>
          </div>
        </div>
      </div>

      <PublicFooter />
    </div>
  );
}
