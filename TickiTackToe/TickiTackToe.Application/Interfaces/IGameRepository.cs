
using TickiTackToe.Domain.Entities;

namespace TickiTackToe.Application.Interfaces
{
    public interface IGameRepository
    {
        Task Add(Game game);
        Task<Game?> GetById(Guid id);
        Task Update(Game game);
    }
}
