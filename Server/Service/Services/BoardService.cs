using DataAccess.Models;
using DataAccess.Repositories;
using Service.DTOs.BoardDto;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Services
{
    public class BoardService : IBoardService
    {
        private readonly BoardRepository _boardRepository;
        private readonly IPlayerService _playerService;

        public BoardService(BoardRepository boardRepository, IPlayerService playerService)
        {
            _boardRepository = boardRepository;
            _playerService = playerService;
        }

        public async Task<Board> CreateBoardAsync(BoardCreateDto boardDto)
        {
            // Validate fields count before creating a board
            if (boardDto.FieldsCount < 5 || boardDto.FieldsCount > 8)
                throw new ArgumentException("FieldsCount must be between 5 and 8");

            // Create the board object using the new constructor
            var board = new Board(boardDto.FieldsCount)
            {
                Id = Guid.NewGuid(),
                PlayerId = boardDto.PlayerId,
                GameId = boardDto.GameId,
                Numbers = boardDto.Numbers,
                Autoplay = boardDto.Autoplay,
                CreatedAt = DateTime.UtcNow
            };

            // Add to repository
            await _boardRepository.AddBoardAsync(board);

            // Return the created board
            return board;
        }

        public async Task<IEnumerable<Board>> GetBoardsByPlayerAsync(Guid playerId)
        {
            return await _boardRepository.GetBoardsByPlayerAsync(playerId);
        }

        public async Task<IEnumerable<Board>> GetBoardsByGameAsync(Guid gameId)
        {
            return await _boardRepository.GetBoardsByGameAsync(gameId);
        }
    }
}