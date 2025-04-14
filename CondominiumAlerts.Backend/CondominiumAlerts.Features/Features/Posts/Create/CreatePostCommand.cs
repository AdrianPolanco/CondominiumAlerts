using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CondominiumAlerts.Features.Features.Posts.Create
{
    public class CreatePostCommand : ICommand<Result<CreatePostResponse>>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile? ImageFile { get; set; }
        public Guid CondominiumId { get; set; }
        public string UserId { get; set; }
        public Guid LevelOfPriorityId { get; set; }
    }
}
