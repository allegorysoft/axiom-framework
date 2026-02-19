param (
    [Alias("s")]
    $source="http://localhost:5555/v3/index.json",
    [Alias("k")]
    $key="key"
)

. "$PSScriptRoot/base.ps1"

foreach ($solution in $solutions) {    

    $packages = Join-Path (Split-Path -Parent $solution) "**/*.nupkg"

    dotnet nuget push $packages -s $source -k $key
    if (-Not $?) {
        Write-Host ("Push failed for the solution: " + $solution)
        exit $LASTEXITCODE
    }
}
