import React, { useEffect, useRef } from "react";
import { Conversation } from "../../types/chatTypes";

type ConversationListProps = {
    conversation?: Conversation;
};

const ConversationList: React.FC<ConversationListProps> = ({ conversation }) => {
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
            {conversation?.conversation.length === 0 ? (
                <p className="text-gray-500">No messages</p>
            ) : (
                conversation?.conversation.map((message) => (
                    <div key={message.id} className="mb-2 rounded-lg border bg-white p-2 shadow-sm">
                        <p className="text-xs text-gray-500">{new Date(message.timestamp).toLocaleString()}</p>
                        <p className="font-semibold text-blue-600">{message.role}</p>
                        <p className="text-gray-800">{message.text}</p>
                    </div>
                ))
            )}
        </div>
    );
};

export default ConversationList;
