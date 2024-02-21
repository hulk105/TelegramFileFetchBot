FROM mcr.microsoft.com/dotnet/aspnet:7.0.5-alpine3.17-amd64 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["TelegramFileFetchBot.App/TelegramFileFetchBot.App.csproj", "TelegramFileFetchBot.App/"]
RUN dotnet restore "TelegramFileFetchBot.App/TelegramFileFetchBot.App.csproj"
COPY . .
WORKDIR /src/TelegramFileFetchBot.App
RUN dotnet build "TelegramFileFetchBot.App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TelegramFileFetchBot.App.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN ls -al
RUN ["mkdir", "/downloads"]
ENTRYPOINT ["dotnet", "TelegramFileFetchBot.dll"]
