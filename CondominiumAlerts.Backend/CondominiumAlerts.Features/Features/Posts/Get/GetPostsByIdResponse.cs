using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondominiumAlerts.Features.Features.Posts.Get
{
    public record GetPostByIdResponse
    {
        public Guid Id { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public string ImageUrl { get; init; }
        public Guid LevelOfPriorityId { get; init; }

    }
}
