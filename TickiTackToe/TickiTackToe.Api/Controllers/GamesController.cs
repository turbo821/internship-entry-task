using MediatR;
using Microsoft.AspNetCore.Mvc;
using TickiTackToe.Application.Commands;
using TickiTackToe.Application.Dtos;
using TickiTackToe.Application.Queries;

namespace TickiTackToe.Api.Controllers
{
    [ApiController]
    [Route("/api/games")]
    public class GamesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GamesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGame()
        {
            var gameId = await _mediator.Send(new CreateGameCommand());
            var response = await _mediator.Send(new GetGameQuery(gameId));
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGame(Guid id)
        {
            var response = await _mediator.Send(new GetGameQuery(id));

            if(response is null) return NotFound();
            return Ok(response);
        }

        [HttpPost("{id}/moves")]
        public async Task<IActionResult> MakeMove(Guid id, [FromBody] MoveRequest request)
        {
            try
            {
                await _mediator.Send(new MakeMoveCommand(id, request.Player, request.Row, request.Column));
            }
            catch (NullReferenceException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            var response = await _mediator.Send(new GetGameQuery(id));

            if(response is null) return NotFound();
            return Ok(response);
        }
    }
}
