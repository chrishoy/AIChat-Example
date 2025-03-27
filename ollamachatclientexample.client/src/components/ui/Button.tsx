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
        //<button onClick={onClick} className="px-4 py-2 border-black-500 border-2 text-red rounded ...">
        //    {children}
        //</button>
    );
}
