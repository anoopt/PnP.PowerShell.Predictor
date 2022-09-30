
$PSDefaultParameterValues.Clear()
Set-StrictMode -Version Latest

if ($true -and ($PSEdition -eq 'Desktop')) {
    throw "Windows PowerShell is not supported. Please install PowerShell Core 7.2 or higher version."
}

if ($true -and ($PSEdition -eq 'Core')) {
    if ($PSVersionTable.PSVersion -lt [Version]'7.2.0') {
        throw "Current PnP.PowerShell.Predictor version doesn't support PowerShell Core versions lower than 7.2.0. Please upgrade to PowerShell Core 7.2.0 or higher."
    }
}

$psReadlineModule = Get-Module -Name PSReadLine
$minimumRequiredVersion = [version]"2.2.2"
$shouldImportPredictor = $true

if ($psReadlineModule -ne $null -and $psReadlineModule.Version -lt $minimumRequiredVersion) {
    $shouldImportPredictor = $false
    throw "This module requires PSReadLine version $minimumRequiredVersion. An earlier version of PSReadLine is imported in the current PowerShell session. Please open a new session before importing this module."
}
elseif ($psReadlineModule -eq $null) {
    try {
        Import-Module PSReadLine -MiniumVersion $minimumRequiredVersion -Scope Global
    }
    catch {
        $shouldImportPredictor = $false
        throw "This module requires PSReadLine version $minimumRequiredVersion. Please install PSReadLine $minimumRequiredVersion or higher."
    }
}

$pnpPowerShellModule = Get-Module -Name PnP.PowerShell -ListAvailable | Select Name,Version

if($pnpPowerShellModule -eq $null) {
    $shouldImportPredictor = $false
    throw "Please make sure you have installed PnP.PowerShell module. See - https://pnp.github.io/powershell/"
} else {
    Import-Module -Name PnP.PowerShell -Scope Global -WarningAction Ignore
}

# Import the predictor module
if ($shouldImportPredictor) {

    # Get all the functions
    $functions = @( Get-ChildItem -Path $PSScriptRoot\scripts\*.ps1 -ErrorAction SilentlyContinue )

    # Dot source all the functions
    $functions | ForEach-Object {
        . $_.FullName
    }

    # Export all the functions
    $functions | ForEach-Object {
        Export-ModuleMember -Function $_.BaseName
    }

    Import-Module (Join-Path -Path $PSScriptRoot -ChildPath PnP.PowerShell.Predictor.dll)
}