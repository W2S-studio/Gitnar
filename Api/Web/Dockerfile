FROM mcr.microsoft.com/dotnet/sdk:8.0 AS development
WORKDIR /app
EXPOSE 5000

COPY *.sln .
COPY Api/Web/*.csproj ./Api/Web/
COPY Api/Core/*.csproj ./Api/Core/
COPY Api/Infrastructure/*.csproj ./Api/Infrastructure/
COPY Api/Tests/*.csproj ./Api/Tests/

RUN dotnet restore "./Api/Web/Web.csproj"

COPY . .

WORKDIR /app/Api/Web

CMD ["dotnet", "watch", "run", "--urls", "http://0.0.0.0:5000"]

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Api/Web/*.csproj", "Api/Web/"]
COPY ["Api/Core/*.csproj", "Api/Core/"]
COPY ["Api/Infrastructure/*.csproj", "Api/Infrastructure/"]
COPY ["Api/Tests/*.csproj", "Api/Tests/"]
RUN dotnet restore "./Api/Web/Web.csproj"
COPY . .
WORKDIR "/src/Api/Web"
RUN dotnet build "./Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Web.dll"]