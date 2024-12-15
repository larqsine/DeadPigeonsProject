using Microsoft.AspNetCore.Mvc;
using Service.Services;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TransactionController : ControllerBase
{
    private readonly TransactionService _transactionService;

    public TransactionController(TransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpPut("{transactionId}/approve")]
    public async Task<IActionResult> ApproveTransaction(Guid transactionId)
    {
        var transaction = await _transactionService.ApproveTransactionAsync(transactionId);
        if (transaction == null) return NotFound("Transaction not found");

        return Ok(transaction);
    }

    [HttpPut("{transactionId}/decline")]
    public async Task<IActionResult> DeclineTransaction(Guid transactionId)
    {
        var transaction = await _transactionService.DeclineTransactionAsync(transactionId);
        if (transaction == null) return NotFound("Transaction not found");

        return Ok(transaction);
    }

    [HttpPut("{playerId}/GetPlayerTransaction")]
    public async Task<IActionResult> GetTransactionsByPlayerId(Guid playerId)
    {
        var transactions = await _transactionService.GetTransactionsByPlayerIdAsync(playerId);
        return Ok(transactions);
    }
}