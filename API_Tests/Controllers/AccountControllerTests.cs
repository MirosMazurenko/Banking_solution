using System.Threading.Tasks;
using API.Controllers;
using API.DTOs;
using API.Exceptions;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class AccountControllerTests
{
    private readonly Mock<IAccountService> _mockAccountService;
    private readonly AccountController _controller;

    public AccountControllerTests()
    {
        _mockAccountService = new Mock<IAccountService>();
        _controller = new AccountController(_mockAccountService.Object);
    }

    [Fact]
    public async Task ListAllAccounts_ReturnsOkWithAccounts()
    {
        // Arrange
        var accounts = new List<AccountDto> { new AccountDto { Id = 1, OwnerName = "Test Account" } };
        _mockAccountService.Setup(s => s.ListAllAccountsAsync()).ReturnsAsync(accounts);

        // Act
        var result = await _controller.ListAllAccounts();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(accounts, okResult.Value);
    }

    [Fact]
    public async Task GetAccountDetails_ReturnsOkWithAccount()
    {
        // Arrange
        var accountId = 1;
        var accountDto = new AccountDto { Id = accountId, OwnerName = "Test Account" };
        _mockAccountService.Setup(s => s.GetAccountDetailsAsync(accountId)).ReturnsAsync(accountDto);

        // Act
        var result = await _controller.GetAccountDetails(accountId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(accountDto, okResult.Value);
    }

    [Fact]
    public async Task GetAccountDetails_ReturnsNotFoundForInvalidId()
    {
        // Arrange
        var invalidAccountId = 999;
        var expectedError = $"Account with ID {invalidAccountId} not found.";

        _mockAccountService.Setup(s => s.GetAccountDetailsAsync(invalidAccountId))
                           .ThrowsAsync(new AccountNotFoundException(expectedError));

        // Act
        var result = await _controller.GetAccountDetails(invalidAccountId);

        // Assert
        var objectResult = Assert.IsType<NotFoundObjectResult>(result);

        var response = objectResult.Value;
        var errorProperty = response.GetType().GetProperty("error");
        Assert.NotNull(errorProperty);
        var errorValue = errorProperty.GetValue(response)?.ToString();

        Assert.Equal(expectedError, errorValue);
    }

    [Fact]
    public async Task CreateAccount_ReturnsCreatedAtAction()
    {
        // Arrange
        var createAccountDto = new CreateAccountDto { OwnerName = "New Account", InitialBalance = 100 };
        var accountDto = new AccountDto { Id = 1, OwnerName = createAccountDto.OwnerName, Balance = createAccountDto.InitialBalance };
        _mockAccountService.Setup(s => s.CreateAccountAsync(createAccountDto)).ReturnsAsync(accountDto);

        // Act
        var result = await _controller.CreateAccount(createAccountDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(accountDto, createdResult.Value);
    }

    [Fact]
    public async Task TransferFunds_ReturnsNoContent()
    {
        // Arrange
        var transferFundsDto = new TransferFundsDto { FromAccountId = 1, ToAccountId = 2, Amount = 50 };
        _mockAccountService.Setup(s => s.TransferAsync(transferFundsDto)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.TransferFunds(transferFundsDto);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task TransferFunds_ReturnsUnprocessableEntityForInsufficientFunds()
    {
        // Arrange
        var transferFundsDto = new TransferFundsDto { FromAccountId = 1, ToAccountId = 2, Amount = 1000 };
        var expectedError = "Insufficient funds in source account.";

        _mockAccountService.Setup(s => s.TransferAsync(It.IsAny<TransferFundsDto>()))
                           .ThrowsAsync(new InsufficientFundsException(expectedError));

        // Act
        var result = await _controller.TransferFunds(transferFundsDto);

        // Assert
        var objectResult = Assert.IsType<UnprocessableEntityObjectResult>(result);

        var response = objectResult.Value;
        var errorProperty = response.GetType().GetProperty("error");
        Assert.NotNull(errorProperty);
        var errorValue = errorProperty.GetValue(response)?.ToString();

        Assert.Equal(expectedError, errorValue);
    }

    [Fact]
    public async Task Deposit_ReturnsNoContent()
    {
        // Arrange
        var depositDto = new DepositDto { Id = 1, Amount = 100 };
        _mockAccountService.Setup(s => s.DepositAsync(depositDto)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Deposit(depositDto);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Withdraw_ReturnsNoContent()
    {
        // Arrange
        var withdrawDto = new WithdrawDto { Id = 1, Amount = 50 };
        _mockAccountService.Setup(s => s.WithdrawAsync(withdrawDto)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Withdraw(withdrawDto);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Withdraw_ReturnsUnprocessableEntityForInsufficientFunds()
    {
        // Arrange
        var withdrawDto = new WithdrawDto { Id = 1, Amount = 1000 };
        var expectedError = "Insufficient funds.";

        _mockAccountService.Setup(s => s.WithdrawAsync(It.IsAny<WithdrawDto>()))
                           .ThrowsAsync(new InsufficientFundsException(expectedError));

        // Act
        var result = await _controller.Withdraw(withdrawDto);

        // Assert
        var objectResult = Assert.IsType<UnprocessableEntityObjectResult>(result);
        var response = objectResult.Value;
        Assert.NotNull(response);

        var error = response.GetType().GetProperty("error")?.GetValue(response)?.ToString();
        Assert.Equal(expectedError, error);
    }
}
