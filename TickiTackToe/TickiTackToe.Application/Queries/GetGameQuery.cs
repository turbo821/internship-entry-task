using MediatR;
using TickiTackToe.Application.Dtos;
using TickiTackToe.Application.Interfaces;
using TickiTackToe.Domain.Entities;
using TickiTackToe.Domain.Enums;

namespace TickiTackToe.Application.Queries
{
    public record GetGameQuery(Guid Id) : IRequest<GameResponse?>;

    public class GetGameQueryHandler : IRequestHandler<GetGameQuery, GameResponse?>
    {
        private readonly IGameRepository _repo;

        public GetGameQueryHandler(IGameRepository repo)
        {
            _repo = repo;
        }

        public async Task<GameResponse?> Handle(GetGameQuery request, CancellationToken cancellationToken)
        {
            var game = await _repo.GetById(request.Id);

            if (game is null)
                return null;

            GameResponse response = new GameResponse 
            { 
                Id = game.Id, 
                GameSize = game.GameSize,
                WinCondition = game.WinCondition,
                CurrentPlayer = game.CurrentPlayer.ToString(), 
                MoveNumber = game.MoveNumber, 
                Status = game.Status.ToString(),
                Field = GetArrField(game)
            };

            return response;
        }

        private string[][] GetArrField(Game game)
        {
            var field = game.GetField();
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            int rows = field.Count();
            int cols = field[0].Count();
            string[][] result = new string[rows][];

            for (int i = 0; i < rows; i++)
            {
                result[i] = new string[cols];
                for (int j = 0; j < cols; j++)
                {
                    result[i][j] = field[i][j] switch
                    {
                        CellState.X => "X",
                        CellState.O => "O",
                        CellState.Empty => string.Empty,
                        _ => throw new InvalidOperationException("Invalid value CellState")
                    };
                }
            }

            return result;
        }
    }
}
