#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["sample/Sample.BlazorServer.App/Sample.BlazorServer.App.csproj", "sample/Sample.BlazorServer.App/"]
COPY ["sample/Sample.Identity.Infrastructure/Sample.Identity.Infrastructure.csproj", "sample/Sample.Identity.Infrastructure/"]
COPY ["sample/Sample.Customers.Infrastructure/Sample.Customers.Infrastructure.csproj", "sample/Sample.Customers.Infrastructure/"]
RUN dotnet restore "sample/Sample.BlazorServer.App/Sample.BlazorServer.App.csproj"
COPY . .
WORKDIR "/src/sample/Sample.BlazorServer.App"
RUN dotnet build "Sample.BlazorServer.App.csproj" -c Release -o /app/build
RUN dotnet ef database update -p "../Sample.Identity.Infrastructure" -c "IdentityDbContext"
RUN dotnet ef database update -p "../Sample.Customers.Infrastructure" -c "CustomersDbContext"

FROM build AS publish
RUN dotnet publish "Sample.BlazorServer.App.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sample.BlazorServer.App.dll"]