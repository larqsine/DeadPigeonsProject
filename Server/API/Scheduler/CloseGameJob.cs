using Quartz;
using Service.Interfaces;

namespace API.Scheduler;

public class CloseGameJob : IJob
{
    private readonly IGameService _gameService;

    public CloseGameJob(IGameService gameService)
    {
        _gameService = gameService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var activeGame = await _gameService.GetActiveGameAsync();

            if (activeGame != null && activeGame.IsClosed.HasValue && !activeGame.IsClosed.Value)
            {
                var winningNumbers = GenerateRandomWinningNumbers();
                await _gameService.CloseGameAsync(activeGame.Id, winningNumbers);
                Console.WriteLine($"Game closed successfully at {DateTime.UtcNow}. Winning Numbers: {string.Join(",", winningNumbers)}");
            }
            else
            {
                Console.WriteLine("No active game to close.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CloseGameJob: {ex.Message}");
        }
    }

    private List<int> GenerateRandomWinningNumbers()
    {
        var random = new Random();
        return Enumerable.Range(1, 16).OrderBy(_ => random.Next()).Take(3).ToList();
    }
}