// -----------------------------------------------------------------------
// <copyright file="CoinInfoService.cs" company="SóiKA Apps">
//      All rights are reserved. Reproduction or transmission in whole or
//      in part, in any form or by any means, electronic, mechanical or
//      otherwise, is prohibited without the prior written consent of the 
//      copyright owner.
// </copyright>
// <summary>
//      Definition of the CoinInfoService.cs class.
// </summary>
// -----------------------------------------------------------------------

namespace ExchangersAnalizer.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using ExchangeSharp;
    using Extensions;
    using Models;

    public class CoinInfoService : ICoinInfoService
    {
        private readonly ExchangeBinanceAPI binanceApi;
        private readonly ExchangeBithumbAPI bithumbApi;
        private readonly ExchangeBittrexAPI bittrexApi;
        private readonly ExchangeHitbtcAPI hitbtcApi;
        private readonly ExchangeOkexAPI okexApi;
        private readonly ExchangePoloniexAPI poloniexApi;

        public CoinInfoService(
            ExchangeBithumbAPI bithumbApi,
            ExchangeBittrexAPI bittrexApi,
            ExchangeBinanceAPI binanceApi,
            ExchangeHitbtcAPI hitbtcApi,
            ExchangeOkexAPI okexApi,
            ExchangePoloniexAPI poloniexApi)
        {
            this.bithumbApi = bithumbApi;
            this.binanceApi = binanceApi;
            this.bittrexApi = bittrexApi;
            this.hitbtcApi = hitbtcApi;
            this.okexApi = okexApi;
            this.poloniexApi = poloniexApi;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<CoinInfo>> GetExchangerCoinInfoAsync()
        {
            var symbols = await binanceApi.GetSymbolsAsync();
            var globalSymbols = symbols.ToGlobalSymbols();
            return globalSymbols.Select(symbol => new CoinInfo {Symbol = symbol}).ToList();
        }

        private async Task<IEnumerable<string>> GetCoinSymbols()
        {
            var symbols = await binanceApi.GetSymbolsAsync();
            return symbols.ToGlobalSymbols();
        }
    }
}