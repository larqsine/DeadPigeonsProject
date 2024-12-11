using Microsoft.AspNetCore.Mvc;
using Service.Services;

namespace API.Controllers;

public class TransactionController : ControllerBase
{
    private readonly TransactionService _transactionService;

    public TransactionController(TransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpPut("transaction/{transactionId}/approve")]
    public async Task<IActionResult> ApproveTransaction(Guid transactionId)
    {
        var transaction = await _transactionService.ApproveTransactionAsync(transactionId);
        if (transaction == null)
        {
            return NotFound("Transaction not found");
        }

        return Ok(transaction);
    }

    [HttpPut("transaction/{transactionId}/decline")]
    public async Task<IActionResult> DeclineTransaction(Guid transactionId)
    {
        var transaction = await _transactionService.DeclineTransactionAsync(transactionId);
        if (transaction == null)
        {
            return NotFound("Transaction not found");
        }

        return Ok(transaction);
    }
    
    //Get deposit transactions
    [HttpGet("transaction/deposit")]
    public async Task<IActionResult> GetDepositTransactions()
    {
        var transactions = await _transactionService.GetTransactionsByTypeAsync("deposit");
        return Ok(transactions);
    }

[HttpPut("{playerId}/GetPlayerTransaction")]
public async Task<IActionResult> GetTransactionsByPlayerId(Guid playerId)
    {
        var transactions = await _transactionService.GetTransactionsByPlayerIdAsync(playerId);
        return Ok(transactions);
    }
}