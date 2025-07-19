using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TickiTackToe.Domain.Entities;
using TickiTackToe.Infrastructure.Data;

namespace TickiTackToe.Tests.UnitTests
{
    public class GameRepositoryTests
    {
        private static TickDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<TickDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Уникальное имя БД на каждый тест
                .Options;

            return new TickDbContext(options);
        }

        [Fact]
        public async Task Add_AddsGameToDatabase()
        {
            // Arrange
            var context = CreateContext();
            var repository = new GameRepository(context);

            var game = new Game(3, 3);

            // Act
            await repository.Add(game);

            // Assert
            var found = await context.Games.FindAsync(game.Id);
            Assert.NotNull(found);
            Assert.Equal(3, found.GameSize);
        }

        [Fact]
        public async Task GetById_ReturnsGame_WhenExists()
        {
            // Arrange
            var context = CreateContext();
            var game = new Game(3, 3);
            context.Games.Add(game);
            await context.SaveChangesAsync();

            var repository = new GameRepository(context);

            // Act
            var result = await repository.GetById(game.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(game.Id, result.Id);
        }

        [Fact]
        public async Task GetById_ReturnsNull_WhenNotExists()
        {
            // Arrange
            var context = CreateContext();
            var repository = new GameRepository(context);

            // Act
            var result = await repository.GetById(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesGameInDatabase()
        {
            // Arrange
            var context = CreateContext();
            var game = new Game(3, 3);
            context.Games.Add(game);
            await context.SaveChangesAsync();

            var repository = new GameRepository(context);

            // Act
            game.MoveNumber = 9;
            await repository.Update(game);

            // Assert
            var updated = await context.Games.FindAsync(game.Id);
            Assert.NotNull(updated);
            Assert.Equal(9, updated.MoveNumber);
        }
    }
}
