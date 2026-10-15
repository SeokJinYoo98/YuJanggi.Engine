[CmdletBinding()]
param(
    [string]$PackageDirectory = 'artifacts/upm'
)

$ErrorActionPreference = 'Stop'
$repoRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$packagePath = if ([IO.Path]::IsPathRooted($PackageDirectory)) {
    $PackageDirectory
}
else {
    Join-Path $repoRoot $PackageDirectory
}

$packages = @(Get-ChildItem -LiteralPath $packagePath -Filter '*.tgz' -File)
if ($packages.Count -eq 0) {
    throw 'No UPM packages were found to publish.'
}

foreach ($package in $packages) {
    # Read the packed metadata; do not rebuild or change the release artifact.
    $metadataJson = & tar -xOf $package.FullName package/package.json
    if ($LASTEXITCODE -ne 0) {
        throw "Could not read UPM package metadata: $($package.Name)"
    }

    $metadata = ($metadataJson -join "`n") | ConvertFrom-Json
    $tag = if ($metadata.version -match '-') { 'next' } else { 'latest' }

    Write-Host "Publishing $($metadata.name)@$($metadata.version) ($tag)"
    & npm publish $package.FullName --registry https://registry.npmjs.org --access public --tag $tag
    if ($LASTEXITCODE -ne 0) {
        throw "UPM publish failed: $($package.Name)"
    }
}
