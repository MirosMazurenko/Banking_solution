using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Repositories.Implementation;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class AccountRepositoryTests
{
    private readonly DatabaseContext _context;
    private readonly AccountRepository _repository;

    public AccountRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

        _context = new DatabaseContext(options);
        _repository = new AccountRepository(_context);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddAccountToDatabase()
    {
        // Arrange
        var account = new Account { OwnerName = "Test Account", Balance = 100 };

        // Act
        var result = await _repository.CreateAsync(account);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(account.OwnerName, result.OwnerName);
        Assert.Single(_context.Accounts);
        Assert.Equal(account.OwnerName, _context.Accounts.First().OwnerName);
    }

    [Fact]
    public async Task GetByAccountIdAsync_ShouldReturnAccount_WhenExists()
    {
        // Arrange
        var account = new Account { Id = 1, OwnerName = "Test Account", Balance = 100 };
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByAccountIdAsync(account.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(account.OwnerName, result.OwnerName);
    }

    [Fact]
    public async Task GetByAccountIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = await _repository.GetByAccountIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllAccounts()
    {
        // Arrange
        var accounts = new List<Account>
        {
            new Account { OwnerName = "Account 1", Balance = 100 },
            new Account { OwnerName = "Account 2", Balance = 200 }
        };
        _context.Accounts.AddRange(accounts);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(accounts.Count, result.Count);
        Assert.Equal(accounts[0].OwnerName, result[0].OwnerName);
        Assert.Equal(accounts[1].OwnerName, result[1].OwnerName);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateAccount()
    {
        // Arrange
        var account = new Account { Id = 1, OwnerName = "Test Account", Balance = 100 };
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        // Act
        account.Balance = 200;
        await _repository.UpdateAsync(account);

        // Assert
        var updatedAccount = await _context.Accounts.FindAsync(account.Id);
        Assert.NotNull(updatedAccount);
        Assert.Equal(200, updatedAccount.Balance);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveAccount()
    {
        // Arrange
        var account = new Account { Id = 1, OwnerName = "Test Account", Balance = 100 };
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(account);

        // Assert
        var result = await _context.Accounts.FindAsync(account.Id);
        Assert.Null(result);
        Assert.Empty(_context.Accounts);
    }
}
