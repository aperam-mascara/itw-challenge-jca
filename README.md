# itw-challenge

Please refer to the [RULES.md](RULES.md) file for the challenge requirements.

## Features

### Routing (REST)

- Route handling by minimal api
- Routes configuration using an extension method [`ConfigureChatRoutes`](src\Chat.Routes\HostExtensions.cs#L59) in the [`HostExtensions.cs`](src\Chat.Routes\HostExtensions.cs) class. The extension method allows you to configure routes in a more concise way by using options delegates.

### Database (EF)

- **Models**
  - [User model](src\Shared\models\User.cs) represent physical user
  - [Message model](src\Shared\models\Message.cs) represent message between users

- **Dto**
  - [SendMessageDto](src\Shared\dtos\SendMessageDto.cs) serve to send message across network from Browser to Server

- **Migration**

  - Database Migration using Entity Framework Core (EF Core) using an extension generic method [`MigrateDatabase<TDbContext>()`](src\EFUtilities\EFExtensions.cs#L19) in [`EFextensions.cs`](src\EFUtilities\EFExtensions.cs) where `TDbContext` must be a `DbContext` and [`IChatDbContext`](src\Shared\data\IChatDbContext.cs) which provides a base class for interacting with the database and for separation of concerns.  The `MigrateDatabase<TDbContext>()` method is used to apply pending migrations to the database.

- **Seeding**

  - Database seeding is implemented only in Debug and Development  mode. In Release mode or Production mode, the database seeding is disabled by default.
    Using an extension method [`SeedAsync.cs`](src\EFUtilities\EFExtensions.cs#L46) in the [`EFextensions.cs`](src\EFUtilities\EFExtensions.cs) class. This method ensures that the database is created before executing the seed action, which can be used to populate the database with initial data.

### UI (Blazor WebAssembly + MudBlazor)

- **[MainLayout](src\Client\Shared\MainLayout.razor)**
  - Encapsulate other pages in a single layout file for better organization and maintainability.
  - Verify the health of the server. If it is offline an overlay will be displayed.
  - Manage the app menu bar (User name and logout)
- **[Login Page](src\Client\Pages\Login.razor)**
  - A Simple Login/registration. When user has type his name, it will saved in LocalStorage's Browser.
  - It will be recovered when thee browser page is refreshed or closed.
  - In case of another "session" for a different user in browser tab,  just logout and login again.
- **[Index Page](src\Client\Pages\Index.razor)**
  - When user logged in, it will automatically redirect to this page
  - User must select a "friend" from list of users to chat with.
  - Type a message and send it
  - Refreshing received messages is automatic or manual
  - Refreshing of Friends list is automatic or manual

## Running Application and Debug

### Deploy Application and Debug with Docker-Compose

`Chat.Server` service is defined in the Docker-Compose file, it use `chatdb` service as a dependency. It is responsible for running the chat server.
`chatdb` service is defined in the Docker-Compose file. It is responsible for running the database server.
`pgAdmin` service is defined in the Docker-Compose file. It is responsible for running the pgAdmin client. This service is overriden by the `docker-compose.override.yml` file when launching the application in release mode.
`Chat.Client` service is defined in the `client.yml` file, since it cannot be debugging in Docken Environment it must be launched manually. It is responsible for running the client application.

### Debug Application in stand alone

Set multiple startup projects to debug the applications with Chat.Client and Chat.Server services. This will launch the client and server applications simultaneously.
Start a PostgreSQL container using `docker run --name chatdb -p 5432:5432  -e POSTGRES_USER=chatuser -e POSTGRES_PASSWORD=chatpassword -d postgres:17.4` command.
If you want Administrate PostgreSql with PgAdmin `docker run -p 8082:80 -e 'PGADMIN_DEFAULT_EMAIL=jc.ambert@free.fr' -e 'PGADMIN_DEFAULT_PASSWORD=1234' -d dpage/pgadmin4`. Add new server with user host.docker.internal and host 5432







