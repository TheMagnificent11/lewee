#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["sample/Sample.Restaurant.App/Sample.Restaurant.App.csproj", "sample/Sample.Restaurant.App/"]
COPY ["src/Lewee.Blazor/Lewee.Blazor.csproj", "src/Lewee.Blazor/"]
COPY ["src/Lewee.Shared/Lewee.Shared.csproj", "src/Lewee.Shared/"]
COPY ["src/Lewee.Infrastructure.AspNet/Lewee.Infrastructure.AspNet.csproj", "src/Lewee.Infrastructure.AspNet/"]
COPY ["src/Lewee.Application/Lewee.Application.csproj", "src/Lewee.Application/"]
COPY ["src/Lewee.Domain/Lewee.Domain.csproj", "src/Lewee.Domain/"]
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