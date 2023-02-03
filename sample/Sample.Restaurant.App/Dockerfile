#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY "Directory.Build.props" .
COPY "Directory.Build.targets" .
COPY "Packages.props" .
COPY ["sample/Sample.Restaurant.App/Sample.Restaurant.App.csproj", "sample/Sample.Restaurant.App/"]
COPY ["src/Lewee.Fluxor/Lewee.Fluxor.csproj", "src/Lewee.Fluxor/"]
COPY ["src/Lewee.Shared/Lewee.Shared.csproj", "src/Lewee.Shared/"]
COPY ["src/Lewee.Infrastructure/Lewee.Infrastructure.csproj", "src/Lewee.Infrastructure/"]
COPY ["src/Lewee.Application/Lewee.Application.csproj", "src/Lewee.Application/"]
COPY ["src/Lewee.Domain/Lewee.Domain.csproj", "src/Lewee.Domain/"]
COPY ["sample/Sample.Restaurant.Infrastructure/Sample.Restaurant.Infrastructure.csproj", "sample/Sample.Restaurant.Infrastructure/"]
COPY ["sample/Sample.Restaurant.Application/Sample.Restaurant.Application.csproj", "sample/Sample.Restaurant.Application/"]
COPY ["sample/Sample.Restaurant.Contracts/Sample.Restaurant.Contracts.csproj", "sample/Sample.Restaurant.Contracts/"]
COPY ["sample/Sample.Restaurant.Domain/Sample.Restaurant.Domain.csproj", "sample/Sample.Restaurant.Domain/"]
RUN dotnet restore "sample/Sample.Restaurant.App/Sample.Restaurant.App.csproj"
COPY . .
WORKDIR "/src/sample/Sample.Restaurant.App"
RUN dotnet build "Sample.Restaurant.App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sample.Restaurant.App.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sample.Restaurant.App.dll"]