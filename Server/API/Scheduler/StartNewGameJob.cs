using Quartz;
using Service.Interfaces;

namespace API.Scheduler
{
    public class StartNewGameJob : IJob
    {
        private readonly IGameService _gameService;
        private readonly ILogger<StartNewGameJob> _logger;

        public StartNewGameJob(IGameService gameService, ILogger<StartNewGameJob> logger)
        {
            _gameService = gameService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var activeGame = await _gameService.GetActiveGameAsync();

                if (activeGame == null)
                {
                    await _gameService.StartNewGameAsync();
                    _logger.LogInformation($"New game started at {DateTime.UtcNow}");
                }
                else
                {
                    _logger.LogInformation("An active game already exists.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in StartNewGameJob: {ex.Message}");
            }
        }
    }
}