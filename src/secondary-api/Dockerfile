FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Secondary.csproj", "Secondary/"]
RUN dotnet restore "Secondary.csproj"
COPY . .
WORKDIR "/src/Secondary"
RUN dotnet build "Secondary.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Secondary.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Secondary.dll"]