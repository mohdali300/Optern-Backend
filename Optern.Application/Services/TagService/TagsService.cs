

namespace Optern.Application.Services.TagService
{
    public class TagsService(IUnitOfWork unitOfWork, OpternDbContext context, IMapper mapper) :  ITagsService
    {

        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        #region Get Tags (All Or Top N Frequent)
        public async Task<Response<IEnumerable<TagDTO>>> GetTopTagsAsync(int? topN)
        {
            try
            {
                // Return all Tags if number not provided (for search)
                if (!topN.HasValue || topN <= 0)
                {
                    var allTags = await _unitOfWork.Tags.GetAllAsync();  
                    var tagDTOs = _mapper.Map<IEnumerable<TagDTO>>(allTags);
                    return Response<IEnumerable<TagDTO>>.Success(tagDTOs, "Tags retrieved successfully.");
                }

                var topTags = await _context.PostTags
                .GroupBy(pt => pt.TagId)
                .OrderByDescending(group => group.Count())
                .Take(topN.Value)
                .Select(group => new
                {
                    TagId = group.Key,
                    Tag = group.FirstOrDefault().Tag
                })
                .ToListAsync();

                if (!topTags.Any())
                {
                    return Response<IEnumerable<TagDTO>>.Success(new List<TagDTO>(), "No tags found.");
                }

                var tagDTOss = _mapper.Map<IEnumerable<TagDTO>>(topTags.Select(t => t.Tag));

                return Response<IEnumerable<TagDTO>>.Success(tagDTOss, "Tags retrieved successfully.");
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<TagDTO>>.Failure("Error occurred while fetching tags.", 500, new List<string> { ex.Message });
            }
        }
        #endregion

    }
}
