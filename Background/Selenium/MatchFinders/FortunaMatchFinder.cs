using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlipalooWeb.DataStructure;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace FlipalooWeb.Background.BettingOddsFinders
{
    internal class FortunaMatchFinder : IMatchFinder
    {
        string _bettingShopName; 
        string url; 
        By cookieButtonElementPath;
        By matchesElementPath;
        By namesElementPath;
        By oddsElementPath;
        By referenceLinkElementPath;
        public FortunaMatchFinder()
        {
            _bettingShopName = "Fortuna";
            url = "https://www.ifortuna.cz/sazeni?selectDates=1#";
            cookieButtonElementPath = By.Id("cookie-consent-button-accept");
            matchesElementPath = By.TagName("tr");
            namesElementPath = By.CssSelector(".market-name");
            oddsElementPath = By.CssSelector(".odds-value");
            referenceLinkElementPath = By.CssSelector(".event-link");
        }

        public ListOfMatches FindAllMatches(IWebDriver driver)
        { 
            //initialize driver and stu
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            driver.Navigate().GoToUrl(url);
            wait.Until(ExpectedConditions.ElementIsVisible(cookieButtonElementPath));
            
            //click on cookie button
            IWebElement buttonConsent = driver.FindElement(cookieButtonElementPath);
            buttonConsent.Click();
            
            ScrollDown(driver, wait);
            
            //finally find and save all macthes odds
            return FindListOfMatches(driver);
        }

        public void ScrollDown(IWebDriver driver, WebDriverWait wait)
        {
            IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;

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
        }

        public ListOfMatches FindListOfMatches(IWebDriver driver)
        {
            var matchesElements = driver.FindElements(matchesElementPath);
            ListOfMatches listOfMatches = new ListOfMatches();
            
            foreach (IWebElement matchElement in matchesElements)
            {
                Match? match = GetMatchFromElement(matchElement);
                if (match != null)
                    listOfMatches.AddMatch(match);
            }
            return listOfMatches;
        }

        private Match? GetMatchFromElement(IWebElement matchElement)
        {
          //not all matchElements have matches. Some of them are just rows with information
            try
            {
                matchElement.FindElement(namesElementPath);
            }
            catch
            {
                return null;
            }
            IWebElement matchNameElement = matchElement.FindElement(namesElementPath);
            string matchName = matchNameElement.Text;
            IWebElement referenceElement = matchElement.FindElement(referenceLinkElementPath);
            string referenceUrl = referenceElement.GetAttribute("href"); 
    
            var roughOdds = GetOddsFromElement(matchElement, referenceUrl);
            MatchOdds? sortedOdds = SortOdds(roughOdds);
            
            var recognitionTeams = GetRecognitionTeams(matchName);

            if (sortedOdds == null)
                return null;
            else if (sortedOdds.Odds.Length == 2)
                return new Match(matchName, recognitionTeams.Item1, recognitionTeams.Item2, sortedOdds);
            else if (sortedOdds.Odds.Length == 6)
                return new Match(matchName, recognitionTeams.Item1, recognitionTeams.Item2, sortedOdds);
            else
                return null;
        }

        private Odd?[] GetOddsFromElement(IWebElement oddsElement, string referenceUrl)
        {
            var oddElements = oddsElement.FindElements(oddsElementPath);
            List<Odd?> odds = new List<Odd?>();
            foreach (IWebElement maybeOddElement in oddElements)
            {
                try
                {
                    float bettingOdd = float.Parse(maybeOddElement.Text, CultureInfo.InvariantCulture.NumberFormat);
                    if (bettingOdd > 1)
                    {
                        Odd odd = new Odd(_bettingShopName, referenceUrl, bettingOdd);
                        odds.Add(odd);
                    }
                    else
                        odds.Add(null);
                }
                catch
                {
                    odds.Add(null);
                }
            }

            return odds.ToArray();
        }

        private MatchOdds? SortOdds(Odd?[] roughOdds)
        {
            if (roughOdds.Length == 2 || roughOdds.Length == 6)
            {
                return new MatchOdds(roughOdds);
            }
            else
            {
                return null;
            }
        }

        private Tuple<string, string> GetRecognitionTeams(string matchName)
        {
            matchName = RemoveDiacritics(matchName);
            matchName = matchName.ToLower();
            string[] teamNames = matchName.Split(" - ", 2);
            if (teamNames.Length == 2)
            {
                Tuple<string, string> recognitionTeamsTuple = new Tuple<string, string>(teamNames[0], teamNames[1]);
                return recognitionTeamsTuple;
            }
            else
            {
                return new Tuple<string, string>(teamNames[0], "");
            }
                
        }
        
        private string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

            for (int i = 0; i < normalizedString.Length; i++)
            {
                char c = normalizedString[i];
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder
                .ToString()
                .Normalize(NormalizationForm.FormC);

            //https://stackoverflow.com/questions/249087/how-do-i-remove-diacritics-accents-from-a-string-in-net
        }
    }
}
