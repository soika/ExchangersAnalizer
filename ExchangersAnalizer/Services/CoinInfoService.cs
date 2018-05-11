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
    using Models;

    public class CoinInfoService : ICoinInfoService
    {
        private readonly ExchangeBinanceAPI binanceApi;
        private readonly ExchangeBithumbAPI bithumbApi;
        private readonly ExchangeBittrexAPI bittrexApi;
        private readonly ExchangeHitbtcAPI hitbtcApi;
        private readonly ExchangeOkexAPI okexApi;

        public CoinInfoService(
            ExchangeBithumbAPI bithumbApi,
            ExchangeBittrexAPI bittrexApi,
            ExchangeBinanceAPI binanceApi,
            ExchangeHitbtcAPI hitbtcApi,
            ExchangeOkexAPI okexApi)
        {
            this.bithumbApi = bithumbApi;
            this.binanceApi = binanceApi;
            this.bittrexApi = bittrexApi;
            this.hitbtcApi = hitbtcApi;
            this.okexApi = okexApi;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<CoinInfo>> GetExchangerCoinInfoAsync()
        {
            var binanceMarket = (await this.binanceApi.GetTickersAsync()).ToList();
            var bittrexMarket = await this.bittrexApi.GetTickersAsync();
            //var okexMarket = await this.okexApi.GetTickersAsync();
            var hitBtcMarket = await this.hitbtcApi.GetTickersAsync();

            var symbols = await GetExchangeSymbols();
            var coins = new List<CoinInfo>();
            foreach (var symbol in symbols)
            {
                var coin = new CoinInfo
                {
                    Symbol = symbol.GlobalSymbol,
                    Currency = BaseCurrencyEnum.BTC,
                    ExchangePrices = new List<ExchangePrice>()
                };

                foreach (var b in binanceMarket.ToList())
                {
                    if (b.Value.Volume.BaseSymbol.Equals(symbol.Binance))
                    {
                        coin.ExchangePrices.Add(new ExchangePrice
                        {
                            Exchanger = ExchangerEnum.Binance,
                            LastPrice = b.Value.Last
                        });
                    }
                }

                coins.Add(coin);
            }

            return coins;
        }

        public async Task<List<ExchangeSymbol>> GetExchangeSymbols()
        {
            var symbolList = new List<ExchangeSymbol>();
            var bigestExchangerSymbols = await binanceApi.GetSymbolsAsync();
            foreach (var binanceSymbol in bigestExchangerSymbols)
            {
                symbolList.Add(
                    new ExchangeSymbol
                    {
                        GlobalSymbol = SymbolHelper.ToGlobalSymbol(binanceSymbol, ExchangerEnum.Okex),
                        Binance = binanceSymbol
                    });
            }

            //symbolList = await FillOkex(symbolList);
            symbolList = await FillBittrex(symbolList);
            symbolList = await FillHitBtc(symbolList);
            var filteredResult = symbolList
                .Where(
                    s => !string.IsNullOrEmpty(s.Bittrex)
                         || !string.IsNullOrEmpty(s.HitBtc)
                         || !string.IsNullOrEmpty(s.Okex))
                .Where(s => s.GlobalSymbol.EndsWith("BTC"))
                .ToList();
            return filteredResult;
        }

        public async Task<List<ExchangeSymbol>> FillOkex(List<ExchangeSymbol> globalSymbols)
        {
            var exchangerSymbols = await okexApi.GetSymbolsAsync();
            foreach (var symbol in exchangerSymbols)
            {
                foreach (var exchangeSymbol in globalSymbols)
                {
                    if (SymbolHelper.ToGlobalSymbol(symbol, ExchangerEnum.Okex).Equals(exchangeSymbol.GlobalSymbol))
                    {
                        exchangeSymbol.Okex = symbol;
                    }
                }
            }

            return globalSymbols;
        }

        public async Task<List<ExchangeSymbol>> FillHitBtc(List<ExchangeSymbol> globalSymbols)
        {
            var exchangerSymbols = await hitbtcApi.GetSymbolsAsync();
            foreach (var symbol in exchangerSymbols)
            {
                foreach (var exchangeSymbol in globalSymbols)
                {
                    if (SymbolHelper.ToGlobalSymbol(symbol, ExchangerEnum.HitBtc).Equals(exchangeSymbol.GlobalSymbol))
                    {
                        exchangeSymbol.HitBtc = symbol;
                    }
                }
            }

            return globalSymbols;
        }

        public async Task<List<ExchangeSymbol>> FillBittrex(List<ExchangeSymbol> globalSymbols)
        {
            var exchangerSymbols = await bittrexApi.GetSymbolsAsync();
            foreach (var symbol in exchangerSymbols)
            {
                foreach (var exchangeSymbol in globalSymbols)
                {
                    if (SymbolHelper.ToGlobalSymbol(symbol, ExchangerEnum.Bittrex).Equals(exchangeSymbol.GlobalSymbol))
                    {
                        exchangeSymbol.Bittrex = symbol;
                    }
                }
            }

            return globalSymbols;
        }
    }
}