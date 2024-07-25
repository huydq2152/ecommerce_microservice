## Prepare environment

- Install dotnet core version in file `global.json`
- IDE: Visual Studio 2022+, Rider, Visual Studio Code
- Docker Desktop
- EF Core tools reference (.NET CLI):

```Powershell
dotnet tool install --global dotnet-ef
```

---

## Warning:

Some docker images are not compatible with Apple Chip (M1, M2). You should replace them with appropriate images. Suggestion images below:

- sql server: mcr.microsoft.com/azure-sql-edge
- mysql: arm64v8/mysql:oracle

---

## How to run the project

Run command for build project

```Powershell
dotnet build
```

Go to folder contain file `docker-compose`

1. Using docker-compose

```Powershell
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d --remove-orphans
```

## Application URLs - LOCAL Environment (Docker Container):

- Identity API: http://localhost:6001
- Product API: http://localhost:6002/api/products
- Customer API: http://localhost:6003/api/customers
- Basket API: http://localhost:6004/api/baskets
- Order API: http://localhost:6005/api/v1/orders
- Inventory API: http://localhost:6006/api/inventory
- Inventory GRPC: http://localhost:6007
- Scheduled Job API: http://localhost:6008
- Ocelot Api Gateway: http://localhost:6020
- Webstatus Health Check: http://localhost:6010

## Docker Application URLs - LOCAL Environment (Docker Container):

- Portainer: http://localhost:9000 - username: admin ; pass: "{your-password}"
- Kibana: http://localhost:5601 - username: elastic ; pass: admin
- RabbitMQ: http://localhost:15672 - username: guest ; pass: guest

2. Using Visual Studio 2022

- Open aspnetcore-microservices.sln - `aspnetcore-microservices.sln`
- Run Compound to start multi projects

---

## Application URLs - DEVELOPMENT Environment:

- Identity API: http://localhost:5001
- Product API: http://localhost:5002/api/products
- Customer API: http://localhost:5003/api/customers
- Basket API: http://localhost:5004/api/baskets
- Order API: http://localhost:5005/api/v1/orders
- Inventory API: http://localhost:5006/api/inventory
- Inventory GRPC: http://localhost:5007
- Scheduled Job API: http://localhost:5008
- Ocelot Api Gateway: http://localhost:5020
- Webstatus Health Check: http://localhost:5010

---

## Application URLs - PRODUCTION Environment:

---

## Packages References

## Install Environment

- https://dotnet.microsoft.com/download/dotnet/6.0
- https://visualstudio.microsoft.com/
- https://www.jetbrains.com/rider/
- Install dotnet tool install --global dotnet-ef

## Docker Commands: (cd into folder contain file `docker-compose.yml`, `docker-compose.override.yml`)

- Up & running:

```Powershell
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d --remove-orphans --build
```

- Stop & Removing:

```Powershell
docker-compose down
```

# If docker desktop takes up too much disk space

1. Run docker desktop
2. Run cmd

- docker system df
- docker image prune
- docker volume prune
- docker builder prune

3. Go to %APPDATA%\Docker or %LOCALAPPDATA%\Docker and delete this folder to delete all log file of docker

## Useful commands:

- ASPNETCORE_ENVIRONMENT=Production dotnet ef database update
- dotnet watch run --environment "Development"
- dotnet restore
- dotnet build
- Migration commands for Ordering API:
  - cd into Ordering folder
  - dotnet ef migrations add "SampleMigration" -p Ordering.Infrastructure --startup-project Ordering.API -o Persistence/Migrations
  - dotnet ef migrations remove -p Ordering.Infrastructure --startup-project Ordering.API
  - dotnet ef database update -p Ordering.Infrastructure --startup-project Ordering.API
