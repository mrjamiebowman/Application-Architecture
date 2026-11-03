# base
FROM mcr.microsoft.com/dotnet/aspnet:11.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# build
FROM mcr.microsoft.com/dotnet/sdk:11.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY ["projects/api/MrJB.API.Sample/MrJB.API.Sample.csproj","projects/api/MrJB.API.Sample/"]

RUN dotnet restore "./projects/api/MrJB.API.Sample/MrJB.API.Sample.csproj"

COPY . .

WORKDIR "/src/projects/api/MrJB.API.Sample"

RUN dotnet build "./MrJB.API.Sample.csproj" -c $BUILD_CONFIGURATION -o /app/build

# publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MrJB.API.Sample.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MrJB.API.Sample.dll"]