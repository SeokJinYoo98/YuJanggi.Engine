[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$repoRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$config = Get-Content -LiteralPath (Join-Path $repoRoot 'workflowConfig.json') -Raw | ConvertFrom-Json
$keys = @('target', 'solutionPath', 'dotnetVersion', 'nodeVersion', 'nugetArtifactName', 'upmArtifactName')
foreach ($key in $keys) {
    $value = $config.$key
    if ($value -isnot [string] -or [string]::IsNullOrWhiteSpace($value) -or $value -match '[\r\n]') {
        throw "Invalid workflow configuration: $key"
    }
}
if ($config.target -notmatch '^[A-Za-z][A-Za-z0-9]*$') {
    throw 'Invalid workflow target.'
}
if (-not (Test-Path -LiteralPath (Join-Path $repoRoot $config.solutionPath) -PathType Leaf)) {
    throw 'Configured solution was not found.'
}
if (-not (Test-Path -LiteralPath (Join-Path $repoRoot "src/YuJanggi.$($config.target).csproj") -PathType Leaf)) {
    throw 'Configured target project was not found.'
}
if ($env:GITHUB_OUTPUT) {
    foreach ($key in $keys) {
        "$key=$($config.$key)" | Out-File -FilePath $env:GITHUB_OUTPUT -Encoding utf8 -Append
    }
}
else {
    $config
}
