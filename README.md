# Simple Chat Application - itw-challenge

A simple chat application built with ASP.NET Core Web API and Blazor WebAssembly.
Please refer to the [RULES.md](RULES.md) file for the challenge requirements.

## Features

- User login with simple username entry
- User list
- Real-time message exchange
- Message history stored in PostgreSQL database
- Modern UI with MudBlazor components
- Responsive design for various screen sizes

## Tech Stack

- .NET 9 with C# 13
- ASP.NET Core Web API with minimal APIs
- Blazor WebAssembly
- Entity Framework Core with PostgreSQL
- MudBlazor component library
- Docker and Docker Compose for containerization
- XUnit for testing

## Project Structure

### Application projetcts

- [`Chat.Server`](src/Server/Chat.Server.csproj): Backend API project with minimal APIs
- [`Chat.Client`](src/Client/Chat.Client.csproj): Blazor WebAssembly client application
- [`Chat.Shared`](src/Shared/Chat.Shared.csproj): Library of Shared models, DTOs, DbContext Abstraction

### Utilities projetcs

- [`XUnitPostgreSQL`](src/Utilities/XUnitPostgreSQL/XUnitPostgreSQL.csproj): PostegreSQL Dockerization for unit testing with XUnit

### Testing Project

- [`Chat.Server.Tests`](src/tests/Chat.Server.Tests/Chat.Server.Tests.csproj): Tests API and Database server

### Running Project

- [`Server Dockerfile`](src/Server/Dockerfile): Multi-stage Docker build file Server
- [`docker-compose.yml`](src/docker-compose.yml): Docker Compose configuration for running Server

---
---

>### Database (EF)

- **Models**
  - [User model](src/Shared/models/User.cs) represent physical user
  - [Message model](src/Shared/models/Message.cs) represent message between users

- **Dto**
  - [SendMessageDto](src/Shared/dtos/SendMessageDto.cs) serve to send message across network from Browser to Server

>### Routing (REST)

- Route handling by minimal api
- Routes configuration using an extension method [`ConfigureChatRoutes`](src\Server\ChatRoutes.cs#L18) in the [`ChatRoutes.cs`](src\Server\ChatRoutes.cs) class. The extension method allows you to configure routes in a more concise way by using options delegates.

>### UI (Blazor WebAssembly + MudBlazor)

- **[MainLayout](src/Client/Shared/MainLayout.razor)**
  - Encapsulate other pages in a single layout file for better organization and maintainability.
  - Manage the app menu bar (User name and logout)
- **[Login Page](src/Client/Pages/Login.razor)**
  - A Simple Login/registration. 
- **[Index Page](src/Client/Pages/Index.razor)**
  - When user logged in, it will automatically redirect to this page
  - User must select a "friend" from list of users to chat with.
  - Type a message and send it
  - Refreshing received messages is automatic or manual
  - Refreshing of Friends list is automatic or manual

- Components
  - **[ChatChatPanel](src/Client/Components/ChatChatPanel.razor)** : Where messages are displayed
  - **[ChatUserPanel](src/Client/Components/ChatUserPanel.razor)** : Where list of registered user are displayed
  - **[LoginForm](src/Client/Components/LoginForm.razor)** : Login component that permit to register and access the chat room
  - **[SendMessage](src/Client/Components/SendMessage.razor)** : Component that permit to send messages and refreshing

>### Testing Server API (XUnit)

## Running Application and Debug with Docker

### Docker-Compose file

- `Chat.Server` service is defined in the Docker-Compose file, it use `chatdb` service as a dependency. It is responsible for running the chat server. It waiting for health check of the database before starting the server.
- `chatdb` service is defined in the Docker-Compose file. It is responsible for running the database server.
- `pgAdmin` service is defined in the [docker-compose override.yml](src\docker-compose.override.yml) file. It is responsible for running the pgAdmin client.

### Debugging 
- set in Visual Sudio Project startup to `Docker Compose`

### Running Application
- type in console `docker-compose -f docker-compose.yml -p chat_application  up -d ` in [src folder](src)

