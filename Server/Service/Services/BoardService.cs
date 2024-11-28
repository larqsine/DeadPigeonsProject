using Service.DTOs;
using Service.Interfaces;
using DataAccess.Models;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Service.DTOs.BoardDto;

namespace Service.Services
{
    public class BoardService : IBoardService
    {
        private readonly BoardRepository _boardRepository;
        private readonly PlayerRepository _playerRepository;
        private readonly GameRepository _gameRepository;

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
            decimal cost = buyBoardRequestDto.FieldsCount switch
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

            // Deduct the cost from the player's balance
            player.Balance -= cost;
            await _playerRepository.UpdatePlayerAsync(player);

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
}
