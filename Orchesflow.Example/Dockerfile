FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Orchesflow.Example/Orchesflow.Example.csproj", "Orchesflow.Example/"]
RUN dotnet restore "Orchesflow.Example/Orchesflow.Example.csproj"
COPY . .
WORKDIR "/src/Orchesflow.Example"
RUN dotnet build "Orchesflow.Example.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Orchesflow.Example.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Orchesflow.Example.dll"]
