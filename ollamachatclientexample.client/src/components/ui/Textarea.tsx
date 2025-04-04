import { ComponentProps } from "react";
import TextareaAutosize from "react-textarea-autosize";

const Textarea: React.FC<ComponentProps<typeof TextareaAutosize>> = (props) => {
    
    return (
        <TextareaAutosize
            {...props}
            className="rounded border p-2"
            minRows={2}
            maxRows={10}
        />
    );
}

export default Textarea;
