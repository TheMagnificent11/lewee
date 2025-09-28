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
Remove-Item .\coverage\coverage-*.cobertura.xml -ErrorAction SilentlyContinue

# Dynamically find unit test projects (exclude integration tests)
$unitTestProjects = @()
$testDirs = @("tests", "sample-tests")

foreach ($testDir in $testDirs) {
    if (Test-Path $testDir) {
        $projects = Get-ChildItem -Path $testDir -Recurse -Filter "*.csproj" | Where-Object { 
            ($_.Name -match "(Tests\.Unit|Domain\.Tests)") -and ($_.Name -notmatch "Integration") 
        }
        foreach ($project in $projects) {
            $unitTestProjects += $project.FullName
        }
    }
}

$unitTestProjects = $unitTestProjects | Sort-Object

Write-Host "Found unit test projects:"
$unitTestProjects | ForEach-Object { Write-Host "  $_" }

$counter = 1
foreach ($project in $unitTestProjects) {
    Write-Host "Running coverage for: $project"
    dotnet-coverage collect "dotnet test `"$project`" --configuration Debug --no-restore --nologo" --include-files "src/Lewee.*/bin/Debug/net9.0/Lewee.*.dll" -f cobertura -o ".\coverage\coverage-$counter.cobertura.xml"
    $counter++
}

# Merge coverage files if any were created
if (Test-Path ".\coverage\coverage-*.cobertura.xml") {
    Write-Host "Merging coverage files..."
    dotnet-coverage merge ".\coverage\coverage-*.cobertura.xml" -f cobertura -o .\coverage\coverage.cobertura.xml
    Write-Host "Coverage files merged successfully"
} else {
    Write-Host "No coverage files found to merge"
}
reportgenerator -reports:".\coverage\coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:"html"
