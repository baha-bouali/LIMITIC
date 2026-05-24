import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Button } from '../../components/ui/Button';
import { Card, CardContent } from '../../components/ui/Card';
import { ArrowLeft, Mail, Lock, Check } from 'lucide-react';
import { toast } from 'sonner';
import { useForgotPasswordMutation, useVerifyResetCodeMutation, useResetPasswordMutation } from '../../api/authApi';
import { useLanguage } from '../../contexts/LanguageContext';
import { LanguageSwitcher } from '../../components/shared/LanguageSwitcher';

type Step = 'email' | 'otp' | 'newPassword' | 'success';

export default function ForgotPasswordPage() {
  const { t } = useLanguage();
  const [step, setStep] = useState<Step>('email');
  const [email, setEmail] = useState('');
  const [otp, setOtp] = useState(['', '', '', '', '', '']);
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const [sendOtp] = useForgotPasswordMutation();
  const [verifyOtp] = useVerifyResetCodeMutation();
  const [resetPassword] = useResetPasswordMutation();
  const [passwordResetToken, setPasswordResetToken] = useState('');

  const handleSendOTP = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!email) {
      toast.error(t('forgotPassword.errorEmail'));
      return;
    }

    setLoading(true);
    try {
      const { data } = await sendOtp({ email }).unwrap();
      toast.success(data?.message ?? t('forgotPassword.codeSent'));
      setStep('otp');
    } catch (err) {
      toast.error(t('forgotPassword.errorSending'));
    } finally {
      setLoading(false);
    }
  };

  const handleVerifyOTP = async (e: React.FormEvent) => {
    e.preventDefault();
    const otpCode = otp.join('');

    if (otpCode.length !== 6) {
      toast.error(t('forgotPassword.errorCode'));
      return;
    }

    setLoading(true);
    try {
      const result = await verifyOtp({ email, otpToken: otpCode });
      const resetTokenFromResponse =
        'data' in result
          ? result.data?.resetToken
          : (result.error as { data?: { resetToken?: string } } | undefined)?.data?.resetToken;

      if (!resetTokenFromResponse) {
        toast.error(t('forgotPassword.errorInvalidCode'));
        return;
      }

      setPasswordResetToken(resetTokenFromResponse);
      toast.success(t('forgotPassword.codeVerified'));
      setStep('newPassword');
    } catch (err) {
      toast.error(t('forgotPassword.errorInvalidCode'));
    } finally {
      setLoading(false);
    }
  };

  const handleResetPassword = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!newPassword || !confirmPassword) {
      toast.error(t('forgotPassword.errorFields'));
      return;
    }

    if (newPassword.length < 8) {
      toast.error(t('forgotPassword.errorPasswordLength'));
      return;
    }

    if (newPassword !== confirmPassword) {
      toast.error(t('forgotPassword.errorPasswordMatch'));
      return;
    }

    if (!passwordResetToken) {
      toast.error(t('forgotPassword.errorInvalidCode'));
      return;
    }

    setLoading(true);
    try {
      const resetBody = { email, newPassword, resetToken: passwordResetToken };
      await resetPassword(resetBody).unwrap();
      toast.success(t('forgotPassword.passwordResetSuccess'));
      setStep('success');
    } catch (err) {
      toast.error(t('forgotPassword.errorReset'));
    } finally {
      setLoading(false);
    }

    // Redirect to login after 2 seconds
    setTimeout(() => {
      navigate('/login');
    }, 2000);
  };

  const handleOtpChange = (index: number, value: string) => {
    if (value.length > 1) return;
    if (!/^\d*$/.test(value)) return;

    const newOtp = [...otp];
    newOtp[index] = value;
    setOtp(newOtp);

    // Auto-focus next input
    if (value && index < 5) {
      const nextInput = document.getElementById(`otp-${index + 1}`);
      nextInput?.focus();
    }
  };

  const handleOtpKeyDown = (index: number, e: React.KeyboardEvent) => {
    if (e.key === 'Backspace' && !otp[index] && index > 0) {
      const prevInput = document.getElementById(`otp-${index - 1}`);
      prevInput?.focus();
    }
  };

  const handleResendOTP = async () => {
    setLoading(true);
    setPasswordResetToken('');
    await new Promise(resolve => setTimeout(resolve, 1000));
    setLoading(false);
    toast.success(t('forgotPassword.newCodeSent'));
  };

  return (
    <div className="min-h-screen brand-gradient-diagonal flex items-center justify-center p-4">
      <div className="absolute top-6 right-6">
        <LanguageSwitcher className="bg-white/10 backdrop-blur-sm rounded-lg p-2" />
      </div>
      <div className="w-full max-w-md">
        <Card className="shadow-2xl">
          <CardContent className="p-8">
            {/* Header */}
            <div className="text-center mb-8">
              <div className="w-16 h-16 bg-gradient-to-br from-navy to-accent-blue rounded-2xl flex items-center justify-center mx-auto mb-4">
                <Lock size={32} className="text-white" />
              </div>
              <h1 className="text-2xl font-bold text-navy dark:text-white mb-2">
                {step === 'email' && t('forgotPassword.title')}
                {step === 'otp' && t('forgotPassword.verification')}
                {step === 'newPassword' && t('forgotPassword.newPasswordTitle')}
                {step === 'success' && t('forgotPassword.success')}
              </h1>
              <p className="text-sm text-text-secondary">
                {step === 'email' && t('forgotPassword.emailSubtitle')}
                {step === 'otp' && `${t('forgotPassword.otpSubtitle')} ${email}`}
                {step === 'newPassword' && t('forgotPassword.passwordSubtitle')}
                {step === 'success' && t('forgotPassword.successMessage')}
              </p>
            </div>

            {/* Email Step */}
            {step === 'email' && (
              <form onSubmit={handleSendOTP} className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-navy dark:text-white mb-2">
                    {t('forgotPassword.emailAddress')}
                  </label>
                  <div className="relative">
                    <Mail size={18} className="absolute left-3 top-1/2 -translate-y-1/2 text-text-muted" />
                    <input
                      type="email"
                      value={email}
                      onChange={(e) => setEmail(e.target.value)}
                      className="w-full pl-10 pr-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background dark:text-white"
                      placeholder="votre.email@exemple.com"
                      required
                    />
                  </div>
                </div>

                <Button type="submit" className="w-full" disabled={loading}>
                  {loading ? t('forgotPassword.sending') : t('forgotPassword.sendCode')}
                </Button>

                <Link to="/login" className="flex items-center justify-center gap-2 text-sm text-accent-blue hover:underline">
                  <ArrowLeft size={16} />
                  {t('forgotPassword.backToLogin2')}
                </Link>
              </form>
            )}

            {/* OTP Step */}
            {step === 'otp' && (
              <form onSubmit={handleVerifyOTP} className="space-y-6">
                <div>
                  <label className="block text-sm font-medium text-navy dark:text-white mb-4 text-center">
                    {t('forgotPassword.enter6Digits')}
                  </label>
                  <div className="flex gap-2 justify-center">
                    {otp.map((digit, index) => (
                      <input
                        key={index}
                        id={`otp-${index}`}
                        type="text"
                        inputMode="numeric"
                        maxLength={1}
                        value={digit}
                        onChange={(e) => handleOtpChange(index, e.target.value)}
                        onKeyDown={(e) => handleOtpKeyDown(index, e)}
                        className="w-12 h-14 text-center text-2xl font-bold border-2 border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-accent-blue dark:bg-input-background dark:text-white"
                      />
                    ))}
                  </div>
                </div>

                <div className="space-y-3">
                  <Button type="submit" className="w-full" disabled={loading}>
                    {loading ? t('forgotPassword.verifying') : t('forgotPassword.verifyCode')}
                  </Button>

                  <button
                    type="button"
                    onClick={handleResendOTP}
                    className="w-full text-sm text-accent-blue hover:underline"
                    disabled={loading}
                  >
                    {t('forgotPassword.resendCode')}
                  </button>

                  <button
                    type="button"
                    onClick={() => setStep('email')}
                    className="flex items-center justify-center gap-2 w-full text-sm text-text-secondary hover:text-navy dark:hover:text-white"
                  >
                    <ArrowLeft size={16} />
                    {t('forgotPassword.changeEmail')}
                  </button>
                </div>
              </form>
            )}

            {/* New Password Step */}
            {step === 'newPassword' && (
              <form onSubmit={handleResetPassword} className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-navy dark:text-white mb-2">
                    {t('forgotPassword.newPassword')}
                  </label>
                  <input
                    type="password"
                    value={newPassword}
                    onChange={(e) => setNewPassword(e.target.value)}
                    className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background dark:text-white"
                    placeholder={t('forgotPassword.passwordPlaceholder')}
                    required
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-navy dark:text-white mb-2">
                    {t('forgotPassword.confirmPassword')}
                  </label>
                  <input
                    type="password"
                    value={confirmPassword}
                    onChange={(e) => setConfirmPassword(e.target.value)}
                    className="w-full px-4 py-3 border border-surface-border rounded-lg focus:ring-2 focus:ring-accent-blue focus:border-transparent dark:bg-input-background dark:text-white"
                    placeholder={t('forgotPassword.confirmPlaceholder')}
                    required
                  />
                </div>

                <div className="bg-accent-blue/5 border border-accent-blue/20 rounded-lg p-3">
                  <p className="text-xs text-text-secondary">
                    {t('forgotPassword.passwordRequirements')}
                  </p>
                </div>

                <Button type="submit" className="w-full" disabled={loading}>
                  {loading ? t('forgotPassword.resetting') : t('forgotPassword.resetPassword')}
                </Button>
              </form>
            )}

            {/* Success Step */}
            {step === 'success' && (
              <div className="text-center py-8">
                <div className="w-20 h-20 bg-success/10 rounded-full flex items-center justify-center mx-auto mb-4">
                  <Check size={40} className="text-success" />
                </div>
                <h3 className="text-xl font-bold text-navy dark:text-white mb-2">
                  Mot de passe réinitialisé !
                </h3>
                <p className="text-sm text-text-secondary mb-6">
                  {t('forgotPassword.redirecting')}
                </p>
                <Link to="/login">
                  <Button className="w-full">
                    {t('forgotPassword.loginNow')}
                  </Button>
                </Link>
              </div>
            )}
          </CardContent>
        </Card>

        {/* Branding */}
        <div className="text-center mt-6 text-white/80 text-sm">
          <p>{t('lab.copyright')}</p>
        </div>
      </div>
    </div>
  );
}
