
using TickiTackToe.Domain.Enums;

namespace TickiTackToe.Application.Dtos
{
    public record MoveRequest(CellState Player, int Row, int Column);
}
