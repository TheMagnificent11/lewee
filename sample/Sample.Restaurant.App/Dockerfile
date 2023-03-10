# Use the official .NET SDK image as the base image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Build.targets", "./"]
COPY ["Packages.props", "./"]
COPY ["sample/Sample.Restaurant.App/Sample.Restaurant.App.csproj", "sample/Sample.Restaurant.App/"]
COPY ["sample/Sample.Restaurant.Contracts/Sample.Restaurant.Contracts.csproj", "sample/Sample.Restaurant.Contracts/"]
COPY ["src/Lewee.Blazor/Lewee.Blazor.csproj", "src/Lewee.Blazor/"]
COPY ["src/Lewee.Contracts/Lewee.Contracts.csproj", "src/Lewee.Contracts/"]
COPY ["src/Lewee.Shared/Lewee.Shared.csproj", "src/Lewee.Shared/"]
RUN dotnet restore "sample/Sample.Restaurant.App/Sample.Restaurant.App.csproj"
COPY . .
WORKDIR "/src/sample/Sample.Restaurant.App"
RUN dotnet build "Sample.Restaurant.App.csproj" -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/build ./
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "Sample.Restaurant.App.dll"]
