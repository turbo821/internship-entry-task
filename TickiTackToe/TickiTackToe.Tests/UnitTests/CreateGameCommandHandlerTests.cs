using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TickiTackToe.Tests.UnitTests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Timers;
    using Microsoft.Extensions.Options;
    using Moq;
    using TickiTackToe.Application.Commands;
    using TickiTackToe.Application.Configurations;
    using TickiTackToe.Application.Interfaces;
    using TickiTackToe.Domain.Entities;
    using Xunit;

    public class CreateGameCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldCreateGameAndReturnId()
        {
            // Arrange
            var fakeRepo = new Mock<IGameRepository>();

            fakeRepo
                .Setup(r => r.Add(It.IsAny<Game>()))
                .Returns(Task.CompletedTask);

            var config = Options.Create(new GameConfig { GameSize = 3, WinCondition = 3 });
            var handler = new CreateGameCommandHandler(fakeRepo.Object, config);

            var command = new CreateGameCommand();

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotEqual(Guid.Empty, result);
        }
    }

}
