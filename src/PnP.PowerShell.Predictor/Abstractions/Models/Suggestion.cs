using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PnP.PowerShell.Predictor.Abstractions.Models
{
    public class Suggestion
    {
        public string? Command { get; set; }
        public int Rank { get; set; }
    }
}
