using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Exceptions;
using API.Repositories.Interfaces;
using AutoMapper;

namespace API.Repositories.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;

        public AccountService(IAccountRepository accountRepository, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
        }

        public async Task<AccountDto> CreateAccountAsync(CreateAccountDto createAccountDto)
        {
            if (createAccountDto.InitialBalance < 0)
                throw new InvalidAmountException("Initial balance cannot be negative.");

            var account = _mapper.Map<Account>(createAccountDto);
            var createdAccount = await _accountRepository.CreateAsync(account);
            return _mapper.Map<AccountDto>(createdAccount);
        }

        public async Task<AccountDto> GetAccountDetailsAsync(long accountId)
        {
            var account = await _accountRepository.GetByAccountIdAsync(accountId);
            if (account is null)
                throw new AccountNotFoundException($"Account with ID {accountId} not found.");
            return _mapper.Map<AccountDto>(account);
        }

        public async Task<List<AccountDto>> ListAllAccountsAsync()
        {
            var accounts = await _accountRepository.GetAllAsync();
            return _mapper.Map<List<AccountDto>>(accounts);
        }

        public async Task DepositAsync(DepositDto depositDto)
        {
            if (depositDto.Amount <= 0)
                throw new InvalidAmountException("Deposit amount must be greater than zero.");

            var account = await _accountRepository.GetByAccountIdAsync(depositDto.Id);
            if (account == null)
                throw new AccountNotFoundException("Account not found.");

            account.Balance += depositDto.Amount;
            await _accountRepository.UpdateAsync(account);
        }

        public async Task WithdrawAsync(WithdrawDto withdrawDto)
        {
            if (withdrawDto.Amount <= 0)
                throw new InvalidAmountException("Withdraw amount must be greater than zero.");

            var account = await _accountRepository.GetByAccountIdAsync(withdrawDto.Id);
            if (account == null)
                throw new AccountNotFoundException("Account not found.");

            if (account.Balance < withdrawDto.Amount)
                throw new InsufficientFundsException("Insufficient funds.");

            account.Balance -= withdrawDto.Amount;
            await _accountRepository.UpdateAsync(account);
        }

        public async Task TransferAsync(TransferFundsDto transferFundsDto)
        {
            if (transferFundsDto.Amount <= 0)
                throw new InvalidAmountException("Transfer amount must be positive.");

            var fromAccount = await _accountRepository.GetByAccountIdAsync(transferFundsDto.FromAccountId);
            if (fromAccount == null)
                throw new TransferAccountNotFoundException("Source account not found.");

            var toAccount = await _accountRepository.GetByAccountIdAsync(transferFundsDto.ToAccountId);
            if (toAccount == null)
                throw new TransferAccountNotFoundException("Destination account not found.");

            if (fromAccount.Balance < transferFundsDto.Amount)
                throw new InsufficientFundsException("Insufficient funds in source account.");

            fromAccount.Balance -= transferFundsDto.Amount;
            toAccount.Balance += transferFundsDto.Amount;

            await _accountRepository.UpdateAsync(fromAccount);
            await _accountRepository.UpdateAsync(toAccount);
        }
    }
}