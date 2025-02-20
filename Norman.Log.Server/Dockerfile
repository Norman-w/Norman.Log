#注意,执行docker build时

#✅
#A: 如果执行目录在 [当前文件所在的目录] ,应当执行的语句为
#docker build .. -f ../Norman.Log.Server/Dockerfile -t norman.log.server:1.0
#因为我们需要当前文件夹的兄弟文件夹中的文件内容,需要使用..来指定上下文为父级目录,否则无法访问到兄弟文件夹中的文件内容导致无法找到要COPY的文件的错误
#而且必须要指定 -f参数指向Dockerfile所在的位置,因为docker build的上下文在父级了,父级中并没有Dockerfile(就算有也是用错了文件),所以要通过这样的方式指定此文件为Dockerfile

#✅
#B: 如果执行目录为该文件的 [上一级] 目录,也就是Norman.Log,则执行的语句应该为
#docker build . -f Norman.Log.Server/Dockerfile -t norman.log.server:1.0
#因为此时上下文就是Norman.Log,下面的文件夹都可以访问,比如Norman.Log.Component.DatabaseWriter等文件夹中的文件都可以.
#但是也要指定 -f参数指向Dockerfile所在的位置,因为docker build的上下文在Norman.Log,并没有Dockerfile(就算有也是用错了文件),所以要通过这样的方式指定此文件为Dockerfile

#❌
#C: 如果执行目录还是当前目录,并且直接写 
#docker build . -t norman.log.server:1.0 
#是不可行的,因为这样会导致无法找到要COPY的文件,因为上下文在当前目录,无法访问父级和兄弟文件夹中的文件内容


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
