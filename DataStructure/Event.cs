using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipalooWeb.DataStructure
{
    public class Event
    {
        public string Name { get; set; }
        public string RecognitionTeamName1 { get; set; }
        public string RecognitionTeamName2 { get; set; }
        public float ImpliedProbability { get; set; }
        public float ProfitPercentage { get; set; }
        public string Date { get; set; }
        public List<Odd> Odds { get; set; }
        //public string UrlReference { get; set; }

        public Event(string name, string recognitionTeamName1, string recognitionTeamName2, 
            float impliedProbability, float profitPercentage, string date, List<Odd> odds)
        {
            Name = name;
            RecognitionTeamName1 = recognitionTeamName1;
            RecognitionTeamName2 = recognitionTeamName2;
            ImpliedProbability = impliedProbability;
            ProfitPercentage = profitPercentage;
            Date = date;
            Odds = odds;      
        }

        // public float GetImpliedProbability()
        // {
        //     float impliedProbability = 0;
        //     foreach (Odd odd in Odds)
        //     {
        //         impliedProbability += 1 / odd.BettingOdd;
        //     }
        //
        //     return impliedProbability;
        // }
        //
        // public float GetProfitPercentage()
        // {
        //     float profitPercentage = (1 / GetImpliedProbability() - 1) * 100;
        //     return profitPercentage;
        // }
    }
}
