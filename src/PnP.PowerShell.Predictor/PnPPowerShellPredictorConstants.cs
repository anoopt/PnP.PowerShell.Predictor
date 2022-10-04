using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PnP.PowerShell.Predictor
{
    internal static class PnPPowerShellPredictorConstants
    {
        public const string LocalSuggestionsFileName = "PnP.PowerShell.Suggestions.json";
        public const string LocalSuggestionsFileRelativePath = "\\Data";
        public const string RemoteSuggestionsFilePath = "https://raw.githubusercontent.com/pnp/powershell/dev/resources/predictor/PnP.PowerShell.Suggestions.{0}.json";
        public const string EnvironmentVariableCommandSearchMethod = "PnPPredictorCommandSearchMethod";
        public const string EnvironmentVariableShowWarning = "PnPPredictorShowWarning";
        public const string LibraryName = "PnP.PowerShell.Predictor.dll";
        public const string WarningMessageOnLoad = "WARNING: Unable to load predictions from GitHub. Loading suggestions from local file. Hence some commands from the predictions might not work. Press enter to continue.";
        public const string GenericErrorMessage = "Unable to load predictions. Press enter to continue.";
    }
}
