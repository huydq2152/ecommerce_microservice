FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Services/Inventory/Inventory.Grpc/Inventory.Grpc.csproj", "Services/Inventory/Inventory.Grpc/"]
COPY ["BuildingBlocks/Shared/Shared.csproj", "BuildingBlocks/Shared/"]
COPY ["BuildingBlocks/Infrastructure/Infrastructure.csproj", "BuildingBlocks/Infrastructure/"]
COPY ["BuildingBlocks/Common.Logging/Common.Logging.csproj", "BuildingBlocks/Common.Logging/"]
COPY ["BuildingBlocks/Contracts/Contracts.csproj", "BuildingBlocks/Contracts/"]
RUN dotnet restore "Services/Inventory/Inventory.Grpc/Inventory.Grpc.csproj"
COPY . .
WORKDIR "/src/Services/Inventory/Inventory.Grpc"
RUN dotnet build "Inventory.Grpc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Inventory.Grpc.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Inventory.Grpc.dll"]

