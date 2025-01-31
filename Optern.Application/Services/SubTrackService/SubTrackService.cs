using Microsoft.EntityFrameworkCore;
using Optern.Application.DTOs.SubTrack;
using Optern.Application.DTOs.Track;
using Optern.Application.Interfaces.ISubTrackService;
using Optern.Domain.Entities;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.Repositories;
using Optern.Infrastructure.Response;
using Optern.Infrastructure.UnitOfWork;
using Optern.Infrastructure.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Services.SubTrackService
{
    public class SubTrackService :ISubTrackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly OpternDbContext _context;

        public SubTrackService(IUnitOfWork unitOfWork, OpternDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        #region Add
        public async Task<Response<SubTrackDTO>> Add(string name, int trackId)
        {
            try
            {
                var subTrack = new SubTrack
                {
                    Name = name,
                    TrackId = trackId
                };

                var validate = new SubTrackValidator().Validate(subTrack);
                if (!validate.IsValid)
                {
                    var errorMessages = string.Join(", ", validate.Errors.Select(e => e.ErrorMessage));
                    return Response<SubTrackDTO>.Failure(new SubTrackDTO(), $"Invalid Data Model: {errorMessages}", 400);
                }

                await _unitOfWork.SubTracks.AddAsync(subTrack);
                return Response<SubTrackDTO>.Success(new SubTrackDTO { Id = subTrack.Id, Name = subTrack.Name }, "SubTrack added successfully.", 200);
            }
            catch (Exception ex)
            {
                return Response<SubTrackDTO>.Failure("Unexpected error occured!", 500, new List<string> { ex.Message });
            }
        }
        #endregion

        #region GetAllByTrackId
        public async Task<Response<List<SubTrackDTO>>> GetAllByTrackId(int trackId)
        {
            try
            {
                var subTracks = await _context.SubTracks
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

                return Response<List<SubTrackDTO>>.Failure("No subTracks found!", 204);
            }
            catch (Exception ex)
            {
                return Response<List<SubTrackDTO>>.Failure("Unexpected error occured!", 500, new List<string> { ex.Message });
            }
        } 
        #endregion
    }
}
