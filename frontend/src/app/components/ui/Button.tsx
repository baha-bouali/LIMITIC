import { ButtonHTMLAttributes, forwardRef } from 'react';
import { clsx } from 'clsx';

export interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'filled' | 'outlined' | 'ghost';
  size?: 'sm' | 'md' | 'lg';
}

export const Button = forwardRef<HTMLButtonElement, ButtonProps>(
  ({ className, variant = 'filled', size = 'md', style, ...props }, ref) => {
    const filledStyle = variant === 'filled' ? {
      background: 'var(--brand-gradient)',
      ...style,
    } : style;

    return (
      <button
        ref={ref}
        style={filledStyle}
        className={clsx(
          'inline-flex items-center justify-center gap-2 rounded-[var(--radius-button)] transition-all duration-200',
          'hover:scale-[1.01] active:scale-[0.99]',
          'focus:outline-none focus:ring-2 focus:ring-accent-blue focus:ring-offset-2',
          'disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:scale-100',
          {
            'text-white hover:opacity-90': variant === 'filled',
            'border-2 border-navy text-navy bg-transparent hover:bg-navy/5 dark:border-white/40 dark:text-white dark:hover:bg-white/5': variant === 'outlined',
            'text-navy hover:bg-navy/5 dark:text-white dark:hover:bg-white/5': variant === 'ghost',
            'px-3 py-1.5 text-sm': size === 'sm',
            'px-6 py-2.5 text-base': size === 'md',
            'px-8 py-3.5 text-lg': size === 'lg',
          },
          className
        )}
        {...props}
      />
    );
  }
);

Button.displayName = 'Button';
