using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Infrastructure.MiddleWares
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseGoogleAuthMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GoogleAuthMiddleware>();
        }
    }
}
