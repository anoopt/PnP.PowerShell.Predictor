function Update-PnPPowerShellPredictions {
    try {
        # Get version of the PnP.PowerShell module
        $module = Get-Module -Name PnP.PowerShell -ListAvailable | Sort-Object -Property Version -Descending | Select-Object -First 1
        $version = $module.Version.ToString();
        #$version = $(Get-InstalledModule -Name "PnP.PowerShell").Version;

        # Get the predictions file location from GitHub based on the version
        $url = "https://raw.githubusercontent.com/pnp/powershell/dev/src/Predictor/Data/PnP.Predictor.Suggestions_$($version).json";

        # Get the local path to the predictions file in Data folder
        $localPath = "$($PSScriptRoot)\Data\PnP.PowerShell.Suggestions.json";

        # Save the file to the local machine
        Invoke-WebRequest -Uri $url -OutFile $localPath;
    }
    catch {
        throw "Unable to update PnP PowerShell predictions. Hence using the one shipped with the module.";
    }
}