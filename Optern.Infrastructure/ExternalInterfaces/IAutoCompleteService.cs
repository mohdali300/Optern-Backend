namespace Optern.Infrastructure.ExternalInterfaces
{
    public interface IAutoCompleteService
    {

        public void LoadWords(List<string> words);
        List<string> GetSuggestions(string prefix);

    }
}
