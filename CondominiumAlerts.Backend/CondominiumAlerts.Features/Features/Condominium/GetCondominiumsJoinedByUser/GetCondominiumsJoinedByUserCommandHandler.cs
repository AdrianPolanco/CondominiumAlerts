using CondominiumAlerts.CrossCutting.CQRS.Interfaces.Handlers;
using CondominiumAlerts.Domain.Aggregates.Entities;
using CondominiumAlerts.Domain.Repositories;
using FluentValidation;
using LightResults;
using Microsoft.Extensions.Logging;

namespace CondominiumAlerts.Features.Features.Condominium.GetCondominiumsJoinedByUser
{
    public class GetCondominiumsJoinedByUserCommandHandler : ICommandHandler<GetCondominiumsJoinedByUserCommand, Result<List<GetCondominiumsJoinedByUserResponse>>>
    {
        private readonly IRepository<User, string> _userRepository;
        private readonly IRepository<CondominiumUser, Guid> _condominiumUserRepository;
        private readonly IValidator<GetCondominiumsJoinedByUserCommand> _validator;
        private readonly ILogger<GetCondominiumsJoinedByUserCommand> _logger;

        public GetCondominiumsJoinedByUserCommandHandler(
            IRepository<User,string> userRepository, 
            IRepository<CondominiumUser,Guid> condominiumUserRepository,
            IValidator<GetCondominiumsJoinedByUserCommand> validator,
            ILogger<GetCondominiumsJoinedByUserCommand> logger)
        {
            _userRepository = userRepository;
            _condominiumUserRepository = condominiumUserRepository;
            _validator = validator;
            _logger = logger;
        }
        public async Task<Result<List<GetCondominiumsJoinedByUserResponse>>> Handle(GetCondominiumsJoinedByUserCommand request, CancellationToken cancellationToken)
        {
            FluentValidation.Results.ValidationResult validResult = _validator.Validate(request);

            if (!validResult.IsValid)
            {
                IEnumerable<string> erros = validResult.Errors.Select(e => e.ErrorMessage);
                _logger.LogWarning("validation failed: \n {errors}", string.Join("\n ", erros));
                return Result.Fail<List<GetCondominiumsJoinedByUserResponse>>(String.Join(", ", erros));
            }

            if(! await _userRepository.AnyAsync(u => u.Id == request.UserId, cancellationToken))
            {
                _logger.LogWarning("No user found with the user id: {request.UserId}", request.UserId);
                return Result.Fail<List<GetCondominiumsJoinedByUserResponse>>("No user was found");
            }

            List<CondominiumUser> condominiums = await _condominiumUserRepository.GetAsync(cancellationToken, u => u.UserId == request.UserId, true, false, [u => u.Condominium]);

            if (condominiums is null)
            {
                _logger.LogWarning("Error getting the user condominium's, for the user with the user id: {request.UserId}", request.UserId);
                return Result.Fail<List<GetCondominiumsJoinedByUserResponse>>("Error getting the user condominium's");
            }

            return Result<List<GetCondominiumsJoinedByUserResponse>>.Ok(condominiums.Select(cu => new GetCondominiumsJoinedByUserResponse(
                cu.CondominiumId, 
                cu.Condominium?.Name, 
                cu.Condominium?.Address, 
                cu.Condominium?.ImageUrl)).ToList());

        }
    }
}
