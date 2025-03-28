export type Message = {
    id: string;
    text: string;
    role: string;
    timestamp: string;
};

export type Conversation = {
    conversation: Message[];
};
