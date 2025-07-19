

using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace TickiTackToe.Application.Dtos
{
    public class MoveRequest
    {
        [RegularExpression(@"^[XO]$", ErrorMessage = "Only 'X' or 'O' allowed")]
        [SwaggerSchema(Description = "Player making the move: 'X' or 'O'")]
        public string Player { get; set; } = null!;

        [Range(0, int.MaxValue)]
        [SwaggerSchema(Description = "Row number (starts from 0)")]
        public int Row { get; set; }

        [Range(0, int.MaxValue)]
        [SwaggerSchema(Description = "Column number (starts from 0)")]
        public int Column { get; set; }
    }
}
