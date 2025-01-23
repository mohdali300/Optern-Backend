using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Optern.Application.Response;

namespace Optern.Application.Interfaces.ITagService
{
    public interface ITagsService
    {
        Task<Response<List<string>>> GetTopTagsAsync(int topN);

    }
 
}
