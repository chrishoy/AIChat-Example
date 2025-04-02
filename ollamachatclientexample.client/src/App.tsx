import { useState } from 'react';
import './App.css';
import ConversationComponent from './components/ui/ConversationComponent';
import { Message, Conversation } from './types/chatTypes';

function App() {
    const [response, setResponse] = useState<Message | null>(null);
    const [conversation, setConversation] = useState<Conversation | null>(null);
    const [awaitingConversation, setAwaitingConversation] = useState(false);

    const handleSubmitMessage = async (message: string) => {
        try {
            const chatId = response?.id;
            const res = await fetch(chatId == null ? "chat" : `chat/${chatId}`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({ message }),
            });
            const data = await res.json();
            setResponse(data);
            await pollConversation(data.id);
        } catch (error) {
            console.error("Error submitting message:", error);
        }
    };

    const pollConversation = async (id: string) => {
        setAwaitingConversation(true);
        const pollInterval = 2000;

        const fetchConversation = async () => {
            try {
                const res = await fetch(`chat/conversation/${id}`);
                const data = await res.json();
                setConversation(data);

                if (data.busy === false) {
                    setAwaitingConversation(false);
                } else {
                    setTimeout(fetchConversation, pollInterval);
                }
            } catch (error) {
                setAwaitingConversation(false);
                console.error("Error getting conversation:", error);
            }
        };

        fetchConversation();
    };

    return (
        <div>
            <h1 id="tableLabel">Ollama Chat...</h1>
            <p>This component demonstrates the uses of the Ollama AI chat service.</p>
            <div className="gap=5 grid grid-flow-row">
                <div>
                    <ConversationComponent
                        conversation={conversation ?? undefined}
                        onsubmit={handleSubmitMessage}
                        busy={awaitingConversation} />
                </div>
            </div>
        </div>
    );
}

export default App;
