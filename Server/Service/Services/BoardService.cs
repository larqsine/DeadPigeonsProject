using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.Extensions.Logging;
using Service.DTOs.BoardDto;
using Service.DTOs.TransactionDto;
using Service.Interfaces;

namespace Service.Services;

public class BoardService : IBoardService
{
    
    private readonly BoardRepository _boardRepository;
    private readonly PlayerRepository _playerRepository;
    private readonly GameRepository _gameRepository;
    private readonly ILogger<BoardService> _logger;
    public BoardService(
        BoardRepository boardRepository,
        PlayerRepository playerRepository,
        GameRepository gameRepository,
        ILogger<BoardService> logger)
    {
        _boardRepository = boardRepository;
        _playerRepository = playerRepository;
        _gameRepository = gameRepository;
        _logger=logger;
    }
        
    public async Task<BoardResponseDto> BuyBoardAsync(Guid playerId, BuyBoardRequestDto buyBoardRequestDto)
    {
        var player = await GetPlayerAsync(playerId);
        var game = await GetGameAsync(buyBoardRequestDto.GameId);

        CheckParticipationDeadline();

        ValidatePlayerBalance(player, buyBoardRequestDto.FieldsCount);

        var transaction = await ProcessPurchaseTransaction(player, buyBoardRequestDto.FieldsCount);
        var board = CreateBoard(buyBoardRequestDto, playerId, transaction.Amount);

        var createdBoard = await _boardRepository.CreateBoardAsync(board);

        return MapToBoardResponseDto(createdBoard);
    }

    private async Task<Player> GetPlayerAsync(Guid playerId)
    {
        var player = await _playerRepository.GetPlayerByIdAsync(playerId);
        if (player == null)
            throw new Exception("Player not found or inactive.");

        if (player.AnnualFeePaid==false)
            throw new Exception("Cannot buy a board if annual fee has not been paid.");

        return player;
    }

    private async Task<Game> GetGameAsync(Guid gameId)
    {
        var game = await _gameRepository.GetGameByIdAsync(gameId);
        if (game == null || game.IsClosed==true)
            throw new Exception("Game not found or closed.");

        return game;
    }

    private void CheckParticipationDeadline()
    {
        var denmarkTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        var currentTimeInDenmark = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, denmarkTimeZone);

        if (currentTimeInDenmark.DayOfWeek == DayOfWeek.Saturday && currentTimeInDenmark.Hour >= 17)
            throw new Exception("You cannot participate after Saturday 5 PM Danish time.");
    }

    private void ValidatePlayerBalance(Player player, int fieldsCount)
    {
        decimal cost = fieldsCount switch
        {
            5 => 20m,
            6 => 40m,
            7 => 80m,
            8 => 160m,
            _ => throw new Exception("Invalid number of fields.")
        };

        if (player.Balance < cost)
            throw new Exception("Insufficient balance.");
    }

    private async Task<Transaction> ProcessPurchaseTransaction(Player player, int fieldsCount)
    {
        var cost = fieldsCount switch
        {
            5 => 20m,
            6 => 40m,
            7 => 80m,
            8 => 160m,
            _ => throw new Exception("Invalid number of fields.")
        };

        var purchaseTransactionDto = new TransactionCreateDto { Amount = cost };
        var transactionId = Guid.NewGuid();
        var purchaseTransaction = purchaseTransactionDto.ToPurchaseTransaction(player.Id, transactionId);

        await _playerRepository.AddTransactionAsync(purchaseTransaction);

        return purchaseTransaction;
    }

    private Board CreateBoard(BuyBoardRequestDto buyBoardRequestDto, Guid playerId, decimal cost)
    {
        var board = buyBoardRequestDto.ToBoard(playerId, cost);

        if (buyBoardRequestDto.RemainingAutoplayWeeks > 0)
        {
            board.Autoplay = true;
            board.RemainingAutoplayWeeks = buyBoardRequestDto.RemainingAutoplayWeeks;
        }

        return board;
    }

    private BoardResponseDto MapToBoardResponseDto(Board createdBoard)
    {
        return new BoardResponseDto
        {
            Id = createdBoard.Id,
            PlayerId = createdBoard.PlayerId,
            GameId = createdBoard.GameId,
            Numbers = createdBoard.Numbers,
            FieldsCount = createdBoard.FieldsCount,
            Cost = createdBoard.Cost,
            CreatedAt = createdBoard.CreatedAt,
            IsWinning = createdBoard.IsWinning
        };
    }

    public async Task HandleAutoplayAsync(Guid gameId)
    {
        try
        {
            var autoplayBoards = await GetAutoplayBoardsAsync();
            if (!autoplayBoards.Any()) return;

            foreach (var board in autoplayBoards)
            {
                var player = await _playerRepository.GetPlayerByIdAsync(board.PlayerId);
                if (player == null || player.Balance < board.Cost)
                {
                    _logger.LogError($"Player {board.PlayerId} not found or insufficient balance.");
                    continue;
                }

                await ProcessAutoplayBoard(player, board, gameId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in HandleAutoplayAsync: {ex.Message}");
        }
    }

    private async Task<List<Board>> GetAutoplayBoardsAsync()
    {
        var autoplayBoards = await _boardRepository.GetBoardsForAutoplayAsync();
        _logger.LogInformation($"Fetched {autoplayBoards.Count} autoplay boards.");
        return autoplayBoards;
    }

    private async Task ProcessAutoplayBoard(Player player, Board board, Guid gameId)
    {
        player.Balance -= board.Cost;

        var newBoard = CreateNewAutoplayBoard(board, gameId);
        if (newBoard.RemainingAutoplayWeeks == 0)
        {
            newBoard.Autoplay = false;
        }

        _logger.LogInformation($"Creating new autoplay board for Player {player.Id}, Game {gameId}, Board ID: {newBoard.Id}");

        await _boardRepository.CreateBoardAsync(newBoard);
        await _playerRepository.UpdatePlayerAsync(player);

        board.Autoplay = false;
        await _boardRepository.UpdateBoardsAsync(new List<Board> { board });

        _logger.LogInformation($"Autoplay disabled for the original board ID {board.Id} because a new board was created.");
    }

    private Board CreateNewAutoplayBoard(Board board, Guid gameId)
    {
        return new Board
        {
            Id = Guid.NewGuid(),
            PlayerId = board.PlayerId,
            GameId = gameId,
            Numbers = board.Numbers,
            FieldsCount = board.FieldsCount,
            Cost = board.Cost,
            CreatedAt = DateTime.UtcNow,
            IsWinning = false,
            Autoplay = true,
            RemainingAutoplayWeeks = board.RemainingAutoplayWeeks - 1
        };
    }
        public async Task<List<BoardResponseDto>> GetBoardsByPlayerIdAsync(Guid playerId)
        {
            var boards = await _boardRepository.GetBoardsByPlayerIdAsync(playerId);
            return boards.Select(BoardResponseDto.FromEntity).ToList();
        }
        public async Task<List<BoardResponseDto>> GetBoardsByGameIdAsync(Guid gameId)
        {
            var boards = await _boardRepository.GetBoardsByGameIdAsync(gameId);
            return boards.Select(BoardResponseDto.FromEntity).ToList();
        }
        public async Task<List<BoardResponseDto>> GetAllBoardsAsync()
        {
            var boards = await _boardRepository.GetAllBoardsAsync();
            return boards.Select(BoardResponseDto.FromEntity).ToList();
        }
}