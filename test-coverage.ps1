rm -r ./coverage
rm -r ./coverage-report
dotnet test  --collect "XPlat Code Coverage" --results-directory ./coverage
reportgenerator -reports:"./coverage/**/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:"html"
