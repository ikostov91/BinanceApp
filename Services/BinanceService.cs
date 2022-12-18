using BinanceWebsocketApp.Contracts;
using BinanceWebsocketApp.Persistance;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using System.Data;

namespace BinanceWebsocketApp.Services
{
    public class BinanceService : IBinanceService
    {
        private readonly DapperContext _context;
        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions memoryCacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(1))
            .SetAbsoluteExpiration(TimeSpan.FromHours(1))
            .SetPriority(CacheItemPriority.Normal);

        public BinanceService(
            DapperContext context,
            IMemoryCache memoryCache)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
            this._memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public async Task<decimal?> Get24hAveragePriceForSymbol(string symbol)
        {
            string queryForLast24h = @"
                SELECT * FROM Symbols
                 WHERE Name = @Name AND Timestamp >= @Timestamp
              ORDER BY Timestamp DESC";

            var parameters = new DynamicParameters();
            parameters.Add("Name", symbol.ToLower());
            parameters.Add("Timestamp", DateTime.Now.AddDays(-1), DbType.DateTime2);

            using var connection = this._context.CreateConnection();
            var result = await connection.QueryAsync<Symbol>(queryForLast24h, parameters);
            if (!result.Any())
            {
                string queryForLastAvailablePrice = @"
                    SELECT TOP(1) * FROM Symbols
                     WHERE Name = @Name
                  ORDER BY Timestamp DESC";

                var lastRow = await connection.QuerySingleOrDefaultAsync<Symbol>(queryForLastAvailablePrice, parameters);
                if (lastRow is null)
                {
                    return null;
                }

                return lastRow.Price;
            }

            var averagePrice = result.Select(x => x.Price).Average();

            return averagePrice;
        }

        public async Task<decimal> GetSimpleMovingAverageForSymbol(string symbol, int dataPointsCount, string timePeriod, DateTime? calculationStartDate)
        {
            DateTime startDate = calculationStartDate ?? DateTime.Now;
            string cacheKey = $"{symbol}/{dataPointsCount}/{timePeriod}/{startDate}";

            if (!this._memoryCache.TryGetValue(cacheKey, out decimal simpleMovingAverage))
            {
                long periodLengthInSeconds = GetPeriodLengthInSeconds(timePeriod);

                string query = @"
                    SELECT * FROM Symbols
                     WHERE Name = @Name AND Timestamp <= @StartDate AND Timestamp > @EndDate
                  ORDER BY Timestamp DESC";

                var parameters = new DynamicParameters();
                parameters.Add("Name", symbol.ToLower());

                List<decimal> periods = new();
                DateTime currentPeriodStart = startDate;

                using var connection = this._context.CreateConnection();
                for (int i = 0; i < dataPointsCount; i++)
                {
                    DateTime currentPeriodEnd = currentPeriodStart.AddSeconds(-periodLengthInSeconds);

                    parameters.Add("StartDate", currentPeriodStart, DbType.DateTime2);
                    parameters.Add("EndDate", currentPeriodEnd, DbType.DateTime2);

                    var result = await connection.QueryAsync<Symbol>(query, parameters);
                    periods.Add(result.Any() ? result.Select(x => x.Price).Average() : 0);

                    currentPeriodStart = currentPeriodEnd;
                }

                simpleMovingAverage = periods.Average();

                this._memoryCache.Set(cacheKey, simpleMovingAverage, memoryCacheEntryOptions);
            }

            return simpleMovingAverage;
        }

        #region Helpers
        private static long GetPeriodLengthInSeconds(string timePeriod)
        {
            TimeSpan timeSpan = timePeriod switch
            {
                "1w" => TimeSpan.FromDays(7),
                "1d" => TimeSpan.FromDays(1),
                "30m" => TimeSpan.FromMinutes(30),
                "5m" => TimeSpan.FromMinutes(5),
                "1m" => TimeSpan.FromMinutes(1),
                _ => throw new NotImplementedException(),
            };

            return (long)timeSpan.TotalSeconds;
        }
        #endregion
    }
}
