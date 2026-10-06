[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidatePattern('^[A-Za-z][A-Za-z0-9]*$')]
    [string]$Target,
    [Parameter(Mandatory = $true)]
    [string]$Version
)

$ErrorActionPreference = 'Stop'

# SemVer: major.minor.patch, optional prerelease and build metadata.
$versionPattern = '^(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)(-(0|[1-9][0-9]*|[0-9]*[A-Za-z-][0-9A-Za-z-]*)(\.(0|[1-9][0-9]*|[0-9]*[A-Za-z-][0-9A-Za-z-]*))*)?(\+[0-9A-Za-z-]+(\.[0-9A-Za-z-]+)*)?$'
if ($Version -cnotmatch $versionPattern) {
    throw "Invalid version format: $Version"
}

$repoRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$projectPath = Join-Path $repoRoot "src/YuJanggi.$Target.csproj"
$versionPath = Join-Path $repoRoot 'src/Version.cs'
$packagePath = Join-Path $repoRoot 'upm/package.json'

# Validate and prepare all changes before writing any file.
$updates = @()
foreach ($path in @($versionPath, $projectPath, $packagePath)) {
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) {
        throw "Required file not found: $path"
    }
    $reader = [IO.StreamReader]::new($path, [Text.UTF8Encoding]::new($false), $true)
    try {
        $text = $reader.ReadToEnd()
        $encoding = $reader.CurrentEncoding
    }
    finally {
        $reader.Dispose()
    }

    if ($path -eq $versionPath) {
        $pattern = '(public\s+const\s+string\s+Current\s*=\s*")(?<value>[^"]*)(")'
    }
    elseif ($path -eq $projectPath) {
        [xml]$projectXml = $text
        $pattern = '<Version>\s*(?<value>[^<\s]+)\s*</Version>'
    }
    else {
        $package = $text | ConvertFrom-Json
        if ($null -eq $package.version -or $package.version -isnot [string]) {
            throw 'package.json must contain a string version.'
        }
        $pattern = '"version"\s*:\s*"(?<value>[^"]*)"'
    }

    $matches = [regex]::Matches($text, $pattern)
    if ($matches.Count -ne 1) {
        throw "Expected exactly one version field in: $path"
    }
    $value = $matches[0].Groups['value']
    $updated = $text.Remove($value.Index, $value.Length).Insert($value.Index, $Version)
    $updates += [pscustomobject]@{ Path = $path; Text = $updated; Encoding = $encoding; Changed = ($text -cne $updated) }
}

foreach ($update in $updates) {
    if ($update.Changed) {
        [IO.File]::WriteAllText($update.Path, $update.Text, $update.Encoding)
    }
}
Write-Host "Version updated: $Target $Version"
