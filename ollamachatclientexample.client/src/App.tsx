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
        <div className="min-h-screen bg-gradient-to-br from-blue-50 to-gray-100 p-6">
            <div className="max-w-4xl mx-auto">
                <header className="mb-6 text-center">
                    <h1 className="text-3xl font-bold text-blue-700 mb-2">Ollama Chat</h1>
                    <p className="text-gray-600">Interact with Ollama AI and explore its capabilities</p>
                </header>
                
                <main className="bg-white rounded-xl shadow-lg p-5 mb-8">
                    <ConversationComponent
                        conversation={conversation ?? undefined}
                        onsubmit={handleSubmitMessage}
                        busy={awaitingConversation} 
                    />
                </main>
                
                <footer className="text-center text-sm text-gray-500">
                    <p>Using Ollama AI chat service. Format your messages with **bold**, *italic*, and `code`.</p>
                </footer>
            </div>
        </div>
    );
}

export default App;
