FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore MatchMaking/MatchMaking.csproj
RUN dotnet publish MatchMaking/MatchMaking.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .

ENV DOTNET_DbgEnableMiniDump=1
ENV DOTNET_DbgMiniDumpType=4
ENV DOTNET_DbgMiniDumpName=/dumps/coredump.dmp

EXPOSE 8080
ENTRYPOINT ["sh", "-c", "mkdir -p /dumps && dotnet /app/MatchMaking.dll"]