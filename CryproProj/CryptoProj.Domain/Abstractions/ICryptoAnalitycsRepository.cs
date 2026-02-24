using CryptoProj.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoProj.Domain.Abstractions
{
    public interface ICryptoAnalyticsRepository
    {
        Task<CryptoAnalytics> GetByCryptoId(int cryptoId);
        Task<CryptoAnalytics> Add(CryptoAnalytics analytics);
        Task Update(CryptoAnalytics analytics);
    }

}
