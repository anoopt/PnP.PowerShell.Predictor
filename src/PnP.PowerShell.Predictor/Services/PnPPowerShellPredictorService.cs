﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Subsystem.Prediction;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PnP.PowerShell.Predictor.Abstractions.Interfaces;
using PnP.PowerShell.Predictor.Abstractions.Models;

namespace PnP.PowerShell.Predictor.Services
{
    internal class PnPPowerShellPredictorService : IPnPPowerShellPredictorService
    {
        private List<Suggestion>? _allPredictiveSuggestions;
        private readonly HttpClient _client;
        private readonly string _suggestionsFilePath;

        public PnPPowerShellPredictorService(IPnPPowerShellContext pnpPowerShellContext)
        {
            _suggestionsFilePath = string.Format(PnPPowerShellPredictorConstants.RemoteSuggestionsFilePath, pnpPowerShellContext.PnPPowerShellVersion);
            _client = new HttpClient();
            RequestAllPredictiveCommands();
        }

        protected virtual void RequestAllPredictiveCommands()
        {
            //TODO: Decide if we need to make an http request here to get all the commands
            //TODO: if the http request fails then fallback to local JSON file?
            _ = Task.Run(async () =>
              {
                  try
                  {
                      _allPredictiveSuggestions = await _client.GetFromJsonAsync<List<Suggestion>>(_suggestionsFilePath);
                  }
                  catch (Exception e)
                  {
                      _allPredictiveSuggestions = null;
                  }

                  if (_allPredictiveSuggestions == null)
                  {
                      try
                      {
                          string executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                          string fileName = Path.Combine($"{executableLocation}{PnPPowerShellPredictorConstants.LocalSuggestionsFileRelativePath}", PnPPowerShellPredictorConstants.LocalSuggestionsFileName);
                          string jsonString = await File.ReadAllTextAsync(fileName);
                          _allPredictiveSuggestions = JsonSerializer.Deserialize<List<Suggestion>>(jsonString)!;
                          Console.ForegroundColor = ConsoleColor.Yellow;
                          Console.Write("WARNING: Unable to load predictions from GitHub. Loading suggestions from local file. Hence some commands from the predictions might not work. Press enter to continue.");
                          Console.ResetColor();
                      }
                      catch (Exception e)
                      {
                          Console.ForegroundColor = ConsoleColor.DarkRed;
                          Console.Write("Unable to load predictions. Press enter to continue.");
                          Console.ResetColor();
                          _allPredictiveSuggestions = null;
                      }
                      
                  }
              });
        }

        public virtual List<PredictiveSuggestion>? GetSuggestions(PredictionContext context)
        {
            var input = context.InputAst.Extent.Text;
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            if (_allPredictiveSuggestions == null)
            {
                return null;
            }

            //TODO: Decide how the source data should be structured and then add a logic to get filtered suggestions
            var filteredSuggestions = _allPredictiveSuggestions?.
                FindAll(pc => pc.Command.ToLower().StartsWith(input.ToLower())).
                OrderBy(pc => pc.Rank);

            var result = filteredSuggestions?.Select(fs => new PredictiveSuggestion(fs.Command)).ToList();

            return result;
        }

    }
}
