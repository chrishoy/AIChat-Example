import React, { useEffect, useState, useRef } from "react";
import { Conversation } from "../../types/chatTypes";
//import ChatComponent from "./ChatComponent";
import AnimatedButton from "./AnimatedButton";
import Textarea from "./Textarea";

type ConversationComponentProps = {
    conversation?: Conversation;
    busy?: boolean;
    onsubmit?: (message: string, id?: string) => Promise<void>
};

const ConversationComponent: React.FC<ConversationComponentProps> = ({ conversation, onsubmit, busy }) => {
    const [message, setMessage] = useState("");
    const containerRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        if (containerRef.current) {
            containerRef.current.scrollTop = containerRef.current.scrollHeight;
        }
    }, [conversation]);

    return (
        <div
            ref={containerRef}
            className="h-128 overflow-y-auto rounded-lg border border-gray-300 bg-gray-100 p-4"
        >
            {conversation?.messages.map((message) => (
                <div key={message.id} className="mb-2 rounded-lg border bg-white p-2 shadow-sm">
                    <p className="text-xs text-gray-500">{new Date(message.timestamp).toLocaleString()}</p>
                    <p className="font-semibold text-blue-600">{message.role}</p>
                    <p className="text-gray-800">{message.text}</p>
                </div>)
            )}
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
            {/*<div>*/}
            {/*    <ChatComponent onsubmit={onsubmit} busy={busy} />*/}
            {/*</div>*/}
        </div>
    );
};

export default ConversationComponent;
