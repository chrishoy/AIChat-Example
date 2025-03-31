# Ollama Chat Client Example

## Introduction
Simple .NET 9 chat client/api example that demonstrates how to use the `Ollama` Chat API.

### Getting Started
- Set up persistent volume local folders and update `docker-compose.yml` with folder locations first.
- `docker-compose up -d` to pull and start `Ollama` and `Redis` docker containers.
- `docker exec -it ollama ollama pull llama3` to pull the `llama3` model into the `Ollama` container (this will take a while).
- Use HTTP GET/POST examples in `Examples.http`, use Postman. A ReactJS client is currently in development.
- Switching between emulator and Ollama AI Chat Client is done in `appSettings.development.json`
```
  "ChatSettings": {
    "UseEmulator": true,
    "EmulatorResponseTime": 10000 // milliseconds
  }
```

### Things to Beware
- I am using `Redis` to store chat history, and this has been 'abused' to provide minimal orchestration of requests.
- The `ChatService` should really be implemented as a scalable background service. I am using `DotNet Channel` for background process management.
- Throttling/request scaling has not been implemented as I am using a non-distributed Chat API (`Ollama` in this case), so if you throw multiple requests at `Ollama` while it is still trying to figure things out, you will run into issues.
- `Redis` has been configured (in code) to invalidate conversations after 15 mins.
- Plenty more that can be done to make this production standard code.
- `ChatClientEmulator` is a simple IChatClient replacement purely there for development

### Other Useful Information
- `docker exec -it ollamma bash` to enter the container.`
