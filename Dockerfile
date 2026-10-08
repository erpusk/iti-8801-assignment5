# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

WORKDIR /src

COPY BoardgamesApi/BoardgamesApi.csproj BoardgamesApi/
RUN dotnet restore BoardgamesApi/BoardgamesApi.csproj

COPY BoardgamesApi/ BoardgamesApi/

RUN dotnet publish BoardgamesApi/BoardgamesApi.csproj \
    -c Release \
    -o /app/publish \
    --no-restore \
    --no-self-contained


# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final

WORKDIR /app

COPY --from=build /app/publish .

USER $APP_UID

EXPOSE 8080

ENTRYPOINT ["dotnet", "BoardgamesApi.dll"]