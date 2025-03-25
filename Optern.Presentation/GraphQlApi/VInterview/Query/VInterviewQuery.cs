
using Optern.Application.DTOs.VInterview;
using Optern.Application.Interfaces.IVInterviewService;
using Optern.Domain.Entities;

[ExtendObjectType("Query")]

	public class VInterviewQuery
	{
		[GraphQLDescription("Get V interview")]
		public async Task<Response<VInterviewDTO>> GetVInterviewQuestion([Service] IVInterviewService _vInterviewervice,int interviewId,string userId)
			=> await _vInterviewervice.GetInterviewQuestion(interviewId,userId);
			


	}
