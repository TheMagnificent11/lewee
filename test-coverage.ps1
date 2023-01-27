if (Test-Path -Path .\coverage) {
  Remove-Item .\coverage -Force
}

if (Test-Path -Path .\coverage-report) {
  Remove-Item .\coverage-report -Force
}

dotnet test .\Lewee-CI.sln  --collect "XPlat Code Coverage" --results-directory ./coverage
reportgenerator -reports:"./coverage/**/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:"html"
