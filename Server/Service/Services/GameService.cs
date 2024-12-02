using DataAccess.Models;
using DataAccess.Repositories;
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

    public async Task<Game> StartNewGameAsync(Guid adminId)
    {
        var activeGame = await _gameRepository.GetActiveGameAsync();
        if (activeGame != null)
            throw new Exception("A game is already active.");

        var game = new Game
        {
            Id = Guid.NewGuid(),
            AdminId = adminId,
            TotalRevenue = 0m,
            PrizePool = 0m,
            IsClosed = false,
            CreatedAt = DateTime.UtcNow
        };

        return await _gameRepository.CreateGameAsync(game);
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
        activeGame.WinningNumbers = winningNumbers.ToList();
        activeGame.IsClosed = true;

        var winners = new List<Winner>();
        var winnerDtos = new List<WinnerDto>();

        var winningBoards = boards.Where(b => IsWinningBoard(b.Numbers, winningNumbers)).ToList();
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


    public bool IsWinningBoard(string boardNumbers, List<int> winningNumbers)
    {
        var boardArray = boardNumbers.Split(',').Select(int.Parse).ToList();
        return boardArray.SequenceEqual(winningNumbers);
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