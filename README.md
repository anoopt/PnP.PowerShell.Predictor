# PnP PowerShell Predictor module

## Overview

[PnP PowerShell Predictor](https://www.powershellgallery.com/packages/PnP.PowerShell.Predictor) is a PowerShell
module that helps you navigate the cmdlets and parameters of
[PnP PowerShell](https://pnp.github.io/powershell/). It provides suggestions for command completion when using PnP PowerShell cmdlets.

PnP PowerShell Predictor uses the
[subsystem plugin model](/powershell/scripting/learn/experimental-features#pssubsystempluginmodel)
available in PowerShell 7.2. This updated version requires
[PSReadLine 2.2.2](https://www.powershellgallery.com/packages/PSReadLine/2.2.2) or higher to display
suggestions.

![demo](./assets/demo.gif)

## Requirements

Required configuration for PnP PowerShell Predictor:

- [PowerShell 7.2](https://github.com/PowerShell/PowerShell/) or higher
- [PSReadline 2.2.2](https://github.com/PowerShell/PSReadLine/) or higher

Install the latest version of PSReadLine:

```powershell
Install-Module -Name PSReadline
```

#### Set preferred source and view for suggestions

Enable predictions from history and plugins

```powershell
Set-PSReadLineOption -PredictionSource HistoryAndPlugin
```

Enable list view:

```powershell
Set-PSReadLineOption -PredictionViewStyle ListView
```

## Getting started

### Install PnP PowerShell Predictor

To install the PnP.PowerShell.Predictor PowerShell module run the following

```powershell
Install-Module -Name PnP.PowerShell.Predictor
```

### Import PnP PowerShell Predictor

To import the PnP PowerShell Predictor PowerShell module in the current session run the following

```powershell
Import-Module -Name PnP.PowerShell.Predictor
```

### Use PnP PowerShell Predictor

Once imported, start typing PnP PowerShell cmdlet (e.g. `Connect-PnPOnline`) and see the predictions loading.

### Predictions source warning

The module tries to get the version of the cmdlets from GitHub based on the PnP PowerShell version on your machine. In case, it is not able to find the version of cmdlets from GitHub then it will load a set of predictions that are shipped with the module.

Hence some commands from the predictions might not be present in the version on PnP PowerShell on your machine and they might not work.

To disable this warning, set the environment vaiable `PnPPredictorShowWarning` to false in your profile.ps1 file.

```powershell
$env:PnPPredictorShowWarning = "false"
```
## Changing predictions search method

By default the module uses `Contains` search i.e. it shows predictions that contain the input entered. This can be changed to either `StartsWith` or `Fuzzy` by using the following cmdlet

```powershell
Set-PnPPredictorToolSearch -Method StartsWith|Contains|Fuzzy
```

`StartsWith` - as per the name shows predictions that start with the entered input
`Fuzzy` - does a Fuzzy search and returns predictions. Sometimes the results might not be as per the expectaion in this case.

## Disabling PnP PowerShell Predictor suggestions in the current session

To disable PnP PowerShell Predictor suggestions in the current session run the following

```powershell
Disable-PnPPredictorToolSuggestions
```

## Enabling PnP PowerShell Predictor suggestions in the current session

To enable PnP PowerShell Predictor suggestions in the current session run the following

```powershell
Enable-PnPPredictorToolSuggestions
```
## Uninstallation

Once installed and enabled, PnP PowerShell Predictor is loaded in the PowerShell profile.
To uninstall the PnP.PowerShell.Predictor module:

1. Close **all** PowerShell sessions including VS Code.

1. Launch a PowerShell session with no profile.

   ```powershell
   pwsh -noprofile
   ```

1. Uninstall PnP PowerShell Predictor

   ```powershell
   Uninstall-Module -Name PnP.PowerShell.Predictor -Force
   ```

1. Close PowerShell