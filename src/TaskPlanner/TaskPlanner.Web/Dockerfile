FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
RUN curl -sL https://deb.nodesource.com/setup_12.x | bash - && apt install -y nodejs && rm -rf /var/lib/apt/lists/*
WORKDIR /app/src
COPY src/ .
RUN dotnet restore TaskPlanner/TaskPlanner.Web/TaskPlanner.Web.csproj
RUN dotnet publish -c Release TaskPlanner/TaskPlanner.Web/TaskPlanner.Web.csproj -o /app/build

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build /app/build/ ./
ENTRYPOINT ["dotnet", "TaskPlanner.Web.dll"]