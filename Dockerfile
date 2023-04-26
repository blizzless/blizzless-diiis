FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 9800/tcp
EXPOSE 1119/tcp
EXPOSE 80/tcp
EXPOSE 9100/tcp
EXPOSE 2001/tcp

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY /src/Blizzless-D3.sln ./
COPY /src/DiIiSNet/*.csproj ./DiIiSNet/
COPY /src/DiIiS-NA/*.csproj ./DiIiS-NA/

RUN dotnet restore
COPY /src/. .

WORKDIR /src
RUN dotnet build -c Debug --property:PublishDir=/app

FROM build AS publish
RUN dotnet publish -c Debug --property:PublishDir=/app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Blizzless.dll"]