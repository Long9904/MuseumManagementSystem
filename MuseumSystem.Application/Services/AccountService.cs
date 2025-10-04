using Microsoft.Extensions.Logging;
using MuseumSystem.Application.Dtos.AccountDtos;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unit;
        private readonly ILogger<AccountService> _logger;
        public AccountService(IUnitOfWork unit, ILogger<AccountService> logger)
        {
            _unit = unit;
            _logger = logger;
        }
        public Task<Account> CreateAccountAsync(AccountRequest account)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAccountAsync(string id)
        {
            var account = await GetAccountByIdAsync(id);
            await _unit.GetRepository<Account>().DeleteAsync(id);
        }

        public async Task<BasePaginatedList<Account>> GetAllAccountsAsync(int pageIndex, int pageSize)
        {
            var query = _unit.GetRepository<Account>().Entity;
            return await _unit.GetRepository<Account>().GetPagging(query, pageIndex, pageSize);
        }

        public async Task<Account?> GetAccountByIdAsync(string id)
        {
            var account = await _unit.GetRepository<Account>().FindAsync(x => x.Id == id);
            if(account == null)
            {
                _logger.LogWarning("Account with ID {AccountId} not found.", id);
                throw new KeyNotFoundException($"Account with ID {id} not found.");
            }
            return account;
        }

        public async Task<Account?> GetAccountByEmailAsync(string email)
        {
            var account = await _unit.GetRepository<Account>().FindAsync(x => x.Email == email);
            if (account == null)
            {
                _logger.LogWarning("Account with email {AccountEmail} not found.", email);
                throw new KeyNotFoundException($"Account with email {email} not found.");
            }
            return account;
        }

        public Task<Account> UpdateAccountAsync(AccountRequest account)
        {
            throw new NotImplementedException();
        }

        
    }
}
