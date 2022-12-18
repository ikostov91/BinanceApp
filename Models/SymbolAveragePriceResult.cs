namespace BinanceWebsocketApp.Models
{
    public class SymbolAveragePriceResult
    {
        public string Name { get; set; } = default!;

        public decimal Price { get; set; }
    }
}
