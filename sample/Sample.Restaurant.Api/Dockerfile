#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Build.targets", "./"]
COPY ["Packages.props", "./"]
COPY ["sample/Sample.Restaurant.Api/Sample.Restaurant.Api.csproj", "sample/Sample.Restaurant.Api/"]
COPY ["src/Lewee.Infrastructure.AspNet.WebApi/Lewee.Infrastructure.AspNet.WebApi.csproj", "src/Lewee.Infrastructure.AspNet.WebApi/"]
COPY ["src/Lewee.Infrastructure.AspNet/Lewee.Infrastructure.AspNet.csproj", "src/Lewee.Infrastructure.AspNet/"]
COPY ["src/Lewee.Application/Lewee.Application.csproj", "src/Lewee.Application/"]
COPY ["src/Lewee.Contracts/Lewee.Contracts.csproj", "src/Lewee.Contracts/"]
COPY ["src/Lewee.Domain/Lewee.Domain.csproj", "src/Lewee.Domain/"]
COPY ["src/Lewee.Shared/Lewee.Shared.csproj", "src/Lewee.Shared/"]
COPY ["sample/Sample.Restaurant.Infrastructure/Sample.Restaurant.Infrastructure.csproj", "sample/Sample.Restaurant.Infrastructure/"]
COPY ["src/Lewee.Infrastructure.Data/Lewee.Infrastructure.Data.csproj", "src/Lewee.Infrastructure.Data/"]
COPY ["sample/Sample.Restaurant.Application/Sample.Restaurant.Application.csproj", "sample/Sample.Restaurant.Application/"]
COPY ["sample/Sample.Restaurant.Contracts/Sample.Restaurant.Contracts.csproj", "sample/Sample.Restaurant.Contracts/"]
COPY ["sample/Sample.Restaurant.Domain/Sample.Restaurant.Domain.csproj", "sample/Sample.Restaurant.Domain/"]
RUN dotnet restore "sample/Sample.Restaurant.Api/Sample.Restaurant.Api.csproj"
COPY . .
WORKDIR "/src/sample/Sample.Restaurant.Api"
RUN dotnet build "Sample.Restaurant.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sample.Restaurant.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sample.Restaurant.Api.dll"]