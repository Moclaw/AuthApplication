# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY AuthApplication.csproj .
RUN dotnet restore AuthApplication.csproj
COPY . .
RUN dotnet build AuthApplication.csproj -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish AuthApplication.csproj -c Release -o /app/publish

# Stage 3: Final
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 5000
ENTRYPOINT ["dotnet", "AuthApplication.dll"]
