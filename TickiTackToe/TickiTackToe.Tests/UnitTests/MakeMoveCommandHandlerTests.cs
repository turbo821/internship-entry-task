using Moq;
using TickiTackToe.Application.Commands;
using TickiTackToe.Application.Interfaces;
using TickiTackToe.Domain.Entities;
using TickiTackToe.Domain.Enums;

namespace TickiTackToe.Tests.UnitTests
{
    public class MakeMoveCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GameNotFound_ThrowsNullReferenceException()
        {
            // Arrange
            var repoMock = new Mock<IGameRepository>();
            var luckyMock = new Mock<ILuckyService>();
            repoMock.Setup(r => r.GetById(It.IsAny<Guid>())).ReturnsAsync((Game?)null);

            var handler = new MakeMoveCommandHandler(repoMock.Object, luckyMock.Object);
            var command = new MakeMoveCommand(Guid.NewGuid(), "X", 0, 0);

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() =>
                handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_NotCurrentPlayer_ThrowsArgumentException()
        {
            // Arrange
            var game = new Game(3, 3);
            game.MakeMove(0, 0, _ => false); // X 

            var repoMock = new Mock<IGameRepository>();
            var luckyMock = new Mock<ILuckyService>();

            repoMock.Setup(r => r.GetById(It.IsAny<Guid>())).ReturnsAsync(game);

            var handler = new MakeMoveCommandHandler(repoMock.Object, luckyMock.Object);
            var command = new MakeMoveCommand(game.Id, "X", 0, 1); // X

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ValidMove_LuckyFalse()
        {
            // Arrange
            var game = new Game(3, 3);
            var repoMock = new Mock<IGameRepository>();
            var luckyMock = new Mock<ILuckyService>();

            repoMock.Setup(r => r.GetById(It.IsAny<Guid>())).ReturnsAsync(game);
            
            repoMock.Setup(r => r.Update(It.IsAny<Game>())).Returns(Task.CompletedTask);
            luckyMock.Setup(s => s.IsLucky(100)).Returns(false);

            var handler = new MakeMoveCommandHandler(repoMock.Object, luckyMock.Object);
            var command1 = new MakeMoveCommand(game.Id, "X", 0, 0); // X
            var command2 = new MakeMoveCommand(game.Id, "O", 0, 1); // O
            var command3 = new MakeMoveCommand(game.Id, "X", 0, 2); // X

            // Act
            await handler.Handle(command1, CancellationToken.None);
            await handler.Handle(command2, CancellationToken.None);
            await handler.Handle(command3, CancellationToken.None);
            var field = game.GetField();

            // Assert
            Assert.NotNull(field);
            Assert.Equal(field[0][0], CellState.X);
            Assert.Equal(field[0][1], CellState.O);
            Assert.Equal(field[0][2], CellState.X);

        }
    }
}
