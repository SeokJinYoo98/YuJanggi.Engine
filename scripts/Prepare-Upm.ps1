# Generated files are disposable; edit only the original .NET sources.
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidatePattern('^[A-Za-z][A-Za-z0-9]*$')]
    [string]$Target
)

$ErrorActionPreference = 'Stop'

$repoRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$sourceRoot = Join-Path $repoRoot 'src'
$runtimeRoot = Join-Path $repoRoot 'upm/Runtime'
$generatedRoot = [IO.Path]::GetFullPath((Join-Path $runtimeRoot 'Generated'))
$expectedRoot = [IO.Path]::GetFullPath((Join-Path $repoRoot 'upm/Runtime/Generated'))

if ($generatedRoot -ne $expectedRoot -or
    -not $generatedRoot.StartsWith($repoRoot + [IO.Path]::DirectorySeparatorChar)) {
    throw 'Generated output must stay inside this repository.'
}

# Resolve project from target.
$project = Join-Path $sourceRoot "YuJanggi.$Target.csproj"

if (-not (Test-Path -LiteralPath $project)) {
    throw "Project file not found: $project"
}

# Resolve the actual Compile items, so future csproj includes/excludes are respected.
$itemsJson = & dotnet msbuild `
    $project `
    -nologo `
    -p:TargetFramework=netstandard2.1 `
    -getItem:Compile

if ($LASTEXITCODE -ne 0) {
    throw 'Could not read project Compile items.'
}

$sourceFiles = @((($itemsJson -join "`n") | ConvertFrom-Json).Items.Compile)

if ($sourceFiles.Count -eq 0) {
    throw "No $Target source files found."
}

foreach ($item in $sourceFiles) {
    if (-not $item.FullPath.StartsWith(
            $sourceRoot + [IO.Path]::DirectorySeparatorChar,
            [StringComparison]::OrdinalIgnoreCase)) {

        throw "Source outside the project requires an explicit export mapping: $($item.FullPath)"
    }
}

# Recreate generated source directory.
if (Test-Path -LiteralPath $generatedRoot) {
    Remove-Item -LiteralPath $generatedRoot -Recurse -Force
}

New-Item -ItemType Directory -Path $generatedRoot -Force | Out-Null

foreach ($item in $sourceFiles) {
    $relative = $item.FullPath.Substring($sourceRoot.Length + 1)
    $destination = Join-Path $generatedRoot $relative

    New-Item `
        -ItemType Directory `
        -Path (Split-Path $destination) `
        -Force | Out-Null

    Copy-Item `
        -LiteralPath $item.FullPath `
        -Destination $destination
}

# Stable GUIDs avoid Unity reference churn across regenerations.
$packageRoot = Join-Path $repoRoot 'upm'
$packageName = "com.seokjinyoo.yujanggi.$($Target.ToLowerInvariant())"

$assets =
    @(Get-ChildItem -LiteralPath $generatedRoot -Recurse) +
    @(Get-Item $generatedRoot)

$hasher = [Security.Cryptography.MD5]::Create()

try {
    foreach ($asset in $assets) {
        $assetPath = $asset.FullName.Substring($packageRoot.Length + 1).Replace('\', '/')

        $hash = $hasher.ComputeHash(
            [Text.Encoding]::UTF8.GetBytes(
                "$packageName/$assetPath"
            )
        )

        $guid = ([BitConverter]::ToString($hash)).Replace('-', '').ToLowerInvariant()

        $meta = "fileFormatVersion: 2`nguid: $guid`n"

        if ($asset.PSIsContainer) {
            $meta += "folderAsset: yes`n"
        }

        [IO.File]::WriteAllText(
            $asset.FullName + '.meta',
            $meta
        )
    }
}
finally {
    $hasher.Dispose()
}

Write-Host "Generated $($sourceFiles.Count) $Target source files in $generatedRoot"
