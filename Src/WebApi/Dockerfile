#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
RUN dotnet tool install --tool-path /tools dotnet-trace
RUN dotnet tool install --tool-path /tools dotnet-counters
RUN dotnet tool install --tool-path /tools dotnet-dump
WORKDIR /src
COPY ["Src/WebApi/WebApi.csproj", "Src/WebApi/"]
COPY ["Src/Domain/Domain.csproj", "Src/Domain/"]
COPY ["Src/Infra/Infra.csproj", "Src/Infra/"]
RUN dotnet restore "Src/WebApi/WebApi.csproj"
COPY . .
WORKDIR "/src/Src/WebApi"
RUN dotnet build "WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /tools
COPY --from=build /tools .
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApi.dll"]