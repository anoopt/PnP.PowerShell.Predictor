using System.Management.Automation;
using System.Reflection;

namespace PnP.PowerShell.Predictor.Commands;

[Cmdlet(VerbsLifecycle.Enable, "PnPPredictorToolSuggestions")]
public class EnablePnPPredictorToolSuggestions: PSCmdlet
{
    private static readonly string[] Statements =
    {
        "Write-Host 'PnP PowerShell Predictor enabled.'",
    };
    
    protected override void ProcessRecord()
    {
        foreach (var statement in Statements)
        {
            this.InvokeCommand.InvokeScript(statement);
        }
    }
}