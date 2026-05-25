import { X, AlertTriangle } from 'lucide-react';
import { Button } from '../ui/Button';

interface ConfirmDialogProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: () => void;
  title: string;
  description: string;
  confirmText?: string;
  cancelText?: string;
  variant?: 'danger' | 'warning' | 'info';
}

export function ConfirmDialog({
  isOpen,
  onClose,
  onConfirm,
  title,
  description,
  confirmText = 'Confirmer',
  cancelText = 'Annuler',
  variant = 'danger'
}: ConfirmDialogProps) {
  if (!isOpen) return null;

  const handleConfirm = () => {
    onConfirm();
    onClose();
  };

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-[60] p-4" onClick={onClose}>
      <div className="bg-white dark:bg-card rounded-2xl shadow-[0_8px_32px_rgba(15,37,87,.16)] max-w-md w-full" onClick={(e) => e.stopPropagation()}>
        <div className="p-6">
          <div className="flex items-start gap-4">
            <div className={`w-12 h-12 rounded-full flex items-center justify-center flex-shrink-0 ${
              variant === 'danger' ? 'bg-error/10' : variant === 'warning' ? 'bg-warning/10' : 'bg-info/10'
            }`}>
              <AlertTriangle className={
                variant === 'danger' ? 'text-error' : variant === 'warning' ? 'text-warning' : 'text-info'
              } size={24} />
            </div>
            <div className="flex-1">
              <h3 className="text-lg font-bold text-navy dark:text-white mb-2">{title}</h3>
              <p className="text-text-secondary dark:text-text-secondary text-sm">{description}</p>
            </div>
            <button onClick={onClose} className="p-1 hover:bg-light-gray dark:hover:bg-input-background rounded">
              <X size={20} />
            </button>
          </div>

          <div className="flex gap-3 mt-6 justify-end">
            <Button onClick={onClose} variant="outlined">{cancelText}</Button>
            <Button
              onClick={handleConfirm}
              className={variant === 'danger' ? '!bg-error hover:!bg-error/90' : ''}
            >
              {confirmText}
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
}
