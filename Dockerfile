# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file nếu có, hoặc từng csproj
# Giả sử bạn có file MuseumSystem.sln
COPY MuseumManagementSystem.sln ./
COPY MuseumSystem.Api/*.csproj ./MuseumSystem.Api/
COPY MuseumSystem.Domain/*.csproj ./MuseumSystem.Domain/
COPY MuseumSystem.Application/*.csproj ./MuseumSystem.Application/
COPY MuseumSystem.Infrastructure/*.csproj ./MuseumSystem.Infrastructure/

# Restore dependencies (cache tốt)
RUN dotnet restore MuseumSystem.Api/MuseumSystem.Api.csproj

# Copy toàn bộ source code
COPY . .

# Publish release
WORKDIR /src/MuseumSystem.Api
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "MuseumSystem.Api.dll"]