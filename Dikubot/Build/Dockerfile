FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src

COPY ["../Dikubot.csproj", "."]
RUN dotnet restore "Dikubot.csproj"
COPY . .
RUN dotnet build "Dikubot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Dikubot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dikubot.dll"]