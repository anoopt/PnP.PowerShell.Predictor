using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;

namespace PnP.PowerShell.Predictor.Abstractions.Interfaces
{
    internal interface IPnPPowerShellContext
    {
        public Version PnPPowerShellVersion { get; }
        public void UpdateContext();
        public Runspace DefaultRunspace { get; }
    }
}
