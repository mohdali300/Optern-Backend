using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Optern.Application.DTOs.Room;
using Optern.Application.DTOs.Position;
using Optern.Application.DTOs.Track;
using Optern.Application.Interfaces.ITrackService;
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

        #region GetAll
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

                return Response<List<TrackDTO>>.Success(new List<TrackDTO>(),"No tracks found!", 204);
            }
            catch (Exception ex)
            {
                return Response<List<TrackDTO>>.Failure("Unexpected error occured!", 500, new List<string> { ex.Message });
            }
        }
        #endregion

        #region Add
        public async Task<Response<TrackDTO>> Add(string name)
        {
            try
            {
                var track = new Track
                {
                    Name = name,
                };

                var validate = new TrackValidator().Validate(track);
                if (!validate.IsValid)
                {
                    var errorMessages = string.Join(", ", validate.Errors.Select(e => e.ErrorMessage));
                    return Response<TrackDTO>.Failure(new TrackDTO(), $"Invalid Data Model: {errorMessages}", 400);
                }

                await _unitOfWork.Tracks.AddAsync(track);
                return Response<TrackDTO>.Success(new TrackDTO { Id = track.Id, Name = track.Name }, "Track added successfully.", 200);
            }
            catch (Exception ex)
            {
                return Response<TrackDTO>.Failure("Unexpected error occured!", 500, new List<string> { ex.Message });
            }
        }
        #endregion

    }
}
