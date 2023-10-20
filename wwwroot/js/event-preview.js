// <div class="event-preview">
//     <div class="information-row">
//         <div class="name">
//             <h6>@matchEvent.Name</h6>
//         </div>
//
//         @if (matchEvent.GetProfitPercentage() > 0)
//         {
//             <div class="profit green">
//                 <b>Profit: @Math.Round(matchEvent.GetProfitPercentage(), 2).ToString() %</b>
//             </div>
//         }
//         else
//         {
//             <div class="profit red">
//                 <b>Profit: @Math.Round(matchEvent.GetProfitPercentage(), 2).ToString() %</b>
//             </div>
//         }
//     </div>
//     <div class="odds">
//         @foreach (Odd odd in matchEvent.Odds)
//         {
//             <a href=@odd.UrlReference class="odd @odd.BettingShop">
//             <div class="odd @odd.BettingShop">
//             @odd.BettingShop: @odd.BettingOdd
//             </div>
//             </a>
//         }
//     </div>
// </div>