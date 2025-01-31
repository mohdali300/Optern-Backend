using Microsoft.AspNetCore.Identity;
using Optern.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Domain.Entities
{
    public class ApplicationUser:IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string JobTitle { get; set; }
		public string ProfilePicture { get; set; }
        public string Gender { get; set; }
        public string Country { get; set; }
		public DateTime CreatedAt { get; set; }
        public DateTime? LastLogIn { get; set; }

		public UserRole Role { get; set; }
        public virtual ICollection<RefreshToken>? RefreshTokens { get; set; } = new List<RefreshToken>();


        // Foreign Key

        #region Navigation Property


        //user create many Rooms
        public virtual ICollection<Room> Rooms { get; set; } 
        public virtual ICollection<UserRoom> UserRooms { get; set; }
        public virtual ICollection<ChatParticipants> JoinedChats  { get; set; }
        public virtual ICollection<Chat> CreatedChats  { get; set; }
        public virtual ICollection<Message> Messages   { get; set; }
        public virtual ICollection<VInterview> JoinedVirtualInterviews { get; set; }
        public virtual ICollection<Post> CreatedPosts { get; set; }
        public virtual ICollection<Reacts> Reacts { get; set; } 
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<FavoritePosts> FavoritePosts { get; set; }
        public virtual ICollection<UserTasks> AssignedTasks { get; set; }
        public virtual ICollection<PTPUsers> PeerToPeerInterviewUsers { get; set; }
        public virtual ICollection<UserSkills> UserSkills { get; set; }
        public virtual ICollection<CV> CVs { get; set; }
        public virtual ICollection<UserNotification> UserNotification { get; set; }
        public virtual ICollection<CommentReacts> CommentReacts { get; set; }
        public virtual ICollection<Experience> Experiences { get; set; }
        public virtual ICollection<Education> Educations { get; set; }

        #endregion


    }
}
