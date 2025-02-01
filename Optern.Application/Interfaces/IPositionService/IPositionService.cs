using Optern.Application.DTOs.Position;
using Optern.Application.DTOs.Track;
using Optern.Infrastructure.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Interfaces.IPositionService
{
    public interface IPositionService
    {
        public Task<Response<List<PositionDTO>>> GetAllAsync();
        public Task<Response<PositionDTO>> AddAsync(string name);
    }
}
