if (Test-Path -Path .\coverage) {
  Remove-Item .\coverage -Recurse -Force
}

if (Test-Path -Path .\coverage-logs) {
  Remove-Item .\coverage-logs -Recurse -Force
}

if (Test-Path -Path .\coverage-report) {
  Remove-Item .\coverage-report -Recurse -Force
}

dotnet-coverage collect "dotnet test .\lewee.sln --configuration Debug --no-restore --nologo" --include-files "src/Lewee.*/bin/Debug/net9.0/Lewee.*.dll" -f cobertura -o .\coverage\coverage.cobertura.xml
reportgenerator -reports:".\coverage\coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:"html"
