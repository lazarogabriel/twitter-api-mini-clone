# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copia el csproj primero para restaurar dependencias
COPY src/twitter.api.web/*.csproj ./src/twitter.api.web/
WORKDIR /app/src/twitter.api.web
RUN dotnet restore

# Copia el resto del codigo 
WORKDIR /app
COPY . .

# Compila y publica
WORKDIR /app/src/twitter.api.web
RUN dotnet publish -c Release -o /app/out

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "twitter.api.web.dll"]