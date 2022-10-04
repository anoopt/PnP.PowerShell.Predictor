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
        private readonly CommandSearchMethod _commandSearchMethod;
        private readonly HttpClient _client;
        private readonly string _suggestionsFilePath;

        public PnPPowerShellPredictorService(IPnPPowerShellContext pnpPowerShellContext, Settings settings)
        {
            _suggestionsFilePath = string.Format(PnPPowerShellPredictorConstants.RemoteSuggestionsFilePath,
                pnpPowerShellContext.PnPPowerShellVersion);
            _commandSearchMethod = settings.CommandSearchMethod;
            _client = new HttpClient();
            RequestAllPredictiveCommands(settings.ShowWarning);
        }

        //For the first 2 versions of this module, the JSON file did not have the "CommandName" property
        private void UpdateCommandNameInSuggestions()
        {
            //if _allPredictiveSuggestions is null, then return
            if (_allPredictiveSuggestions == null)
            {
                return;
            }
            
            //if the first suggestion has the CommandName property, then return
            if (!string.IsNullOrEmpty(_allPredictiveSuggestions[0].CommandName))
            {
                return;
            }
            
            //For each suggestion in the list, set the CommandName property to the first word in the Command property using Regex
            foreach (var suggestion in _allPredictiveSuggestions)
            {
                if (suggestion.Command != null)
                    suggestion.CommandName = Regex.Match(suggestion.Command, @"^\S+").Value;
            }
        }
        
        private void RemoveInvalidSuggestions()
        {
            //if _allPredictiveSuggestions is null, then return
            if (_allPredictiveSuggestions == null)
            {
                return;
            }
            
            //filter out suggestions where CommandName and Command are not null or empty
            _allPredictiveSuggestions = _allPredictiveSuggestions.Where(suggestion => !string.IsNullOrEmpty(suggestion.CommandName) && !string.IsNullOrEmpty(suggestion.Command)).ToList();
        }

        protected virtual void RequestAllPredictiveCommands(bool showWarning)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    _allPredictiveSuggestions = await _client.GetFromJsonAsync<List<Suggestion>>(_suggestionsFilePath);
                    UpdateCommandNameInSuggestions();
                    RemoveInvalidSuggestions();
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
                        string fileName =
                            Path.Combine(
                                $"{executableLocation}{PnPPowerShellPredictorConstants.LocalSuggestionsFileRelativePath}",
                                PnPPowerShellPredictorConstants.LocalSuggestionsFileName);
                        string jsonString = await File.ReadAllTextAsync(fileName);
                        _allPredictiveSuggestions = JsonSerializer.Deserialize<List<Suggestion>>(jsonString)!;
                        RemoveInvalidSuggestions();
                        if (showWarning)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write(PnPPowerShellPredictorConstants.WarningMessageOnLoad);
                            Console.ResetColor();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write(PnPPowerShellPredictorConstants.GenericErrorMessage);
                        Console.ResetColor();
                        _allPredictiveSuggestions = null;
                    }
                }
            });
        }

        private IEnumerable<Suggestion>? GetFilteredSuggestions(string input)
        {
            IEnumerable<Suggestion>? filteredSuggestions = null;

            /*
              BenchmarkDotNet=v0.13.2, OS=Windows 10 (10.0.19043.2006/21H1/May2021Update)
              Intel Core i7-10510U CPU 1.80GHz, 1 CPU, 8 logical and 4 physical cores
              .NET SDK=6.0.109
                [Host]     : .NET 6.0.9 (6.0.922.41905), X64 RyuJIT AVX2
                DefaultJob : .NET 6.0.9 (6.0.922.41905), X64 RyuJIT AVX2 
            */

            #region Search

            switch (_commandSearchMethod)
            {
                /*
                   |     Method |     Mean |    Error |   StdDev | Allocated |
                   |----------- |---------:|---------:|---------:|----------:|
                   | StartsWith | 26.80 ns | 0.555 ns | 0.831 ns |     128 B |
                */
                case CommandSearchMethod.StartsWith:
                    filteredSuggestions = _allPredictiveSuggestions
                        ?.Where(pc => pc.CommandName != null && pc.CommandName.ToLower().StartsWith(input.ToLower()))
                        .OrderBy(pc => pc.Rank);
                    break;
                /*
                    |   Method |     Mean |    Error |   StdDev | Allocated |
                    |--------- |---------:|---------:|---------:|----------:|
                    | Contains | 27.52 ns | 0.539 ns | 0.901 ns |     128 B |
                 */
                case CommandSearchMethod.Contains:
                    filteredSuggestions = _allPredictiveSuggestions
                        ?.Where(pc => pc.CommandName != null && pc.CommandName.ToLower().Contains(input.ToLower()))
                        .OrderBy(pc => pc.Rank);
                    break;
                /*
                    |Method |     Mean |   Error |  StdDev | Allocated |
                    |------ |---------:|--------:|--------:|----------:|
                    | Fuzzy | 959.7 us | 6.49 us | 5.76 us | 115.31 KB |
                */
                case CommandSearchMethod.Fuzzy:
                {
                    var inputWithoutSpaces = Regex.Replace(input, @"\s+", "");

                    var matches = new List<Suggestion>();

                    foreach (var suggestion in CollectionsMarshal.AsSpan(_allPredictiveSuggestions))
                    {
                        FuzzyMatcher.Match(suggestion.CommandName, inputWithoutSpaces, out var score);
                        suggestion.Rank = score;
                        matches.Add(suggestion);
                    }

                    filteredSuggestions = matches.OrderByDescending(m => m.Rank);
                    break;
                }
            }

            #endregion
            
            return filteredSuggestions;
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

            IEnumerable<Suggestion>? filteredSuggestions = GetFilteredSuggestions(input);

            var result = filteredSuggestions?.Select(fs => new PredictiveSuggestion(fs.Command)).ToList();

            return result;
        }
    }
}