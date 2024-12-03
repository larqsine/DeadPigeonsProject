using DataAccess.Models;
using DataAccess.Repositories;
using Service.DTOs.GameDto;
using Service.DTOs.WinnerDto;
using Service.Interfaces;

namespace Service.Services;

public class GameService : IGameService
{
    private readonly BoardRepository _boardRepository;
    private readonly GameRepository _gameRepository;
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
    
    public async Task<Game?> GetActiveGameAsync()
    {
        return await _gameRepository.GetActiveGameAsync();
    }

    
    public async Task<GameDetailsDto> StartNewGameAsync()
    {
        var activeGame = await _gameRepository.GetActiveGameAsync();
        if (activeGame != null)
            throw new Exception("A game is already active.");

        var game = new Game
        {
            Id = Guid.NewGuid(),
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate = null,
            TotalRevenue = 0m,
            PrizePool = 0m,
            IsClosed = false,
            CreatedAt = DateTime.UtcNow
        };

        var createdGame = await _gameRepository.CreateGameAsync(game);

        return new GameDetailsDto
        {
            Id = createdGame.Id,
            StartDate = createdGame.StartDate,
            EndDate = createdGame.EndDate,
            TotalRevenue = createdGame.TotalRevenue,
            PrizePool = createdGame.PrizePool,
            IsClosed = createdGame.IsClosed.Value,
            WinningNumbers = createdGame.WinningNumbers
        };
    }

   
   public async Task<List<WinnerDto>> CloseGameAsync(Guid gameId, List<int> winningNumbers)
{
    if (winningNumbers == null || !winningNumbers.Any())
        throw new ArgumentException("Winning numbers cannot be null or empty.");

    var activeGame = await _gameRepository.GetActiveGameAsync();
    if (activeGame == null) throw new InvalidOperationException("No active game found.");
    if (activeGame.Id != gameId) throw new InvalidOperationException("Game ID mismatch.");

    var boards = await _boardRepository.GetBoardsByGameIdAsync(gameId);
    var totalRevenue = boards.Sum(b => b.Cost);
    if (totalRevenue == 0) throw new InvalidOperationException("Total revenue cannot be zero. Check board costs.");

    var prizePool = totalRevenue * 0.7m;

    activeGame.TotalRevenue = totalRevenue;
    activeGame.PrizePool = prizePool;
    activeGame.WinningNumbers = winningNumbers;
    activeGame.IsClosed = true;

    var winners = new List<Winner>();
    var winnerDtos = new List<WinnerDto>();

    var winningBoards = boards.Where(b => IsWinningBoard(
        b.Numbers.Split(',').Select(int.Parse).ToList(), winningNumbers)).ToList();
        
    if (winningBoards.Any())
    {
        var winningAmount = prizePool / winningBoards.Count;
        foreach (var board in winningBoards)
        {
            board.IsWinning = true;

            var winner = new Winner
            {
                Id = Guid.NewGuid(),
                GameId = gameId,
                PlayerId = board.PlayerId,
                BoardId = board.Id,
                WinningAmount = winningAmount,
                CreatedAt = DateTime.UtcNow
            };

            winners.Add(winner);
            winnerDtos.Add(WinnerDto.FromEntity(winner));
        }
    }

    await _boardRepository.UpdateBoardsAsync(boards);
    await _winnerRepository.AddWinnersAsync(winners);
    await _gameRepository.UpdateGameAsync(activeGame);

    var rolloverAmount = activeGame.PrizePool > 5000
        ? activeGame.PrizePool - 5000
        : 0;

    await _gameRepository.CloseGameAsync(gameId, winningNumbers, rolloverAmount);

    return winnerDtos;
}



public bool IsWinningBoard(List<int> boardNumbers, List<int> winningNumbers)
{
    // Convert both lists to sets (HashSets) to disregard order and duplicates
    var boardNumberSet = new HashSet<int>(boardNumbers);
    var winningNumberSet = new HashSet<int>(winningNumbers);

    // Ensure that all winning numbers are contained in the board's numbers
    return winningNumberSet.All(winningNumber => boardNumberSet.Contains(winningNumber));
}

    /*public async Task DeductForAutoplayAsync()
    {
        var activeGame = await _gameRepository.GetActiveGameAsync();
        if (activeGame == null) throw new InvalidOperationException("No active game found.");

        var autoplayBoards = await _boardRepository.GetBoardsByGameIdAsync(activeGame.Id, true);

        foreach (var autoplayBoard in autoplayBoards)
            if (autoplayBoard.Autoplay == true)
            {
                var player = await _playerRepository.GetPlayerByIdAsync(autoplayBoard.PlayerId);

                if (player.Balance < autoplayBoard.Cost) throw new Exception("Insufficient balance, please refill.");

                player.Balance -= autoplayBoard.Cost;
                await _playerRepository.UpdatePlayerAsync(player);
            }
    }*/
}