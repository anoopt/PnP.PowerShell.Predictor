using System.Collections.Generic;
using System.Management.Automation.Runspaces;

namespace PnP.PowerShell.Predictor.Abstractions.Interfaces
{
    using PowerShell = System.Management.Automation.PowerShell;

    /// <summary>
    /// A PowerShell environment to run PowerShell cmdlets and scripts.
    /// </summary>
    internal interface IPowerShellRuntime
    {
        /// <summary>
        /// Gets the minimum PowerShell Runspace. This isn't the necessary the same one as the PowerShell environment that Az
        /// Predictor is running on.
        /// </summary>
        Runspace DefaultRunspace { get; }

        /// <summary>
        /// The PowerShell environment that the module is imported into.
        /// </summary>
        PowerShell? ConsoleRuntime { get; }

        /// <summary>
        /// Gets the current PowerShell host name.
        /// </summary>
        public string? HostName { get; }

        /// <summary>
        /// Executes the PowerShell cmdlet in the current powershell session.
        /// </summary>
        IList<T> ExecuteScript<T>(string contents);
    }
}
