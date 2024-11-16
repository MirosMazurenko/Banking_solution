using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Exceptions;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> ListAllAccounts()
        {
            var accounts = await _accountService.ListAllAccountsAsync();
            return Ok(accounts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountDetails(long id)
        {
            try
            {
                var accountDto = await _accountService.GetAccountDetailsAsync(id);
                return Ok(accountDto);
            }
            catch (AccountNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAccount(CreateAccountDto createAccountDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var accountDto = await _accountService.CreateAccountAsync(createAccountDto);
                return CreatedAtAction(
                    nameof(GetAccountDetails),
                    new { id = accountDto.Id },
                    accountDto
                );
            }
            catch (InvalidAmountException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> TransferFunds(TransferFundsDto transferFundsDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _accountService.TransferAsync(transferFundsDto);
                return NoContent();
            }
            catch (TransferAccountNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidAmountException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InsufficientFundsException ex)
            {
                return UnprocessableEntity(new { error = ex.Message });
            }
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit(DepositDto depositDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _accountService.DepositAsync(depositDto);
                return NoContent();
            }
            catch (AccountNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidAmountException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw(WithdrawDto withdrawDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _accountService.WithdrawAsync(withdrawDto);
                return NoContent();
            }
            catch (AccountNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidAmountException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InsufficientFundsException ex)
            {
                return UnprocessableEntity(new { error = ex.Message });
            }
        }
    }
}