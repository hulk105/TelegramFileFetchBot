FROM mcr.microsoft.com/dotnet/aspnet:6.0.1-alpine3.14-amd64 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AmazingFileSaverBot/AmazingFileSaverBot.csproj", "AmazingFileSaverBot/"]
RUN dotnet restore "AmazingFileSaverBot/AmazingFileSaverBot.csproj"
COPY . .
WORKDIR /src/AmazingFileSaverBot
RUN dotnet build "AmazingFileSaverBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AmazingFileSaverBot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN ls -al
RUN ["mkdir", "/downloads"]
ENTRYPOINT ["dotnet", "AmazingFileSaverBot.dll"]
