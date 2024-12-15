# Use the .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source
EXPOSE 8080


# Copy the solution file and restore dependencies as distinct layers
COPY Server/DeadPigeons.sln ./Server/
COPY Server/API/*.csproj ./Server/API/
COPY Server/DataAccess/*.csproj ./Server/DataAccess/
COPY Server/Service/*.csproj ./Server/Service/
COPY Server/Tests/*.csproj ./Server/Tests/

# Run dotnet restore on the solution file
RUN dotnet restore ./Server/DeadPigeons.sln

# Copy the rest of the application code and build it
COPY Server/API/. ./Server/API/
COPY Server/DataAccess/. ./Server/DataAccess/
COPY Server/Service/. ./Server/Service/
COPY Server/Tests/. ./Server/Tests/

# Publish the application to the /app directory
RUN dotnet publish ./Server/DeadPigeons.sln -c Release -o /app --no-restore

# Use the .NET ASP.NET runtime image for the final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./

# Set the environment variable for ASP.NET Core to listen on port 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

# Set the entry point for the application
ENTRYPOINT ["dotnet", "API.dll"]
