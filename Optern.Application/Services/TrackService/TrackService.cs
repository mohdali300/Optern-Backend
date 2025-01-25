using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Optern.Application.DTOs.SubTrack;
using Optern.Application.DTOs.Track;
using Optern.Application.Interfaces.ITrackService;
using Optern.Domain.Entities;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.Repositories;
using Optern.Infrastructure.Response;
using Optern.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Services.TrackService
{
    public class TrackService :ITrackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly OpternDbContext _context;

        public TrackService(IUnitOfWork unitOfWork, OpternDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<Response<List<TrackDTO>>> GetAll()
        {
            try
            {
                var tracks = await _unitOfWork.Tracks.GetAllAsync();
                if (tracks != null && tracks.Any())
                {
                    var trackDtos = tracks.Select(t => new TrackDTO
                    {
                        Id = t.Id,
                        Name = t.Name,
                    }).ToList();

                    return Response<List<TrackDTO>>.Success(trackDtos);
                }

                return Response<List<TrackDTO>>.Failure("No tracks found!", 204);
            }
            catch(Exception ex)
            {
                return Response<List<TrackDTO>>.Failure("Unexpected error occured!", 500, new List<string> { ex.Message });
            }
        }

        public async Task<Response<TrackDTO>> Add(string name)
        {
            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    var track = new Track
                    {
                        Name = name,
                    };

                    await _unitOfWork.Tracks.AddAsync(track);
                    if (track != null)
                    {
                        var dto=new TrackDTO { Id = track.Id, Name=track.Name };
                        return Response<TrackDTO>.Success(dto, "Track added successfully.");
                    }
                    return Response<TrackDTO>.Failure("Failed to add the track, please try again later.", 400);
                }

                return Response<TrackDTO>.Failure("Please enter data to be added.", 400);
            }
            catch (Exception ex)
            {
                return Response<TrackDTO>.Failure("Unexpected error occured!", 500, new List<string> { ex.Message });
            }
        }

        public async Task<Response<List<TrackWithSubTracksDTO>>> GetAllWithSubTracks()
        {
            try
            {
                var tracks = await _context.Tracks.Include(t => t.SubTracks)
                    .ToListAsync();

                if (tracks.Any())
                {
                    var trackDtos = tracks.Select(t => new TrackWithSubTracksDTO
                    {
                        Id = t.Id,
                        Name = t.Name,
                        SubTracks = t.SubTracks.Select(s => new SubTrackDTO
                        {
                            Id = s.Id,
                            Name = s.Name,
                        }).ToList()
                    }).ToList();

                    return Response<List<TrackWithSubTracksDTO>>.Success(trackDtos);
                }
                return Response<List<TrackWithSubTracksDTO>>.Failure("No Tracks Found!", 204);
            }
            catch (Exception ex)
            {
                return Response<List<TrackWithSubTracksDTO>>.Failure("Unexpected error occured!", 500, new List<string> { ex.Message });
            }
        }
    }
}
