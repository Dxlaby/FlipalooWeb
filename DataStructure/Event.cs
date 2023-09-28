using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipalooWeb.DataStructure
{
    public class Event
    {
        public string TeamName1 { get; set; }
        public string TeamName2 { get; set; }
        public float ImpliedProbability { get; set; }
        public List<Odd> Odds { get; set; }
        //public string UrlReference { get; set; }

        public Event(string teamName1, string teamName2, List<Odd> odds)
        {
            TeamName1 = teamName1;
            TeamName2 = teamName2;
            Odds = odds;            
            SetImpliedProbability();
        }

        public void SetImpliedProbability()
        {
            float impliedProbability = 0;
            foreach (Odd odd in Odds)
            {
                impliedProbability += 1 / odd.BettingOdd;
            }

            ImpliedProbability = impliedProbability;
        }
    }
}
