

namespace Optern.Application.Interfaces.ITagService
{
    public interface ITagsService
    {
        Task<Response<IEnumerable<TagDTO>>> GetTopTagsAsync(int? topN);

    }
 
}
