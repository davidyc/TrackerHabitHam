# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Сначала копируем .csproj из подпапки
COPY TrackerHabiHamApi/TrackerHabiHamApi.csproj ./TrackerHabiHamApi/
RUN dotnet restore ./TrackerHabiHamApi/TrackerHabiHamApi.csproj

# Потом копируем весь проект
COPY TrackerHabiHamApi/ ./TrackerHabiHamApi/

WORKDIR /src/TrackerHabiHamApi
RUN dotnet build "TrackerHabiHamApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TrackerHabiHamApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "TrackerHabiHamApi.dll"]
