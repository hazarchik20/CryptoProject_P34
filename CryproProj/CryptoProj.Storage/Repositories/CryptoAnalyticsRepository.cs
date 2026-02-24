using CryptoProj.Domain.Abstractions;
using CryptoProj.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoProj.Storage.Repositories
{
    public class CryptoAnalyticsRepository : BaseRepository<CryptoAnalytics>, ICryptoAnalyticsRepository
    {

        public CryptoAnalyticsRepository(CryptoContext context) : base(context)
        {
            
        }

        public async Task<CryptoAnalytics?> GetByCryptoId(int cryptoId)
        {
            return await Context.CryptoAnalytics
                .FirstOrDefaultAsync(x => x.CryptocurrencyId == cryptoId);
        }

        public async Task Update(CryptoAnalytics analytics)
        {
            Context.CryptoAnalytics.Update(analytics);
            await Context.SaveChangesAsync();
        }
    }

}
