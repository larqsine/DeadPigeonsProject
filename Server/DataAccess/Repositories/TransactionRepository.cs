using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class TransactionRepository
    {
        private readonly DBContext _context;

        public TransactionRepository(DBContext context)
        {
            _context = context;
        }
        
        public async Task<Transaction?> GetTransactionByIdAsync(Guid transactionId)
        {
            return await _context.Set<Transaction>()
                .FirstOrDefaultAsync(t => t.Id == transactionId);
        }
        
        public async Task AddTransactionAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }
        
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<List<Transaction>> GetTransactionsByPlayerIdAsync(Guid playerId)
        {
            return await _context.Transactions
                .Where(t => t.PlayerId == playerId)
                .ToListAsync();
        }
    }
}