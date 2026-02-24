using CryptoProj.Domain.Abstractions;
using CryptoProj.Domain.Models;
using CryptoProj.Domain.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptoProj.Domain.Services.Cryptocurrencies
{
    public class CryptoAnalyticsService 
    {
        private readonly ICryptoAnalyticsRepository _analyticsRepository;
        private readonly ICryptoHistoryRepository _historyrepository;


        public CryptoAnalyticsService(ICryptoAnalyticsRepository analyticsRepository, ICryptoHistoryRepository historyRepository )
        {
            _analyticsRepository = analyticsRepository;
            _historyrepository = historyRepository; 
        }

        public async Task<CryptoAnalytics?> GetAnalyticsAsync(int cryptoId)
        {
            return await _analyticsRepository.GetByCryptoId(cryptoId);
        }

        public async Task<CryptoAnalytics> CalculateAnalyticsAsync(int cryptoId, int limit, int offset)
        {
            HistoryRequest req = new HistoryRequest
            {
                CryptocurrencyId = cryptoId,
                Limit = limit,
                Offset = offset
            };
            var history = await _historyrepository.GetAll(req);

            if (history.Count() < 2)
                throw new Exception("Not enough data for analytics");

            var prices = history.Select(x => (x.Buy + x.Sell) / 2).ToList();
            var forecastPrice = prices.Average();

           
            var avg = prices.Average();
            var variance = prices.Average(p => Math.Pow((double)(p - avg), 2));
            var volatility = (decimal)Math.Sqrt(variance);


            var sharpe = volatility == 0 ? 0 : forecastPrice / volatility;

            var marketAverage = prices.Average();
            var beta = marketAverage == 0 ? 0 : forecastPrice / marketAverage;

            var alpha = forecastPrice - (beta * marketAverage);

            var existing = await _analyticsRepository.GetByCryptoId(cryptoId);

            if (existing == null)
            {
                var analytics = new CryptoAnalytics
                {
                    Id = Guid.NewGuid(),
                    CryptocurrencyId = cryptoId,
                    ForecastPrice = forecastPrice,
                    Volatility = volatility,
                    SharpeRatio = sharpe,
                    Alpha = alpha,
                    Beta = beta,
                    CalculatedAt = DateTime.UtcNow
                };

                await _analyticsRepository.Add(analytics);
                return analytics;
            }
            else
            {
                existing.ForecastPrice = forecastPrice;
                existing.Volatility = volatility;
                existing.SharpeRatio = sharpe;
                existing.Alpha = alpha;
                existing.Beta = beta;
                existing.CalculatedAt = DateTime.UtcNow;

                await _analyticsRepository.Update(existing);
                return existing;
            }
        }
    }

}
