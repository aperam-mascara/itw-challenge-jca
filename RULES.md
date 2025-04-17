# Challenge for interviews

## Objective

Please implement a simple chat application with the following requirements:

- **Business logic**
  - A simple chat application with a list of users and a chat window.
  - Users can send messages to each other.
  - Messages should be stored in a database and retrieved when the user opens the chat window.
  - Users should be able to see the list of users and select one to chat with.
  - Messages should be displayed in the chat window in real-time (no need for SignalR, just have a refresh mechanism/button to see new messages).

- **Frontend**
  - Make it simple, no frills.
  - Don’t spend too much time on the UI, but it should be usable.
  - Use Blazor WebAssembly (not server-side)
  - You don't need to use real-time communication (SignalR).
  - Use any component library you want. We use MudBlazor, but any other is perfect.
  - Bonus: Ensure the application is responsive and works on different screen sizes.

- **Backend**
  - Use ASP.NET Core Web API, with minimal APIs. Avoid using controllers, avoid third-party libraries, and avoid unnecessary complexity.
  - Use Entity Framework Core for data access, and use migrations to create the database schema.
  - Use a database of your choice, but EF is required. PostgreSQL or SQLite is the simplest.
  - A Docker Compose file to run everything
  - Authentication can be ignored, just ask for the username at the start of the application (you can open two Chrome windows and send messages to each other)
  - No scaling or unnecessary optimization

- Process
  - Use Git for version control and commit often. We will review the commit history.
  - Use a README.md file to explain how to run the application and any other relevant information.

## Provided starter code

Chat.sln is a solution file for a Blazor WebAssembly project. It contains the following projects:

- Chat.Client: The Blazor WebAssembly client application.
- Chat.Server: The ASP.NET Core Web API server application.
- Chat.Shared: A shared library for common code between the client and server.
- docker-compose: A Docker compose project to run the application in a containerized environment.

You can use this starter code as a base for your implementation. 
The solution is already set up with the necessary projects and references. 
You can run the application using Docker Compose, which will start both the client and server applications.
You are free to add any additional projects or files as needed, but please keep the provided structure in mind.


**Good luck with your implementation!**