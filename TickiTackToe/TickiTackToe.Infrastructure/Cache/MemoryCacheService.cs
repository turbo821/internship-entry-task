using Microsoft.Extensions.Caching.Memory;
using TickiTackToe.Application.Interfaces;

namespace TickiTackToe.Infrastructure.Cache
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }
        public Task<T?> GetAsync<T>(string key)
        {
            return Task.FromResult(_cache.Get<T?>(key));
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration)
        {
            expiration = expiration ?? TimeSpan.FromHours(1);
            _cache.Set(key, value, expiration.Value);
            return Task.CompletedTask;
        }
    }
}
