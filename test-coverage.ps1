if (Test-Path -Path .\coverage) {
  Remove-Item .\coverage -Recurse -Force
}

if (Test-Path -Path .\coverage-logs) {
  Remove-Item .\coverage-logs -Recurse -Force
}

if (Test-Path -Path .\coverage-report) {
  Remove-Item .\coverage-report -Recurse -Force
}

Remove-Item .\coverage\coverage-*.cobertura.xml -ErrorAction SilentlyContinue

dotnet-coverage collect "dotnet test --no-restore --nologo" --settings coverage.xml -f cobertura -o .\coverage\coverage.cobertura.xml

reportgenerator -reports:".\coverage\coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:"html"
