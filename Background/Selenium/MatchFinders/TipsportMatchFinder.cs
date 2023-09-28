﻿using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using FlipalooWeb.DataStructure;


namespace FlipalooWeb.Background.BettingOddsFinders
{
    internal class TipsportMatchFinder : IMatchFinder
    {
        string bettingShopName;
        string urlAll;
        string urlTomorrow;
        By matchElementPath;
        By namesElementPath;
        By oddsElementPath;
        By referenceLinkElementPath;
        string oddClassName;
        string blockedOddClassName;

        public TipsportMatchFinder()
        {
            bettingShopName = "TipSport";
            urlAll = "https://www.tipsport.cz/kurzy?timeFilter=form.period.anytime&limit=10000";
            urlTomorrow = "https://www.tipsport.cz/kurzy/zitra?limit=10000";
            matchElementPath = By.CssSelector(".o-matchRow");
            namesElementPath = By.CssSelector(".o-matchRow__matchName");
            oddsElementPath = By.CssSelector(".m-matchRowOdds");
            referenceLinkElementPath = By.CssSelector("a");
            oddClassName = "btnRate";
            blockedOddClassName = "btnRate disabled";
        }

        public ListOfMatches FindAllMatches(IWebDriver driver)
        {
            ListOfMatches listOfMatches = FindMatchesByUrl(driver, urlAll);
            return listOfMatches;
        }

        public ListOfMatches FindTomorrowMatches(IWebDriver driver)
        {
            ListOfMatches listOfMatches = FindMatchesByUrl(driver, urlTomorrow);
            return listOfMatches;
        }

        private ListOfMatches FindMatchesByUrl(IWebDriver driver, string url)
        {
            driver.Navigate().GoToUrl(url);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            wait.Until(ExpectedConditions.ElementIsVisible(matchElementPath));
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
            IWebElement matchNameElement = matchElement.FindElement(namesElementPath);
            string matchName = matchNameElement.Text;
            IWebElement referenceElement = matchElement.FindElement(referenceLinkElementPath);
            string referenceUrl = referenceElement.GetAttribute("href");

            var oddsElement = matchElement.FindElement(oddsElementPath);
            var roughOdds = GetOddsFromElement(oddsElement, referenceUrl);
            MatchOdds sortedOdds = SortOdds(roughOdds);

            var recognitionTeams = GetRecognitionTeams(matchName);

            if (sortedOdds == null)
                return null;
            else if (sortedOdds.Odds.Length == 2)
                return new Match(recognitionTeams.Item1, recognitionTeams.Item2, sortedOdds);
            else if (sortedOdds.Odds.Length == 6)
                return new Match(recognitionTeams.Item1, recognitionTeams.Item2, sortedOdds);
            else
                return null;
        }

        private Odd?[] GetOddsFromElement(IWebElement oddsElement, string referenceUrl)
        {
            var maybeOddElements = oddsElement.FindElements(By.TagName("div"));
            List<Odd?> oddsList = new List<Odd?>();

            foreach (IWebElement maybeOddElement in maybeOddElements)
            {
                if (maybeOddElement.GetAttribute("class") == oddClassName)
                    oddsList.Add(GetOddFromElement(maybeOddElement, referenceUrl));
                else if (maybeOddElement.GetAttribute("class") == blockedOddClassName)
                    oddsList.Add(null);
            }

            return oddsList.ToArray();
        }

        private Odd? GetOddFromElement(IWebElement element, string referenceUrl)
        {
            float? odd = float.Parse(element.Text, CultureInfo.InvariantCulture.NumberFormat);
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
                Odd?[] finalOdds = { null, null};
                return new MatchOdds(finalOdds);
            }
        }

        private Tuple<string, string> GetRecognitionTeams(string matchName)
        {
            string[] teamNames = matchName.Split(" - ", 2);
            string[] recognitionTeams = new string[2];
            for(int i = 0; i < teamNames.Length; i++)
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