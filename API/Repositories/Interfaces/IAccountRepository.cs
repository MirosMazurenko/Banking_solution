using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

namespace API.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account> CreateAsync(Account account);
        Task<Account> GetByAccountIdAsync(long accountId);
        Task<List<Account>> GetAllAsync();
        Task UpdateAsync(Account account);
        Task DeleteAsync(Account account);
    }
}