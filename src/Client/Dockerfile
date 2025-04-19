# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# Copy csproj files and restore dependencies
COPY ["Directory.Build.props","./"]
COPY ["Client/Chat.Client.csproj", "Client/"]
COPY ["Shared/Chat.Shared.csproj", "Shared/"]
RUN dotnet restore "./Client/Chat.Client.csproj"

# Copy the rest of the files and build
COPY . .
RUN dotnet build "Client/Chat.Client.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet publish "Client/Chat.Client.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:PublishTrimmed=false

# Final stage
FROM nginx:stable-alpine as final
WORKDIR /usr/share/nginx/html
COPY --from=build /app/publish/wwwroot .
EXPOSE 80
EXPOSE 443
CMD ["nginx", "-g", "daemon off;"]