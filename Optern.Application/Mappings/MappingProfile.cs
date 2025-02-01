
using Optern.Application.DTOs.Tags;
using Optern.Domain.Entities;
using AutoMapper;
using Optern.Application.DTOs.Post;
using Optern.Application.DTOs.Comment;
using Optern.Application.DTOs.React;
using Optern.Application.DTOs.Room;
using Optern.Application.DTOs.FavoritePosts;
using Optern.Application.DTOs.WorkSpace;
using Optern.Application.DTOs.Task;
using Task = Optern.Domain.Entities.Task;
using Optern.Application.DTOs.Sprint;
using Optern.Application.DTOs.RoomUser;

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

            #region Room

            // Map Room To RoomDTO
            CreateMap<Room, CreateRoomDTO>()
              .ForMember(dest => dest.NumberOfParticipants,
               opt => opt.MapFrom(src => src.UserRooms.Count));

			//CreateMap<Room, ResponseRoomDTO>();
			CreateMap<Room, ResponseRoomDTO>();


            CreateMap<WorkSpace, WorkSpaceDTO>();

			CreateMap<Sprint, SprintResponseDTO>();

            CreateMap<AddTaskDTO, Task>()
           .ForMember(dest => dest.AssignedTasks, opt => opt.Ignore());

            CreateMap<Task, TaskResponseDTO>()
                .ForMember(dest => dest.AssignedUsers, opt => opt.MapFrom(src => src.AssignedTasks.Select(a => new AssignedUserDTO
                {
                    UserId = a.UserId,
                    FullName = $"{a.User.FirstName} {a.User.LastName}",
                    ProfilePicture = a.User.ProfilePicture
                }).ToList()));


            #endregion


            //RoomUser
            CreateMap<UserRoom, RoomUserDTO>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
            .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.User.ProfilePicture))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.IsAdmin ? "Leader" : "Collaborator"));


        }
    }
}
