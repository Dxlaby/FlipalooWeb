using OpenQA.Selenium.Edge;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Interactions;
using FlipalooWeb.DataStructure;

/*
namespace FlipalooWeb.Background.BettingOddsFinders
{
    internal partial class MatchFinder
    {
        public ListOfMatches TipsportFindMatches(IWebDriver driver)
        {
            string bettingShopName = "TipSport";
            string url = "https://www.tipsport.cz/kurzy?timeFilter=form.period.anytime&limit=10000";
            By matchesElementPath = By.CssSelector(".o-matchRow");
            By namesElementPath = By.CssSelector(".o-matchRow__matchName");
            By oddsElementPath = By.CssSelector(".btnRate");

            driver.Navigate().GoToUrl(url);

            IReadOnlyCollection<IWebElement>? matchesElements = driver.FindElements(matchesElementPath);
            ListOfMatches listOfMatches = new ListOfMatches();

            foreach (IWebElement matchElement in matchesElements)
            {
                IWebElement matchNameElement = matchElement.FindElement(namesElementPath);
                string matchName = matchNameElement.Text;

                var matchOddsElements = matchElement.FindElements(oddsElementPath);
                short numberOfOdds = new short();

                if (matchOddsElements.Count() > 2)
                    numberOfOdds = 6;
                else if (matchOddsElements.Count() == 2)
                    numberOfOdds = 2;
                else
                    continue;

                Tuple<string, float?>[] matchOdds = new Tuple<string, float?>[numberOfOdds];
                for (int i = 0; i < matchOdds.Length; i++)
                {
                    matchOdds[i] = new Tuple<string, float?>(bettingShopName, null);
                }

                Boolean hasSomeOdds = false;
                for (int i = 0; i < matchOddsElements.Count; i++)
                {
                    float? pureOdds = new float();

                    try
                    {
                        pureOdds = float.Parse(matchOddsElements[i].Text, CultureInfo.InvariantCulture.NumberFormat);
                        hasSomeOdds = true;
                    }
                    catch
                    {
                        pureOdds = null;
                    }

                    if (matchOdds.Length == 6 && matchOddsElements.Count() == 5)
                    {
                        //prohazeni kurzu tak aby byly ve formatu 1, 0, 2, 10, 20, 12
                        if (i == 0) matchOdds[0] = new Tuple<string, float?>(bettingShopName, pureOdds);
                        else if (i == 1) matchOdds[3] = new Tuple<string, float?>(bettingShopName, pureOdds);
                        else if (i == 2) matchOdds[1] = new Tuple<string, float?>(bettingShopName, pureOdds);
                        else if (i == 3) matchOdds[4] = new Tuple<string, float?>(bettingShopName, pureOdds);
                        else if (i == 4) matchOdds[2] = new Tuple<string, float?>(bettingShopName, pureOdds);
                        //else if (i == 5) matchOdds[5] = new Tuple<string, float?>(bettingShopName, pureOdds);
                        // na tipsportu se nikdy neobjevi sesta moznost sazeni, takze ji muzeme vynechat
                    }
                    else if (matchOdds.Length == 6 && matchOddsElements.Count() == 3)
                    {
                        matchOdds[i] = new Tuple<string, float?>(bettingShopName, pureOdds);
                    }
                    else if (matchOdds.Length == 2)
                    {
                        matchOdds[i] = new Tuple<string, float?>(bettingShopName, pureOdds);
                    }
                }

                //create recognition team names
                string[] teamNames = matchName.Split(" - ", 2);
                string[] recognitionTeams = new string[2];
                for (int i = 0; i < teamNames.Length; i++)
                {
                    string[] teamWordsArray = teamNames[i].Split(" ");
                    List<string> teamWordList = teamWordsArray.ToList();
                    List<string> finalTeamWordList = new List<string>();
                    foreach (string word in teamWordList)
                    {
                        if (!(word.Length < 3 || word.StartsWith("(") || word.EndsWith(".")))
                        {
                            finalTeamWordList.Add(word);
                        }
                    }
                    recognitionTeams[i] = String.Join("", finalTeamWordList);
                    recognitionTeams[i] = RemoveDiacritics(recognitionTeams[i]);
                    recognitionTeams[i] = recognitionTeams[i].ToLowerInvariant();
                }
                
                //finally add match
                if (matchOdds.Length == 2 && hasSomeOdds)
                {
                    TwoOutcomeMatchOdds match = new TwoOutcomeMatchOdds(matchName, recognitionTeams[0], recognitionTeams[1], matchOdds);
                    listOfMatches.AddMatch(match);
                }
                else if(matchOdds.Length == 6 && hasSomeOdds)
                {
                    ThreeOutcomeMatchOdds match = new ThreeOutcomeMatchOdds(matchName, recognitionTeams[0], recognitionTeams[1], matchOdds);
                    listOfMatches.AddMatch(match);
                }

            }

            driver.Close();
            return listOfMatches;
        }
    }
}
*/