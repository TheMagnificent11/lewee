if (Test-Path -Path .\coverage) {
  Remove-Item .\coverage -Recurse -Force
}

if (Test-Path -Path .\coverage-logs) {
  Remove-Item .\coverage-logs -Recurse -Force
}

if (Test-Path -Path .\coverage-report) {
  Remove-Item .\coverage-report -Recurse -Force
}

# Run only unit tests for coverage to avoid third-party library coverage
Remove-Item .\coverage\*-coverage.cobertura.xml -ErrorAction SilentlyContinue
dotnet-coverage collect "dotnet test tests/Lewee.Domain.Tests.Unit/Lewee.Domain.Tests.Unit.csproj --configuration Debug --no-restore --nologo" --include-files "src/Lewee.*/bin/Debug/net9.0/Lewee.*.dll" -f cobertura -o .\coverage\domain-coverage.cobertura.xml
dotnet-coverage collect "dotnet test tests/Lewee.Application.Tests.Unit/Lewee.Application.Tests.Unit.csproj --configuration Debug --no-restore --nologo" --include-files "src/Lewee.*/bin/Debug/net9.0/Lewee.*.dll" -f cobertura -o .\coverage\application-coverage.cobertura.xml
dotnet-coverage collect "dotnet test tests/Lewee.Shared.Tests.Unit/Lewee.Shared.Tests.Unit.csproj --configuration Debug --no-restore --nologo" --include-files "src/Lewee.*/bin/Debug/net9.0/Lewee.*.dll" -f cobertura -o .\coverage\shared-coverage.cobertura.xml
dotnet-coverage collect "dotnet test sample-tests/Pizzeria.Store.Domain.Tests/Pizzeria.Store.Domain.Tests.csproj --configuration Debug --no-restore --nologo" --include-files "src/Lewee.*/bin/Debug/net9.0/Lewee.*.dll" -f cobertura -o .\coverage\pizzeria-coverage.cobertura.xml
# Merge coverage files
dotnet-coverage merge ".\coverage\*-coverage.cobertura.xml" -f cobertura -o .\coverage\coverage.cobertura.xml
reportgenerator -reports:".\coverage\coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:"html"
