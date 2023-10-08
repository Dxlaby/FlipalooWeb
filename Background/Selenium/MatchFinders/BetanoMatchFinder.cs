using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using FlipalooWeb.DataStructure;

namespace FlipalooWeb.Background.BettingOddsFinders
{
    internal class BetanoMatchFinder : IMatchFinder
    {
        string bettingShopName;
        List<string> sportUrls;
        By matchElementPath;
        By namesElementPath;
        By nameElementPath;
        By oddsElementPath;
        By oddElementPath;
        By showButtonElementPath;
        //By sportBoxElementPath;
        By regionBoxElementPath;
        By regionNameElementPath;
        By checkBoxElementPath;
        By closePopUpButton;
        By referenceLinkElementPath;
        //By gridElementPath;

        //string oddClassName;
        //string blockedOddClassName;

        public BetanoMatchFinder()
        {
            bettingShopName = "Betano";
            sportUrls = new List<string> 
            { 
                "https://www.betano.cz/sport/fotbal/", 
                "https://www.betano.cz/sport/ledni-hokej/",
                "https://www.betano.cz/sport/tenis/", 
                "https://www.betano.cz/sport/mma/", 
                "https://www.betano.cz/sport/stolni-tenis/", 
                "https://www.betano.cz/sport/esports/", 
                "https://www.betano.cz/sport/baseball/", 
                "https://www.betano.cz/sport/sipky/", 
                "https://www.betano.cz/sport/americky-fotbal/", 
                "https://www.betano.cz/sport/volejbal/",
                "https://www.betano.cz/sport/hazena/", 
                "https://www.betano.cz/sport/futsal/", 
                "https://www.betano.cz/sport/plazovy-volejbal/",
                "https://www.betano.cz/sport/vodni-polo/", 
                "https://www.betano.cz/sport/box/", 
                "https://www.betano.cz/sport/rugby-union/"
            };
            matchElementPath = By.CssSelector(".events-list__grid__event");
            namesElementPath = By.CssSelector(".events-list__grid__info__main__participants");
            nameElementPath = By.CssSelector(".events-list__grid__info__main__participants__participant-name");
            oddsElementPath = By.CssSelector(".selections");
            oddElementPath = By.CssSelector(".selections__selection__odd");
            showButtonElementPath = By.CssSelector(".tw-bg-n-17-black-pearl");
            //sportBoxElementPath = By.CssSelector(".sport-block__item");
            regionBoxElementPath = By.CssSelector(".tw-flex.tw-flex-col.tw-pb-m.tw-pt-n.tw-bg-white-snow");
            regionNameElementPath = By.CssSelector("div.tw-flex.tw-justify-center.tw-items-center a");
            checkBoxElementPath = By.CssSelector(".tw-inline-flex");
            closePopUpButton = By.CssSelector(".sb-modal__close__btn");
            //gridElementPath = By.CssSelector(".grid__column");

            //oddClassName = "btnRate";
            //blockedOddClassName = "btnRate disabled";
            referenceLinkElementPath = By.CssSelector(".GTM-event-link");
        }

        public ListOfMatches FindAllMatches(IWebDriver driver)
        {
            List<string> regionUrls = new List<string>();
            ListOfMatches finalListOfMatches = new ListOfMatches();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));

            driver.Navigate().GoToUrl("https://www.betano.cz/");
            Thread.Sleep(1000);

            foreach (string url in sportUrls)
            {
                driver.Navigate().GoToUrl(url);
                try
                {
                    wait.Until(ExpectedConditions.ElementIsVisible(regionNameElementPath));
                }
                catch
                {
                    continue;
                }
                
                try
                {
                    driver.FindElement(closePopUpButton).Click();
                }
                catch
                {

                }

                var regionNameElements = driver.FindElements(regionNameElementPath);

                foreach (var regionName in regionNameElements)
                {
                    string regionUrl = regionName.GetAttribute("href");
                    regionUrls.Add(regionUrl);
                }
                
            }

