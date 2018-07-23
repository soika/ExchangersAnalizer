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
    using Exchangers;
    using ExchangeSharp;
    using Extensions;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Options;
    using Models;
    using Settings;

    public class CoinInfoService : ICoinInfoService
    {
        private const string CoinInfoKey = "COINS";
        private const string SymbolsKey = "SYMBOLS";
        private readonly ExchangeBinanceAPI binanceApi;
        private readonly ExchangeBittrexAPI bittrexApi;
        private readonly ExchangeCryptopiaAPI cryptopiaApi;
        private readonly IOptions<ExchangerEnableSettings> enableOptions;
        private readonly MinExchangeGateAPI gateApi;
        private readonly ExchangeHitbtcAPI hitbtcApi;
        private readonly MinExchangeHoubiAPI huobiApi;
        private readonly ExchangeKucoinAPI kucoinApi;

        private readonly IMemoryCache memoryCache;
        private readonly MinExchangeOkexAPI okexApi;
        private readonly IOptions<SiteSettings> options;
        private readonly MinExchangeUpbitAPI upbitApi;

        public CoinInfoService(
            ExchangeBittrexAPI bittrexApi,
            ExchangeBinanceAPI binanceApi,
            ExchangeHitbtcAPI hitbtcApi,
            ExchangeKucoinAPI kucoinApi,
            ExchangeCryptopiaAPI cryptopiaApi,
            MinExchangeOkexAPI okexApi,
            MinExchangeUpbitAPI upbitApi,
            MinExchangeHoubiAPI huobiApi,
            MinExchangeGateAPI gateApi,
            IMemoryCache memoryCache,
            IOptions<SiteSettings> options,
            IOptions<ExchangerEnableSettings> enableOptions)
        {
            this.binanceApi = binanceApi;
            this.bittrexApi = bittrexApi;
            this.hitbtcApi = hitbtcApi;
            this.kucoinApi = kucoinApi;
            this.cryptopiaApi = cryptopiaApi;
            this.memoryCache = memoryCache;
            this.okexApi = okexApi;
            this.options = options;
            this.gateApi = gateApi;
            this.huobiApi = huobiApi;
            this.upbitApi = upbitApi;
            this.enableOptions = enableOptions;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<CoinInfo>> GetExchangerCoinInfoAsync(
            BaseCurrencyEnum currency = BaseCurrencyEnum.BTC)
        {
            var coins = memoryCache.Get<List<CoinInfo>>(CoinInfoKey);
            if (coins != null)
            {
                return coins;
            }

            var binanceMarket = enableOptions.Value.Binance ? (await binanceApi.GetTickersAsync()).ToList() : null;
            var bittrexMarket = enableOptions.Value.Bittrex ? (await bittrexApi.GetTickersAsync()).ToList() : null;
            var kucoinMarket = enableOptions.Value.KuCoin ? (await kucoinApi.GetTickersAsync()).ToList() : null;
            var cryptopiaMarket =
                enableOptions.Value.Cryptopia ? (await cryptopiaApi.GetTickersAsync()).ToList() : null;
            var okexMarket = enableOptions.Value.Okex ? (await okexApi.GetTickersAsync()).ToList() : null;
            var gateMarket = enableOptions.Value.Gate ? (await gateApi.GetTickersAsync()).ToList() : null;
            var huobiMarket = enableOptions.Value.Huobi ? (await huobiApi.GetTickersAsync()).ToList() : null;

            var symbols = await GetExchangeSymbols();

            coins = symbols.Select(
                x => new CoinInfo
                {
                    ExchangeSymbol = x,
                    Currency = BaseCurrencyEnum.BTC,
                    ExchangePrices = new List<ExchangePrice>()
                }).ToList();

            coins = coins.FillCoinPrices(ExchangerEnum.Binance, binanceMarket);
            coins = coins.FillCoinPrices(ExchangerEnum.Bittrex, bittrexMarket);
            coins = coins.FillCoinPrices(ExchangerEnum.KuCoin, kucoinMarket);
            coins = coins.FillCoinPrices(ExchangerEnum.Cryptopia, cryptopiaMarket);
            coins = coins.FillCoinPrices(ExchangerEnum.Okex, okexMarket);
            coins = coins.FillCoinPrices(ExchangerEnum.Gate, gateMarket);
            coins = coins.FillCoinPrices(ExchangerEnum.Huobi, huobiMarket);

            coins = coins.ToAnalizedExchangePrice();
            coins = coins
                .OrderByDescending(opt => opt.ExchangePrices.Max(price => price.Percent))
                .ToList();

            memoryCache.Set(CoinInfoKey, coins);
            return coins;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<CoinInfo>> ForceUpdateCoinInfoAsync(
            BaseCurrencyEnum currency = BaseCurrencyEnum.BTC)
        {
            var binanceMarket = enableOptions.Value.Binance ? (await binanceApi.GetTickersAsync()).ToList() : null;
            var bittrexMarket = enableOptions.Value.Bittrex ? (await bittrexApi.GetTickersAsync()).ToList() : null;
            var kucoinMarket = enableOptions.Value.KuCoin ? (await kucoinApi.GetTickersAsync()).ToList() : null;
            var cryptopiaMarket =
                enableOptions.Value.Cryptopia ? (await cryptopiaApi.GetTickersAsync()).ToList() : null;
            var okexMarket = enableOptions.Value.Okex ? (await okexApi.GetTickersAsync()).ToList() : null;
            var gateMarket = enableOptions.Value.Gate ? (await gateApi.GetTickersAsync()).ToList() : null;
            var huobiMarket = enableOptions.Value.Huobi ? (await huobiApi.GetTickersAsync()).ToList() : null;

            var symbols = await GetExchangeSymbols();
            var coins = symbols.Select(
                x => new CoinInfo
                {
                    ExchangeSymbol = x,
                    Currency = BaseCurrencyEnum.BTC,
                    ExchangePrices = new List<ExchangePrice>()
                }).ToList();

            coins = coins.FillCoinPrices(ExchangerEnum.Binance, binanceMarket);
            coins = coins.FillCoinPrices(ExchangerEnum.Bittrex, bittrexMarket);
            coins = coins.FillCoinPrices(ExchangerEnum.KuCoin, kucoinMarket);
            coins = coins.FillCoinPrices(ExchangerEnum.Cryptopia, cryptopiaMarket);
            coins = coins.FillCoinPrices(ExchangerEnum.Okex, okexMarket);
            coins = coins.FillCoinPrices(ExchangerEnum.Gate, gateMarket);
            coins = coins.FillCoinPrices(ExchangerEnum.Huobi, huobiMarket);

            coins = coins.ToAnalizedExchangePrice();
            coins = coins
                .OrderByDescending(opt => opt.ExchangePrices.Max(price => price.Percent))
                .ToList();

            memoryCache.Set(CoinInfoKey, coins);
            return coins;
        }

        public async Task<List<ExchangeSymbol>> GetExchangeSymbols(BaseCurrencyEnum currency = BaseCurrencyEnum.BTC)
        {
            var ignoreCoinSettings = options.Value.IgnoreCoinSettings;
            var cachedSymbols = memoryCache.Get<List<ExchangeSymbol>>(SymbolsKey);
            if (cachedSymbols != null)
            {
                return cachedSymbols;
            }

            var binanceSymbols = (await binanceApi.GetSymbolsAsync()).ToList().FilterByBaseCurency(currency);

            var bittrexSymbols = enableOptions.Value.Binance
                ? (await bittrexApi.GetSymbolsAsync()).ToList().FilterByBaseCurency(currency)
                : null;
            var kucoinSymbols = enableOptions.Value.Binance
                ? (await kucoinApi.GetSymbolsAsync()).ToList().FilterByBaseCurency(currency)
                : null;
            var cryptopiaSymbols = enableOptions.Value.Binance
                ? (await cryptopiaApi.GetSymbolsAsync()).ToList().FilterByBaseCurency(currency)
                : null;
            var okexSymbols = enableOptions.Value.Binance
                ? (await okexApi.GetSymbolsAsync()).ToList().FilterByBaseCurency(currency)
                : null;
            var gateSymbols = enableOptions.Value.Binance
                ? (await gateApi.GetSymbolsAsync()).ToList()
                : null;
            var huobiSymbols = enableOptions.Value.Binance
                ? (await huobiApi.GetSymbolsAsync()).ToList()
                : null;

            var globalSymbols = binanceSymbols.Select(
                x =>
                    new ExchangeSymbol
                    {
                        GlobalSymbol = SymbolHelper.ToGlobalSymbol(x, ExchangerEnum.Binance)
                    }).ToList();

            if (ignoreCoinSettings != null)
            {
                binanceSymbols = binanceSymbols.FilterByIgnoreCoins(ignoreCoinSettings, ExchangerEnum.Binance);
                bittrexSymbols = bittrexSymbols.FilterByIgnoreCoins(ignoreCoinSettings, ExchangerEnum.Bittrex);
                kucoinSymbols = kucoinSymbols.FilterByIgnoreCoins(ignoreCoinSettings, ExchangerEnum.KuCoin);
                cryptopiaSymbols = cryptopiaSymbols.FilterByIgnoreCoins(ignoreCoinSettings, ExchangerEnum.Cryptopia);
                okexSymbols = okexSymbols.FilterByIgnoreCoins(ignoreCoinSettings, ExchangerEnum.Okex);
                gateSymbols = gateSymbols.FilterByIgnoreCoins(ignoreCoinSettings, ExchangerEnum.Gate);
                huobiSymbols = huobiSymbols.FilterByIgnoreCoins(ignoreCoinSettings, ExchangerEnum.Huobi);
            }

            globalSymbols = globalSymbols.FillExchangerSymbols(ExchangerEnum.Binance, binanceSymbols);
            globalSymbols = globalSymbols.FillExchangerSymbols(ExchangerEnum.Bittrex, bittrexSymbols);
            globalSymbols = globalSymbols.FillExchangerSymbols(ExchangerEnum.KuCoin, kucoinSymbols);
            globalSymbols = globalSymbols.FillExchangerSymbols(ExchangerEnum.Cryptopia, cryptopiaSymbols);
            globalSymbols = globalSymbols.FillExchangerSymbols(ExchangerEnum.Okex, okexSymbols);
            globalSymbols = globalSymbols.FillExchangerSymbols(ExchangerEnum.Gate, gateSymbols);
            globalSymbols = globalSymbols.FillExchangerSymbols(ExchangerEnum.Huobi, huobiSymbols);

            memoryCache.Set(SymbolsKey, globalSymbols);
            return globalSymbols;
        }
    }
}