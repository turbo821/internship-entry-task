
using System.ComponentModel.DataAnnotations;

namespace TickiTackToe.Application.Dtos
{
    public record MoveRequest(
        [RegularExpression(@"^[XO]$", ErrorMessage = "Only characters 'X' or 'O' are allowed (exactly one)")] 
        string Player, 
        int Row, 
        int Column
    );
}
