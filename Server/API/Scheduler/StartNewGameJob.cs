using Quartz;
using Service.Interfaces;

public class StartNewGameJob : IJob
{
    private readonly IGameService _gameService;
    private readonly IBoardService _boardService;
    private readonly ILogger<StartNewGameJob> _logger;

    public StartNewGameJob(IGameService gameService, IBoardService boardService, ILogger<StartNewGameJob> logger)
    {
        _gameService = gameService;
        _boardService = boardService;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            // Get the current active game
            var activeGame = await _gameService.GetActiveGameAsync();

            if (activeGame == null)
            {
                // Start a new game and get its ID
                var newGame = await _gameService.StartNewGameAsync();
                _logger.LogInformation($"New game started at {DateTime.UtcNow} with ID: {newGame.Id}");

                // Trigger autoplay for the newly created game
                await _boardService.HandleAutoplayAsync(newGame.Id);
            }
            else
            {
                _logger.LogInformation("An active game already exists, skipping new game creation.");

                // If an active game exists, ensure autoplay is processed for it
                await _boardService.HandleAutoplayAsync(activeGame.Id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in StartNewGameJob: {ex.Message}");
        }
    }

}