using System.Management.Automation;
using PnP.PowerShell.Predictor.Abstractions.Models;

namespace PnP.PowerShell.Predictor.Commands
{
    [Cmdlet(VerbsCommon.Set, "PnPPredictorSearch")]
    public class SetPnPPredictorSearch : Cmdlet
    {
        [Parameter(Mandatory = true, Position = 0)]
        public CommandSearchMethod Method { get; set; }

        protected override void ProcessRecord()
        {
            if (Method.GetType() == typeof(CommandSearchMethod))
            {
                Environment
                    .SetEnvironmentVariable(
                        PnPPowerShellPredictorConstants.EnvironmentVariableCommandSearchMethod,
                        Method.ToString()
                    );
            }

        }
    }
}
