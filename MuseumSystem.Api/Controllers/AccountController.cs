using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MuseumSystem.Application.Dtos;
using MuseumSystem.Application.Dtos.AccountDtos;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Abstractions;
using Swashbuckle.AspNetCore.Annotations;

namespace MuseumSystem.Api.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    [Route("api/v1/accounts")]
    [ApiController]
    [SwaggerTag("Account Manage - SuperAdmin")]
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
        [SwaggerOperation(Summary = "Create a new account")]
        public async Task<IActionResult> CreateAccount(string roleId, string museumId, [FromBody] AccountRequest accountRequest)
        {

            var account = await _accountService.CreateAccountAsync(roleId, museumId, accountRequest);
            return Ok(ApiResponse<AccountRespone>.OkResponse(account, "Account created successfully", "200"));

        }
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get account by ID")]
        public async Task<IActionResult> GetAccountById(string id)
        {

            var account = await _accountService.GetAccountByIdAsync(id);
            return Ok(ApiResponse<AccountRespone>.OkResponse(account, "Get account by id successfully", "200"));


        }
        [HttpGet]
        [SwaggerOperation(Summary = "Get all accounts")]
        public async Task<IActionResult> GetAll(int pageIndex = 1, int pageSize = 10)
        {

            var accounts = await _accountService.GetAllAccountsAsync(pageIndex, pageSize);
            return Ok(ApiResponse<BasePaginatedList<AccountRespone>>.OkResponse(accounts, "Get all accounts successfully", "200"));

        }
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete account by ID, change status")]
        public async Task<IActionResult> DeleteAccount(string id)
        {

            await _accountService.DeleteAccountAsync(id);
            return Ok(ApiResponse<string>.OkResponse(id, "Account deleted successfully", "200"));

        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update account by ID")]
        public async Task<IActionResult> UpdateAccount(string id, [FromBody] AccountRequest accountRequest)
        {
            var updatedAccount = await _accountService.UpdateAccountAsync(id, accountRequest);
            return Ok(ApiResponse<AccountRespone>.OkResponse(updatedAccount, "Account updated successfully", "200"));

        }
    }
}
