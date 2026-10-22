[CmdletBinding()]
param(
    [switch]$NoSign
)

$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSScriptRoot

$projects = @(
    @{ Name = 'ASM02'; Solution = 'FA26_PRN222_SE1905_ASM02_QuangTT\FA26_PRN222_SE1905_ASM02_QuangTT.sln' },
    @{ Name = 'ASM01'; Solution = 'FA26_PRN222_SE1905_ASM01_QuangTT\FA26_PRN222_SE1905_ASM01_QuangTT.sln' },
    @{ Name = 'ASM03'; Solution = 'FA26_PRN222_SE1905_ASM03_QuangTT\FA26_PRN222_SE1905_ASM03_QuangTT.sln' },
    @{ Name = 'ASM04'; Solution = 'FA26_PRN222_SE1905_ASM04_QuangTT\FA26_PRN222_SE1905_ASM04_QuangTT.slnx' }
)

if (-not $NoSign -and -not (Get-Command pwsh -ErrorAction SilentlyContinue)) {
    throw 'PowerShell 7 (pwsh) is required by the local code-signing build target. Install it or run with -NoSign on a machine without application control.'
}

foreach ($project in $projects) {
    $solution = Join-Path $repoRoot $project.Solution
    Write-Host ""
    Write-Host "=== Building $($project.Name) ===" -ForegroundColor Cyan

    $arguments = @('build', $solution, '--nologo', '-v:minimal')
    if ($NoSign) {
        $arguments += '-p:EnableLocalCodeSigning=false'
    }

    & dotnet @arguments
    if ($LASTEXITCODE -ne 0) {
        throw "Build failed for $($project.Name)."
    }
}

$canonicalDir = Join-Path $repoRoot 'FA26_PRN222_SE1905_ASM02_QuangTT\zMembershipFee.RazorWebApp.QuangTT\bin\Debug\net8.0'
$sharedNames = @(
    'zMembershipFee.Entities.QuangTT.dll',
    'zMembershipFee.Repositories.QuangTT.dll',
    'zMembershipFee.Services.QuangTT.dll'
)

$appDirs = @{
    ASM01 = 'FA26_PRN222_SE1905_ASM01_QuangTT\zMembershipFee.MVCWebApp.QuangTT\bin\Debug\net8.0'
    ASM02 = 'FA26_PRN222_SE1905_ASM02_QuangTT\zMembershipFee.RazorWebApp.QuangTT\bin\Debug\net8.0'
    ASM03 = 'FA26_PRN222_SE1905_ASM03_QuangTT\zMembershipFee.BlazorWebApp.QuangTT\bin\Debug\net8.0'
    ASM04 = 'FA26_PRN222_SE1905_ASM04_QuangTT\zMembershipFee.WorkerService.QuangTT\bin\Debug\net8.0'
}

Write-Host ""
Write-Host '=== Shared assembly verification ===' -ForegroundColor Cyan
foreach ($entry in $appDirs.GetEnumerator() | Sort-Object Name) {
    $appDir = Join-Path $repoRoot $entry.Value
    foreach ($name in $sharedNames) {
        $canonical = Join-Path $canonicalDir $name
        $candidate = Join-Path $appDir $name
        if (-not (Test-Path -LiteralPath $candidate -PathType Leaf)) {
            Write-Warning "$($entry.Name): missing $name"
            continue
        }

        $same = (Get-FileHash -LiteralPath $canonical).Hash -eq (Get-FileHash -LiteralPath $candidate).Hash
        if ($same) {
            Write-Host "  $($entry.Name): $name OK"
        }
        else {
            Write-Warning "$($entry.Name): $name hash differs from ASM02"
        }
    }
}

Write-Host ""
Write-Host 'Build complete.' -ForegroundColor Green
