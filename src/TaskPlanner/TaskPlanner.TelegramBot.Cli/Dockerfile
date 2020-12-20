FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
# this is not actually needed for this container but it increases build time because the build cache from TaskPlanner.Web can be used
RUN curl -sL https://deb.nodesource.com/setup_12.x | bash - && apt install -y nodejs && rm -rf /var/lib/apt/lists/*
WORKDIR /app/src
COPY src/ .
RUN dotnet restore HouseholdPlanner.sln
RUN dotnet publish -c Release TaskPlanner/TaskPlanner.TelegramBot.Cli/TaskPlanner.TelegramBot.Cli.csproj -o /app/build

FROM mcr.microsoft.com/dotnet/core/runtime:3.1
WORKDIR /app
COPY --from=build /app/build/ ./
ENTRYPOINT ["dotnet", "TaskPlanner.TelegramBot.Cli.dll"]