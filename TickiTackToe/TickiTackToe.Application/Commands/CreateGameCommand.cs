using MediatR;
using Microsoft.Extensions.Options;
using TickiTackToe.Application.Configurations;
using TickiTackToe.Application.Interfaces;
using TickiTackToe.Domain.Entities;

namespace TickiTackToe.Application.Commands
{
    public record CreateGameCommand : IRequest<Guid>;

    public class CreateGameCommandHandler : IRequestHandler<CreateGameCommand, Guid>
    {
        private readonly IGameRepository _repo;
        private readonly GameConfig _gameConfig;

        public CreateGameCommandHandler(IGameRepository repo, IOptions<GameConfig> gameConfig)
        {
            _repo = repo;
            _gameConfig = gameConfig.Value;
        }

        public async Task<Guid> Handle(CreateGameCommand request, CancellationToken cancellationToken)
        {
            var game = new Game(_gameConfig.GameSize, _gameConfig.WinCondition);
            await _repo.Add(game);

            return game.Id;
        }
    }
}
