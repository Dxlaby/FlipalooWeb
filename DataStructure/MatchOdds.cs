using System.Xml.Linq;

namespace FlipalooWeb.DataStructure
{
    public class MatchOdds
    {
        public Odd?[] Odds { get; set; }

        public MatchOdds(Odd?[] odds) 
        {
            Odds = odds;

            if (Odds.Length != 2 && Odds.Length != 6)
                throw new Exception("You can only give array as argument, if it has 2 or 6 odds");
        }

        public void Merge(MatchOdds matchOdds) 
        {
            for (int i = 0; i < Odds.Length; i++)
            {
                if (matchOdds.Odds[i] == null)
                {
                    continue;
                }
                else if (Odds[i] == null)
                {
                    Odds[i] = matchOdds.Odds[i];
                }
                else if (Odds[i].BettingOdd > matchOdds.Odds[i].BettingOdd)
                {
                    continue;
                }
                else
                {
                    Odds[i] = matchOdds.Odds[i];
                }
            }
        }

        public List<Event> SplitToEvents(string name, string teamName1, string teamName2, DateTime date)
        {
            List<Event> events = new List<Event>();

            if (Odds.Length == 2)
            {
                if (Odds[0] != null && Odds[1] != null)
                {
                    List<Odd> odds = new List<Odd>();
                    odds.Add(Odds[0]);
                    odds.Add(Odds[1]);

                    float impliedProbability = GetImpliedProbability(odds);
                    float profitPercentage = GetProfitPercentage(odds);
                    
                    Event newEvent = new Event(name, teamName1,  teamName2,
                        impliedProbability, profitPercentage, date.ToString("d. M. H:mm"), odds);
                    events.Add(newEvent);
                }
            }
            else if (Odds.Length == 6)
            {
                //BettingOdds are in oder 1, 0, 2, 10, 02, 12 for three outcome matches
                if (Odds[0] != null && Odds[1] != null && Odds[2] != null)
                {
                    List<Odd> odds = new List<Odd>();
                    odds.Add(Odds[0]);
                    odds.Add(Odds[1]);
                    odds.Add(Odds[2]);

                    float impliedProbability = GetImpliedProbability(odds);
                    float profitPercentage = GetProfitPercentage(odds);
                    
                    Event newEvent = new Event(name, teamName1,  teamName2,
                        impliedProbability, profitPercentage, date.ToString("d. M. H:mm"), odds);                    
                    events.Add(newEvent);
                }
                if (Odds[0] != null && Odds[4] != null)
                {
                    List<Odd> odds = new List<Odd>();
                    odds.Add(Odds[0]);
                    odds.Add(Odds[4]);

                    float impliedProbability = GetImpliedProbability(odds);
                    float profitPercentage = GetProfitPercentage(odds);
                    
                    Event newEvent = new Event(name, teamName1,  teamName2,
                        impliedProbability, profitPercentage, date.ToString("d. M. H:mm"), odds);                    
                    events.Add(newEvent);
                }
                if (Odds[1] != null && Odds[5] != null)
                {
                    List<Odd> odds = new List<Odd>();
                    odds.Add(Odds[1]);
                    odds.Add(Odds[5]);

                    float impliedProbability = GetImpliedProbability(odds);
                    float profitPercentage = GetProfitPercentage(odds);
                    
                    Event newEvent = new Event(name, teamName1,  teamName2,
                        impliedProbability, profitPercentage, date.ToString("d. M. H:mm"), odds);
                    events.Add(newEvent);
                }
                if (Odds[2] != null && Odds[3] != null)
                {
                    List<Odd> odds = new List<Odd>();
                    odds.Add(Odds[2]);
                    odds.Add(Odds[3]);

                    float impliedProbability = GetImpliedProbability(odds);
                    float profitPercentage = GetProfitPercentage(odds);
                    
                    Event newEvent = new Event(name, teamName1,  teamName2,
                        impliedProbability, profitPercentage, date.ToString("d. M. H:mm"), odds);
                    events.Add(newEvent);
                }
            }

            return events;
        }

        private float GetImpliedProbability(List<Odd> odds)
        {
            float impliedProbability = 0;
            foreach (Odd odd in odds)
            {
                impliedProbability += 1 / odd.BettingOdd;
            }
            
            return impliedProbability;
        }
        
        public float GetProfitPercentage(List<Odd> odds)
        {
            float profitPercentage = (1 / GetImpliedProbability(odds) - 1) * 100;
            return (float)Math.Round(profitPercentage, 2);
        }
    }
}
