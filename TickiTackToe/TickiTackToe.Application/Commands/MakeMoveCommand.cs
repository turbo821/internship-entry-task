
using MediatR;
using TickiTackToe.Application.Interfaces;
using TickiTackToe.Domain.Enums;

namespace TickiTackToe.Application.Commands
{
    public record MakeMoveCommand(Guid Id, string Player, int Row, int Column) : IRequest<Unit>;

    public class MakeMoveCommandHandler : IRequestHandler<MakeMoveCommand, Unit>
    {
        private readonly IGameRepository _repo;
        private readonly ILuckyService _luckyService;

        public MakeMoveCommandHandler(IGameRepository repo, ILuckyService luckyService)
        {
            _repo = repo;
            _luckyService = luckyService;
        }

        public async Task<Unit> Handle(MakeMoveCommand request, CancellationToken cancellationToken)
        {
            var game = await _repo.GetById(request.Id);
            if (game == null) throw new NullReferenceException("Game not found");

            CellState cell = Enum.Parse<CellState>(request.Player);

            if (game.CurrentPlayer != cell)
                throw new ArgumentException($"{request.Player} not current player");

            game.MakeMove(request.Row-1, request.Column-1, _luckyService.IsLucky);

            await _repo.Update(game);

            return Unit.Value;
        }
    }
}
