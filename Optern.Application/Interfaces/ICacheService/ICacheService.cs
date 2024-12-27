namespace Optern.Application.Interfaces.ICacheService
{
	public interface ICacheService
	{
		T? GetData<T>(string key);
		void SetData<T>(string key, T value);
	}
}