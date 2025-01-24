using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Optern.Application.DTOs.Room.RoomDTO;
using Optern.Application.DTOs.Track;
using Optern.Application.Interfaces.IRoomTrackService;
using Optern.Domain.Enums;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.Response;
using Optern.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Services.RoomTrackService
{
    public class RoomTrackService : IRoomTrackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly OpternDbContext _context;

        public RoomTrackService(IUnitOfWork unitOfWork, OpternDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public Task<Response<IEnumerable<RoomDTO>>> GetSubTrackRooms(int subTrackId)
        {
            throw new NotImplementedException();
        }
    }
}
