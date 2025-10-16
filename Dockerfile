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

# Cài EF Core CLI
RUN apt-get update && apt-get install -y wget unzip \
    && dotnet tool install --global dotnet-ef \
    && ln -s /root/.dotnet/tools/dotnet-ef /usr/local/bin/dotnet-ef

# Add dotnet tools to PATH
ENV PATH="$PATH:/root/.dotnet/tools"

# --- Tùy chọn: Environment (Cloud Run có thể override ENV này)
ENV ASPNETCORE_ENVIRONMENT=Production

# --- 🧠 Migration trước khi chạy app
CMD dotnet ef database update -p MuseumSystem.Infrastructure -s MuseumSystem.Api && \
    dotnet MuseumSystem.Api.dll