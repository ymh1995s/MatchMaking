FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

# 서버 프로젝트만 지정
RUN dotnet restore MatchMaking/MatchMaking.csproj
RUN dotnet publish MatchMaking/MatchMaking.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .

EXPOSE 8080
ENTRYPOINT ["dotnet", "MatchMaking.dll"]