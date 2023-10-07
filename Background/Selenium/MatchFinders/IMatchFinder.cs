using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlipalooWeb.DataStructure;
using OpenQA.Selenium;

namespace FlipalooWeb.Background.BettingOddsFinders
{
    interface IMatchFinder
    {
        public ListOfMatches FindAllMatches(IWebDriver webDriver);
    }
}
