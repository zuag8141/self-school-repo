[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$TargetPath
)

$ErrorActionPreference = 'Stop'

$OutputDirectory = Split-Path -Parent -Path $TargetPath

if (-not (Test-Path -LiteralPath $OutputDirectory -PathType Container)) {
    Write-Warning "Output directory not found: $OutputDirectory"
    exit 0
}

$subject = 'CN=zMembershipFee Local Dev Code Signing'

# Reuse one local code-signing certificate for all builds. Smart App Control /
# WDAC application control policies block newly built, unsigned assemblies with
# 0x800711C7; Authenticode-signing the build output keeps local builds runnable
# without disabling the machine policy.
$certificate = Get-ChildItem Cert:\CurrentUser\My -CodeSigningCert -ErrorAction SilentlyContinue |
    Where-Object { $_.Subject -eq $subject -and $_.HasPrivateKey } |
    Sort-Object NotAfter -Descending |
    Select-Object -First 1

if (-not $certificate) {
    $certificate = New-SelfSignedCertificate `
        -Type CodeSigningCert `
        -Subject $subject `
        -CertStoreLocation 'Cert:\CurrentUser\My' `
        -KeyExportPolicy Exportable `
        -NotAfter (Get-Date).AddYears(5)

    Write-Host "Created local code-signing certificate $($certificate.Thumbprint) in CurrentUser\My."
}

$files = @(
    Get-ChildItem -LiteralPath $OutputDirectory -File -ErrorAction SilentlyContinue |
        Where-Object {
            $_.Extension -in '.dll', '.exe' -and
            $_.Name -match '^zMembershipFee\..*\.QuangTT\.(dll|exe)$'
        }
)

foreach ($file in $files) {
    try {
        Set-AuthenticodeSignature -LiteralPath $file.FullName -Certificate $certificate -HashAlgorithm SHA256 | Out-Null
    }
    catch {
        Write-Warning "Could not code-sign $($file.Name): $($_.Exception.Message)"
    }
}

Write-Host "Code-signed $($files.Count) output file(s) for application control compatibility."
