import React, { useEffect, useState, useRef } from "react";
import { Conversation } from "../../types/chatTypes";
import AnimatedButton from "./AnimatedButton";
import Textarea from "./Textarea";

type ConversationComponentProps = {
    conversation?: Conversation;
    busy?: boolean;
    onsubmit?: (message: string, id?: string) => Promise<void>
};

// Helper function to format text with Markdown-like features
const formatText = (text: string) => {
    // Process code blocks first to avoid interference with other formatting
    let formattedText = text.replace(/```([\s\S]*?)```/g, '<pre class="bg-gray-100 p-2 rounded font-mono text-sm overflow-x-auto">$1</pre>');
    
    // Format bold text (**text**)
    formattedText = formattedText.replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>');
    
    // Format italic text (*text*)
    formattedText = formattedText.replace(/\*([^*]+)\*/g, '<em>$1</em>');
    
    // Format inline code (`text`)
    formattedText = formattedText.replace(/`([^`]+)`/g, '<code class="bg-gray-100 px-1 rounded font-mono">$1</code>');
    
    // Format bullet points
    formattedText = formattedText.replace(/^- (.*?)$/gm, '<li class="ml-4">$1</li>');
    formattedText = formattedText.replace(/<li.*?>(.*?)<\/li>/gs, '<ul class="list-disc ml-5 my-2">$&</ul>');
    
    // Replace newlines with <br> tags
    formattedText = formattedText.replace(/\n/g, '<br>');
    
    return formattedText;
};

const ConversationComponent: React.FC<ConversationComponentProps> = ({ conversation, onsubmit, busy }) => {
    const [message, setMessage] = useState("");
    const containerRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        if (containerRef.current) {
            containerRef.current.scrollTop = containerRef.current.scrollHeight;
        }
    }, [conversation]);

    // Get message style based on role
    const getMessageStyle = (role: string) => {
        switch (role.toLowerCase()) {
            case 'assistant':
                return 'bg-blue-50 border-blue-200 ml-8 mr-2 rounded-tr-lg rounded-tl-lg rounded-bl-lg rounded-br-sm';
            case 'user':
                return 'bg-green-50 border-green-200 mr-8 ml-2 rounded-tl-lg rounded-tr-lg rounded-br-lg rounded-bl-sm';
            case 'system':
                return 'bg-yellow-50 border-yellow-200 mx-auto max-w-[85%] italic border-dashed';
            default:
                return 'bg-white';
        }
    };

    // Get message text color based on role
    const getTextStyle = (role: string) => {
        switch (role.toLowerCase()) {
            case 'assistant':
                return 'text-gray-800';
            case 'user':
                return 'text-gray-900';
            case 'system':
                return 'text-gray-600';
            default:
                return 'text-gray-800';
        }
    };

    return (
        <div
            ref={containerRef}
            className="h-128 overflow-y-auto rounded-lg border border-gray-300 bg-gray-100 p-4 flex flex-col"
        >
            <div className="flex-grow overflow-y-auto mb-4">
                {conversation?.messages.map((message) => (
                    <div 
                        key={message.id} 
                        className={`mb-3 rounded-lg border p-3 shadow-sm ${getMessageStyle(message.role)}`}
                    >
                        <div className="flex justify-between items-center mb-1">
                            <p className={`font-semibold ${message.role.toLowerCase() === 'assistant' ? 'text-blue-600' : message.role.toLowerCase() === 'user' ? 'text-green-600' : 'text-yellow-600'}`}>
                                {message.role}
                            </p>
                            <p className="text-xs text-gray-500">{new Date(message.timestamp).toLocaleString()}</p>
                        </div>
                        <p 
                            className={`${getTextStyle(message.role)} text-left`}
                            dangerouslySetInnerHTML={{ __html: formatText(message.text) }}
                        />
                    </div>)
                )}
            </div>
            <div className="flex flex-col space-y-3 p-3 border-t border-gray-200 pt-4 bg-white rounded-lg shadow-sm">
                <Textarea
                    hidden={busy}
                    value={message}
                    onChange={(e) => setMessage(e.target.value)}
                    placeholder="Type your message here... (Use **bold**, *italic*, `code`)"
                    className="w-full min-h-[80px] focus:ring-2 focus:ring-blue-300 transition-all duration-200"
                />
                <div className="flex justify-between items-center">
                    <p className="text-xs text-gray-500">
                        Formatting: **bold**, *italic*, `code`, ```codeblock```
                    </p>
                    <div>
                        <AnimatedButton
                            onClick={() => {
                                if (message.trim() && onsubmit) {
                                    onsubmit(message);
                                    setMessage("");
                                }
                            }}
                            animationMinPeriod={2000}
                            busy={busy}>
                            Submit
                        </AnimatedButton>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default ConversationComponent;
