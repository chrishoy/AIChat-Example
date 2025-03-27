import { useState } from 'react';
import { Button } from "./components/ui/Button";
import { Textarea } from "./components/ui/Textarea";
import './App.css';
import ChatComponent from './components/ui/ChatComponent';

interface ChatResponse {
    id: string;
    text: string;
    role: string;
    timestamp: string;
}

function App() {
    const [message, setMessage] = useState("");
    const [response, setResponse] = useState<ChatResponse | null>(null);

    const handleSubmit = async () => {
        try {
            const res = await fetch("chat", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({ message }),
            });
            const data = await res.json();
            setResponse(data);
        } catch (error) {
            console.error("Error submitting message:", error);
        }
    };

    const messageContents =
        <div className="space-y-4 p-4">
            <Textarea
                value={message}
                onChange={(e) => setMessage(e.target.value)}
                placeholder="Type your message here..."
                className="w-full"
            />
                <Button onClick={handleSubmit}>Submit</Button>
            {response && (
                <div className="mt-2 rounded border p-2">
                    <p><strong>Id:</strong> {response.id}</p>
                    <p><strong>Role:</strong> {response.role}</p>
                    <p><strong>Message:</strong> {response.text}</p>
                    <p><strong>Timestamp:</strong> {response.timestamp}</p>
                </div>
            )}
        </div>;

    return (
        <div>
            <h1 id="tableLabel">Ollama Chat...</h1>
            <p>This component demonstrates the uses of the Ollama AI chat service.</p>
            <div className="gap=5 grid grid-flow-row">
                <div>{messageContents}</div>
                <div>
                    <ChatComponent />
                </div>
            </div>
        </div>
    );
}

export default App;