using Optern.Domain.Trie;
using Optern.Infrastructure.ExternalInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Infrastructure.ExternalServices.AutoCompleteService
{
    public class AutoCompleteService : IAutoCompleteService
    {
        private readonly Trie _trie = new Trie();

        public void LoadWords(List<string> words)
        {
            foreach (var word in words)
            {
                _trie.Insert(word);
            }
        }

        public List<string> GetSuggestions(string searchTerm)
        {
            return _trie.GetSuggestionsWords(searchTerm);
        }
    }
}
