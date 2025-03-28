import React, { useEffect, useState } from "react";
import clsx from "clsx";

type AnimatedButtonProps = {
    busy?: boolean;
    animationMinPeriod?: number;
    children: React.ReactNode;
} & React.ButtonHTMLAttributes<HTMLButtonElement>;

const AnimatedButton: React.FC<AnimatedButtonProps> = ({ busy, animationMinPeriod = 0, children, className, ...props }) => {
    const [showBusy, setShowBusy] = useState(busy);

    useEffect(() => {
        if (busy) {
            setShowBusy(true);
        } else if (animationMinPeriod > 0) {
            const timeout = setTimeout(() => setShowBusy(false), animationMinPeriod);
            return () => clearTimeout(timeout);
        } else {
            setShowBusy(false);
        }
    }, [busy, animationMinPeriod]);

    return (
        <button
            className={clsx(
                "relative flex items-center justify-center px-1 py-1 rounded-lg bg-blue-600 text-black font-medium transition-all duration-200",
                "hover:bg-blue-700 disabled:opacity-50",
                className
            )}
            disabled={showBusy || props.disabled}
            {...props}
        >
            <span className="relative flex h-full w-full items-center justify-center">
                {!showBusy && children}
                {showBusy && (
                    <span className="flex space-x-1">
                        <span className="h-2 w-2 animate-pulse rounded bg-red-700 [animation-delay:-0.4s]"></span>
                        <span className="h-2 w-2 animate-pulse rounded bg-green-700 [animation-delay:-0.2s]"></span>
                        <span className="h-2 w-2 animate-pulse rounded bg-blue-700"></span>
                    </span>
                )}
            </span>
        </button>
    );
};

export default AnimatedButton;