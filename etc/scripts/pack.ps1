param(
    [Alias("c")]
    [string]$configuration = "Debug"
)

. "$PSScriptRoot/base.ps1"

foreach ($solution in $solutions) {    
    dotnet pack $solution --no-build -c $configuration
    if (-Not $?) {
        Write-Host ("Pack failed for the solution: " + $solution)
        exit $LASTEXITCODE
    }
}
