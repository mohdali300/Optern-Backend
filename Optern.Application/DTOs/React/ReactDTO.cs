using Optern.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.DTOs.React
{
    public class ReactDTO
    {
        public DateTime ReactDate { get; set; }
        public string UserId { get; set; }
        public ReactType ReactType { get; set; }
        public string UserName { get; set; }
    }
}
