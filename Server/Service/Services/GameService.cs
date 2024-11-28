using DataAccess;
using DataAccess.Models;
using DataAccess.Repositories;
using Service.DTOs;
using Service.DTOs.BoardDto;
using Service.Interfaces;
using System.Linq;

namespace Service.Services;

public class GameService : IGameService
{
    private readonly GameRepository _gameRepository;
    private readonly BoardRepository _boardRepository;
    private readonly PlayerRepository _playerRepository;
    private readonly WinnerRepository _winnerRepository;

    public GameService(GameRepository gameRepository, BoardRepository boardRepository,
        PlayerRepository playerRepository, WinnerRepository winnerRepository)
    {
        _gameRepository = gameRepository;
        _boardRepository = boardRepository;
        _playerRepository = playerRepository;
        _winnerRepository = winnerRepository;
    }

    public async Task<Game> StartNewGameAsync(Guid adminId)
    {
        var activeGame = await _gameRepository.GetActiveGameAsync();
        if (activeGame != null)
            throw new Exception("A game is already active.");

        var game = new Game
        {
            Id = Guid.NewGuid(),
            AdminId = adminId, // Set the foreign key
            TotalRevenue = 0m,
            PrizePool = 0m,
            IsClosed = false,
            CreatedAt = DateTime.UtcNow
        };

        return await _gameRepository.CreateGameAsync(game);
    }

    public async Task CloseGameAsync(Guid gameId, List<int> winningNumbers)
    {
        var activeGame = await _gameRepository.GetActiveGameAsync();
        if (activeGame == null || activeGame.Id != gameId)
            throw new Exception("No active game found or game mismatch.");

        activeGame.WinningNumbers = winningNumbers.ToList();
        activeGame.IsClosed = true;
        await _gameRepository.UpdateGameAsync(activeGame);

        var boards = await _boardRepository.GetBoardsByGameIdAsync(gameId);
        var totalRevenue = boards.Sum(b => b.Cost);
        var prizePool = totalRevenue * 0.7m;
        activeGame.PrizePool = prizePool;
        await _gameRepository.UpdateGameAsync(activeGame);

        foreach (var board in boards)
        {
            var boardNumbers = board.Numbers.Split(',').Select(int.Parse).ToList();

            if (boardNumbers.SequenceEqual(winningNumbers))
            {
                board.IsWinning = true;
                await _boardRepository.UpdateBoardAsync(board);
            }
        }

        var winners = boards.Where(b => IsWinningBoard(b.Numbers, winningNumbers)).ToList();
        decimal rolloverAmount = activeGame.PrizePool > 5000
            ? activeGame.PrizePool - 5000
            : 0;


        await _gameRepository.CloseGameAsync(gameId, winningNumbers, rolloverAmount);
    }

    public bool IsWinningBoard(string boardNumbers, List<int> winningNumbers)
    {
        // Convert the board's string of numbers into a list of integers
        var boardArray = boardNumbers.Split(',').Select(int.Parse).ToList();

        // Check if all the winning numbers are present in the board's numbers
        return winningNumbers.All(boardArray.Contains);

    }

    public async Task DeductForAutoplayAsync()
    {
        var activeGame = await _gameRepository.GetActiveGameAsync();
        if (activeGame == null)
        {
            throw new InvalidOperationException("No active game found.");
        }
        
        var autoplayBoards = await _boardRepository.GetBoardsByGameIdAsync(activeGame.Id, groupByPlayer: true);
    
        foreach (var autoplayBoard in autoplayBoards)
        {
            if (autoplayBoard.Autoplay == true)
            {
                var player = await _playerRepository.GetPlayerByIdAsync(autoplayBoard.PlayerId);
                
                if (player.Balance < autoplayBoard.Cost)
                {
                    throw new Exception("Insufficient balance, please refill.");
                }
                
                player.Balance -= autoplayBoard.Cost;
                await _playerRepository.UpdatePlayerAsync(player);
            }
        }
    }
}
