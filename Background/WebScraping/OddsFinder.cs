using FlipalooWeb.Models;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using FlipalooWeb.DataStructure;
using FlipalooWeb.Background.BettingOddsFinders;
using OpenQA.Selenium.Edge;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium.Chrome;

namespace FlipalooWeb.Background
{
    public class OddsFinder
    {
        public void FindOdds()
        {
            List<IMatchFinder> matchFinders = new List<IMatchFinder>();
            
            matchFinders.Add(new BetanoMatchFinder());
            matchFinders.Add(new TipsportMatchFinder());
            matchFinders.Add(new FortunaMatchFinder());

            string geckoDriverDirectory = @"wwwroot/Drivers";
            var firefoxOptions = new FirefoxOptions();
            firefoxOptions.AddArgument("--headless");
            firefoxOptions.AddArgument("-no-sandbox");
            firefoxOptions.AddArgument("--no-sandbox");
            TimeSpan commandTimeOut = TimeSpan.FromSeconds(600);
            
            ListOfMatches finalListOfMatches = new ListOfMatches();
            
            foreach (var matchFinder in matchFinders)
            {
                var listOfMatches = matchFinder.FindAllMatches(geckoDriverDirectory, firefoxOptions, commandTimeOut);
                finalListOfMatches.Merge(listOfMatches);
            }

            List<Event> finalListOfEvents = finalListOfMatches.SplitToEvents();
            finalListOfEvents.Sort((a, b) => a.ImpliedProbability.CompareTo(b.ImpliedProbability));
            var listOfEvents = finalListOfEvents.Take(500);
            string json = JsonSerializer.Serialize<IEnumerable<Event>>(listOfEvents);
            File.WriteAllText(@"wwwroot/Data/BettingOdds.json", json);
            //https://stackoverflow.com/questions/16921652/how-to-write-a-json-file-in-c
        }
        
        public List<Event>? GetEvents(int page, int sizeOfPage)
        {
            string json = File.ReadAllText(@"wwwroot/Data/BettingOdds.json");
            List<Event>? bettingOdds = JsonSerializer.Deserialize<List<Event>>(json);
            if (bettingOdds == null)
                return new List<Event>();
            if (sizeOfPage * page <= bettingOdds.Count() - sizeOfPage) 
            {
                List<Event> events = bettingOdds.GetRange(sizeOfPage * (page), sizeOfPage);
                return events;
            }
            else if (sizeOfPage*page < bettingOdds.Count()) //returns rest of matches, when there are remaining less that pageSize
            {
                List<Event> events = bettingOdds.GetRange(sizeOfPage * (page), bettingOdds.Count - sizeOfPage * (page));
                return events;
            }
            else //return null when page is too big
            {
                return null;
            }
            
            
        }
    }
}
