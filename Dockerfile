# ======================
# Stage 1: Build
# ======================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file (nếu có)
COPY MuseumManagementSystem.sln ./
COPY MuseumSystem.Api/*.csproj ./MuseumSystem.Api/
COPY MuseumSystem.Domain/*.csproj ./MuseumSystem.Domain/
COPY MuseumSystem.Application/*.csproj ./MuseumSystem.Application/
COPY MuseumSystem.Infrastructure/*.csproj ./MuseumSystem.Infrastructure/

# Restore dependencies
RUN dotnet restore MuseumSystem.Api/MuseumSystem.Api.csproj

# Copy toàn bộ source code
COPY . .

WORKDIR /src/MuseumSystem.Api

# Cài đặt EF CLI (phải cài ở SDK image)
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# 👉 Chạy migration ở đây
RUN dotnet ef database update -p ../MuseumSystem.Infrastructure -s .

# Build app
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false


# ======================
# Stage 2: Runtime
# ======================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy app từ build stage
COPY --from=build /app/publish .

# Start app
ENTRYPOINT ["dotnet", "MuseumSystem.Api.dll"]