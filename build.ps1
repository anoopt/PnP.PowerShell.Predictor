[CmdletBinding(DefaultParameterSetName = 'Build')]
param(
    [Parameter(ParameterSetName = 'Build')]
    [ValidateSet('Debug', 'Release')]
    [string] $Configuration = 'Debug'
)


$srcDir = Join-Path $PSScriptRoot 'src\PnP.PowerShell.Predictor'
dotnet publish -c $Configuration $srcDir

Write-Host "`nThe module 'PnP.PowerShell.Predictor' is published to 'PnP.PowerShell.Predictor.Module\PnP.PowerShell.Predictor'`n" -ForegroundColor Green