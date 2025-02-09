

namespace Optern.Application.DTOs.Track
{
    public class TrackWithPositionsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<PositionDTO> Positions { get; set; }
    }
}
