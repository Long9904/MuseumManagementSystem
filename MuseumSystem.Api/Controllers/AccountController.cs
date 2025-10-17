using Microsoft.AspNetCore.Mvc;
using MuseumSystem.Application.Dtos;
using MuseumSystem.Application.Dtos.AccountDtos;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Abstractions;
using Swashbuckle.AspNetCore.Annotations;

namespace MuseumSystem.Api.Controllers
{
    [Route("api/v1/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;

        }

        [HttpPost("{roleId}/{museumId}")]
        [SwaggerOperation(
            Summary = "Create a new account",
            Description = "Creates a new account with the provided details.")]

        public async Task<IActionResult> CreateAccount(string roleId, string museumId, [FromBody] AccountRequest accountRequest)
        {

            var account = await _accountService.CreateAccountAsync(roleId, museumId, accountRequest);
            return Ok(ApiResponse<AccountRespone>.OkResponse(account, "Account created successfully", "200"));

        }
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Get an account by ID",
            Description = "Retrieves the details of a specific account identified by its ID.")]
        public async Task<IActionResult> GetAccountById(string id)
        {

            var account = await _accountService.GetAccountByIdAsync(id);
            return Ok(ApiResponse<AccountRespone>.OkResponse(account, "Get account by id successfully", "200"));


        }
        [HttpGet]
        [SwaggerOperation(
            Summary = "Get all accounts",
            Description = "Retrieves a paginated list of all accounts.")]
        public async Task<IActionResult> GetAll(int pageIndex = 1, int pageSize = 10)
        {

            var accounts = await _accountService.GetAllAccountsAsync(pageIndex, pageSize);
            return Ok(ApiResponse<BasePaginatedList<AccountRespone>>.OkResponse(accounts, "Get all accounts successfully", "200"));

        }
        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Delete an account",
            Description = "Deletes an existing account identified by its ID.")]
        public async Task<IActionResult> DeleteAccount(string id)
        {

            await _accountService.DeleteAccountAsync(id);
            return Ok(ApiResponse<string>.OkResponse(id, "Account deleted successfully", "200"));

        }

        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Update an existing account",
            Description = "Updates the details of an existing account identified by its ID.")]
        public async Task<IActionResult> UpdateAccount(string id, [FromBody] AccountRequest accountRequest)
        {
            var updatedAccount = await _accountService.UpdateAccountAsync(id, accountRequest);
            return Ok(ApiResponse<AccountRespone>.OkResponse(updatedAccount, "Account updated successfully", "200"));

        }
    }
}
