# base
FROM mcr.microsoft.com/dotnet/aspnet:11.0 AS base

# install packages as root, then drop to the non-root app user
RUN apt-get update && apt-get install -y --no-install-recommends \
	curl \
	&& rm -rf /var/lib/apt/lists/*

USER $APP_UID

WORKDIR /app

EXPOSE 8080
EXPOSE 8081

# build
FROM mcr.microsoft.com/dotnet/sdk:11.0 AS build
ARG BUILD_CONFIGURATION=Release

# nuget credential provider (Azure Artifacts)
# https://github.com/microsoft/artifacts-credprovider

# nuget credential helper
ARG NUGET_FEED_URL
ARG NUGET_USER
ARG NUGET_PAT
ARG NUGET_CRED_URL="https://aka.ms/install-artifacts-credprovider.sh"

ENV VSS_NUGET_EXTERNAL_FEED_ENDPOINTS="{\"endpointCredentials\": [{\"endpoint\":\"${NUGET_FEED_URL}\", \"username\":\"${NUGET_USER}\", \"password\":\"${NUGET_PAT}\"}]}"
RUN curl -fsSL ${NUGET_CRED_URL} | bash

WORKDIR /src

# nuget
COPY ["nuget.config", "."]

# restore (project files only, so this layer is cached until a csproj changes)
COPY ["projects/api/MrJB.API.Sample/MrJB.API.Sample.csproj", "projects/api/MrJB.API.Sample/"]
COPY ["projects/api/MrJB.Sample.Application/MrJB.Sample.Application.csproj", "projects/api/MrJB.Sample.Application/"]
COPY ["projects/api/MrJB.Sample.Data/MrJB.Sample.Data.csproj", "projects/api/MrJB.Sample.Data/"]
COPY ["projects/api/MrJB.Sample.Domain/MrJB.Sample.Domain.csproj", "projects/api/MrJB.Sample.Domain/"]

RUN dotnet restore "./projects/api/MrJB.API.Sample/MrJB.API.Sample.csproj"

COPY . .

WORKDIR "/src/projects/api/MrJB.API.Sample"

RUN dotnet build "./MrJB.API.Sample.csproj" -c $BUILD_CONFIGURATION -o /app/build --no-restore

# publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MrJB.API.Sample.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore

# final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MrJB.API.Sample.dll"]
