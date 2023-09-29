namespace FlipalooWeb.DataStructure
{
    public class Match
    {
        public string Name { get; set; }
        public string TeamName1 { get; set; }
        public string TeamName2 { get; set; }
        public MatchOdds Odds { get; set; }
        public Match (string name, string teamName1, string teamName2, Odd?[] odds)
        {
            Name = name;
            TeamName1 = teamName1;
            TeamName2 = teamName2;
            Odds = new MatchOdds(odds);
        }
        public Match(string name, string teamName1, string teamName2, MatchOdds matchOdds)
        {
            Name = name;
            TeamName1 = teamName1;
            TeamName2 = teamName2;
            Odds = matchOdds;
        }

        public void Merge(Match match)
        {
            Odds.Merge(match.Odds);
        }

        public ListOfEvents SplitToEvents()
        {
            List<Event> events = Odds.SplitToEvents(Name, TeamName1, TeamName2);
            return new ListOfEvents(events);
        }

        public bool IsSame(Match otherMatch)
        {
            return false;
        }
    }
}
