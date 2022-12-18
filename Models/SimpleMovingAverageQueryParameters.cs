using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;

namespace BinanceWebsocketApp.Models
{
    public class SimpleMovingAverageQueryParameters
    {
        [BindRequired]
        [FromQuery(Name = "n")]
        public int? DataPointsCount { get; set; }

        [BindRequired]
        [FromQuery(Name = "p")]
        public string? TimePeriod { get; set; }

        [FromQuery(Name = "s")]
        public DateTime? StartingDate { get; set; }
    }
}
