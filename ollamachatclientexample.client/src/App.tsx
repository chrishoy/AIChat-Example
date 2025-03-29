import { useState } from 'react';
import './App.css';
import ChatComponent from './components/ui/ChatComponent';
import ConversationList from './components/ui/ConversationList';
import AnimatedButton from './components/ui/AnimatedButton';

interface ChatResponse {
    id: string;
    text: string;
    role: string;
    timestamp: string;
}

interface Conversation {
    conversation: ChatResponse[];
}

function App() {
    const [response, setResponse] = useState<ChatResponse | null>(null);
    const [conversation, setConversation] = useState<Conversation | null>(null);
    const [awaitingResponse, setAwaitingResponse] = useState(false);
    const [awaitingConversation, setAwaitingConversation] = useState(false);

    const handleSubmitMessage = async (message: string) => {
        try {
            // Check for continuation chat
            const chatId = response?.id;
            setAwaitingResponse(true);
            const res = await fetch(chatId == null ? "chat" : `chat/${chatId}`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({ message }),
            });
            const data = await res.json();
            setResponse(data);
            await getConversation(data.id);
            setAwaitingResponse(false);
        } catch (error) {
            setAwaitingResponse(false);
            console.error("Error submitting message:", error);
        }
    };

    const getConversation = async (id: string) => {
        try {
            console.log(`conversation/${id}`);
            setAwaitingConversation(true);
            const res = await fetch(`chat/conversation/${id}`);
            const data = await res.json();
            setConversation(data);
            setAwaitingConversation(false);
        } catch (error) {
            setAwaitingConversation(false);
            console.error("Error getting conversation:", error);
        }
    };

    return (
        <div>
            <h1 id="tableLabel">Ollama Chat...</h1>
            <p>This component demonstrates the uses of the Ollama AI chat service.</p>
            <div className="gap=5 grid grid-flow-row">
                <div>
                    <ChatComponent onsubmit={handleSubmitMessage} busy={awaitingResponse} />
                </div>
                {response && (
                    <div>
                        <AnimatedButton
                            onClick={() => getConversation(response.id)}
                            animationMinPeriod={2000}
                            busy={awaitingConversation}>
                            Get Conversation
                        </AnimatedButton>
                    </div>
                )}
                {conversation && (
                    <div>
                        <ConversationList conversation={conversation} />
                    </div>
                )}
            </div>
        </div>
    );
}

export default App;