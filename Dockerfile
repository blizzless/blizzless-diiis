# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copy the project file and restore dependencies
COPY ["src/DiIiS-NA/Blizzless.csproj", "src/DiIiS-NA/"]
RUN dotnet restore "src/DiIiS-NA/Blizzless.csproj"

# Copy the rest of the project files and build the application
COPY ["src/", "src/"]
WORKDIR "/app/src/DiIiS-NA"
RUN dotnet publish "Blizzless.csproj" -c Release --runtime linux-x64 --self-contained true -o /app/publish

# Use the official .NET runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /app/publish .

# Expose the port your application is running on (if needed)
EXPOSE 1345 1119 83 2001 9800 9100
# Start the application
ENTRYPOINT ["./Blizzless"]