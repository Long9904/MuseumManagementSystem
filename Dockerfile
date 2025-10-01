# -------------------
# 1. Build stage
# -------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj và restore
COPY *.csproj ./
RUN dotnet restore

# Copy toàn bộ source code
COPY . ./

# Publish release
RUN dotnet publish -c Release -o out

# -------------------
# 2. Runtime stage
# -------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy kết quả publish từ build stage
COPY --from=build /app/out .

# Cloud Run yêu cầu listen port 8080
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Chạy app: tên DLL chính = project
ENTRYPOINT ["dotnet", "MuseumSystem.Api.dll"]