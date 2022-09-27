using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using PnP.PowerShell.Predictor.Abstractions.Interfaces;

namespace PnP.PowerShell.Predictor
{
    internal sealed class PnPPowerShellContext : IPnPPowerShellContext
    {
        private static readonly Version DefaultVersion = new Version("0.0.0.0");
        private IPowerShellRuntime _powerShellRuntime;

        public Version PnPPowerShellVersion { get; private set; } = DefaultVersion;

        public PnPPowerShellContext(IPowerShellRuntime powerShellRuntime) => _powerShellRuntime
            = powerShellRuntime;

        public void UpdateContext()
        {
            PnPPowerShellVersion = GetPnPPowerShellVersion();
        }

        private Version GetPnPPowerShellVersion()
        {
            Version latestPnPPowerShellVersion = DefaultVersion;

            try
            {
                var outputs = _powerShellRuntime.ExecuteScript<PSObject>("Get-Module -Name PnP.PowerShell -ListAvailable");
                
                if (outputs?.Any() == true)
                {
                    ExtractAndSetLatestPnPPowerShellVersion(outputs);
                }
            }
            catch (Exception)
            {
            }

            return latestPnPPowerShellVersion;

            void ExtractAndSetLatestPnPPowerShellVersion(IEnumerable<PSObject> outputs)
            {
                foreach (var psObject in outputs)
                {
                    string versionOutput = psObject.Properties["Version"].Value.ToString();
                    Version currentPnPPowerShellVersion = new Version(versionOutput);
                    if (currentPnPPowerShellVersion > latestPnPPowerShellVersion)
                    {
                        latestPnPPowerShellVersion = currentPnPPowerShellVersion;
                    }
                }
            }
        }
    }
}
