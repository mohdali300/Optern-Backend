using Optern.Application.DTOs.Position;
using Optern.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.Track
{
    public class TrackWithPositionsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<PositionDTO> Positions { get; set; }
    }
}
