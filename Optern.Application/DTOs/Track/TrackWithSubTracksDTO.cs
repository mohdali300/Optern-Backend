using Optern.Application.DTOs.SubTrack;
using Optern.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.Track
{
    public class TrackWithSubTracksDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<SubTrackDTO> SubTracks { get; set; }
    }
}
