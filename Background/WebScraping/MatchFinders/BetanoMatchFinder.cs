﻿using OpenQA.Selenium.Support.UI;
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
using OpenQA.Selenium.Firefox;
using HtmlAgilityPack;

namespace FlipalooWeb.Background.BettingOddsFinders
{
    internal class BetanoMatchFinder : IMatchFinder
    {
        private readonly string _bettingShopName;
        private readonly List<string> _sportUrls;
        private readonly string _matchElementPath;
        private readonly string _namesElementPath;
        private readonly string _nameElementPath;
        private readonly string _oddsElementPath;
        private readonly string _oddElementPath;
        private readonly string _showButtonElementPath;
        //By sportBoxElementPath;
        private readonly string _regionBoxElementPath;
        private readonly string _regionNameElementPath;
        private readonly string _checkBoxElementPath;
        private readonly string _closePopUpButton;
        private readonly string _referenceLinkElementPath;
        private readonly string _timeElementPath;
        //By gridElementPath;

        //string oddClassName;
        //string blockedOddClassName;

        public BetanoMatchFinder()
        {
            _bettingShopName = "Betano";
            _sportUrls = new List<string> 
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
            _matchElementPath = ".//*[@class='" + "events-list__grid__event" + "']";
            _namesElementPath = ".//*[@class='" + "events-list__grid__info__main__participants" + "']";
            _nameElementPath = ".//*[@class='" + "events-list__grid__info__main__participants__participant-name  tw-truncate" + "']";
            _oddsElementPath = ".//*[@class='" + "table__markets__market" + "']";
            _oddElementPath = ".//*[@class='" + "selections__selection__odd" + "']";
            _showButtonElementPath = ".//*[@class='" + "tw-bg-n-17-black-pearl" + "']";
            //sportBoxElementPath = By.CssSelector(".sport-block__item");
            _regionBoxElementPath = ".//*[@class='" + "tw-flex tw-flex-col tw-pb-m tw-pt-n tw-bg-white-snow" + "']";
            _regionNameElementPath = ".//*[@class='" + "tw-pl-[4px] tw-text-s tw-leading-s tw-text-n-13-steel tw-no-underline hover:tw-font-bold tw-line-clamp-1" + "']";
            _checkBoxElementPath = ".//*[@class='" + "tw-inline-flex" + "']";
            _closePopUpButton = ".//*[@class='" + "sb-modal__close__btn" + "']";
            //gridElementPath = By.CssSelector(".grid__column");

            //oddClassName = "btnRate";
            //blockedOddClassName = "btnRate disabled";
            _referenceLinkElementPath = ".//*[@class='" + "GTM-event-link events-list__grid__info__main" + "']";
            _timeElementPath = ".//*[@class='" + "tw-flex tw-flex-row tw-justify-start tw-items-center tw-text-xs tw-leading-s tw-text-n-48-slate tw-flex-col-reverse tw-justify-center"
                                              + "']";
        }

        public ListOfMatches FindAllMatches(string geckoDriverDirectory, FirefoxOptions options, TimeSpan commandTimeOut) //
        {
            HtmlWeb web = new HtmlWeb();
            
            List<string> regionUrls = new List<string>();
            ListOfMatches finalListOfMatches = new ListOfMatches();
            //var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));
            
            //driver.Navigate().GoToUrl("https://www.betano.cz/");
            //Thread.Sleep(1000);
            
            foreach (string url in _sportUrls)
            {
                // driver.Manage().Cookies.DeleteAllCookies();
                // driver.Navigate().GoToUrl(url);
                // try
                // {
                //     wait.Until(ExpectedConditions.ElementIsVisible(regionNameElementPath));
                // }
                // catch
                // {
                //     continue;
                // }
                //
                // try
                // {
                //     driver.FindElement(closePopUpButton).Click();
                // }
                // catch
                // {
                //
                // }
                var sportHtml = new HtmlDocument();
                using (var driver = new FirefoxDriver(geckoDriverDirectory, options, commandTimeOut))
                {
                    driver.Navigate().GoToUrl(url);
                    var sportContent = driver.PageSource;
                    sportHtml.LoadHtml(sportContent);
                    driver.Quit();
                }
                
                // Console.WriteLine(sportContent);
                // Console.WriteLine("\n\n\n\n\n\n");
                // Console.WriteLine(sportHtml);
                var regionNameElements = sportHtml.DocumentNode.SelectNodes(_regionNameElementPath);
            
                foreach (var regionName in regionNameElements)
                {
                    if (regionName == null)
                        continue;
                    string regionUrl = "https://betano.cz" + regionName.Attributes["href"].Value;
                    regionUrls.Add(regionUrl); 
                }
                
            }
            
            foreach (string regionUrl in regionUrls)
            {
                var regionUrlHtml = new HtmlDocument();
               
                using (var driver = new FirefoxDriver(geckoDriverDirectory, options, commandTimeOut))
                {
                    driver.Navigate().GoToUrl(regionUrl);
                    var regionUrlContent = driver.PageSource;
                    regionUrlHtml.LoadHtml(regionUrlContent);
                    driver.Quit();
                }
                
                ListOfMatches listOfMatches = FindMatches(regionUrlHtml);
                finalListOfMatches.AddListOfMatches(listOfMatches);
            }
            
            return finalListOfMatches;
        }

