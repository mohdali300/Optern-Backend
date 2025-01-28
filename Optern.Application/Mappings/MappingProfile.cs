
using Optern.Application.DTOs.Tags;
using Optern.Domain.Entities;
using AutoMapper;
using Optern.Application.DTOs.Post;
using Optern.Application.DTOs.Comment;
using Optern.Application.DTOs.React;
using Optern.Application.DTOs.Room;
using Optern.Application.DTOs.FavoritePosts;

namespace Optern.Application.Mappings
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			#region Blogs

			CreateMap<Tags, TagDTO>();

			CreateMap<PostTags, TagDTO>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Tag.Id))
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Tag.Name));

			CreateMap<FavoritePosts, FavouritePostsDTO>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Post.Id))
				.ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Post.Title))
				.ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Post.Content))
				.ForMember(dest => dest.CreatorId, opt => opt.MapFrom(src => src.Post.CreatorId))
				.ForMember(dest => dest.CreatorUserName, opt => opt.MapFrom(src => src.Post.Creator.UserName))
				.ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Post.PostTags));


			CreateMap<AddToFavoriteDTO, FavoritePosts>();


		   CreateMap<Post, PostWithDetailsDTO>()
		  .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Creator.FirstName + " " + src.Creator.LastName))
		  .ForMember(dest => dest.ReactCount, opt => opt.MapFrom(src => src.Reacts.Count))
		  .ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => src.Comments.Count));

			//post
			CreateMap<Post, PostDTO>()
			.ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.Creator.ProfilePicture))
			.ForMember(dest => dest.CreatorName, opt => opt.MapFrom(src => src.Creator.FirstName + " " + src.Creator.LastName))
			.ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.PostTags.Select(pt => pt.Tag.Name).ToList()));

			CreateMap<Post, PostWithDetailsDTO>()
			.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Creator.FirstName + " " + src.Creator.LastName));

			//comment
			CreateMap<Comment, CommentDTO>()
			.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
			.ForMember(dest => dest.Reacts, opt => opt.MapFrom(src => src.CommentReacts));


			//react
			CreateMap<Reacts, ReactDTO>()
			.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName));

			#endregion


            // Map Room To RoomDTO
            CreateMap<Room, CreateRoomDTO>()
              .ForMember(dest => dest.NumberOfParticipants,
               opt => opt.MapFrom(src => src.UserRooms.Count));


		}
	}
}
