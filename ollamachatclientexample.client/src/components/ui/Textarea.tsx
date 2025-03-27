import { ComponentProps } from "react";
import TextareaAutosize from "react-textarea-autosize";

// eslint-disable-next-line @typescript-eslint/no-empty-object-type
interface TextareaProps extends ComponentProps<typeof TextareaAutosize> { }

export function Textarea(props: TextareaProps) {
    return (
        <TextareaAutosize
            {...props}
            className="w-96 p-2 border rounded"
            minRows={2}
            maxRows={10}
        />
    );
}