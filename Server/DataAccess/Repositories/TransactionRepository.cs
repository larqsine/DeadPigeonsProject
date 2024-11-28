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
        
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}