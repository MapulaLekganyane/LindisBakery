# ===== Build Stage =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy everything
COPY . .

# Restore dependencies
RUN dotnet restore

# Build & publish
RUN dotnet publish -c Release -o /app/publish

# ===== Runtime Stage =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy published output
COPY --from=build /app/publish .

# Render port
EXPOSE 8080

# Tell ASP.NET to use Render port
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

# Start application
ENTRYPOINT ["dotnet", "LindisBakery.dll"]