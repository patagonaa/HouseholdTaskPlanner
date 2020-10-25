FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app/src
COPY src/ .
RUN dotnet restore TaskPlanner/TaskPlanner.TelegramBot.Cli/TaskPlanner.TelegramBot.Cli.csproj
RUN dotnet publish -c Release TaskPlanner/TaskPlanner.TelegramBot.Cli/TaskPlanner.TelegramBot.Cli.csproj -o /app/build

FROM mcr.microsoft.com/dotnet/core/runtime:3.1
WORKDIR /app
COPY --from=build /app/build/ ./
ENTRYPOINT ["dotnet", "TaskPlanner.TelegramBot.Cli.dll"]