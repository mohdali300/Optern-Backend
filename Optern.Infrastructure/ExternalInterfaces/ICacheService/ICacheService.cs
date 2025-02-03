namespace Optern.Infrastructure.ExternalInterfaces.ICacheService
{
	public interface ICacheService
	{
		T? GetData<T>(string key);
		void SetData<T>(string key, T value, TimeSpan? expiration = null);
		public Task RemoveDataAsync(string key);
    }
}