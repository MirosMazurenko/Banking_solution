using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Exceptions;
using API.Repositories.Interfaces;
using API.Validators;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly CreateAccountDtoValidator _createAccountValidator;
        private readonly DepositDtoValidator _depositValidator;
        private readonly TransferFundsDtoValidator _transferFundsValidator;
        private readonly WithdrawDtoValidator _withdrawValidator;

        public AccountController(
            IAccountService accountService,
            CreateAccountDtoValidator createAccountValidator,
            DepositDtoValidator depositValidator,
            TransferFundsDtoValidator transferFundsValidator,
            WithdrawDtoValidator withdrawValidator)
        {
            _accountService = accountService;
            _createAccountValidator = createAccountValidator;
            _depositValidator = depositValidator;
            _transferFundsValidator = transferFundsValidator;
            _withdrawValidator = withdrawValidator;
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
            var validationResult = await _createAccountValidator.ValidateAsync(createAccountDto);
            if (validationResult.IsValid == false)
                return BadRequest(validationResult.Errors);

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
            var validationResult = await _transferFundsValidator.ValidateAsync(transferFundsDto);
            if (validationResult.IsValid == false)
                return BadRequest(validationResult.Errors);

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
            var validationResult = await _depositValidator.ValidateAsync(depositDto);
            if (validationResult.IsValid == false)
                return BadRequest(validationResult.Errors);

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
            var validationResult = await _withdrawValidator.ValidateAsync(withdrawDto);
            if (validationResult.IsValid == false)
                return BadRequest(validationResult.Errors);

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