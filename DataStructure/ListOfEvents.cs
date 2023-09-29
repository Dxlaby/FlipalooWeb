using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FlipalooWeb.DataStructure
{
    public class ListOfEvents
    {
        public List<Event> Events { get; set; }

        public ListOfEvents()
        {
            Events = new List<Event>();
        }

        public ListOfEvents(List<Event> events)
        {
            Events = events;
        }

        public void AddEvent(Event oddsEvent)
        {
            Events.Add(oddsEvent);
        }

        public void AddListOfEvents(ListOfEvents listOfEvents)
        {
            Events.AddRange(listOfEvents.Events);
        }

        public void SortByImpliedProbability()
        {
            Events.Sort((a, b) => b.GetImpliedProbability().CompareTo(a.GetImpliedProbability()));
            Events.Reverse();
        }
        /*
        public void SortByName()
        {
            Events.Sort((a, b) => a.Name.CompareTo(b.Name));
        }
        */

        public void WriteToJson(string JSONPath)
        {
            string json = JsonSerializer.Serialize<ListOfEvents>(this);
            File.WriteAllText(JSONPath, json);

            //https://stackoverflow.com/questions/16921652/how-to-write-a-json-file-in-c
        }

        public void PrintToConsole()
        {
            Console.Clear();
            Console.Write("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");

            foreach (Event oddsEvent in Events)
            {
                Console.WriteLine(oddsEvent.RecognitionTeamName1 + " - " + oddsEvent.RecognitionTeamName2);
                foreach (Odd value in oddsEvent.Odds)
                {
                    Console.Write(value.BettingShop + ": ");
                    Console.Write(value.BettingOdd + "; ");
                }
                Console.WriteLine();
                Console.WriteLine("Implied probability: " + oddsEvent.GetImpliedProbability());
                float profit = 1 / oddsEvent.GetImpliedProbability() - 1;
                Console.WriteLine("Profit: " + profit * 100 + "%");
                Console.WriteLine();
                Console.WriteLine();
            }
            Console.WriteLine("Celkovy pocet udalosti: " + Events.Count);
        }
    }
}
