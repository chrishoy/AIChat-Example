import { ReactNode } from "react";

interface ButtonProps {
    children: ReactNode;
    onClick?: () => void;
}

export function Button({ children, onClick }: ButtonProps) {
    return (
        <button onClick={onClick}>
          {children}
        </button>
    );
}
