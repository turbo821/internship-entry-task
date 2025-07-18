
using Microsoft.EntityFrameworkCore;
using TickiTackToe.Application.Interfaces;
using TickiTackToe.Domain.Entities;

namespace TickiTackToe.Infrastructure.Data
{
    public class GameRepository : IGameRepository
    {
        private readonly TickDbContext _context;

        public GameRepository(TickDbContext context)
        {
            _context = context;
        }

        public async Task Add(Game game)
        {
            await _context.Games.AddAsync(game);
            await _context.SaveChangesAsync();
        }

        public async Task<Game?> GetById(Guid id)
        {
            return await _context.Games.FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task Update(Game game)
        {
            _context.Games.Update(game);
            await _context.SaveChangesAsync();
        }
    }
}
