using BinanceWebsocketApp.Persistance;
using Dapper;
using Newtonsoft.Json;
using System.Data;
using Websocket.Client;

namespace BinanceWebsocketApp.Services
{
    public class BackgroundDataCollectionService : BackgroundService
    {
        private readonly IConfiguration _configuration;

        public BackgroundDataCollectionService(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var exitEvent = new ManualResetEvent(false);
            var url = new Uri("wss://stream.binance.com:9443/stream?streams=btcusdt@aggTrade/adausdt@aggTrade/ethusdt@aggTrade");

            using var client = new WebsocketClient(url);

            client.ReconnectTimeout = TimeSpan.FromSeconds(30);
            client.ReconnectionHappened.Subscribe(info =>
                Console.WriteLine($"Reconnection happened, type: {info.Type}"));

            var dapperContext = new DapperContext(this._configuration);

            client.MessageReceived.Subscribe(async (msg) => await LogBinanceDataToDB(dapperContext, msg));
            await client.Start();

            await Task.Run(() => client.Send("{ message }"), stoppingToken);

            exitEvent.WaitOne();
        }

        private static async Task LogBinanceDataToDB(DapperContext context, ResponseMessage message)
        {
            var result = JsonConvert.DeserializeObject<BinanceLog>(message.Text);

            if (result?.Data is { Symbol: not null, Timestamp: not null })
            {
                string query = "INSERT INTO Symbols (Name, Price, Timestamp) VALUES (@Name, @Price, @Timestamp)";

                var parameters = new DynamicParameters();
                parameters.Add("Name", result.Data.Symbol!.ToLower(), DbType.String);
                parameters.Add("Price", result.Data.Price, DbType.Decimal);
                parameters.Add("Timestamp", DateTimeOffset.FromUnixTimeSeconds(result.Data.Timestamp!.Value / 1000).DateTime, DbType.DateTime2);

                using var connection = context.CreateConnection();
                await connection.ExecuteAsync(query, parameters);

                Console.WriteLine(message.Text);
            }
        }
    }
}
