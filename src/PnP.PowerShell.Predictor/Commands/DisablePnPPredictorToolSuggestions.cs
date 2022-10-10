using System.Management.Automation;
using System.Reflection;

namespace PnP.PowerShell.Predictor.Commands;

[Cmdlet(VerbsLifecycle.Disable, "PnPPredictorToolSuggestions")]
public class DisablePnPPredictorToolSuggestions : PSCmdlet
{
    private static readonly string[] Statements =
    {
        "Remove-Module -Name PnP.PowerShell.Predictor -Force",
        "Write-Host 'PnP PowerShell Predictor disabled.'",
    };

    protected override void ProcessRecord()
    {
        foreach (var statement in Statements)
        {
            this.InvokeCommand.InvokeScript(statement);
        }
    }
}