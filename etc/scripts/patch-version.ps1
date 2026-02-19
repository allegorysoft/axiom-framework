param (
    [Alias("v")]
    $version="latest"
)

# Update the Directory.Build.props file
$xmlFile = Join-Path $PSScriptRoot "../../Directory.Build.props"
$xmlContent = Get-Content $xmlFile -Raw  # Read the file as a single string

# Extract the current version
if ($version -match '(\d+)\.(\d+)\.(\d+)') {
    $major = $matches[1]
    $minor = $matches[2]
    $patch = $matches[3]

    # Increment the minor version
    $newPatch = [int]$patch + 1
    $newVersion = "$major.$minor.$newPatch"

    # Replace only the version in the XML
    $xmlContent = $xmlContent -replace '<Version>\d+\.\d+\.\d+</Version>', "<Version>$newVersion</Version>"

    # Save the updated file
    $xmlContent | Set-Content $xmlFile -NoNewline
}