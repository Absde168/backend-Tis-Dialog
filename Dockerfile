FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY deaplom/deaplom.csproj ./deaplom/
RUN dotnet restore ./deaplom/deaplom.csproj

COPY deaplom/ ./deaplom/
WORKDIR /src/deaplom
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Render использует порт 10000 для Docker-сервисов
ENV ASPNETCORE_URLS=http://+:10000
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 10000

ENTRYPOINT ["dotnet", "deaplom.dll"]
