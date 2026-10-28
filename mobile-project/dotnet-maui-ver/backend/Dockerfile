# Multi-stage Dockerfile for ExpenseTracker WebApi (.NET 10)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["ExpenseTracker.Domain/ExpenseTracker.Domain.csproj", "ExpenseTracker.Domain/"]
COPY ["ExpenseTracker.Application/ExpenseTracker.Application.csproj", "ExpenseTracker.Application/"]
COPY ["ExpenseTracker.Infrastructure/ExpenseTracker.Infrastructure.csproj", "ExpenseTracker.Infrastructure/"]
COPY ["ExpenseTracker.WebApi/ExpenseTracker.WebApi.csproj", "ExpenseTracker.WebApi/"]

RUN dotnet restore "ExpenseTracker.WebApi/ExpenseTracker.WebApi.csproj"

# Copy full source and build
COPY . .
WORKDIR "/src/ExpenseTracker.WebApi"
RUN dotnet build "ExpenseTracker.WebApi.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "ExpenseTracker.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 5018

ENV ASPNETCORE_URLS=http://+:5018
ENV ASPNETCORE_ENVIRONMENT=Development

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExpenseTracker.WebApi.dll"]
