using System.Management.Automation;
using System.Management.Automation.Runspaces;
using PnP.PowerShell.Predictor.Abstractions.Interfaces;

namespace PnP.PowerShell.Predictor
{
    internal sealed class PnPPowerShellContext : IPnPPowerShellContext
    {
        private static readonly Version DefaultVersion = new Version("0.0.0");
        private readonly IPowerShellRuntime _powerShellRuntime;

        public Version PnPPowerShellVersion { get; private set; } = DefaultVersion;

        public PnPPowerShellContext(IPowerShellRuntime powerShellRuntime) => _powerShellRuntime
            = powerShellRuntime;

        public Runspace DefaultRunspace => _powerShellRuntime.DefaultRunspace;

        public void UpdateContext()
        {
            PnPPowerShellVersion = GetPnPPowerShellVersion();
        }

        private Version GetPnPPowerShellVersion()
        {
            var latestPnPPowerShellVersion = DefaultVersion;

            try
            {
                var outputs = _powerShellRuntime.ExecuteScript<PSObject>("Get-Module -Name PnP.PowerShell -ListAvailable");
                
                if (outputs.Any())
                {
                    ExtractAndSetLatestPnPPowerShellVersion(outputs);
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return latestPnPPowerShellVersion;

            void ExtractAndSetLatestPnPPowerShellVersion(IEnumerable<PSObject> outputs)
            {
                foreach (var psObject in outputs)
                {
                    var versionOutput = psObject.Properties["Version"].Value.ToString();
                    if (versionOutput == null) continue;
                    var currentPnPPowerShellVersion = new Version(versionOutput);
                    if (currentPnPPowerShellVersion > latestPnPPowerShellVersion)
                    {
                        latestPnPPowerShellVersion = currentPnPPowerShellVersion;
                    }
                }
            }
        }
    }
}
