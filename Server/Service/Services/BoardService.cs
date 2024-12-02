using DataAccess.Repositories;
using Service.DTOs.BoardDto;
using Service.DTOs.TransactionDto;
using Service.Interfaces;

namespace Service.Services;

public class BoardService : IBoardService
{
    private readonly BoardRepository _boardRepository;
    private readonly GameRepository _gameRepository;
    private readonly PlayerRepository _playerRepository;

    public BoardService(
        BoardRepository boardRepository,
        PlayerRepository playerRepository,
        GameRepository gameRepository)
    {
        _boardRepository = boardRepository;
        _playerRepository = playerRepository;
        _gameRepository = gameRepository;
    }

    public async Task<BoardResponseDto> BuyBoardAsync(Guid playerId, BuyBoardRequestDto buyBoardRequestDto)
    {
        // Get the player
        var player = await _playerRepository.GetPlayerByIdAsync(playerId);
        if (player == null)
            throw new Exception("Player not found or inactive.");

        // Get the game by GameId
        var game = await _gameRepository.GetGameByIdAsync(buyBoardRequestDto.GameId);
        if (game == null)
            throw new Exception("Invalid GameId.");

        // Calculate the cost based on FieldsCount from the DTO
        var cost = buyBoardRequestDto.FieldsCount switch
        {
            5 => 20m,
            6 => 40m,
            7 => 80m,
            8 => 160m,
            _ => throw new Exception("Invalid number of fields.")
        };

        // Check if the player has sufficient balance
        if (player.Balance < cost)
            throw new Exception("Insufficient balance.");

        // Create a purchase transaction
        var purchaseTransactionDto = new TransactionCreateDto
        {
            Amount = cost
        };

        var transactionId = Guid.NewGuid();
        var purchaseTransaction = purchaseTransactionDto.ToPurchaseTransaction(playerId, transactionId);

        // Add the purchase transaction (Transaction Repository handles persisting and deducting balance)
        await _playerRepository.AddTransactionAsync(purchaseTransaction);

        // Use the ToBoard method from the DTO to map to the Board entity
        var board = buyBoardRequestDto.ToBoard(playerId, cost);

        // Save the board to the database
        var createdBoard = await _boardRepository.CreateBoardAsync(board);

        // Return the BoardResponseDto with the board details
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
}