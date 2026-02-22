param(
    [Alias("c")]
    [string]$configuration = "Debug"
)

. "$PSScriptRoot/base.ps1"

foreach ($solution in $solutions) {    
    dotnet test $solution --no-build -c $configuration
    if (-Not $?) {
        Write-Host ("Test failed for the solution: " + $solution)
        exit $LASTEXITCODE
    }
}
