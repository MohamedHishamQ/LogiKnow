import { cva, type VariantProps } from 'class-variance-authority';
import clsx from 'clsx';
import { HTMLAttributes, forwardRef } from 'react';
import { twMerge } from 'tailwind-merge';

const badgeVariants = cva(
  'inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-semibold transition-colors focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2',
  {
    variants: {
      variant: {
        default: 'bg-blue-600 border-transparent text-white hover:bg-blue-600/80',
        secondary: 'bg-gray-100 border-transparent text-gray-900 hover:bg-gray-100/80 dark:bg-zinc-800 dark:text-gray-100',
        destructive: 'bg-red-600 border-transparent text-white hover:bg-red-600/80',
        outline: 'text-gray-900 border border-input hover:bg-accent hover:text-accent-foreground',
      },
    },
    defaultVariants: {
      variant: 'default',
    },
  }
);

export interface BadgeProps extends HTMLAttributes<HTMLDivElement>, VariantProps<typeof badgeVariants> {}

function Badge({ className, variant, ...props }: BadgeProps) {
  return (
    <div className={twMerge(clsx(badgeVariants({ variant }), className))} {...props} />
  );
}

export { Badge, badgeVariants };
