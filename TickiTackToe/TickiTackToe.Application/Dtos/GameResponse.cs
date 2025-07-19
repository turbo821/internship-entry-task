
namespace TickiTackToe.Application.Dtos
{
    public class GameResponse
    {
        public Guid Id { get; set; }
        public int GameSize { get; set; }
        public int WinCondition { get; set; }
        public string Status { get; set; } = null!;
        public int MoveNumber { get; set; }
        public string? CurrentPlayer { get; set; }
        public string[][]? Field { get; set; }
    }
}
