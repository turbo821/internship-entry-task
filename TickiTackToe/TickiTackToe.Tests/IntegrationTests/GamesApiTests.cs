
using System.Net.Http.Json;
using System.Net;
using TickiTackToe.Application.Dtos;

namespace TickiTackToe.Tests.IntegrationTests
{
    public class GamesApiTests : IClassFixture<GameApiWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public GamesApiTests(GameApiWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateGame_ReturnsOk_WithGameData()
        {
            var response = await _client.PostAsync("/api/games", null);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var game = await response.Content.ReadFromJsonAsync<GameResponse>();
            Assert.NotNull(game);
            Assert.NotEqual(Guid.Empty, game.Id);
            Assert.Equal("InProgress", game.Status);
            Assert.InRange(game.GameSize, 3, int.MaxValue);
        }

        [Fact]
        public async Task GetGame_ReturnsGame_WhenExists()
        {
            var createResponse = await _client.PostAsync("/api/games", null);
            createResponse.EnsureSuccessStatusCode();

            var createdGame = await createResponse.Content.ReadFromJsonAsync<GameResponse>();
            var getResponse = await _client.GetAsync($"/api/games/{createdGame!.Id}");

            getResponse.EnsureSuccessStatusCode();
            var game = await getResponse.Content.ReadFromJsonAsync<GameResponse>();
            Assert.Equal(createdGame.Id, game!.Id);
        }

        [Fact]
        public async Task GetGame_ReturnsNotFound_WhetNotExists()
        {
            var response = await _client.GetAsync($"/api/games/{Guid.NewGuid()}");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task MakeMove_ReturnsUpdatedGame()
        {
            var createResponse = await _client.PostAsync("/api/games", null);
            var createdGame = await createResponse.Content.ReadFromJsonAsync<GameResponse>();

            var move = new MoveRequest
            {
                Player = "X",
                Row = 0,
                Column = 0
            };  

            var moveResponse = await _client.PostAsJsonAsync($"/api/games/{createdGame!.Id}/moves", move);
            moveResponse.EnsureSuccessStatusCode();

            var updatedGame = await moveResponse.Content.ReadFromJsonAsync<GameResponse>();
            Assert.NotNull(updatedGame);
            Assert.Equal(createdGame.Id, updatedGame!.Id);
            Assert.Equal("X", updatedGame.Field[0][0]);
        }

        [Fact]
        public async Task MakeMove_ReturnsBadRequest_WhenMoveIsInvalid()
        {
            var createResponse = await _client.PostAsync("/api/games", null);
            var createdGame = await createResponse.Content.ReadFromJsonAsync<GameResponse>();

            var invalidMove = new MoveRequest
            {
                Player = "Z",
                Row = 0,
                Column = 0
            };

            var response = await _client.PostAsJsonAsync($"/api/games/{createdGame!.Id}/moves", invalidMove);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task MakeMove_ReturnsNotFound_WhenGameNotExists()
        {
            var move = new MoveRequest
            {
                Player = "X",
                Row = 0,
                Column = 0
            };

            var response = await _client.PostAsJsonAsync($"/api/games/{Guid.NewGuid()}/moves", move);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
