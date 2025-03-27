import { useState } from 'react';
import { Button } from "./components/ui/Button";
import { Textarea } from "./components/ui/Textarea";
import './App.css';

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
        <div className="p-4 space-y-4">
            <Textarea
                value={message}
                onChange={(e) => setMessage(e.target.value)}
                placeholder="Type your message here..."
                className="w-full"
            />
                <Button onClick={handleSubmit}>Submit</Button>
            {response && (
                <div className="p-2 border rounded mt-2">
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

            <div className="grid grid-flow-col grid-rows-3 gap-4">
                <div className="row-span-3 bg-fuchsia-600 border-blue-500 border-2">Spans 3 rows</div>
                <div className="col-span-2 border-red-500 border-2">Spans 2 cols</div>
                <div className="col-span-2 row-span-2 border-green-500 border-2">Spans 2 cols and 2 rows</div>
            </div>

            <div>{messageContents}</div>
        </div>
    );
}

export default App;