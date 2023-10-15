using FlipalooWeb.Models;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using FlipalooWeb.DataStructure;
using FlipalooWeb.Background.BettingOddsFinders;
using OpenQA.Selenium.Edge;
using System.Text.Json;
using OpenQA.Selenium.Chrome;

namespace FlipalooWeb.Background
{
    public class OddsFinder
    {
        public void FindOdds()
        {
            List<IMatchFinder> matchFinders = new List<IMatchFinder>();
            
            matchFinders.Add(new TipsportMatchFinder());
            matchFinders.Add(new FortunaMatchFinder());
            matchFinders.Add(new BetanoMatchFinder());
            
            
            var firefoxOptions = new FirefoxOptions();
            firefoxOptions.AddArgument("--headless");
            firefoxOptions.AddArgument("-no-sandbox");
            var driver = new FirefoxDriver(@"wwwroot/Drivers", firefoxOptions, TimeSpan.FromSeconds(600));

            /*@"wwwroot/Drivers/geckodriver",
            var options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("no-sandbox");
            var driver = new ChromeDriver(@"wwwroot/Drivers/chromedriver-linux64/chromedriver",options);
            */
            
            ListOfMatches finalListOfMatches = new ListOfMatches();
            
            foreach (var matchFinder in matchFinders)
            {
                var listOfMatches = matchFinder.FindAllMatches(driver);
                finalListOfMatches.Merge(listOfMatches);
            }
            driver.Quit();
            
            List<Event> finalListOfEvents = finalListOfMatches.SplitToEvents();
            finalListOfEvents.Sort((a, b) => a.GetImpliedProbability().CompareTo(b.GetImpliedProbability()));
            var listOfEvents = finalListOfEvents.Take(500);
            string json = JsonSerializer.Serialize<IEnumerable<Event>>(listOfEvents);
            File.WriteAllText(@"wwwroot/Data/BettingOdds.json", json);
            //https://stackoverflow.com/questions/16921652/how-to-write-a-json-file-in-c
        }

        public List<Event> GetEvents()
        {
            string json = File.ReadAllText(@"wwwroot/Data/BettingOdds.json");
            List<Event>? weatherForecast = JsonSerializer.Deserialize<List<Event>>(json);
            if (weatherForecast != null)
            {
                return weatherForecast;
            }
            else
                return new List<Event>();
        }
    }
}
