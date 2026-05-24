import { HTMLAttributes } from 'react';
import { clsx } from 'clsx';

export interface BadgeProps extends HTMLAttributes<HTMLDivElement> {
  variant?: 'default' | 'success' | 'warning' | 'error' | 'info' | 'q1' | 'q2' | 'q3' | 'q4' | 'core-a-star' | 'core-a' | 'core-b' | 'core-c';
}

export function Badge({ className, variant = 'default', children, ...props }: BadgeProps) {
  return (
    <div
      className={clsx(
        'inline-flex items-center gap-1 px-2.5 py-1 rounded-[var(--radius-badge)] text-xs font-medium',
        {
          'bg-light-gray dark:bg-[#2d3d4e] text-text-secondary dark:text-text-primary': variant === 'default',
          'bg-[#D1FAE5] dark:bg-[#065F46]/30 text-[#065F46] dark:text-[#6EE7B7] border border-[#6EE7B7]/50 dark:border-[#6EE7B7]/30': variant === 'success',
          'bg-[#FEF3C7] dark:bg-[#92400E]/30 text-[#92400E] dark:text-[#FCD34D] border border-[#FCD34D]/50 dark:border-[#FCD34D]/30': variant === 'warning',
          'bg-[#FEE2E2] dark:bg-[#991B1B]/30 text-[#991B1B] dark:text-[#FCA5A5] border border-[#FCA5A5]/50 dark:border-[#FCA5A5]/30': variant === 'error',
          'bg-[#EFF6FF] dark:bg-[#1e3f6e]/50 text-[#1D4ED8] dark:text-[#93C5FD]': variant === 'info',
          'bg-[#059669] text-white': variant === 'q1',
          'bg-[#1d3964] text-white': variant === 'q2',
          'bg-[#D97706] text-white': variant === 'q3',
          'bg-[#9CA3AF] text-white': variant === 'q4',
          'bg-[#F59E0B] text-white font-bold': variant === 'core-a-star',
          'bg-[#1a672f] text-white': variant === 'core-a',
          'bg-[#6B7280] text-white': variant === 'core-b',
          'bg-[#D1D5DB] dark:bg-[#2d3d4e] text-[#374151] dark:text-text-primary': variant === 'core-c',
        },
        className
      )}
      {...props}
    >
      {children}
    </div>
  );
}
