using System.Management.Automation;
using System.Reflection;
using System.Text;
using PnP.PowerShell.Predictor.Abstractions.Models;

namespace PnP.PowerShell.Predictor.Commands
{
    [Cmdlet(VerbsCommon.Set, "PnPPredictorToolSearch")]
    public class SetPnPPredictorToolSearch : PSCmdlet
    {
        private static readonly string[] ReloadModuleStatements =
        {
            "Remove-Module -Name PnP.PowerShell.Predictor -Force",
#if DEBUG
            $"Import-Module {Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, PnPPowerShellPredictorConstants.LibraryName)} -Force"
#else
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
                Environment
                    .SetEnvironmentVariable(
                        PnPPowerShellPredictorConstants.EnvironmentVariableShowWarning,
                        "false"
                    );
                InvokeCommand.InvokeScript(scriptToRun.ToString());
            }
        }
    }
}