using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MuseumSystem.Application.Dtos;
using MuseumSystem.Application.Dtos.AccountDtos;
using MuseumSystem.Application.Dtos.AuthDtos;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Application.Services;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Entities;

namespace MuseumSystem.Api.Controllers
{
    [Route("api/v1/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IAccountService _accountService;

        public AccountController(ILogger<AccountController> logger, IAccountService accountService)
        {
            _logger = logger;
            _accountService = accountService;

        }

        [HttpPost("{roleId}/{museumId}")]
        public async Task<IActionResult> CreateAccount(string roleId, string museumId, [FromBody] AccountRequest accountRequest)
        {
            try
            {
                var account = await _accountService.CreateAccountAsync(roleId, museumId, accountRequest);
                return Ok(ApiResponse<Account>.OkResponse(account, "Account created successfully", "200"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<Account>.InternalErrorResponse(ex.Message));
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountById(string id)
        {
            try
            {
                var account = await _accountService.GetAccountByIdAsync(id);
                return Ok(ApiResponse<Account>.OkResponse(account, "Get account by id successfully", "200"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<Account>.InternalErrorResponse(ex.Message));
            }

        }
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                var accounts = await _accountService.GetAllAccountsAsync(pageIndex, pageSize);
                return Ok(ApiResponse<BasePaginatedList<Account>>.OkResponse(accounts, "Get all accounts successfully", "200"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<Account>.InternalErrorResponse(ex.Message));
            }
        }   
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            try
            {
                await _accountService.DeleteAccountAsync(id);
                return Ok(ApiResponse<string>.OkResponse(id, "Account deleted successfully", "200"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<Account>.InternalErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(string id, [FromBody] AccountRequest accountRequest)
        {
            try
            {
                var updatedAccount = await _accountService.UpdateAccountAsync(id, accountRequest);
                return Ok(ApiResponse<Account>.OkResponse(updatedAccount, "Account updated successfully", "200"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<Account>.InternalErrorResponse(ex.Message));
            }
        }
    }
}
