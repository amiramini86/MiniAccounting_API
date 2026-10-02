
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY MiniAccounting.slnx ./
COPY src/MiniAccounting.Api/MiniAccounting.Api.csproj src/MiniAccounting.Api/
COPY src/MiniAccounting.Application/MiniAccounting.Application.csproj src/MiniAccounting.Application/
COPY src/MiniAccounting.Domain/MiniAccounting.Domain.csproj src/MiniAccounting.Domain/
COPY src/MiniAccounting.Infrastructure/MiniAccounting.Infrastructure.csproj src/MiniAccounting.Infrastructure/

RUN dotnet restore src/MiniAccounting.Api/MiniAccounting.Api.csproj

COPY src/ ./src/

RUN dotnet publish src/MiniAccounting.Api/MiniAccounting.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "MiniAccounting.Api.dll"]