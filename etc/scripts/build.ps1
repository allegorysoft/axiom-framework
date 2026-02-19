param(
    [Alias("c")]
    [string]$configuration = "Debug"
)

. "$PSScriptRoot/base.ps1"

foreach ($solution in $solutions) {    
    dotnet build $solution -c $configuration
    if (-Not $?) {
        Write-Host ("Build failed for the solution: " + $solution)
        exit $LASTEXITCODE
    }
}