FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Norman.Log.Server/Norman.Log.Server.csproj", "Norman.Log.Server/"]
COPY ["Norman.Log.Component.DatabaseReader/Norman.Log.Component.DatabaseReader.csproj", "Norman.Log.Component.DatabaseReader/"]
COPY ["Norman.Log.Component.Database.Mysql/Norman.Log.Component.Database.Mysql.csproj", "Norman.Log.Component.Database.Mysql/"]
COPY ["Norman.Log/Norman.Log.csproj", "Norman.Log/"]
COPY ["Norman.Log.Component.DatabaseWriter/Norman.Log.Component.DatabaseWriter.csproj", "Norman.Log.Component.DatabaseWriter/"]
COPY ["Norman.Log.Component.FileWriter/Norman.Log.Component.FileWriter.csproj", "Norman.Log.Component.FileWriter/"]
RUN dotnet restore "Norman.Log.Server/Norman.Log.Server.csproj"
COPY . .
WORKDIR "/src/Norman.Log.Server"
RUN dotnet build "Norman.Log.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Norman.Log.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Norman.Log.Server.dll"]
