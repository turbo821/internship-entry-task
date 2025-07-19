
using Moq;
using TickiTackToe.Application.Interfaces;
using TickiTackToe.Application.Queries;
using TickiTackToe.Domain.Entities;
using TickiTackToe.Domain.Enums;

namespace TickiTackToe.Tests.UnitTests
{
    public class GetGameQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ReturnsGameResponse_WhenGameExists()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var mockRepo = new Mock<IGameRepository>();

            var game = new Game(3, 3); // Предположим, что такой конструктор существует
            typeof(Game).GetProperty(nameof(Game.Id))!.SetValue(game, gameId); // Установка ID вручную, если нужно

            mockRepo.Setup(r => r.GetById(gameId)).ReturnsAsync(game);

            var handler = new GetGameQueryHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new GetGameQuery(gameId), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(gameId, result!.Id);
            Assert.Equal(3, result.GameSize);
            Assert.Equal(3, result.WinCondition);
            Assert.Equal(game.CurrentPlayer.ToString(), result.CurrentPlayer);
            Assert.Equal(game.Status.ToString(), result.Status);
            Assert.Equal(game.MoveNumber, result.MoveNumber);

            var field = game.GetField();
            for (int i = 0; i < field.Length; i++)
            {
                for (int j = 0; j < field[i].Length; j++)
                {
                    var expected = field[i][j] switch
                    {
                        CellState.X => "X",
                        CellState.O => "O",
                        CellState.Empty => string.Empty,
                        _ => throw new Exception()
                    };

                    Assert.Equal(expected, result.Field[i][j]);
                }
            }
        }

        [Fact]
        public async Task Handle_ReturnsNull_WhenGameDoesNotExist()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var mockRepo = new Mock<IGameRepository>();
            mockRepo.Setup(r => r.GetById(gameId)).ReturnsAsync((Game)null!);

            var handler = new GetGameQueryHandler(mockRepo.Object);

            // Act
            var result = await handler.Handle(new GetGameQuery(gameId), CancellationToken.None);

            // Assert
            Assert.Null(result);
        }
    }
}
