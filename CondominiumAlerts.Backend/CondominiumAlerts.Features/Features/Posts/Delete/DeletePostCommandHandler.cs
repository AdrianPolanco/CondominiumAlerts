using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using CondominiumAlerts.Features.Extensions;
using FluentValidation;
using LightResults;
using Microsoft.Extensions.Logging;

namespace CondominiumAlerts.Features.Features.Posts.Delete
{
    public class DeletePostCommandHandler : ICommandHandler<DeletePostCommand, Result<DeletePostResponse>>
    {
        private readonly IRepository<Post, Guid> _postrepository;
        private readonly IRepository<Condominium, Guid> _condominiumRepository;
        private readonly ILogger<DeletePostCommand> _logger;
        private readonly IValidator<DeletePostCommand> _validator;

        public DeletePostCommandHandler(
            IRepository<Post, Guid> postrepository,
            IRepository<Condominium, Guid> condominiumRepository,
            ILogger<DeletePostCommand> logger,
            IValidator<DeletePostCommand> validator
            )
        {
            _postrepository = postrepository;
            _condominiumRepository = condominiumRepository;
            _logger = logger;
            _validator = validator;
        }
        public async Task<Result<DeletePostResponse>> Handle(DeletePostCommand request, CancellationToken cancellationToken)
        {
            FluentValidation.Results.ValidationResult validationResult = _validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return validationResult.ToLightResult<DeletePostResponse>();
            }

            if (!await _postrepository.AnyAsync(l => l.Id == request.Id && l.CondominiumId == request.CondominiumId, cancellationToken))
            {
                _logger.LogWarning("No level with the Id: {request.Id} and belonging to the condominium with the Id:{CondominiumId} was found", request.Id, request.CondominiumId);
                return Result<DeletePostResponse>.Fail("No post was found");
            }

            Post? post = await _postrepository.DeleteAsync(request.Id, cancellationToken);

            if (post == null)
            {
                _logger.LogWarning("Error while deleting the post with the folowing request: {@request} ", request);
                return Result<DeletePostResponse>.Fail("Error while deleting the post");
            }

            return Result<DeletePostResponse>.Ok(post);
        }
    }
}
