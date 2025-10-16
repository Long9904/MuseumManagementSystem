# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY MuseumManagementSystem.sln ./
COPY MuseumSystem.Api/*.csproj ./MuseumSystem.Api/
COPY MuseumSystem.Domain/*.csproj ./MuseumSystem.Domain/
COPY MuseumSystem.Application/*.csproj ./MuseumSystem.Application/
COPY MuseumSystem.Infrastructure/*.csproj ./MuseumSystem.Infrastructure/

RUN dotnet restore MuseumSystem.Api/MuseumSystem.Api.csproj
COPY . .
WORKDIR /src/MuseumSystem.Api
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Copy EF tools from build stage
COPY --from=build /root/.dotnet /root/.dotnet
ENV PATH="$PATH:/root/.dotnet/tools"

# CMD: migrate + start app
CMD dotnet ef database update -p MuseumSystem.Infrastructure -s MuseumSystem.Api && \
    dotnet MuseumSystem.Api.dll