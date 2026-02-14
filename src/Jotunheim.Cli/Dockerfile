FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Jotunheim.slnx .
COPY src/Shared.Domain/Shared.Domain.csproj src/Shared.Domain/
COPY src/Shared.Infrastructure/Shared.Infrastructure.csproj src/Shared.Infrastructure/
COPY src/Jotunheim.Cli/Jotunheim.Cli.csproj src/Jotunheim.Cli/
COPY src/Gateway.Api/Gateway.Api.csproj src/Gateway.Api/

RUN dotnet restore Jotunheim.slnx

COPY . .
RUN dotnet publish src/Jotunheim.Cli/Jotunheim.Cli.csproj -c Release -o /out /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /out ./

ENTRYPOINT ["dotnet", "Jotunheim.Cli.dll"]
