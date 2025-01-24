using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Optern.Application.DTOs;
using Optern.Application.Interfaces.ITagService;
using Optern.Infrastructure.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optern.Application.Services.TagService
{
    public class TagsService : ITagsService
    {
        public Task<Response<List<string>>> GetTopTagsAsync(int topN)
        {
            throw new NotImplementedException();
        }
    }
}
