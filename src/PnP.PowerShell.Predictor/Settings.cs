using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PnP.PowerShell.Predictor.Abstractions.Models;

namespace PnP.PowerShell.Predictor
{
    internal class Settings
    {
        public CommandSearchMethod CommandSearchMethod { get; set; }
        public bool ShowWarning { get; set; }

        private static CommandSearchMethod GetCommandSearchMethod()
        {
            var pnpPredictorCommandSearchMethod = Environment.GetEnvironmentVariable(PnPPowerShellPredictorConstants.EnvironmentVariableCommandSearchMethod);
            if (pnpPredictorCommandSearchMethod == null)
            {
                return CommandSearchMethod.Contains;
            }

            switch (pnpPredictorCommandSearchMethod)
            {
                case "Fuzzy":
                    return CommandSearchMethod.Fuzzy;
                case "Contains":
                    return CommandSearchMethod.Contains;
                case "StartsWith":
                    return CommandSearchMethod.StartsWith;
                default:
                    return CommandSearchMethod.StartsWith;
            }
        }
        
        private static bool GetShowWarning()
        {
            var cliM365PredictorShowWarning = Environment.GetEnvironmentVariable(PnPPowerShellPredictorConstants.EnvironmentVariableShowWarning);
        
            if (cliM365PredictorShowWarning == null)
            {
                return true;
            }

            return bool.Parse(cliM365PredictorShowWarning);
        }

        public static Settings GetSettings()
        {
            return new Settings()
            {
                CommandSearchMethod = GetCommandSearchMethod(),
                ShowWarning = GetShowWarning()
            };
        }
}
}
