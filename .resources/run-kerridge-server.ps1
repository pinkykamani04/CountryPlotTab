# Starts IBMG.SCS.KerridgeApi.Server and cleans up on exit.
# Usage: pwsh -File .resources/run-kerridge-server.ps1 [-Build] [-Port 5200]

[CmdletBinding()] param(
    [string]$WorkspaceRoot = ([System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))),
    [switch]$Build,
    [int]$Port = 5200,
    [switch]$Https,
    [int]$HttpsPort = 5201,
    [string]$Environment = 'Development'
)

$ErrorActionPreference = 'Stop'

function Write-Info($msg) { Write-Host ("[run-kerridge-server] " + $msg) }

$projectPath = [System.IO.Path]::Combine($WorkspaceRoot, 'kerridge-api', 'source', 'IBMG.SCS.KerridgeApi.Server')
if (-not (Test-Path $projectPath)) { throw "Project path not found: $projectPath" }

if ($Build.IsPresent) {
    Write-Info "Building server at $projectPath"
    dotnet build $projectPath | Write-Output
}

Write-Info "Starting server at $projectPath"
if ($Https) {
    Write-Info "URLs: http://localhost:$Port, https://localhost:$HttpsPort"
} else {
    Write-Info "URL: http://localhost:$Port"
}

$httpUrl = "http://localhost:$Port"
$httpsUrl = "https://localhost:$HttpsPort"
$urls = if ($Https) { "$httpUrl;$httpsUrl" } else { $httpUrl }

$previousEnv = $Env:ASPNETCORE_ENVIRONMENT
try {
    $Env:ASPNETCORE_ENVIRONMENT = $Environment
    Push-Location $projectPath
    try {
        Write-Info "Running 'dotnet run -- --urls $urls' (env=$Environment)... (Ctrl+C to stop)"
        dotnet run -- --urls $urls
    } finally {
        Pop-Location
    }
}
finally {
    if ($null -ne $previousEnv) {
        $Env:ASPNETCORE_ENVIRONMENT = $previousEnv
    } else {
        Remove-Item Env:ASPNETCORE_ENVIRONMENT -ErrorAction SilentlyContinue
    }
    Write-Info 'Server exited.'
}
