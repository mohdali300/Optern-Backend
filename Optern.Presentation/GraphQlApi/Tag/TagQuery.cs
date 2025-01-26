using Optern.Application.DTOs.Tags;
using Optern.Application.Interfaces.ITagService;
using Optern.Infrastructure.Response;

namespace Optern.Presentation.GraphQlApi.Tag
{
    [ExtendObjectType("Query")]
    public class TagQuery
    {
        [GraphQLDescription("Retrieve the top most frequent tags.")]
        public async Task<Response<IEnumerable<TagDTO>>> GetTopTags(
            [Service] ITagsService _tagsService,
            int? topN
        )
            => await _tagsService.GetTopTagsAsync(topN);
    }
}