            foreach (string regionUrl in regionUrls)
            {
                try
                {
                    driver.Navigate().GoToUrl(regionUrl);
                }
                catch
                {
                    continue;
                }
                ListOfMatches listOfMatches = FindMatches(driver);
                finalListOfMatches.AddListOfMatches(listOfMatches);
            }

            return finalListOfMatches;
        }

        private ListOfMatches FindMatches(IWebDriver driver)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));
            try
            {
                wait.Until(ExpectedConditions.ElementIsVisible(oddElementPath));
            }
            catch
            {
                return new ListOfMatches();
            }

            IReadOnlyCollection<IWebElement>? matchesElements = driver.FindElements(matchElementPath);
            ListOfMatches listOfMatches = new ListOfMatches();

            foreach (IWebElement matchElement in matchesElements)
            {
                Match? match = GetMatchFromElement(matchElement);
                if (match != null)
                    listOfMatches.Matches.Add(match);
            }

            return listOfMatches;
        }

        private ListOfMatches FindMatchesByUrl(IWebDriver driver, string url)
        {
            driver.Navigate().GoToUrl(url);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));
            try
            {
                wait.Until(ExpectedConditions.ElementIsVisible(oddElementPath));
            }
            catch
            {
                return new ListOfMatches();
            }

            IReadOnlyCollection<IWebElement>? matchesElements = driver.FindElements(matchElementPath);
            ListOfMatches listOfMatches = new ListOfMatches();

            foreach (IWebElement matchElement in matchesElements)
            {
                Match? match = GetMatchFromElement(matchElement);
                if (match != null)
                    listOfMatches.Matches.Add(match);
            }

            return listOfMatches;
        }

        private Match? GetMatchFromElement(IWebElement matchElement)
        {
            var matchNameElements = matchElement.FindElements(nameElementPath);
            string matchName = matchNameElements.ElementAt(0).Text + " - " + matchNameElements.ElementAt(1).Text;
            IWebElement referenceElement = matchElement.FindElement(referenceLinkElementPath);
            string referenceUrl = referenceElement.GetAttribute("href");

            Odd?[]? roughOdds = null;
            
            try
            {
                var oddsElement = matchElement.FindElement(oddsElementPath);
                roughOdds = GetOddsFromElement(oddsElement, referenceUrl);
            }
            catch
            {
               return null;
            }
            
            
            var sortedOdds = SortOdds(roughOdds);

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
            var oddElements = oddsElement.FindElements(oddElementPath);
            List<Odd?> oddsList = new List<Odd?>();

            foreach (IWebElement oddElement in oddElements)
            {
                var odd = GetOddFromElement(oddElement, referenceUrl);
                oddsList.Add(odd);
            }

            return oddsList.ToArray();
        }

        private Odd? GetOddFromElement(IWebElement element, string referenceUrl)
        {
            float? odd = float.Parse(element.Text.Replace(" ", ""), CultureInfo.InvariantCulture.NumberFormat);
            if (odd == null)
                return null;
            else
                return new Odd(bettingShopName, referenceUrl, odd.Value);
        }

        private MatchOdds SortOdds(Odd?[] roughOdds)
        {

            if (roughOdds.Count() == 2)
            {
                return new MatchOdds(roughOdds);
            }
            else if (roughOdds.Count() == 3)
            {
                Odd?[] finalOdds =
                { roughOdds[0], roughOdds[1], roughOdds[2],
                    null, null, null};
                return new MatchOdds(finalOdds);
            }
            else if (roughOdds.Count() == 5)
            {
                Odd?[] finalOdds =
                { roughOdds[0], roughOdds[2], roughOdds[4],
                    roughOdds[1], roughOdds[3], null};
                //prohazeni kurzu tak aby byly ve formatu 1, 0, 2, 10, 20, 12
                return new MatchOdds(finalOdds);
            }
            else
            {
                Odd?[] finalOdds = { null, null };
                return new MatchOdds(finalOdds);
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
