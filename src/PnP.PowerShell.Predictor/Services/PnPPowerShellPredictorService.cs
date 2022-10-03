using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Subsystem.Prediction;
using System.Net.Http.Json;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PnP.PowerShell.Predictor.Abstractions.Interfaces;
using PnP.PowerShell.Predictor.Abstractions.Models;
using PnP.PowerShell.Predictor.Utilities;

namespace PnP.PowerShell.Predictor.Services
{
    internal class PnPPowerShellPredictorService : IPnPPowerShellPredictorService
    {
        private List<Suggestion>? _allPredictiveSuggestions;
        private CommandSearchMethod _commandSearchMethod;
        private readonly HttpClient _client;
        private readonly string _suggestionsFilePath;

        public PnPPowerShellPredictorService(IPnPPowerShellContext pnpPowerShellContext, CommandSearchMethod commandSearchMethod)
        {
            _suggestionsFilePath = string.Format(PnPPowerShellPredictorConstants.RemoteSuggestionsFilePath, pnpPowerShellContext.PnPPowerShellVersion);
            _commandSearchMethod = commandSearchMethod;
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

            IEnumerable<Suggestion>? filteredSuggestions = null;

            #region BeginsWith search

            /*
            BenchmarkDotNet=v0.13.2, OS=Windows 10 (10.0.19043.2006/21H1/May2021Update)
            Intel Core i7-10510U CPU 1.80GHz, 1 CPU, 8 logical and 4 physical cores
            .NET SDK=6.0.109
              [Host]     : .NET 6.0.9 (6.0.922.41905), X64 RyuJIT AVX2
              DefaultJob : .NET 6.0.9 (6.0.922.41905), X64 RyuJIT AVX2

            |     Method |     Mean |    Error |   StdDev | Allocated |
            |----------- |---------:|---------:|---------:|----------:|
            | BeginsWith | 26.80 ns | 0.555 ns | 0.831 ns |     128 B |
             */

            if (_commandSearchMethod == CommandSearchMethod.BeginsWith)
            {
                filteredSuggestions = _allPredictiveSuggestions?.
                    Where(pc => pc.CommandName != null && pc.CommandName.ToLower().StartsWith(input.ToLower())).
                    OrderBy(pc => pc.Rank);
            }

            #endregion

            #region Contains search

            /*
            BenchmarkDotNet=v0.13.2, OS=Windows 10 (10.0.19043.2006/21H1/May2021Update)
            Intel Core i7-10510U CPU 1.80GHz, 1 CPU, 8 logical and 4 physical cores
            .NET SDK=6.0.109
              [Host]     : .NET 6.0.9 (6.0.922.41905), X64 RyuJIT AVX2
              DefaultJob : .NET 6.0.9 (6.0.922.41905), X64 RyuJIT AVX2

            |   Method |     Mean |    Error |   StdDev | Allocated |
            |--------- |---------:|---------:|---------:|----------:|
            | Contains | 27.52 ns | 0.539 ns | 0.901 ns |     128 B |
             */

            if (_commandSearchMethod == CommandSearchMethod.Contains)
            {
                filteredSuggestions = _allPredictiveSuggestions?.
                    Where(pc => pc.CommandName != null && pc.CommandName.ToLower().Contains(input.ToLower())).
                    OrderBy(pc => pc.Rank);
            }

            #endregion

            #region Fuzzy Search

            /*
            BenchmarkDotNet=v0.13.2, OS=Windows 10 (10.0.19043.2006/21H1/May2021Update)
            Intel Core i7-10510U CPU 1.80GHz, 1 CPU, 8 logical and 4 physical cores
            .NET SDK=6.0.109
              [Host]     : .NET 6.0.9 (6.0.922.41905), X64 RyuJIT AVX2
              DefaultJob : .NET 6.0.9 (6.0.922.41905), X64 RyuJIT AVX2

            |Method |     Mean |   Error |  StdDev | Allocated |
            |------ |---------:|--------:|--------:|----------:|
            | Fuzzy | 959.7 us | 6.49 us | 5.76 us | 115.31 KB |
             */

            if (_commandSearchMethod == CommandSearchMethod.Fuzzy)
            {
                var inputWithoutSpaces = Regex.Replace(input, @"\s+", "");
                
                var macthes = new List<Suggestion>();

                foreach (var suggestion in CollectionsMarshal.AsSpan(_allPredictiveSuggestions))
                {
                    FuzzyMatcher.Match(suggestion.CommandName, inputWithoutSpaces, out var score);
                    suggestion.Rank = score;
                    macthes.Add(suggestion);
                }

                filteredSuggestions = macthes.OrderByDescending(m => m.Rank);
            }

            #endregion
            

            if (filteredSuggestions == null)
            {
                return null;
            }

            var result = filteredSuggestions?.Select(fs => new PredictiveSuggestion(fs.Command)).ToList();

            return result;
        }

    }
}
