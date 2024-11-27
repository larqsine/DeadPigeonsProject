using DataAccess.Models;
using DataAccess.Repositories;
using Service.Interfaces;

namespace Service
{
    public class BoardService : IBoardService
    {
        private readonly BoardRepository _boardRepository;
        private readonly PlayerRepository _playerRepository;

        public BoardService(BoardRepository boardRepository, PlayerRepository playerRepository)
        {
            _boardRepository = boardRepository;
            _playerRepository = playerRepository;
        }

        public async Task<Board> BuyBoardAsync(Guid playerId, int fieldsCount, List<int> numbers, Guid gameId)
        {
            var player = await _playerRepository.GetPlayerByIdAsync(playerId);
            if (player == null)
                throw new Exception("Player not found or inactive.");

            // Validate the GameId
            var game = await _boardRepository.GetGameByIdAsync(gameId);
            if (game == null)
                throw new Exception("Invalid GameId.");

            // Calculate the board cost based on selected fields
            decimal cost = fieldsCount switch
            {
                5 => 20m,
                6 => 40m,
                7 => 80m,
                8 => 160m,
                _ => throw new Exception("Invalid number of fields.")
            };

            // Check if the player has enough balance
            if (player.Balance < cost)
                throw new Exception("Insufficient balance.");

            // Deduct the cost from the player's balance
            player.Balance -= cost;
            await _playerRepository.UpdatePlayerAsync(player);

            // Create a new board
            var board = new Board
            {
                Id = Guid.NewGuid(),
                PlayerId = playerId,
                GameId = gameId, // Set the GameId provided by the client
                Numbers = string.Join(",", numbers),
                FieldsCount = fieldsCount,
                Cost = cost,
                CreatedAt = DateTime.UtcNow,
                IsWinning = false // Initially, it's not a winning board
            };

            // Save the board
            return await _boardRepository.CreateBoardAsync(board);
        }

    }
}