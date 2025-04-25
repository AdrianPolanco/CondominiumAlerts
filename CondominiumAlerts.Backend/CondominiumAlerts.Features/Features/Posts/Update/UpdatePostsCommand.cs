using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;
using Microsoft.AspNetCore.Http;

namespace CondominiumAlerts.Features.Features.Posts.Update
{
    public class UpdatePostsCommand : ICommand<Result<UpdatePostsResponse>>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile? ImageFile { get; set; }
        public Guid LevelOfPriorityId { get; set; }
        public bool RemoveImage { get; set; }
    }
}