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

        public void AddMatch(Match match)
        {
            Matches.Add(match);
        }

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

        public List<Event> SplitToEvents()
        {
            List<Event> listOfEvents = new List<Event>();
            foreach (Match match in Matches)
            {
                List<Event> addingListOfEvents = match.SplitToEvents();
                listOfEvents.AddRange(addingListOfEvents);
            }
            return listOfEvents;
        }
    }
}
