# Base image để chạy app (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Image để build source
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj và restore theo solution
COPY ["MuseumSystem.Api/MuseumSystem.Api.csproj", "MuseumSystem.Api/"]
COPY ["MuseumSystem.Application/MuseumSystem.Application.csproj", "MuseumSystem.Application/"]
COPY ["MuseumSystem.Domain/MuseumSystem.Domain.csproj", "MuseumSystem.Domain/"]
COPY ["MuseumSystem.Infrastructure/MuseumSystem.Infrastructure.csproj", "MuseumSystem.Infrastructure/"]

RUN dotnet restore "./MuseumSystem.Api/MuseumSystem.Api.csproj"

# Copy toàn bộ source và build
COPY . .
WORKDIR "/src/MuseumSystem.Api"
RUN dotnet build "./MuseumSystem.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish ra thư mục /app/publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MuseumSystem.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image để chạy app
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:$PORT
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
ENTRYPOINT ["dotnet", "MuseumSystem.Api.dll"]