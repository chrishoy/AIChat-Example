# Ollama Chat Client Example

## Introduction
Simple .NET 9 chat client/api example that demonstrates how to use the `Ollama` Chat API.

### Getting Started
- `docker-compose up -d` to pull and start `Ollama` and `Redis` docker containers.
- `docker exec -it ollama ollama pull llama3` to pull the `llama3` model into the `Ollama` container (this will take a while).
- Use the examples in `Examples.http`
- Currently switching between emulator and AI Chat Client is done by modifying Program.cs (this is temporary)
```
builder.Services.AddChatClient(new OllamaChatClient(new Uri("http://localhost:11434"), "llama3"));
//builder.Services.AddChatClient(new ChatClientEmulator());
```

### Things to Beware
- We are using `Redis` to store chat history, and this has been 'abused' to provide minimal orchestration of requests.
- We should really be implementing the ChatService as a scalable background service.
- Throttling/request scaling has not been implemented and we are using a non-distributed Chat API (`Ollama` in this case).
- This was developed on a low spec PC which is slow to respond, so `Redis` has been configured (in code) to invalidate after 15 mins.
- This is very much playground, not production, code.
- `ChatClientEmulator` is a simple IChatClient replacement purely there for development

### Other Useful Information
- `docker exec -it ollamma bash` to enter the container.`
