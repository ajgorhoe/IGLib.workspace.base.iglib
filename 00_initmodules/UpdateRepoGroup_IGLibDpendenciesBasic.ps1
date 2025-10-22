
# Clones or updates all repositories needed by IGLib.
Write-Host "`n`nCloning / updating all repositories needed by IGLib ...`n"

# Get the script directory such that relative paths can be resolved:
$scriptPath = $MyInvocation.MyCommand.Path
$scriptDir = Split-Path $scriptPath -Parent
$scriptFilename = [System.IO.Path]::GetFileName($scriptPath)

Write-Host "Script directory: $scriptDir"

Write-Host "`nUpdating iglibexternal WITH contained dependencies:"
& $(Join-Path $scriptDir "UpdateRepo_iglibexternaWithIGLibDependenciesBasic.ps1.ps1")

Write-Host "`nUpdating IGLibCore:"
& $(Join-Path $scriptDir "UpdateRepo_IGLibCore.ps1")

Write-Host "`nUpdating IGLibScript:"
& $(Join-Path $scriptDir "UpdateRepo_IGLibScripts.ps1")

Write-Host "`nUpdating unittests:"
& $(Join-Path $scriptDir "UpdateRepo_unittests.ps1")


Write-Host "  ... updating repositoris needed by IGLib completed.`n`n"

