FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app/src
COPY src/ .
RUN dotnet restore User/User.Web/User.Web.csproj
RUN dotnet publish -c Release User/User.Web/User.Web.csproj -o /app/build

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build /app/build/ ./
ENTRYPOINT ["dotnet", "User.Web.dll"]