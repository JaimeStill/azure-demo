FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Primary.csproj", "Primary/"]
RUN dotnet restore "Primary.csproj"
COPY . .
WORKDIR "/src/Primary"
RUN dotnet build "Primary.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Primary.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Primary.dll"]