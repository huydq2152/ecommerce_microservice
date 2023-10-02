FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Services/ScheduledJob/Hangfire.API/Hangfire.API.csproj", "src/Services/ScheduledJob/Hangfire.API/"]
COPY ["src/BuildingBlocks/Common.Logging/Common.Logging.csproj", "src/BuildingBlocks/Common.Logging/"]
COPY ["src/BuildingBlocks/Contracts/Contracts.csproj", "src/BuildingBlocks/Contracts/"]
COPY ["src/BuildingBlocks/Shared/Shared.csproj", "src/BuildingBlocks/Shared/"]
COPY ["src/BuildingBlocks/Infrastructure/Infrastructure.csproj", "src/BuildingBlocks/Infrastructure/"]
RUN dotnet restore "src/Services/ScheduledJob/Hangfire.API/Hangfire.API.csproj"
COPY . .
WORKDIR "/src/src/Services/ScheduledJob/Hangfire.API"
RUN dotnet build "Hangfire.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Hangfire.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Hangfire.API.dll"]
