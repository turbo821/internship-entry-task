using TickiTackToe.Domain.Entities;
using TickiTackToe.Domain.Enums;

namespace TickiTackToe.Tests.UnitTests
{
    public class GameTests
    {
        private readonly Func<int, bool> _isNotLucky = _ => false;
        private readonly Func<int, bool> _isLucky = _ => true;

        [Fact]
        public void Constructor_ValidParams_InitializesGameCorrectly()
        {
            // Arrange
            int gameSize = 3;
            int winCondition = 3;

            // Act
            var game = new Game(gameSize, winCondition);

            // Assert
            Assert.Equal(gameSize, game.GameSize);
            Assert.Equal(winCondition, game.WinCondition);
            Assert.Equal(GameStatus.InProgress, game.Status);
            Assert.Equal(1, game.MoveNumber);
            Assert.Equal(CellState.X, game.CurrentPlayer);
            Assert.NotNull(game.Field);

            var field = game.GetField();
            Assert.Equal(gameSize, field.Length);
            foreach (var row in field)
                Assert.All(row, cell => Assert.Equal(CellState.Empty, cell));
        }

        [Fact]
        public void MakeMove_ValidMove_UpdatesFieldAndPlayer()
        {
            // Arrange
            var game = new Game(3, 3);
            int row = 0, col = 0;

            // Act
            game.MakeMove(row, col, _isNotLucky);

            // Assert
            var field = game.GetField();
            Assert.Equal(CellState.X, field[row][col]);
            Assert.Equal(2, game.MoveNumber);
            Assert.Equal(CellState.O, game.CurrentPlayer);
            Assert.Equal(GameStatus.InProgress, game.Status);
        }

        [Fact]
        public void MakeMove_ShouldSetOpponentSymbol_WhenMoveIsLucky()
        {
            // Arrange
            var game = new Game(3, 3);
            game.MakeMove(0, 0, _isNotLucky); // X
            game.MakeMove(1, 1, _isNotLucky); // O

            // Act
            game.MakeMove(0, 1, _isLucky); // O

            // Assert
            var field = game.GetField();
            Assert.Equal(CellState.O, field[0][1]);
            Assert.Equal(CellState.O, game.CurrentPlayer);
        }

        [Fact]
        public void MakeMove_SameCellTwice_ThrowsException()
        {
            // Arrange
            var game = new Game(3, 3);
            game.MakeMove(0, 0, _isNotLucky);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => game.MakeMove(0, 0, _isNotLucky));
        }

        [Fact]
        public void MakeMove_OutOfBounds_ThrowsException()
        {
            var game = new Game(3, 3);

            Assert.Throws<ArgumentOutOfRangeException>(() => game.MakeMove(-1, 0, _isNotLucky));
            Assert.Throws<ArgumentOutOfRangeException>(() => game.MakeMove(3, 0, _isNotLucky));
            Assert.Throws<ArgumentOutOfRangeException>(() => game.MakeMove(0, 3, _isNotLucky));
        }

        [Fact]
        public void Constructor_InvalidParams_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new Game(2, 3));
            Assert.Throws<ArgumentException>(() => new Game(3, 4));
        }

        [Fact]
        public void MakeMove_WinCondition_WinsTheGame()
        {
            // Arrange
            var game = new Game(3, 3);

            game.MakeMove(0, 0, _isNotLucky); // X
            game.MakeMove(1, 0, _isNotLucky); // O
            game.MakeMove(0, 1, _isNotLucky); // X
            game.MakeMove(1, 1, _isNotLucky); // O
            game.MakeMove(0, 2, _isNotLucky); // X wins

            // Assert
            Assert.Equal(GameStatus.XWinPlayer, game.Status);
        }

        [Fact]
        public void MakeMove_LastMove_Draw()
        {
            // Arrange
            var game = new Game(3, 3);
            var moves = new[]
            {
                (0, 0), (0, 1), (0, 2),
                (1, 1), (1, 0), (1, 2),
                (2, 1), (2, 0), (2, 2)
            };

            foreach (var (r, c) in moves)
            {
                game.MakeMove(r, c, _isNotLucky);
            }

            // Assert
            Assert.Equal(GameStatus.Draw, game.Status);
            Assert.Equal(CellState.Empty, game.CurrentPlayer);
        }
    }

}
