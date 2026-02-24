using CryptoProj.Domain.Models;
using CryptoProj.Domain.Models.Requests;
using CryptoProj.Domain.Services.Cryptocurrencies;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace CryptoProj.API.Controllers;

//[Authorize]
[ApiController]
[Route("api/v1/cryptos")]
public class CryptocurrenciesController : ControllerBase
{
    private readonly CryptocurrenciesService _cryptocurrenciesService;
    private readonly CryptoAnalyticsService _cryptoAnalyticsServices;
    private readonly IValidator<CryptocurrencyRequest> _validator;
    private readonly IMemoryCache _memoryCache;


    public CryptocurrenciesController(CryptocurrenciesService cryptocurrenciesService, IMemoryCache memoryCache, IValidator<CryptocurrencyRequest> validator, CryptoAnalyticsService cryptoAnalytics )
    {
        _cryptocurrenciesService = cryptocurrenciesService;
        _cryptoAnalyticsServices = cryptoAnalytics;
        _memoryCache = memoryCache;
        _validator = validator;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetCryptos([FromQuery] CryptocurrencyRequest request)
    {
        _validator.ValidateAndThrow(request);
        
        if (_memoryCache.TryGetValue("cryptos", out Cryptocurrency[] cryptos))
        {
            return Ok(cryptos);
        }
        
        var crypto = await _cryptocurrenciesService.GetCryptocurrencies(request);

        var options = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(10))
            .SetPriority(CacheItemPriority.High)
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
        
        _memoryCache.Set("cryptos", crypto, options);
        
        return Ok(crypto);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCrypto([FromRoute] int id)
    {
        var crypto = await _cryptocurrenciesService.GetById(id);
        return Ok(crypto);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddCrypto([FromBody] CreateCryptocurrencyRequest request)
    {
        var crypto = await _cryptocurrenciesService.Add(request);
        
        _memoryCache.Remove("cryptos");
        
        return CreatedAtAction(nameof(GetCrypto), new { id = crypto.Id }, crypto);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCrypto([FromRoute] int id, [FromBody] CreateCryptocurrencyRequest request)
    {
        var crypto = await _cryptocurrenciesService.Update(id, request);
        return Ok(crypto);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCrypto([FromRoute] int id)
    {
        await _cryptocurrenciesService.Delete(id);
        return NoContent();
    }

    [HttpGet("{id}/history")]
    public async Task<IActionResult> GetHistory([FromRoute] int id, [FromQuery] int limit = 10, [FromQuery] int offset = 0)
    {
        var request = new HistoryRequest
        {
            CryptocurrencyId = id,
            Limit = limit,
            Offset = offset
        };
        
        var history = await _cryptocurrenciesService.GetHistories(request);

        return Ok(history);
    }
    
    [HttpPost("{id}/history")]
    public async Task<IActionResult> AddHistory([FromRoute] int id, [FromBody] CreateCryptoHistoryRequest request)
    {
        await _cryptocurrenciesService.AddHistoryItem(id, request);
        return Created();
    }


   
    [HttpGet("/analytics{cryptoId}")]
    public async Task<IActionResult> Get(int cryptoId)
    {
        var analytics = await _cryptoAnalyticsServices.GetAnalyticsAsync(cryptoId);

        if (analytics == null)
            return NotFound();

        return Ok(analytics);
    }
    
    [HttpPost("/analytics{cryptoId}/calculate")]
    public async Task<IActionResult> Calculate(int cryptoId, [FromQuery] int limit=10, [FromQuery] int offset = 0)
    {
        var analytics = await _cryptoAnalyticsServices.CalculateAnalyticsAsync(cryptoId, limit, offset);
        return Ok(analytics);
    }

}