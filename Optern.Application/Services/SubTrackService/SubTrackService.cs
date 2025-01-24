using Microsoft.EntityFrameworkCore;
using Optern.Application.DTOs.SubTrack;
using Optern.Application.DTOs.Track;
using Optern.Application.Interfaces.ISubTrackService;
using Optern.Domain.Entities;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.Repositories;
using Optern.Infrastructure.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Services.SubTrackService
{
    public class SubTrackService : GenericRepository<SubTrack>, ISubTrackService
    {
        public SubTrackService(OpternDbContext context) : base(context)
        {
        }

        public async Task<Response<SubTrackDTO>> Add(string name, int trackId)
        {
            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    var subTrack = new SubTrack
                    {
                        Name = name,
                        TrackId = trackId
                    };

                    await AddAsync(subTrack);
                    if (subTrack != null)
                    {
                        var dto = new SubTrackDTO { Id = subTrack.Id, Name = subTrack.Name };
                        return Response<SubTrackDTO>.Success(dto, "SubTrack added successfully.");
                    }
                    return Response<SubTrackDTO>.Failure("Failed to add the track, please try again later.", 400);
                }

                return Response<SubTrackDTO>.Failure("Please enter data to be added.", 400);
            }
            catch (Exception ex)
            {
                return Response<SubTrackDTO>.Failure("Unexpected error occured!", 500, new List<string> { ex.Message });
            }
        }

        public async Task<Response<List<SubTrackDTO>>> GetAllByTrackId(int trackId)
        {
            try
            {
                var subTracks = await _dbContext.SubTracks
                    .Include(s => s.Track)
                    .Where(s => s.TrackId == trackId).ToListAsync();

                if (subTracks.Any())
                {
                    var subTrackDtos = subTracks.Select(s => new SubTrackDTO
                    {
                        Id = s.Id,
                        Name = s.Name,
                    }).ToList();

                    return Response<List<SubTrackDTO>>.Success(subTrackDtos);
                }

                return Response<List<SubTrackDTO>>.Failure("No subTracks found!",404);
            }
            catch (Exception ex)
            {
                return Response<List<SubTrackDTO>>.Failure("Unexpected error occured!", 500, new List<string> { ex.Message });
            }
        }
    }
}
