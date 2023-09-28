using OpenQA.Selenium.Edge;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using FlipalooWeb.DataStructure;
/*
namespace FlipalooWeb.Background.BettingOddsFinders
{
    internal partial class MatchFinder
    {
        public ListOfMatches FortunaFindMatches()
        {
            string bettingShopName = "Fortuna";
            string url = "https://www.ifortuna.cz/sazeni?selectDates=1#";
            By cookieButtonElementPath = By.Id("cookie-consent-button-accept");
            By matchesElementPath = By.TagName("tr");
            By namesElementPath = By.CssSelector(".market-name");
            By oddsElementPath = By.CssSelector(".odds-value");

            //initilize driver and stuff
            IWebDriver driver = new EdgeDriver();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
            driver.Navigate().GoToUrl(url);
            wait.Until(ExpectedConditions.ElementIsVisible(cookieButtonElementPath));


            //click on cookie button
            IWebElement buttonConsent = driver.FindElement(cookieButtonElementPath);
            buttonConsent.Click();
            

            //scroll down to load all the content of the page
            while (true)
            {
                var previousMatchesNames = driver.FindElements(By.CssSelector(".event-name"));
                jse.ExecuteScript("window.scrollBy(0,document.body.scrollHeight)");
                try
                {
                    wait.Until(d => d.FindElements(By.CssSelector(".event-name")).Count > previousMatchesNames.Count);
                }
                catch
                {
                    break;
                }
            }

            //finally find and save all macthes odds
            var matchesElements = driver.FindElements(matchesElementPath);
            ListOfMatches listOfMatches = new ListOfMatches();

            foreach (IWebElement matchElement in matchesElements)
            {
                //not all matchElements have matches. Some of them are just rows with information
                try
                {
                    matchElement.FindElement(namesElementPath);
                }
                catch
                {
                    continue;
                }

                IWebElement matchNameElement = matchElement.FindElement(namesElementPath);
                string matchName = matchNameElement.Text;

                var matchOddsElements = matchElement.FindElements(oddsElementPath);

                Tuple<string, float?>[] matchOdds = new Tuple<string, float?>[matchOddsElements.Count];
                for (int i = 0; i < matchOdds.Length; i++)
                {
                    matchOdds[i] = new Tuple<string, float?>(bettingShopName, null);
                }

                Boolean hasSomeOdds = false;
                for (int i = 0; i < matchOdds.Length; i++)
                {
                    float? pureOdds = new float();

                    try
                    {
                        pureOdds = float.Parse(matchOddsElements[i].Text, CultureInfo.InvariantCulture.NumberFormat);
                        if (pureOdds > 1)
                            hasSomeOdds = true;
                        else
                            pureOdds = null;
                    }
                    catch
                    {
                        pureOdds = null;
                    }

                    matchOdds[i] = new Tuple<string, float?>(bettingShopName, pureOdds);
                }

                //create recognition team names
                string[] teamNames = matchName.Split(" - ", 2);
                string[] recognitionTeams = new string[2];
                for (int i = 0; i < teamNames.Length; i++)
                {
                    string[] teamWordsArray = teamNames[i].Split(" ");
                    List<string> finalTeamWordList = new List<string>();
                    if (teamNames[i].Length > 3)
                    {
                        List<string> teamWordList = teamWordsArray.ToList();

                        foreach (string word in teamWordList)
                        {
                            if (word.Length > 2 && !word.StartsWith("(") && !word.EndsWith("."))
                            {
                                finalTeamWordList.Add(word);
                            }
                        }
                        
                    }
                    else
                    {
                        finalTeamWordList = teamWordsArray.ToList();
                    }
                    recognitionTeams[i] = String.Join("", finalTeamWordList);
                    recognitionTeams[i] = RemoveDiacritics(recognitionTeams[i]);
                    recognitionTeams[i] = recognitionTeams[i].ToLowerInvariant();
                }

                if (matchOdds.Length == 2 && hasSomeOdds)
                {
                    TwoOutcomeMatchOdds match = new TwoOutcomeMatchOdds(matchName, recognitionTeams[0], recognitionTeams[1], matchOdds);
                    listOfMatches.AddMatch(match);
                }
                else if (matchOdds.Length == 6 && hasSomeOdds)
                {
                    ThreeOutcomeMatchOdds match = new ThreeOutcomeMatchOdds(matchName, recognitionTeams[0], recognitionTeams[1], matchOdds);
                    listOfMatches.AddMatch(match);
                }

            }
            driver.Manage().Cookies.DeleteAllCookies();
            driver.Close();
            return listOfMatches;
        }
    }
}
*/