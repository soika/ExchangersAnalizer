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
    using Enums;
    using ExchangeSharp;
    using Extensions;
    using Microsoft.Extensions.Caching.Memory;
    using Models;

    public class CoinInfoService : ICoinInfoService
    {
        private const string CoinInfoKey = "COINS";
        private const string SymbolsKey = "SYMBOLS";
        private readonly ExchangeBinanceAPI _binanceApi;
        private readonly ExchangeBittrexAPI _bittrexApi;
        private readonly ExchangeHitbtcAPI _hitbtcApi;
        private readonly ExchangeKucoinAPI _kucoinApi;
        private readonly ExchangeCryptopiaAPI _cryptopiaApi;
        private readonly ExchangeYobitAPI _yobitApi;
        private readonly IMemoryCache _memoryCache;

        public CoinInfoService(
            ExchangeBittrexAPI bittrexApi,
            ExchangeBinanceAPI binanceApi,
            ExchangeHitbtcAPI hitbtcApi,
            ExchangeKucoinAPI kucoinApi,
            ExchangeCryptopiaAPI cryptopiaApi,
            ExchangeYobitAPI yobitApi,
            IMemoryCache memoryCache)
        {
            _binanceApi = binanceApi;
            _bittrexApi = bittrexApi;
            _hitbtcApi = hitbtcApi;
            _kucoinApi = kucoinApi;
            _cryptopiaApi = cryptopiaApi;
            _yobitApi = yobitApi;
            _memoryCache = memoryCache;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<CoinInfo>> GetExchangerCoinInfoAsync(
            BaseCurrencyEnum currency = BaseCurrencyEnum.BTC)
        {
            var coins = _memoryCache.Get<List<CoinInfo>>(CoinInfoKey);
            if (coins != null)
            {
                return coins;
            }

            var binanceMarket = (await _binanceApi.GetTickersAsync()).ToList();
            var bittrexMarket = (await _bittrexApi.GetTickersAsync()).ToList();
            var hitbtcMarket = (await _hitbtcApi.GetTickersAsync()).ToList();
            var kucoinMarket = (await _kucoinApi.GetTickersAsync()).ToList();
            var cryptopiaMarket = (await _cryptopiaApi.GetTickersAsync()).ToList();
            //var yobitMarket = (await _yobitApi.GetTickersAsync()).ToList();

            var symbols = await GetExchangeSymbols();
            coins = symbols.Select(
                x => new CoinInfo
                {
                    ExchangeSymbol = x,
                    Currency = BaseCurrencyEnum.BTC,
                    ExchangePrices = new List<ExchangePrice>()
                }).ToList();

            coins = coins.FillExchangePrice(ExchangerEnum.Binance, binanceMarket);
            coins = coins.FillExchangePrice(ExchangerEnum.Bittrex, bittrexMarket);
            coins = coins.FillExchangePrice(ExchangerEnum.HitBtc, hitbtcMarket);
            coins = coins.FillExchangePrice(ExchangerEnum.KuCoin, kucoinMarket);
            coins = coins.FillExchangePrice(ExchangerEnum.Cryptopia, cryptopiaMarket);
            //coins = coins.FillExchangePrice(ExchangerEnum.Yobit, yobitMarket);

            _memoryCache.Set(CoinInfoKey, coins);
            return coins;
        }

        public async Task<List<ExchangeSymbol>> GetExchangeSymbols(BaseCurrencyEnum currency = BaseCurrencyEnum.BTC)
        {
            var cachedSymbols = _memoryCache.Get<List<ExchangeSymbol>>(SymbolsKey);
            if (cachedSymbols != null)
            {
                return cachedSymbols;
            }

            var binanceSymbols = (await _binanceApi.GetSymbolsAsync()).ToArray();
            var bittrexSymbols = (await _bittrexApi.GetSymbolsAsync()).ToArray();
            var hitBtcSymbols = (await _hitbtcApi.GetSymbolsAsync()).ToArray();
            var kucoinSymbols = (await _kucoinApi.GetSymbolsAsync()).ToArray();
            var cryptopiaSymbols = (await _cryptopiaApi.GetSymbolsAsync()).ToArray();
            //var yobitSymbols = (await _yobitApi.GetSymbolsAsync()).ToArray();

            var globalSymbols = binanceSymbols.Select(
                x =>
                    new ExchangeSymbol
                    {
                        GlobalSymbol = SymbolHelper.ToGlobalSymbol(x, ExchangerEnum.Binance)
                    }).ToList();

            globalSymbols = globalSymbols.FillExchangerSymbols(ExchangerEnum.Binance, binanceSymbols);
            globalSymbols = globalSymbols.FillExchangerSymbols(ExchangerEnum.Bittrex, bittrexSymbols);
            globalSymbols = globalSymbols.FillExchangerSymbols(ExchangerEnum.HitBtc, hitBtcSymbols);
            globalSymbols = globalSymbols.FillExchangerSymbols(ExchangerEnum.KuCoin, kucoinSymbols);
            globalSymbols = globalSymbols.FillExchangerSymbols(ExchangerEnum.Cryptopia, cryptopiaSymbols);
            //globalSymbols = globalSymbols.FillExchangerSymbols(ExchangerEnum.Yobit, yobitSymbols);

            _memoryCache.Set(SymbolsKey, globalSymbols);
            return globalSymbols.FilterByBaseCurency(currency);
        }
    }
}