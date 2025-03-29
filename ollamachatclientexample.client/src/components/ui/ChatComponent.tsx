import { useState } from 'react';
import AnimatedButton from "./AnimatedButton";
import { Textarea } from "./Textarea";

type ChatComponentProps = {
    busy?: boolean;
    onsubmit?: (message: string, id?: string) => Promise<void>
}

const ChatComponent: React.FC<ChatComponentProps> = ({ busy, onsubmit }) => {
    const [message, setMessage] = useState("");

  return (
      <div className="flex space-y-4 p-4">
          <Textarea
              value={message}
              onChange={(e) => setMessage(e.target.value)}
              placeholder="Type your message here..."
              className="w-full"
          />
          <AnimatedButton
              onClick={() => onsubmit && onsubmit(message)}
              animationMinPeriod={2000}
              busy={busy}>
              Submit
          </AnimatedButton>
      </div>
  );
}

export default ChatComponent;
