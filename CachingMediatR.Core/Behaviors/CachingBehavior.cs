using CachingMediatR.Core.Abstractions;
using CachingMediatR.Core.Settings;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CachingMediatR.Core.Behaviors
{
    public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest,TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IDistributedCache _cache; 
        private readonly ILogger<TResponse> _logger;
        private readonly IOptions<CacheSettings> _settings;

        public CachingBehavior(IDistributedCache cache, ILogger<TResponse> logger, IOptions<CacheSettings> settings)
        {
            _cache = cache;
            _settings = settings;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (request is ICacheableMediatrQuery cacheableQuery) 
            {
                TResponse response;
                if (cacheableQuery.BypassCache) return await next();
                async Task<TResponse> GetResponseAndAddToCache()
                {
                    response = await next();
                    var slidingExpiration = cacheableQuery.SlidingExpiration == null ? TimeSpan.FromHours(_settings.Value.SlidingExpiration) : cacheableQuery.SlidingExpiration;
                    var options = new DistributedCacheEntryOptions {SlidingExpiration = slidingExpiration};   
                    var serializedData = Encoding.Default.GetBytes(JsonSerializer.Serialize(response));
                    await _cache.SetAsync(cacheableQuery.CacheKey, serializedData, options, cancellationToken);
                    return response;
                }

                var cachedResponse = await _cache.GetAsync(cacheableQuery.CacheKey, cancellationToken);
                if (cachedResponse != null)
                {
                    response = JsonSerializer.Deserialize<TResponse>(Encoding.Default.GetString(cachedResponse));
                    _logger.LogInformation($"Fetched from Cache -> '{cacheableQuery.CacheKey}'.");
                }
                else 
                {
                    response = await GetResponseAndAddToCache();
                    _logger.LogInformation($"Added to Cache -> '{cacheableQuery.CacheKey}'.");
                }
                return response;
            }
            else
            {
                return await next(); 
            }
        }

    }
}