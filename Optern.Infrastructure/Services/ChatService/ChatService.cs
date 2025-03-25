
namespace Optern.Infrastructure.Services.ChatService
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly OpternDbContext _context;

        public ChatService(IUnitOfWork unitOfWork, OpternDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        #region CreatePrivateChat

        public async Task<Response<ChatDTO>> CreatePrivateChatAsync(string creatorId, string receiverId)
        {
            if (string.IsNullOrEmpty(creatorId) || string.IsNullOrEmpty(receiverId))
                return Response<ChatDTO>.Failure(new ChatDTO(), "Invalid Creator or Receiver Id.", 400);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var creator = await _unitOfWork.Users.GetByIdAsync(creatorId);
                var receiver = await _unitOfWork.Users.GetByIdAsync(receiverId);
                if (creator is null || receiver is null)
                {
                    return Response<ChatDTO>.Failure(new ChatDTO(), "Creator or Receiver not found.", 404);
                }

                var existingChat = await IsChatExisted(creatorId, receiverId);
                if (existingChat)
                {
                    return Response<ChatDTO>.Failure(new ChatDTO(), "Already there is a chat between the two users.", 400);
                }

                var chat = new Chat
                {
                    Type = ChatType.Private,
                    CreatedDate = DateTime.UtcNow,
                    CreatorId = creatorId,
                };
                await _unitOfWork.Chats.AddAsync(chat);
                await _unitOfWork.SaveAsync();

                if (await AddChatParticipants(chat.Id, [creatorId, receiverId]))
                {
                    await transaction.CommitAsync();
                    return Response<ChatDTO>.Success(new ChatDTO { Id = chat.Id, Type = chat.Type }, "Chat created successfully.", 201);
                }
                return Response<ChatDTO>.Failure(new ChatDTO(), "Failed to create chat.", 400);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Response<ChatDTO>.Failure(new ChatDTO(), $"Server error: {ex.Message}", 500);
            }
        }

        #endregion CreatePrivateChat

        #region Create room chat

        public async Task<Response<ChatDTO>> CreateRoomChatAsync(string creatorId, ChatType type)
        {
            try
            {
                if (!string.IsNullOrEmpty(creatorId))
                {
                    var chat = new Chat
                    {
                        Type = type,
                        CreatedDate = DateTime.UtcNow,
                        CreatorId = creatorId
                    };

                    await _unitOfWork.Chats.AddAsync(chat);
                    await _unitOfWork.SaveAsync();

                    return Response<ChatDTO>.Success(new ChatDTO { Id = chat.Id, Type = type }, "", 201);
                }
                return Response<ChatDTO>.Failure(new ChatDTO(), "Invalid creator Id.", 400);
            }
            catch (Exception ex)
            {
                return Response<ChatDTO>.Failure(new ChatDTO(), $"Server error: {ex.Message}", 500);
            }
        }

        #endregion Create room chat

        #region Join to room chat

        public async Task<Response<bool>> JoinToRoomChatAsync(string roomId, string participantId)
        {
            try
            {
                if (!string.IsNullOrEmpty(roomId) && !string.IsNullOrEmpty(participantId))
                {
                    var room = await _unitOfWork.Rooms.GetByIdAsync(roomId);
                    var user = await _unitOfWork.Users.GetByIdAsync(participantId);
                    if (room != null && user != null && !_context.ChatParticipants.Any(p => p.UserId == participantId && p.ChatId == room.ChatId))
                    {
                        var chatParticipant = new ChatParticipants
                        {
                            JoinedAt = DateTime.UtcNow,
                            UserId = participantId,
                            ChatId = room.ChatId
                        };

                        await _unitOfWork.ChatParticipants.AddAsync(chatParticipant);
                        await _unitOfWork.SaveAsync();

                        return Response<bool>.Success(true, $"{user.FirstName} {user.LastName} joined to room chat.", 201);
                    }
                    return Response<bool>.Failure(false, "Room or User is not existed!", 404);
                }
                return Response<bool>.Failure(false, "Invalid Room Or User Id!", 400);
            }
            catch (Exception ex)
            {
                return Response<bool>.Failure(false, $"Server error: {ex.Message}", 500);
            }
        }

        #endregion Join to room chat

        #region Join All To Room Chat

        public async Task<Response<bool>> JoinAllToRoomChatAsync(string roomId, List<string> participantsIds)
        {
            try
            {
                if (!string.IsNullOrEmpty(roomId) && participantsIds.Count() > 0)
                {
                    var room = await _unitOfWork.Rooms.GetByIdAsync(roomId);
                    if (room != null)
                    {
                        var participants = new List<ChatParticipants>();
                        foreach (var id in participantsIds)
                        {
                            var user = await _unitOfWork.Users.GetByIdAsync(id);
                            if (user != null)
                            {
                                var participant = new ChatParticipants
                                {
                                    UserId = id,
                                    ChatId = room.ChatId
                                };
                                participants.Add(participant);
                            }
                        }

                        await _unitOfWork.ChatParticipants.AddRangeAsync(participants);
                        await _unitOfWork.SaveAsync();

                        return Response<bool>.Success(true, "New participants joined to room chat.", 201);
                    }
                    return Response<bool>.Failure(false, "This Room is not existed!", 404);
                }
                return Response<bool>.Failure(false, "Invalid Room or Users IDs!", 400);
            }
            catch (Exception ex)
            {
                return Response<bool>.Failure(false, $"Server error: {ex.Message}", 500);
            }
        }

        #endregion Join All To Room Chat

        #region Remove from room chat

        public async Task<Response<bool>> RemoveFromRoomChatAsync(int chatId, string userId)
        {
            try
            {
                var chatParticipant = await _unitOfWork.ChatParticipants
                    .GetByExpressionAsync(p => p.ChatId == chatId && p.UserId == userId);
                if (chatParticipant != null)
                {
                    await _unitOfWork.ChatParticipants.DeleteAsync(chatParticipant);
                    await _unitOfWork.SaveAsync();

                    return Response<bool>.Success(true, "User left room chat.", 200);
                }
                return Response<bool>.Failure(false, "User already left th chat!", 400);
            }
            catch (Exception ex)
            {
                return Response<bool>.Failure(false, $"Server error: {ex.Message}", 500);
            }
        }

        #endregion Remove from room chat

        #region Get chat participants for user or chat

        public async Task<Response<List<ChatParticipants>>> GetChatParticipantsAsync(int? chatId = null, string? userId = null)
        {
            try
            {
                if ( ( (!chatId.HasValue || chatId==0) && string.IsNullOrEmpty(userId) ) || (chatId.HasValue && !string.IsNullOrEmpty(userId)))
                    return Response<List<ChatParticipants>>.Failure(new List<ChatParticipants>(), "Specify either ChatId or UserId, only one from them.", 400);

                List<ChatParticipants> participants = new();
                if (chatId.HasValue)
                {
                    var chat = await _context.Chats.Include(c => c.Creator).FirstOrDefaultAsync(c => c.Id == chatId);
                    if (chat == null)
                        return Response<List<ChatParticipants>>.Failure(new List<ChatParticipants>(), "This chat not existed.", 404);

                    participants = await _context.ChatParticipants.Include(p => p.User)
                        .Include(p => p.Chat)
                        .Where(p => p.ChatId == chatId)
                        .ToListAsync();
                }
                else
                {
                    var user = await _unitOfWork.Users.GetByIdAsync(userId);
                    if (user == null)
                        return Response<List<ChatParticipants>>.Failure(new List<ChatParticipants>(), "This user not existed.", 404);

                    participants = await _context.ChatParticipants.Include(p => p.User)
                        .Include(p => p.Chat)
                        .Where(p => p.UserId == userId)
                        .ToListAsync();
                }

                return (participants.Any()) ? Response<List<ChatParticipants>>.Success(participants)
                    : Response<List<ChatParticipants>>.Success(participants, "There is no Chat Participants yet.", 204);
            }
            catch (Exception ex)
            {
                return Response<List<ChatParticipants>>.Failure(new List<ChatParticipants>(), $"Server error: {ex.Message}", 500);
            }
        }

        #endregion Get chat participants for user or chat

        #region Helper

        private async Task<bool> AddChatParticipants(int chatId, List<string> participantsId)
        {
            try
            {
                var participants = new List<ChatParticipants>();
                foreach (var id in participantsId)
                {
                    var participant = new ChatParticipants
                    {
                        ChatId = chatId,
                        UserId = id
                    };
                    participants.Add(participant);
                }

                await _unitOfWork.ChatParticipants.AddRangeAsync(participants);
                await _unitOfWork.SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> IsChatExisted(string creatorId, string receiverId)
        {
            var existingChat = await _context.Chats.Include(c => c.ChatParticipants)
                    .Where(c => c.Type == ChatType.Private && c.ChatParticipants.Count == 2
                    && c.ChatParticipants.Any(cp => cp.UserId == creatorId)
                    && c.ChatParticipants.Any(cp => cp.UserId == receiverId)).FirstOrDefaultAsync();
            if (existingChat != null)
                return true;
            return false;
        }

        #endregion Helper
    }
}