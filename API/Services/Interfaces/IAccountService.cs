using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;

namespace API.Repositories.Interfaces
{
    public interface IAccountService
    {
        Task<AccountDto> CreateAccountAsync(CreateAccountDto createAccountDto);
        Task<AccountDto> GetAccountDetailsAsync(long accountId);
        Task<List<AccountDto>> ListAllAccountsAsync();
        Task DepositAsync(DepositDto depositDto);
        Task WithdrawAsync(WithdrawDto withdrawDto);
        Task TransferAsync(TransferFundsDto transferFundsDto);
    }
}