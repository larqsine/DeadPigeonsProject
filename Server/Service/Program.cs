using System;
using System.Collections.Generic;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Service;

namespace Service
{
    class Program
    {
        static void Main(string[] args)
        {
            TestIsWinningBoard();
        }

        public static void TestIsWinningBoard()
        {
            // Step 1: Set up an in-memory database
            var options = new DbContextOptionsBuilder<DBContext>()
                .UseInMemoryDatabase(databaseName: "GameTestDB")
                .Options;

            using (var dbContext = new DBContext(options))
            {

                // Step 2: Initialize the GameService with the in-memory DBContext
                var gameService = new GameService(dbContext);

                // Step 3: Define winning numbers
                var winningNumbers = new List<int> { 8, 11, 1 };

                // Test 1: Board that contains all winning numbers
                var boardNumbers1 = "4,5,1,11,6,8"; // Expected to win
                var result1 = gameService.IsWinningBoard(boardNumbers1, winningNumbers);
                Console.WriteLine($"Board 1 is a winning board: {result1}");  // Expected: True

                // Test 2: Board that does not contain all winning numbers
                var boardNumbers2 = "5,6,7,8,9,10"; // Expected to lose
                var result2 = gameService.IsWinningBoard(boardNumbers2, winningNumbers);
                Console.WriteLine($"Board 2 is a winning board: {result2}");  // Expected: False

                // Test 3: Board that contains exactly the 3 winning numbers
                var boardNumbers3 = "1,8,11";  // Expected to win
                var result3 = gameService.IsWinningBoard(boardNumbers3, winningNumbers);
                Console.WriteLine($"Board 3 is a winning board: {result3}");  // Expected: True
            }
        }
    }
}