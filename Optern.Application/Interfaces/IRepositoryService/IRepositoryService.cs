using Optern.Infrastructure.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Interfaces.IRepositoryService
{
    public interface IRepositoryService
    {
        public Task<Response<bool>> AddRepository(string roomId);
    }
}
