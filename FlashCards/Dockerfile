# Use the official Microsoft SQL Server image from the Docker Hub
FROM mcr.microsoft.com/mssql/server:2019-latest AS sqlserver

# Set environment variables for SQL Server
ENV SA_PASSWORD=P@assword1
ENV ACCEPT_EULA=Y

# Expose the SQL Server port
EXPOSE 1433

# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Set the working directory
WORKDIR /app

# Copy the project files
COPY . .

# Restore the dependencies
RUN dotnet restore

# Build the application
RUN dotnet publish -c Release -o out

# Use the official .NET runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime

# Set the working directory
WORKDIR /app

# Copy the built files from the build stage
COPY --from=build /app/out .

# Expose the application port
EXPOSE 80

# Set the entry point for the application
ENTRYPOINT ["dotnet", "FlashCards.dll"]