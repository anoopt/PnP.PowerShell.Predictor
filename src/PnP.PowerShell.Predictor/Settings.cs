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

        private static CommandSearchMethod GetCommandSearchMethod()
        {
            var pnpPredictorCommandSearchMethod = Environment.GetEnvironmentVariable(PnPPowerShellPredictorConstants.EnvironmentVariableCommandSearchMethod);
            if (pnpPredictorCommandSearchMethod == null)
            {
                return CommandSearchMethod.BeginsWith;
            }

            switch (pnpPredictorCommandSearchMethod)
            {
                case "Fuzzy":
                    return CommandSearchMethod.Fuzzy;
                case "Contains":
                    return CommandSearchMethod.Contains;
                case "BeginsWith":
                    return CommandSearchMethod.BeginsWith;
                default:
                    return CommandSearchMethod.BeginsWith;
            }
        }

        public static Settings GetSettings()
        {
            return new Settings()
            {
                CommandSearchMethod = GetCommandSearchMethod()
            };
        }
}
}
