using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Infrastructure.ExternalInterfaces
{
    public interface IAutoCompleteService
    {

        public void LoadWords(List<string> words);
        List<string> GetSuggestions(string prefix);

    }
}
