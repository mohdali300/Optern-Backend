using Optern.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Interfaces.IUserService
{
    public interface IUserService
    {
        public Task<ApplicationUser> GetCurrentUserAsync();
    }
}
