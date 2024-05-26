FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["IdentityServer/IdentityServer/IdentityServer.csproj", "IdentityServer/IdentityServer/"]
#COPY ["IdentityServer/IdentityServer.Infrastructure/IdentityServer.Infrastructure.csproj", "IdentityServer/IdentityServer.Infrastructure/IdentityServer.Infrastructure/"]
#COPY ["IdentityServer/IdentityServer.Presentation/IdentityServer.Presentation.csproj", "IdentityServer/IdentityServer.Presentation/IdentityServer.Presentation/"]
RUN dotnet restore "IdentityServer/IdentityServer/IdentityServer.csproj"
COPY . .
WORKDIR "/src/IdentityServer/IdentityServer"
RUN dotnet build "IdentityServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IdentityServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityServer.dll"]
