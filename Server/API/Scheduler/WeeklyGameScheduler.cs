using DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Quartz;
using Service.Interfaces;

public class WeeklyGameJob : IJob
{
    private readonly IGameService _gameService;
    private readonly UserManager<User> _userManager;

    public WeeklyGameJob(IGameService gameService, UserManager<User> userManager)
    {
        _gameService = gameService;
        _userManager = userManager;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        // Replace with the username or email of the admin responsible for the game
        const string adminUsername = "hehe";

        var admin = await _userManager.FindByNameAsync(adminUsername);
        if (admin == null)
        {
            throw new Exception($"Admin user with username {adminUsername} not found.");
        }

        await _gameService.StartNewGameAsync(admin.Id); // Pass the admin's GUID
    }
}