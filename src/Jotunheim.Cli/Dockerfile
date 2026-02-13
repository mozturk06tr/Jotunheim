FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Jotunheim.Cli/Jotunheim.Cli.csproj", "Jotunheim.Cli/"]
RUN dotnet restore "Jotunheim.Cli/Jotunheim.Cli.csproj"
COPY . .
WORKDIR "/src/Jotunheim.Cli"
RUN dotnet build "./Jotunheim.Cli.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Jotunheim.Cli.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Jotunheim.Cli.dll"]
