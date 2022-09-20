using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Subsystem.Prediction;
using System.Text;
using System.Threading.Tasks;

namespace PnP.PowerShell.Predictor.Abstractions.Interfaces
{
    public interface IPnPPowerShellPredictorService
    {
        public List<PredictiveSuggestion>? GetSuggestions(PredictionContext context);
    }
}
