

using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using FluentValidation;
using LightResults;
using Microsoft.Extensions.Logging;
using CondominiumEntity = CondominiumAlerts.Domain.Aggregates.Entities.Condominium;

namespace CondominiumAlerts.Features.Features.Users.GetCondominiumsUsers
{
    public class GetCondominiumsUsersCommandHandler : ICommandHandler<GetCondominiumsUsersCommand, Result<List<GetCondominiumsUsersResponse>>>
    {
        private readonly IRepository<CondominiumEntity, Guid> _condominiumRepository;
        private readonly IRepository<User, string> _userRepository;
        private readonly IRepository<CondominiumUser, Guid> _condominiumUserRepository;
        private readonly IValidator<GetCondominiumsUsersCommand> _validator;
        private readonly ILogger<GetCondominiumsUsersCommand> _logger;

        public GetCondominiumsUsersCommandHandler(
            IRepository<CondominiumEntity, Guid> condominiumRepository,
            IRepository<User, string> userRepository,
                        IRepository<CondominiumUser, Guid> condominiumUserRepository,
            IValidator<GetCondominiumsUsersCommand> validator,
            ILogger<GetCondominiumsUsersCommand> logger
            )
        {
            _condominiumRepository = condominiumRepository;
            _userRepository = userRepository;
            _condominiumUserRepository = condominiumUserRepository;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<List<GetCondominiumsUsersResponse>>> Handle(GetCondominiumsUsersCommand request, CancellationToken cancellationToken)
        {
            FluentValidation.Results.ValidationResult validation = _validator.Validate(request);
            
            if (!validation.IsValid)
            {
                IEnumerable<string> errors = validation.Errors.Select(e => e.ErrorMessage);
                _logger.LogWarning($"Validation failed {errors}");
                return Result.Fail<List<GetCondominiumsUsersResponse>>(string.Join(", ", errors));
            }

            Guid condominiumId = Guid.Parse(request.CondominiumId);

            if(! await _condominiumRepository.AnyAsync(c => c.Id == condominiumId, cancellationToken))
            {
                _logger.LogWarning("No condominium found with the id {condominiumId}", condominiumId);
                return Result.Fail<List<GetCondominiumsUsersResponse>>("No was condominium found");
            }

            List<CondominiumUser> condominia = await _condominiumUserRepository.GetAsync(cancellationToken, cu => cu.CondominiumId == condominiumId, true, false, [u => u.User]);

            if (condominia == null)
            {
                _logger.LogWarning("Error while trying to get the user's of the condominium  with the id {condominiumId}", condominiumId);
                return Result.Fail<List<GetCondominiumsUsersResponse>>("Error while trying to get the user's of the condominium");
            }

            return Result<List<GetCondominiumsUsersResponse>>.Ok(condominia.Select(cu => new GetCondominiumsUsersResponse(
                cu.UserId,
                cu.User?.Name + " " + cu.User?.Lastname,
                cu.User?.Email,
                cu.User?.ProfilePictureUrl
                )).ToList());
        }
    }
}
