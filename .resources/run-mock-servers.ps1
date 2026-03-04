# Starts Kerridge mock servers (Instance1, Instance2, Instance3) and cleans up on exit.
# Usage: pwsh -File .resources/run-mock-servers.ps1

[CmdletBinding()] param(
    [string]$WorkspaceRoot = ([System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))),
    [switch]$Build
)

$ErrorActionPreference = 'Stop'

function Write-Info($msg) { Write-Host ("[run-mock-servers] " + $msg) }

# Paths to the three sample instances
$instancePaths = @(
    [System.IO.Path]::Combine($WorkspaceRoot, 'kerridge-api', 'samples', 'Kerridge.Instance1'),
    [System.IO.Path]::Combine($WorkspaceRoot, 'kerridge-api', 'samples', 'Kerridge.Instance2'),
    [System.IO.Path]::Combine($WorkspaceRoot, 'kerridge-api', 'samples', 'Kerridge.Instance3')
)

# Default ports per instance (customize as needed)
$instancePorts = @(5101, 5102, 5103)

# Validate paths
foreach ($p in $instancePaths) {
    if (-not (Test-Path $p)) { throw "Project path not found: $p" }
}

if ($Build.IsPresent) {
    Write-Info 'Building sample instances...'
    foreach ($p in $instancePaths) {
        Write-Info "dotnet build -> $p"
        dotnet build $p | Write-Output
    }
}

# Track running jobs
$global:ServerJobs = @()

# Start each instance as a background job
for ($i = 0; $i -lt $instancePaths.Count; $i++) {
    $projPath = $instancePaths[$i]
    $label = "Instance$($i+1)"
    $port = $instancePorts[$i]
    Write-Info "Starting $label at $projPath"
    Write-Info "URL: http://localhost:$port"

    $job = Start-Job -Name "Kerridge.$label" -ScriptBlock {
        param($path, $url)
        Push-Location $path
        try {
            # Run the app; inherit environment
            dotnet run -- --urls $url
        } finally {
            Pop-Location
        }
    } -ArgumentList @($projPath, "http://localhost:$port")

    $global:ServerJobs += $job
}

Write-Info 'Servers starting... (this can take a few seconds)'

try {
    # Show simple status loop; Ctrl+C to exit
    while ($true) {
        Start-Sleep -Seconds 2
        $statuses = $global:ServerJobs | ForEach-Object { "{0}:{1}" -f $_.Name, $_.State }
        Write-Host ("[run-mock-servers] Status -> " + ($statuses -join ', '))
    }
}
finally {
    try {
        Write-Host '[run-mock-servers] Shutting down mock servers...'
        foreach ($j in $global:ServerJobs) {
            if ($null -ne $j) {
                try { Stop-Job -Job $j -Force -ErrorAction SilentlyContinue } catch {}
                try { Remove-Job -Job $j -Force -ErrorAction SilentlyContinue } catch {}
            }
        }
    } catch {}
}
