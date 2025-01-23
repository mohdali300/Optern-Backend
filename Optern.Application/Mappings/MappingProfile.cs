
using Optern.Application.DTOs.Tags;
using Optern.Domain.Entities;
using AutoMapper;

namespace Optern.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {


            // Mapping Tag entity to TagDTO
            CreateMap<Tags, TagDTO>();

            // Mapping PostTags entity to a DTO 
            CreateMap<PostTags, TagDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Tag.Id))
                 .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Tag.Name));
        }
    }
}
