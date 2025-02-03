using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Optern.Infrastructure.ExternalInterfaces.ICacheService;

namespace Optern.Infrastructure.ExternalServices.CacheService
{
	public class CacheService : ICacheService
	{
		private readonly IDistributedCache _cache;
		
		public CacheService(IDistributedCache cache)
		{
			_cache = cache;
		}
		public T? GetData<T>(string key)
		{
			var data = _cache?.GetString(key);
			
			if(data == null)
			{
				return default(T);
			} 
			
			return JsonSerializer.Deserialize<T>(data); 
		}

		public void SetData<T>(string key, T value,TimeSpan? expiration=null)
		{
			var options = new DistributedCacheEntryOptions();

			if (expiration.HasValue)
			{
				options.AbsoluteExpirationRelativeToNow = expiration.Value;
			}
			else
			{
				options.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
			}
			
			_cache?.SetString(key, JsonSerializer.Serialize(value),options);
		}

        public async Task RemoveDataAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}