# Multi-platform Unity batch builder.
#
# Usage:
#   .\build.ps1                 # build all
#   .\build.ps1 windows
#   .\build.ps1 linux
#   .\build.ps1 mac
#   .\build.ps1 webgl
#
# Override Unity location:
#   $env:UNITY_PATH = "C:\Program Files\Unity\Hub\Editor\6000.3.14f1\Editor\Unity.exe"

[CmdletBinding()]
param(
    [ValidateSet("all", "windows", "linux", "mac", "webgl")]
    [string]$Target = "all"
)

$ErrorActionPreference = "Stop"

$ProjectPath = $PSScriptRoot
$LogDir = Join-Path $ProjectPath "Builds\_logs"
$BuildsRoot = Join-Path $ProjectPath "Builds"
New-Item -ItemType Directory -Force -Path $LogDir | Out-Null

$UnityVersion = "6000.3.14f1"
$DefaultUnity = "C:\Program Files\Unity\Hub\Editor\$UnityVersion\Editor\Unity.exe"

if ($env:UNITY_PATH -and (Test-Path $env:UNITY_PATH)) {
    $UnityExe = $env:UNITY_PATH
} elseif (Test-Path $DefaultUnity) {
    $UnityExe = $DefaultUnity
} else {
    Write-Error "Unity $UnityVersion not found at '$DefaultUnity' and `$env:UNITY_PATH unset/invalid."
    exit 1
}

Write-Host "Unity: $UnityExe" -ForegroundColor DarkGray
Write-Host "Project: $ProjectPath" -ForegroundColor DarkGray

$LockFile = Join-Path $ProjectPath "Temp\UnityLockfile"
if (Test-Path $LockFile) {
    Write-Error "UnityLockfile present at '$LockFile'. Editor likely has project open. Close Unity Editor and retry. (If Editor not running, delete the lockfile manually.)"
    exit 1
}

# (label, executeMethod, expected artifact relative to Builds/)
$Targets = @{
    windows = @("BuildScript.BuildWindows", "Windows\SpringGameJam2026.exe")
    linux   = @("BuildScript.BuildLinux",   "Linux\SpringGameJam2026.x86_64")
    mac     = @("BuildScript.BuildMac",     "Mac\SpringGameJam2026.app")
    webgl   = @("BuildScript.BuildWebGL",   "WebGL\index.html")
}

function Invoke-UnityBuild {
    param([string]$Name)

    $entry = $Targets[$Name]
    $method = $entry[0]
    $expected = Join-Path $BuildsRoot $entry[1]
    $log = Join-Path $LogDir "$Name.log"
    if (Test-Path $log) { Remove-Item $log -Force }

    Write-Host ""
    Write-Host ">>> Building $Name -> $expected" -ForegroundColor Cyan
    Write-Host "    log: $log"

    $argList = @(
        "-batchmode",
        "-nographics",
        "-quit",
        "-projectPath", "`"$ProjectPath`"",
        "-executeMethod", $method,
        "-logFile", "`"$log`""
    )

    $proc = Start-Process -FilePath $UnityExe -ArgumentList $argList -NoNewWindow -Wait -PassThru
    $exit = $proc.ExitCode

    # Tail last 40 lines of log so console shows what happened.
    if (Test-Path $log) {
        Write-Host "--- log tail ($Name) ---" -ForegroundColor DarkGray
        Get-Content $log -Tail 40 | ForEach-Object { Write-Host $_ -ForegroundColor DarkGray }
        Write-Host "--- end log tail ---" -ForegroundColor DarkGray
    } else {
        Write-Warning "No log written. Unity may not have started."
    }

    if ($exit -ne 0) {
        Write-Error "Unity exited $exit for $Name. Inspect log: $log"
        exit $exit
    }

    if (-not (Test-Path $expected)) {
        Write-Error "Build $Name reported success (exit 0) but expected artifact missing: $expected. Inspect log: $log"
        exit 1
    }

    Write-Host "<<< $Name OK ($expected)" -ForegroundColor Green
}

if ($Target -eq "all") {
    foreach ($t in @("windows","linux","mac","webgl")) { Invoke-UnityBuild $t }
} else {
    Invoke-UnityBuild $Target
}

Write-Host ""
Write-Host "Builds in: $BuildsRoot" -ForegroundColor Yellow
