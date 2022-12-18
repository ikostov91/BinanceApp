using Newtonsoft.Json;

namespace BinanceWebsocketApp.Persistance
{
    public class BinanceLog
    {
        [JsonProperty("stream")]
        public string? Stream { get; set; }

        [JsonProperty("data")]
        public SymbolData? Data { get; set; }

        public override string ToString()
        {
            return $"{Data?.Symbol} - {Data?.Price} - {Data?.Timestamp}";
        }
    }

    public class SymbolData
    {
        [JsonProperty("s")]
        public string? Symbol { get; set; }

        [JsonProperty("p")]
        public decimal? Price { get; set; }

        [JsonProperty("T")]
        public long? Timestamp { get; set; }
    }
}
