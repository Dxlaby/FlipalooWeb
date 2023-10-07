namespace FlipalooWeb.DataStructure
{
    public class Match
    {
        public string Name { get; set; }
        public string RecognitionTeamName1 { get; set; }
        public string RecognitionTeamName2 { get; set; }
        public MatchOdds Odds { get; set; }
        public Match (string name, string recognitionTeamName1, string recognitionTeamName2, Odd?[] odds)
        {
            Name = name;
            RecognitionTeamName1 = NormalizeString(recognitionTeamName1);
            RecognitionTeamName2 = NormalizeString(recognitionTeamName2);
            Odds = new MatchOdds(odds);
        }
        public Match(string name, string recognitionTeamName1, string recognitionTeamName2, MatchOdds matchOdds)
        {
            Name = name;
            RecognitionTeamName1 = NormalizeString(recognitionTeamName1);
            RecognitionTeamName2 = NormalizeString(recognitionTeamName2);
            Odds = matchOdds;
        }

        public void Merge(Match match)
        {
            Odds.Merge(match.Odds);
        }

        public List<Event> SplitToEvents()
        {
            List<Event> events = Odds.SplitToEvents(Name, RecognitionTeamName1, RecognitionTeamName2);
            return new List<Event>(events);
        }

        public bool IsSame(Match otherMatch)
        {
            string[] teamNames1 = {RecognitionTeamName1, RecognitionTeamName2};
            string[] teamNames2 = { otherMatch.RecognitionTeamName1, otherMatch.RecognitionTeamName2 };
            if (RecognitionTeamName1 == otherMatch.RecognitionTeamName2
                && RecognitionTeamName2 == otherMatch.RecognitionTeamName2)
            {
                return true;
            }

            return false;
        }

        private string NormalizeString(string matchName)
        {
            matchName = matchName.Replace("/", " ");
            matchName = matchName.Replace(".", " ");
            
            string[] teamWordsArray = matchName.Split(" ");
            List<string> finalTeamWordList = new List<string>();
            if (matchName.Length > 3)
            {
                List<string> teamWordList = teamWordsArray.ToList();

                foreach (string word in teamWordList)
                {
                    if (word.Length > 2 && !word.StartsWith("("))
                    {
                        finalTeamWordList.Add(word);
                    }
                }

            }
            else
            {
                finalTeamWordList = teamWordsArray.ToList();
            }

            matchName = String.Join("", finalTeamWordList);

            return matchName;
        }
    }
}
