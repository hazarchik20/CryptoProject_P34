using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoProj.Domain.Models
{
    public class CryptoAnalytics
    {
        public Guid Id { get; set; }
        public int CryptocurrencyId { get; set; }
        public Cryptocurrency Cryptocurrency { get; set; } = null!;
        public decimal ForecastPrice { get; set; }     
        public decimal Volatility { get; set; }
        public decimal SharpeRatio { get; set; }
        public decimal Alpha { get; set; }
        public decimal Beta { get; set; }
        public DateTime CalculatedAt { get; set; }
    }
}
