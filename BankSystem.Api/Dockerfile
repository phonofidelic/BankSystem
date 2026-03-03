FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY BankSystem.Api/BankSystem.Api.csproj BankSystem.Api/
COPY BankSystem.Application/BankSystem.Application.csproj BankSystem.Application/
COPY BankSystem.Domain/BankSystem.Domain.csproj BankSystem.Domain/
COPY BankSystem.Intrastructure/BankSystem.Infrastructure.csproj BankSystem.Intrastructure/

RUN dotnet restore BankSystem.Api/BankSystem.Api.csproj

COPY BankSystem.Api/ BankSystem.Api/
COPY BankSystem.Application/ BankSystem.Application/
COPY BankSystem.Domain/ BankSystem.Domain/
COPY BankSystem.Intrastructure/ BankSystem.Intrastructure/

RUN dotnet publish BankSystem.Api/BankSystem.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "BankSystem.Api.dll"]
