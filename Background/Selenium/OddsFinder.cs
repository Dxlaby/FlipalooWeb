using FlipalooWeb.Models;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using FlipalooWeb.DataStructure;
using FlipalooWeb.Background.BettingOddsFinders;
using OpenQA.Selenium.Edge;
using System.Text.Json;

namespace FlipalooWeb.Background
{
    public class OddsFinder
    {
        public void FindOdds() 
        {
            //vytvorit nazvy tymu v zapasech k rozpoznani
            //upravit split events v ThreeOutcomeMatchOdds
            //*
            
            //MatchFinder matchFinder = new MatchFinder();
            ListOfMatches finalListOfMatches = new ListOfMatches();
            TipsportMatchFinder tipsportMatchFinder = new TipsportMatchFinder();
            //BetanoMatchFinder betanoMatchFinder = new BetanoMatchFinder();


            /*
            FirefoxDriverService service = FirefoxDriverService.CreateDefaultService(@"wwwroot\Drivers\", "geckodriver.exe");
            service.FirefoxBinaryPath = @"C:\Program Files\Mozilla Firefox\firefox.exe";            
            var firefoxOptions = new FirefoxOptions();
            firefoxOptions.AddArgument("--headless");
            //firefoxOptions.AddArgument("no-sandbox");
            firefoxOptions.BinaryLocation = @"C:\Program Files\Mozilla Firefox\firefox.exe";       
            */
            EdgeOptions edgeOptions = new EdgeOptions();
            edgeOptions.AddArgument("--headless");
            var driver = new EdgeDriver();





            ListOfMatches tipsportMatches = tipsportMatchFinder.FindAllMatches(driver);
            //ListOfMatches betanoMatches = betanoMatchFinder.FindAllMatches(driver);
            driver.Close();
            //ListOfMatches fortunaMatches = matchFinder.FortunaFindMatches();

            finalListOfMatches.Merge(tipsportMatches);
            //finalListOfMatches.Merge(fortunaMatches);
            //finalListOfMatches.Merge(betanoMatches);

            ListOfEvents finalListOfEvents = finalListOfMatches.SplitToEvents();
            finalListOfEvents.SortByImpliedProbability();
            finalListOfEvents.WriteToJson(@"wwwroot/Data/BettingOdds.json");
            
            //write list of events to .json file
            //finalListOfEvents.PrintToConsole();
            
            
            /*/
            WebDriver driver = new EdgeDriver();
            BetanoMatchFinder betanoMatchFinder = new BetanoMatchFinder();
            ListOfMatches betanoMatches = betanoMatchFinder.FindAllMatches(driver);
            driver.Close();
            betanoMatches.SortByName();
            betanoMatches.PrintToConsole();
            ListOfEvents finalListOfEvents = betanoMatches.SplitToEvents();
            finalListOfEvents.SortByImpliedProbability();
            finalListOfEvents.PrintToConsole();
            //*/
        }

        public ListOfEvents GetEvents()
        {
            string json = File.ReadAllText(@"wwwroot/Data/BettingOdds.json");
            ListOfEvents? weatherForecast = JsonSerializer.Deserialize<ListOfEvents>(json);
            if (weatherForecast != null)
            {
                return weatherForecast;
            }
            else
                return new ListOfEvents();
        }
    }
}
