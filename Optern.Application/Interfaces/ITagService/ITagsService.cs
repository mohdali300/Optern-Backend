using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Optern.Application.DTOs.Tags;
using Optern.Infrastructure.Response;

namespace Optern.Application.Interfaces.ITagService
{
    public interface ITagsService
    {
        Task<Response<IEnumerable<TagDTO>>> GetTopTagsAsync(int? topN);

    }
 
}
