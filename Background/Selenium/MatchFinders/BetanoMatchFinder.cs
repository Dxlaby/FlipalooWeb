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
/*
namespace FlipalooWeb.Background.BettingOddsFinders
{
    internal class BetanoMatchFinder
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
        }

        public ListOfMatches FindAllMatches(IWebDriver driver)
        {
            List<string> regionUrls = new List<string>();
            ListOfMatches finalListOfMatches = new ListOfMatches();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));

            driver.Navigate().GoToUrl("https://www.betano.cz/");
            driver.Manage().Cookies.DeleteAllCookies();
            driver.FindElement(closePopUpButton).Click();
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
                driver.Navigate().GoToUrl(regionUrl);
                ListOfMatches listOfMatches = FindMatches(driver);
                finalListOfMatches.Merge(listOfMatches);
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
                IMatch? match = GetMatchFromElement(matchElement);
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
                IMatch? match = GetMatchFromElement(matchElement);
                if (match != null)
                    listOfMatches.Matches.Add(match);
            }

            return listOfMatches;
        }

        private IMatch? GetMatchFromElement(IWebElement matchElement)
        {
            var matchNameElements = matchElement.FindElements(nameElementPath);
            string matchName = matchNameElements.ElementAt(0).Text + " - " + matchNameElements.ElementAt(1).Text;
            Tuple<string, float?>[]? roughOdds = null;
            
            try
            {
                var oddsElement = matchElement.FindElement(oddsElementPath);
                roughOdds = GetOddsFromElement(oddsElement);
            }
            catch
            {
               
            }
            
            
            var sortedOdds = SortOdds(roughOdds);

            var recognitionTeams = GetRecognitionTeams(matchName);

            if (sortedOdds == null)
                return null;
            else if (sortedOdds.Length == 2)
                return new TwoOutcomeMatchOdds(matchName, recognitionTeams.Item1, recognitionTeams.Item2, sortedOdds);
            else if (sortedOdds.Length == 6)
                return new ThreeOutcomeMatchOdds(matchName, recognitionTeams.Item1, recognitionTeams.Item2, sortedOdds);
            else
                return null;
        }

        private Tuple<string, float?>[] GetOddsFromElement(IWebElement oddsElement)
        {
            var oddElements = oddsElement.FindElements(oddElementPath);
            List<Tuple<string, float?>> oddsList = new List<Tuple<string, float?>>();

            foreach (IWebElement oddElement in oddElements)
            {
                var odd = GetOddFromElement(oddElement);
                oddsList.Add(odd);
            }

            return oddsList.ToArray();
        }

        private Tuple<string, float?> GetOddFromElement(IWebElement element)
        {
            float? odd = float.Parse(element.Text.Replace(" ", ""), CultureInfo.InvariantCulture.NumberFormat);
            return new Tuple<string, float?>(bettingShopName, odd);
        }

        private Tuple<string, float?>[]? SortOdds(Tuple<string, float?>[]? roughOdds)
        {
            if (roughOdds == null)
            {
                return null;
            }
            else if (roughOdds.Count() == 2)
            {
                return roughOdds;
            }
            else if (roughOdds.Count() == 3)
            {
                Tuple<string, float?>[] finalOdds =
                { roughOdds[0], roughOdds[1], roughOdds[2],
                    new Tuple<string, float?>(bettingShopName, null),
                    new Tuple<string, float?>(bettingShopName, null),
                    new Tuple<string, float?>(bettingShopName, null)};
                return finalOdds;
            }
            else
            {
                return null;
            }
        }

        private Tuple<string, string> GetRecognitionTeams(string matchName)
        {
            matchName.Replace("/", " ");
            matchName.Replace(".", " ");
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
                recognitionTeams[i] = String.Join("", finalTeamWordList);
                recognitionTeams[i] = RemoveDiacritics(recognitionTeams[i]);
                recognitionTeams[i] = recognitionTeams[i].ToLowerInvariant();
            }

            Tuple<string, string> recognitionTeamsTuple = new Tuple<string, string>(recognitionTeams[0], recognitionTeams[1]);
            return recognitionTeamsTuple;
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
        }
    }
}
*/