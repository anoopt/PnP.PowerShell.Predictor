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
        public const string RemoteSuggestionsFilePath = "https://raw.githubusercontent.com/pnp/powershell/dev/resources/PnP.PowerShell.Suggestions.{0}.json";
        public const string EnvironmentVariableCommandSearchMethod = "PnPPredictorCommandSearchMethod";
    }
}
