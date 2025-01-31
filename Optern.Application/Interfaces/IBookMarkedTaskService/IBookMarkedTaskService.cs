using Optern.Application.DTOs.BookMarkedTask;
using Optern.Infrastructure.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Interfaces.IBookMarkedTaskService
{
    public interface IBookMarkedTaskService
    {
        public Task<Response<List<BookMarkedTaskDTO>>> GetAll(string userId);
        public Task<Response<string>> Add(int roomUserId, int taskId);
        public Task<Response<string>> Delete(int bookMarkId);
    }
}
