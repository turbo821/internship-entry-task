using Microsoft.AspNetCore.Mvc;
using TickiTackToe.Application.Dtos;

namespace TickiTackToe.Api.Controllers
{
    [ApiController]
    [Route("/api/games")]
    public class GamesController : ControllerBase
    {
        public GamesController()
        {
            
        }

        [HttpPost]
        public async Task<IActionResult> CreateGame()
        {
            throw new NotImplementedException();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGame(Guid id)
        {
            throw new NotImplementedException();
        }

        [HttpPost("{id}/moves")]
        public async Task<IActionResult> MakeMove(Guid id, [FromBody] MoveRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
