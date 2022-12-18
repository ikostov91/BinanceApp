namespace BinanceWebsocketApp.Contracts
{
    public interface IBinanceService
    {
        Task<decimal?> Get24hAveragePriceForSymbol(string symbol);
        Task<decimal> GetSimpleMovingAverageForSymbol(string symbol, int dataPointsCount, string timePeriod, DateTime? calculationStartDate);
    }
}
