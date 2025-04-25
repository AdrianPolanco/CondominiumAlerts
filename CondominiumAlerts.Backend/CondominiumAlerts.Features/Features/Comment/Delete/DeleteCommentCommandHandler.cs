using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Features.Extensions;
using CondominiumAlerts.Features.Features.Comment.Delete;
using FluentValidation;
using LightResults;
using Microsoft.Extensions.Logging;

namespace CondominiumAlerts.Features.Features.Posts.Delete
{
    public class DeleteCommentCommandHandler : ICommandHandler<DeleteCommentCommand, Result<DeleteCommentResponse>>
    {
        private readonly IRepository<Domain.Aggregates.Entities.Comment, Guid> _commentRepository;
        private readonly IRepository<Post, Guid> _postrepository;
        private readonly ILogger<DeleteCommentCommand> _logger;
        private readonly IValidator<DeleteCommentCommand> _validator;

        public DeleteCommentCommandHandler(
            IRepository<Post, Guid> postrepository,
            IRepository<Domain.Aggregates.Entities.Comment, Guid> commentrepository,
            ILogger<DeleteCommentCommand> logger,
            IValidator<DeleteCommentCommand> validator
            )
        {
            _postrepository = postrepository;
            _commentRepository = commentrepository;
            _logger = logger;
            _validator = validator;
        }
        public async Task<Result<DeleteCommentResponse>> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            FluentValidation.Results.ValidationResult validationResult = _validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return validationResult.ToLightResult<DeleteCommentResponse>();
            }

            if (!await _commentRepository.AnyAsync(l => l.Id == request.Id && l.PostId == request.PostId, cancellationToken))
            {
                _logger.LogWarning("No level with the Id: {request.Id} and belonging to the condominium with the Id:{CondominiumId} was found", request.Id, request.PostId);
                return Result<DeleteCommentResponse>.Fail("No post was found");
            }

            Domain.Aggregates.Entities.Comment? comment = await _commentRepository.DeleteAsync(request.Id, cancellationToken);

            if (comment == null)
            {
                _logger.LogWarning("Error while deleting the post with the folowing request: {@request} ", request);
                return Result<DeleteCommentResponse>.Fail("Error while deleting the post");
            }

            return Result<DeleteCommentResponse>.Ok(comment);
        }
    }
}
