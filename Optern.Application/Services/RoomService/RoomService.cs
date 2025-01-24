using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Optern.Application.DTOs.Room.RoomDTO;
using Optern.Application.Interfaces.IRoomService;
using Optern.Domain.Entities;
using Optern.Domain.Enums;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.Response;
using Optern.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Services.RoomService
{
    public class RoomService(IUnitOfWork unitOfWork, OpternDbContext context) :IRoomService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly OpternDbContext _context = context;

        public async Task<Response<IEnumerable<RoomDTO>>> GetAllAsync()
        {
            try
            {
                var rooms = await _unitOfWork.Rooms.GetAllAsync();
                if(rooms == null)
                {
                    return Response<IEnumerable<RoomDTO>>.Failure("No Rooms Found", 404);
                }
                var roomsDtos = rooms.OrderByDescending(r=>r.CreatedAt)
                    .Select(r=>new RoomDTO
                {
                    Name = r.Name,
                    Description = r.Description,
                    Capacity = r.Capacity,
                    CoverPicture = r.CoverPicture,
                    CreatedAt = r.CreatedAt,
                });

                return roomsDtos.Any() ? Response<IEnumerable<RoomDTO>>.Success(roomsDtos, "", 200) :
                                         Response<IEnumerable<RoomDTO>>.Failure(new List<RoomDTO>(),"No Rooms Found", 204);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<RoomDTO>>.Failure($"There is a server error. Please try again later", 500);
            }

        }

        public async Task<Response<IEnumerable<RoomDTO>>> GetPopularRooms()
        {
            try
            {
                var rooms = await _context.Rooms
                    .GroupJoin(_context.UserRooms,
                      room => room.Id,
                      userRooms => userRooms.RoomId,
                      (room, userRooms) => new
                      {
                          Room = room,
                          NumberOfUsers = userRooms.Count()
                      }
                    )
                    .OrderByDescending(r => r.NumberOfUsers)
                     .Select(r => new RoomDTO
                     {
                         Name = r.Room.Name,
                         Description = r.Room.Description,
                         Capacity = r.Room.Capacity,
                         CoverPicture = r.Room.CoverPicture,
                         NumberOfParticipants = r.NumberOfUsers,
                         CreatedAt = r.Room.CreatedAt,
                     })
                     .ToListAsync();

                return rooms.Any() ? Response<IEnumerable<RoomDTO>>.Success(rooms, "", 200) :
                                     Response<IEnumerable<RoomDTO>>.Failure(new List<RoomDTO>(), "There Are No Created Rooms Until Now", 204);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<RoomDTO>>.Failure($"There is a server error. Please try again later.{ex.Message}", 500);
            }
        }
        public async Task<Response<IEnumerable<RoomDTO>>> GetCreatedRooms(string id)
        {
            try {
                var createdRooms = await _context.Rooms
                    .Include(r => r.UserRooms)
                    .Where(r => r.CreatorId == id)
                    .Select(r => new RoomDTO
                    {
                        Name = r.Name,
                        Description = r.Description,
                        Capacity = r.Capacity,
                        CoverPicture = r.CoverPicture,
                        NumberOfParticipants = r.UserRooms.Count(),
                        CreatedAt = r.CreatedAt,
                        RoomType= r.RoomType,   
                    })
                    .ToListAsync();

                return createdRooms.Any() ? Response<IEnumerable<RoomDTO>>.Success(createdRooms, "", 200) :
                                            Response<IEnumerable<RoomDTO>>.Failure(new List<RoomDTO>(), "There is no Created Rooms Until Now", 204);

            }
            catch(Exception ex)
            {
                return Response<IEnumerable<RoomDTO>>.Failure($"There is a server error. Please try again later.{ex.Message}", 500);

            }
        }

        public async Task<Response<IEnumerable<RoomDTO>>> JoinedRooms(string id)
        {
            try
            {
               var joinedRooms= await _context.Rooms.Join(_context.UserRooms,
                   room => room.Id,
                   userRoom=>userRoom.RoomId,
                   (room, userRoom)=> new {Room=room,UserRoom=userRoom}
                   )
                    .Where(r=>r.UserRoom.UserId==id)
                    .Select(r=>new RoomDTO
                    {
                        Name = r.Room.Name,
                        Description = r.Room.Description,
                        Capacity = r.Room.Capacity,
                        CoverPicture = r.Room.CoverPicture,
                        NumberOfParticipants = r.Room.UserRooms.Count(),
                        CreatedAt = r.Room.CreatedAt,
                        RoomType = r.Room.RoomType,
                    })
                   .ToListAsync();

                return joinedRooms.Any() ? Response<IEnumerable<RoomDTO>>.Success(joinedRooms, "", 200) :
                                           Response<IEnumerable<RoomDTO>>.Failure(new List<RoomDTO>(), "You have not joined any room yet.", 204);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<RoomDTO>>.Failure($"There is a server error. Please try again later.{ex.Message}", 500);
            }
        }
    }
}
