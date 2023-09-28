using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

/*
namespace FlipalooWeb.DataStructure
{
    internal class ThreeOutcomeMatchOdds : IMatch
    {
        public string Name { get; set; }
        public string RecognitionTeam1 { get; set; }
        public string RecognitionTeam2 { get; set; }
        public Tuple<Odd?>[] Odds { get; set; }
        public short NumberOfOdds { get; private set; }
        //string represents name of betting company who is declaring betting odds
        //BettingOdds are in oder 1, 0, 2, 10, 02, 12 for three outcome matches

        public ThreeOutcomeMatchOdds(string name, string recognitionTeam1, string recognitionTeam2, Tuple<Odd?>[] bettingOdds)
        {
            Name = name;
            Odds = new Tuple<Odd?>[NumberOfOdds];
            Odds = bettingOdds;
            RecognitionTeam1 = recognitionTeam1;
            RecognitionTeam2 = recognitionTeam2;
        }

        public void Merge(IMatch mergingMatch)
        {
            if (mergingMatch is ThreeOutcomeMatchOdds)
            {
                for (int i = 0; i < Odds.Length; i++)
                {
                    if (mergingMatch.Odds[i].Item2 == null)
                    {
                        continue;
                    }
                    else if (Odds[i].Item2 == null)
                    {
                        Odds[i] = mergingMatch.Odds[i];
                    }
                    else if (Odds[i].Item2 > mergingMatch.Odds[i].Item2)
                    {
                        continue;
                    }
                    else
                    {
                        Odds[i] = mergingMatch.Odds[i];
                    }

                }
            }
        }

        public ListOfEvents SplitToEvents()
        {
            ListOfEvents listOfEvents = new ListOfEvents();

            if (Odds[0].Item2.HasValue && Odds[1].Item2.HasValue && Odds[2].Item2.HasValue)
            {
                List<Tuple<string, float>> bettingOdds = new List<Tuple<string, float>>();

                Tuple<string, float> bettingOdd0 = new Tuple<string, float>(Odds[0].Item1, Odds[0].Item2.Value);
                Tuple<string, float> bettingOdd1 = new Tuple<string, float>(Odds[1].Item1, Odds[1].Item2.Value);
                Tuple<string, float> bettingOdd2 = new Tuple<string, float>(Odds[2].Item1, Odds[2].Item2.Value);

                bettingOdds.Add(bettingOdd0);
                bettingOdds.Add(bettingOdd1);
                bettingOdds.Add(bettingOdd2);

                Event oddsEvent = new OddsEvent(Name + " - 1, 0, 2", bettingOdds);
                listOfEvents.AddEvent(oddsEvent);
            }

            if (Odds[0].Item2.HasValue && Odds[4].Item2.HasValue)
            {
                List<Tuple<string, float>> bettingOdds = new List<Tuple<string, float>>();

                Tuple<string, float> bettingOdd0 = new Tuple<string, float>(Odds[0].Item1, Odds[0].Item2.Value);
                Tuple<string, float> bettingOdd1 = new Tuple<string, float>(Odds[4].Item1, Odds[4].Item2.Value);

                bettingOdds.Add(bettingOdd0);
                bettingOdds.Add(bettingOdd1);

                Event oddsEvent = new Event(Name + " - 1, 02", bettingOdds);
                listOfEvents.AddEvent(oddsEvent);
            }

            if (Odds[1].Item2.HasValue && Odds[5].Item2.HasValue)
            {
                List<Tuple<string, float>> bettingOdds = new List<Tuple<string, float>>();

                Tuple<string, float> bettingOdd0 = new Tuple<string, float>(Odds[1].Item1, Odds[1].Item2.Value);
                Tuple<string, float> bettingOdd1 = new Tuple<string, float>(Odds[5].Item1, Odds[5].Item2.Value);

                bettingOdds.Add(bettingOdd0);
                bettingOdds.Add(bettingOdd1);

                Event oddsEvent = new OddsEvent(Name + " - 0, 12", bettingOdds);
                listOfEvents.AddEvent(oddsEvent);
            }

            if (Odds[2].Item2.HasValue && Odds[3].Item2.HasValue)
            {
                List<Tuple<string, float>> bettingOdds = new List<Tuple<string, float>>();

                Tuple<string, float> bettingOdd0 = new Tuple<string, float>(Odds[2].Item1, Odds[2].Item2.Value);
                Tuple<string, float> bettingOdd1 = new Tuple<string, float>(Odds[3].Item1, Odds[3].Item2.Value);

                bettingOdds.Add(bettingOdd0);
                bettingOdds.Add(bettingOdd1);

                Event oddsEvent = new OddsEvent(Name + " - 2, 01", bettingOdds);
                listOfEvents.AddEvent(oddsEvent);
            }

            return listOfEvents;
        }

        public bool IsSame(IMatch match)
        {
            if (match is ThreeOutcomeMatchOdds)
            {
                if (match.RecognitionTeam1.Contains(RecognitionTeam1) || RecognitionTeam1.Contains(match.RecognitionTeam1))
                {
                    if (match.RecognitionTeam2.Contains(RecognitionTeam2) || RecognitionTeam2.Contains(match.RecognitionTeam2))
                        return true;
                }
            }

            return false;
        }
    }
}
*/