        private ListOfMatches FindMatches(HtmlDocument htmlDocument)
        {
            // try
            // {
            //     wait.Until(ExpectedConditions.ElementIsVisible(_oddElementPath));
            // }
            // catch
            // {
            //     return new ListOfMatches();
            // }

            var matchesElements = htmlDocument.DocumentNode.SelectNodes(_matchElementPath);
            if (matchesElements == null)
                return new ListOfMatches();
            ListOfMatches listOfMatches = new ListOfMatches();

            foreach (var matchElement in matchesElements)
            {
                Match? match = GetMatchFromElement(matchElement);
                if (match != null)
                    listOfMatches.Matches.Add(match);
            }

            return listOfMatches;
        }

        // private ListOfMatches FindMatchesByUrl(IWebDriver driver, WebDriverWait wait, string url)
        // {
        //     driver.Manage().Cookies.DeleteAllCookies();
        //     driver.Navigate().GoToUrl(url);
        //     try
        //     {
        //         wait.Until(ExpectedConditions.ElementIsVisible(_oddElementPath));
        //     }
        //     catch
        //     {
        //         return new ListOfMatches();
        //     }
        //
        //     IReadOnlyCollection<IWebElement>? matchesElements = driver.FindElements(_matchElementPath);
        //     ListOfMatches listOfMatches = new ListOfMatches();
        //
        //     foreach (IWebElement matchElement in matchesElements)
        //     {
        //         Match? match = GetMatchFromElement(matchElement);
        //         if (match != null)
        //             listOfMatches.Matches.Add(match);
        //     }
        //
        //     return listOfMatches;
        // }

        private Match? GetMatchFromElement(HtmlNode matchElement)
        {
            var matchNameElements = matchElement.SelectNodes(_nameElementPath);
            string matchName = matchNameElements.ElementAt(0).InnerText + " - " + matchNameElements.ElementAt(1).InnerText;
            var referenceElement = matchElement.SelectSingleNode(_referenceLinkElementPath);
            string referenceUrl = "https://betano.cz" + referenceElement.Attributes["href"].Value;
            
            var timeElement = matchElement.SelectSingleNode(_timeElementPath);
            var timeElements = timeElement.SelectNodes(".//span");
            DateTime? dateTime = GetDate(timeElements.ElementAt(0).InnerText, timeElements.ElementAt(1).InnerText);
            if (dateTime == null)
                return null;
            if ((dateTime.Value - DateTime.Now).TotalHours < 2)
                return null;
            
            Odd?[]? roughOdds = null;
            
            try
            {
                var oddsElement = matchElement.SelectSingleNode(_oddsElementPath);
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
                return new Match(matchName, recognitionTeams.Item1, 
                    recognitionTeams.Item2, dateTime.Value, sortedOdds);
            else if (sortedOdds.Odds.Length == 6)
                return new Match(matchName, recognitionTeams.Item1,
                    recognitionTeams.Item2, dateTime.Value, sortedOdds);
            else
                return null;
        }

        private Odd?[] GetOddsFromElement(HtmlNode oddsElement, string referenceUrl)
        {
            var oddElements = oddsElement.SelectNodes(_oddElementPath);
            List<Odd?> oddsList = new List<Odd?>();

            foreach (var oddElement in oddElements)
            {
                var odd = GetOddFromElement(oddElement, referenceUrl);
                oddsList.Add(odd);
            }

            return oddsList.ToArray();
        }

        private Odd? GetOddFromElement(HtmlNode element, string referenceUrl)
        {
            float? odd = float.Parse(element.InnerText.Replace(" ", ""), CultureInfo.InvariantCulture.NumberFormat);
            if (odd == null)
                return null;
            else
                return new Odd(_bettingShopName, referenceUrl, odd.Value);
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

        private DateTime? GetDate(string date, string time) //not ideal to how try method here
        {
            string[] dates = date.Split(".", 2);
            string[] times = time.Split(":", 2);
            try
            {
                DateTime dateYearNow = new DateTime(DateTime.Now.Year, int.Parse(dates[1]), int.Parse(dates[0]),
                    int.Parse(times[0]), int.Parse(times[1]), 0);
                DateTime dateYearLater = new DateTime(DateTime.Now.Year + 1, int.Parse(dates[1]), int.Parse(dates[0]),
                    int.Parse(times[0]), int.Parse(times[1]), 0);
                if (dateYearNow > DateTime.Now)
                    return dateYearNow;
                return dateYearLater;
            }
            catch
            {
                return null;
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