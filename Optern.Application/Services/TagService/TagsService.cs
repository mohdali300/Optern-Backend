using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Optern.Application.DTOs;
using Optern.Application.DTOs.Tags;
using Optern.Application.Interfaces.ITagService;
using Optern.Domain.Entities;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.Repositories;
using Optern.Infrastructure.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optern.Application.Services.TagService
{
    public class TagsService : GenericRepository<PostTags>, ITagsService
    {

        private readonly OpternDbContext _context;
        private readonly IMapper _mapper;

        public TagsService(OpternDbContext context, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<Response<IEnumerable<TagDTO>>> GetTopTagsAsync(int topN)
        {
            try
            {
                var topTags = await _context.PostTags
                    .GroupBy(pt => pt.TagId)
                    .OrderByDescending(group => group.Count())
                    .Take(topN)
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

                var tagDTOs = _mapper.Map<IEnumerable<TagDTO>>(topTags.Select(t => t.Tag));

                return Response<IEnumerable<TagDTO>>.Success(tagDTOs, "Tags retrieved successfully.");
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<TagDTO>>.Failure("Error occurred while fetching tags.", 500, new List<string> { ex.Message });
            }
        }





    }
}
