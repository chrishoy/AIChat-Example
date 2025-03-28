import React from "react";
import { Conversation } from "../../types/chatTypes";

type ConversationListProps = {
    conversation?: Conversation;
};

const ConversationList: React.FC<ConversationListProps> = ({ conversation }) => {
    return (
        <div className="space-y-2 rounded-lg bg-gray-100 p-4">
            {conversation && conversation.conversation.length === 0 ? (
                <p className="text-gray-500">No messages</p>
            ) : (
                conversation && conversation.conversation.map((message) => (
                    <div key={message.id} className="rounded-lg border bg-white p-2 shadow-sm">
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
