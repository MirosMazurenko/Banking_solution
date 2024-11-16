using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Exceptions;
using API.Repositories.Interfaces;
using API.Repositories.Implementation;
using AutoMapper;
using Moq;
using Xunit;

public class AccountServiceTests
{
    private readonly Mock<IAccountRepository> _mockAccountRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly AccountService _accountService;

    public AccountServiceTests()
    {
        _mockAccountRepository = new Mock<IAccountRepository>();
        _mockMapper = new Mock<IMapper>();
        _accountService = new AccountService(_mockAccountRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task CreateAccountAsync_ShouldReturnAccountDto_WhenValid()
    {
        // Arrange
        var createAccountDto = new CreateAccountDto { OwnerName = "Test Account", InitialBalance = 100 };
        var account = new Account { Id = 1, OwnerName = createAccountDto.OwnerName, Balance = createAccountDto.InitialBalance };
        var accountDto = new AccountDto { Id = account.Id, OwnerName = account.OwnerName, Balance = account.Balance };

        _mockMapper.Setup(m => m.Map<Account>(createAccountDto)).Returns(account);
        _mockAccountRepository.Setup(r => r.CreateAsync(account)).ReturnsAsync(account);
        _mockMapper.Setup(m => m.Map<AccountDto>(account)).Returns(accountDto);

        // Act
        var result = await _accountService.CreateAccountAsync(createAccountDto);

        // Assert
        Assert.Equal(accountDto, result);
    }

    [Fact]
    public async Task CreateAccountAsync_ShouldThrowInvalidAmountException_WhenInitialBalanceIsNegative()
    {
        // Arrange
        var createAccountDto = new CreateAccountDto { OwnerName = "Test Account", InitialBalance = -100 };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidAmountException>(() => _accountService.CreateAccountAsync(createAccountDto));
    }

    [Fact]
    public async Task GetAccountDetailsAsync_ShouldReturnAccountDto_WhenAccountExists()
    {
        // Arrange
        var accountId = 1;
        var account = new Account { Id = accountId, OwnerName = "Test Account", Balance = 100 };
        var accountDto = new AccountDto { Id = account.Id, OwnerName = account.OwnerName, Balance = account.Balance };

        _mockAccountRepository.Setup(r => r.GetByAccountIdAsync(accountId)).ReturnsAsync(account);
        _mockMapper.Setup(m => m.Map<AccountDto>(account)).Returns(accountDto);

        // Act
        var result = await _accountService.GetAccountDetailsAsync(accountId);

        // Assert
        Assert.Equal(accountDto, result);
    }

    [Fact]
    public async Task GetAccountDetailsAsync_ShouldThrowAccountNotFoundException_WhenAccountDoesNotExist()
    {
        // Arrange
        var accountId = 1;
        _mockAccountRepository.Setup(r => r.GetByAccountIdAsync(accountId)).ReturnsAsync((Account)null);

        // Act & Assert
        await Assert.ThrowsAsync<AccountNotFoundException>(() => _accountService.GetAccountDetailsAsync(accountId));
    }

    [Fact]
    public async Task ListAllAccountsAsync_ShouldReturnListOfAccountDtos()
    {
        // Arrange
        var accounts = new List<Account>
        {
            new Account { Id = 1, OwnerName = "Account 1", Balance = 100 },
            new Account { Id = 2, OwnerName = "Account 2", Balance = 200 }
        };
        var accountDtos = new List<AccountDto>
        {
            new AccountDto { Id = 1, OwnerName = "Account 1", Balance = 100 },
            new AccountDto { Id = 2, OwnerName = "Account 2", Balance = 200 }
        };

        _mockAccountRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(accounts);
        _mockMapper.Setup(m => m.Map<List<AccountDto>>(accounts)).Returns(accountDtos);

        // Act
        var result = await _accountService.ListAllAccountsAsync();

        // Assert
        Assert.Equal(accountDtos, result);
    }

    [Fact]
    public async Task DepositAsync_ShouldUpdateBalance_WhenValid()
    {
        // Arrange
        var depositDto = new DepositDto { Id = 1, Amount = 100 };
        var account = new Account { Id = depositDto.Id, OwnerName = "Test Account", Balance = 100 };

        _mockAccountRepository.Setup(r => r.GetByAccountIdAsync(depositDto.Id)).ReturnsAsync(account);
        _mockAccountRepository.Setup(r => r.UpdateAsync(account)).Returns(Task.CompletedTask);

        // Act
        await _accountService.DepositAsync(depositDto);

        // Assert
        Assert.Equal(200, account.Balance);
    }

    [Fact]
    public async Task WithdrawAsync_ShouldUpdateBalance_WhenValid()
    {
        // Arrange
        var withdrawDto = new WithdrawDto { Id = 1, Amount = 50 };
        var account = new Account { Id = withdrawDto.Id, OwnerName = "Test Account", Balance = 100 };

        _mockAccountRepository.Setup(r => r.GetByAccountIdAsync(withdrawDto.Id)).ReturnsAsync(account);
        _mockAccountRepository.Setup(r => r.UpdateAsync(account)).Returns(Task.CompletedTask);

        // Act
        await _accountService.WithdrawAsync(withdrawDto);

        // Assert
        Assert.Equal(50, account.Balance);
    }

    [Fact]
    public async Task WithdrawAsync_ShouldThrowInsufficientFundsException_WhenBalanceIsInsufficient()
    {
        // Arrange
        var withdrawDto = new WithdrawDto { Id = 1, Amount = 200 };
        var account = new Account { Id = withdrawDto.Id, OwnerName = "Test Account", Balance = 100 };

        _mockAccountRepository.Setup(r => r.GetByAccountIdAsync(withdrawDto.Id)).ReturnsAsync(account);

        // Act & Assert
        await Assert.ThrowsAsync<InsufficientFundsException>(() => _accountService.WithdrawAsync(withdrawDto));
    }

    [Fact]
    public async Task TransferAsync_ShouldUpdateBalances_WhenValid()
    {
        // Arrange
        var transferFundsDto = new TransferFundsDto { FromAccountId = 1, ToAccountId = 2, Amount = 50 };
        var fromAccount = new Account { Id = transferFundsDto.FromAccountId, OwnerName = "Source Account", Balance = 100 };
        var toAccount = new Account { Id = transferFundsDto.ToAccountId, OwnerName = "Destination Account", Balance = 50 };

        _mockAccountRepository.Setup(r => r.GetByAccountIdAsync(transferFundsDto.FromAccountId)).ReturnsAsync(fromAccount);
        _mockAccountRepository.Setup(r => r.GetByAccountIdAsync(transferFundsDto.ToAccountId)).ReturnsAsync(toAccount);
        _mockAccountRepository.Setup(r => r.UpdateAsync(It.IsAny<Account>())).Returns(Task.CompletedTask);

        // Act
        await _accountService.TransferAsync(transferFundsDto);

        // Assert
        Assert.Equal(50, fromAccount.Balance);
        Assert.Equal(100, toAccount.Balance);
    }
}
