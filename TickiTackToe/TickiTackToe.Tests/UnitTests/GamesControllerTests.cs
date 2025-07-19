
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TickiTackToe.Api.Controllers;
using TickiTackToe.Application.Commands;
using TickiTackToe.Application.Dtos;
using TickiTackToe.Application.Queries;

namespace TickiTackToe.Tests.UnitTests
{
    public class GamesControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly GamesController _controller;

        public GamesControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new GamesController(_mediatorMock.Object);
        }

        [Fact]
        public async Task CreateGame_ReturnsOk_WithGameResponse()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var gameResponse = new GameResponse { Id = gameId };

            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateGameCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(gameId);

            _mediatorMock.Setup(m => m.Send(It.Is<GetGameQuery>(q => q.Id == gameId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(gameResponse);

            // Act
            var result = await _controller.CreateGame();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<GameResponse>(okResult.Value);
            Assert.Equal(gameId, response.Id);
        }

        [Fact]
        public async Task GetGame_ReturnsOk_WhenGameExists()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var gameResponse = new GameResponse { Id = gameId };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(gameResponse);

            // Act
            var result = await _controller.GetGame(gameId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<GameResponse>(okResult.Value);
            Assert.Equal(gameId, response.Id);
        }

        [Fact]
        public async Task GetGame_ReturnsNotFound_WhenGameDoesNotExist()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GameResponse?)null);

            // Act
            var result = await _controller.GetGame(gameId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task MakeMove_ReturnsOk_WhenMoveIsValid()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var moveRequest = new MoveRequest { Player = "X", Row = 0, Column = 0 };
            var gameResponse = new GameResponse { Id = gameId };

            _mediatorMock.Setup(m => m.Send(It.IsAny<MakeMoveCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(gameResponse);

            // Act
            var result = await _controller.MakeMove(gameId, moveRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<GameResponse>(okResult.Value);
            Assert.Equal(gameId, response.Id);
        }

        [Theory]
        [InlineData(typeof(NullReferenceException))]
        public async Task MakeMove_ReturnsNotFound_OnNullReferenceException(Type exceptionType)
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var moveRequest = new MoveRequest { Player = "X", Row = 0, Column = 0 };

            _mediatorMock.Setup(m => m.Send(It.IsAny<MakeMoveCommand>(), It.IsAny<CancellationToken>()))
                .Throws((Exception)Activator.CreateInstance(exceptionType)!);

            // Act
            var result = await _controller.MakeMove(gameId, moveRequest);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);

        }

        [Theory]
        [InlineData(typeof(ArgumentOutOfRangeException))]
        [InlineData(typeof(ArgumentException))]
        [InlineData(typeof(InvalidOperationException))]
        public async Task MakeMove_ReturnsBadRequest_OnExpectedExceptions(Type exceptionType)
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var moveRequest = new MoveRequest { Player = "O", Row = 1, Column = 1 };

            _mediatorMock.Setup(m => m.Send(It.IsAny<MakeMoveCommand>(), It.IsAny<CancellationToken>()))
                .Throws((Exception)Activator.CreateInstance(exceptionType)!);

            // Act
            var result = await _controller.MakeMove(gameId, moveRequest);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task MakeMove_ReturnsNotFound_WhenGameNotFoundAfterMove()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var moveRequest = new MoveRequest { Player = "X", Row = 0, Column = 0 };

            _mediatorMock.Setup(m => m.Send(It.IsAny<MakeMoveCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetGameQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GameResponse?)null);

            // Act
            var result = await _controller.MakeMove(gameId, moveRequest);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
