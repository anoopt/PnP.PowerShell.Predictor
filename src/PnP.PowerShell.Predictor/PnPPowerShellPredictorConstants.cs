using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PnP.PowerShell.Predictor
{
    internal static class PnPPowerShellPredictorConstants
    {
        public const string SuggestionsFileName = "PnP.PowerShell.Suggestions.json";
        public const string SuggestionsFileRelativePath = "\\Data";
        public const string CommandsFilePath = "https://raw.githubusercontent.com/anoopt/spfx-ace-image-button-text/master/predictor-commands/{0}.json";
    }
}
