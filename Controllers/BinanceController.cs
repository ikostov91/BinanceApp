using BinanceWebsocketApp.Contracts;
using BinanceWebsocketApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BinanceWebsocketApp.Controllers
{
    [ApiController]
    [Route("api")]
    public class BinanceController : ControllerBase
    {
        private readonly IBinanceService _binanceService;

        public BinanceController(IBinanceService binanceService)
        {
            this._binanceService = binanceService;
        }

        [HttpGet("{symbol}/24hAvgPrice")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get24hAverageSymbolPrice(string symbol)
        {
            var averagePrice = await this._binanceService.Get24hAveragePriceForSymbol(symbol); 
            if (averagePrice is null)
            {
                return NotFound();
            }

            var result = new SymbolAveragePriceResult() { Name = symbol, Price = averagePrice.Value };
            return Ok(result);
        }

        [HttpGet("{symbol}/SimpleMovingAverage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSymbolSimpleMovingAverage(string symbol, [FromQuery] SimpleMovingAverageQueryParameters queryParameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var symbolSimpleMovingAverage = await this._binanceService.GetSimpleMovingAverageForSymbol(
                symbol,
                queryParameters.DataPointsCount!.Value,
                queryParameters.TimePeriod!,
                queryParameters.StartingDate);

            var result = new SymbolAveragePriceResult() { Name = symbol, Price = symbolSimpleMovingAverage };
            return Ok(result);            
        }
    }
}