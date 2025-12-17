# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set working directory
WORKDIR /src

# Copy the project file
COPY server/ZhmApi.csproj ./server/

# Restore dependencies for the main project only
RUN dotnet restore ./server/ZhmApi.csproj

# Copy the rest of the application code
COPY . .

# Build and publish the application
WORKDIR /src/server
RUN dotnet publish ZhmApi.csproj -c Release -o /app/publish --no-restore

# Use the official .NET runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set working directory
WORKDIR /app

# Copy the published app from the build stage
COPY --from=build /app/publish .

# Expose the port the app runs on
EXPOSE 8080

# Set the entry point
ENTRYPOINT ["dotnet", "ZhmApi.dll"]