namespace BinanceWebsocketApp.Persistance
{
    public class Symbol
    {
        public int Id { get; set; }

        public string Name { get; set; } = default!;

        public decimal Price { get; set; } = default;

        public DateTime Timestamp { get; set; } = default!;
    }
}
