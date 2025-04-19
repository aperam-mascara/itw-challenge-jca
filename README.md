# itw-challenge

Please refer to the [RULES.md](RULES.md) file for the challenge requirements.

## Features
### Routes
Routes configuration using an extension method `ConfigureChatRoutes` in the `HostExtensions` class. The extension method allows you to configure routes in a more concise way by using options delegates. This makes the code cleaner and easier to read.

### Migrations
Database Migration using Entity Framework Core (EF Core) using an extension generic method `MigrateDatabase<TDbContext>()` in`EFextensions.cs` where `TDbContext` must be a `DbContext` and `IChatDbContext` which provides a base class for interacting with the database and for separation of concerns.  The `MigrateDatabase<TDbContext>()` method is used to apply pending migrations to the database. The `OnModelCreating` method in `ChatDbContext.cs` is overridden to configure the model mappings using options delegates.



### Seeding

Database seeding is implemented only in Debug and Development  mode. In Release mode or Production mode, the database seeding is disabled by default.
Using an extension method `SeedAsync` in the `EFExtensions.cs` class. This method ensures that the database is created before executing the seed action, which can be used to populate the database with initial data.

## Run Application and Debug
### Deploy Application and Debug with Docker-Compose
`Chat.Server` service is defined in the Docker-Compose file, it use `chatdb` service as a dependency. It is responsible for running the chat server.
`chatdb` service is defined in the Docker-Compose file. It is responsible for running the database server.
`pgAdmin` service is defined in the Docker-Compose file. It is responsible for running the pgAdmin client. This service is overriden by the `docker-compose.override.yml` file when launching the application in release mode.
`Chat.Client` service is defined in the `client.yml` file, since it cannot be debugging in Docken Environment it must be launched manually. It is responsible for running the client application.

### Debug Application in stand alone
Set multiple startup projects to debug the applications with Chat.Client and Chat.Server services. This will launch the client and server applications simultaneously.
Start a PostgreSQL container using `docker run --name chatdb -p 5432:5432  -e POSTGRES_USER=chatuser -e POSTGRES_PASSWORD=chatpassword -d postgres:17.4` command.







