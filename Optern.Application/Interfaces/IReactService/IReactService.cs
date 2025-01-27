using Optern.Application.DTOs.React;
using Optern.Domain.Enums;
using Optern.Infrastructure.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Interfaces.IReactService
{
    public interface IReactService
    {
        public Task<Response<ReactDTO>> ManageReactAsync(int postId, string userId, ReactType reactType);
    }
}
