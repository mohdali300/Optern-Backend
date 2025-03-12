using Optern.Application.DTOs.VInterview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Interfaces.IVInterviewService
{
    public interface IVInterviewService
    {
        public Task<Response<VInterviewDTO>> CreateVInterviewAsync(CreateVInterviewDTO dto, int questionCount, string userId);

    }
}
