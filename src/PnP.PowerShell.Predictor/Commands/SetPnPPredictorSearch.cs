using System.Management.Automation;
using System.Reflection;
using System.Text;
using PnP.PowerShell.Predictor.Abstractions.Models;

namespace PnP.PowerShell.Predictor.Commands
{
    
    [Cmdlet(VerbsCommon.Set, "PnPPredictorSearch")]
    public class SetPnPPredictorSearch : PSCmdlet
    {
        private static readonly string[] ReloadModuleStatements = {
#if DEBUG
            $"Remove-Module {Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),PnPPowerShellPredictorConstants.LibraryName)} -Force",
            $"Import-Module {Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),PnPPowerShellPredictorConstants.LibraryName)} -Force"
#else
            "Remove-Module -Name PnP.PowerShell.Predictor -Force",
            "Import-Module -Name PnP.PowerShell.Predictor -Force"
#endif
            
        };
        
        [Parameter(Mandatory = true, Position = 0)]
        public CommandSearchMethod Method { get; set; }

        protected override void ProcessRecord()
        {
            var scriptToRun = new StringBuilder();
            var _ = scriptToRun.Append(string.Join(";", ReloadModuleStatements));
            
            if (Method.GetType() == typeof(CommandSearchMethod))
            {
                Environment
                    .SetEnvironmentVariable(
                        PnPPowerShellPredictorConstants.EnvironmentVariableCommandSearchMethod,
                        Method.ToString()
                    );
                InvokeCommand.InvokeScript(scriptToRun.ToString());
            }

        }
    }
}
