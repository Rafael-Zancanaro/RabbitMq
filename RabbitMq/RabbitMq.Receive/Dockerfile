#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RabbitMq.Receive/RabbitMq.Receive.csproj", "RabbitMq.Receive/"]
RUN dotnet restore "RabbitMq.Receive/RabbitMq.Receive.csproj"
COPY . .
WORKDIR "/src/RabbitMq.Receive"
RUN dotnet build "RabbitMq.Receive.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RabbitMq.Receive.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RabbitMq.Receive.dll"]