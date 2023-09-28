using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlipalooWeb.DataStructure
{
    internal class ListOfMatches
    {
        public List<Match> Matches { get; set; }

        public ListOfMatches()
        {
            Matches = new List<Match>();
        }

        public void AddMatch(Match _match)
        {
            Matches.Add(_match);
        }

        /*
        public void SortByName()
        {
            Matches.Sort((a, b) => a.Name.CompareTo(b.Name));
        }
        */

        public void Merge(ListOfMatches mergingListOfMatches)
        {   
            foreach (Match match in Matches)
            {
                foreach (Match  mergingMatch in mergingListOfMatches.Matches)
                {
                    if (match.IsSame(mergingMatch))
                    {
                        match.Merge(mergingMatch);
                        mergingListOfMatches.Matches.Remove(mergingMatch);
                        break;
                    }
                }
            }
            Matches.AddRange(mergingListOfMatches.Matches);
        }

        public ListOfEvents SplitToEvents()
        {
            ListOfEvents listOfEvents = new ListOfEvents();
            foreach (Match match in Matches)
            {
                ListOfEvents addingListOfEvents = match.SplitToEvents();
                listOfEvents.AddListOfEvents(addingListOfEvents);
            }
            return listOfEvents;
        }

        /*
        public void PrintToConsole()
        {
            Console.Write("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
            foreach (IMatch match in Matches)
            {
                Console.WriteLine(match.Name);
                foreach (Tuple<string, float?> value in match.Odds)
                {
                    Console.Write(value.Item1 + ": ");

                    if (value.Item2 != null)
                    {
                        Console.Write(value.Item2 + "; ");
                    }
                    else
                    {
                        Console.Write("none; ");
                    }

                }
                Console.WriteLine();
                Console.WriteLine();
            }
            Console.WriteLine("Celkovy pocet zapasu: " + Matches.Count);
        }
        */
    }
}
