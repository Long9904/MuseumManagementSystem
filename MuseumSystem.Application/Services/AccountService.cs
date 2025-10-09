using Microsoft.Extensions.Logging;
using MuseumSystem.Application.Dtos.AccountDtos;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Entities;
using MuseumSystem.Domain.Enums;
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
        public async Task<Account> CreateAccountAsync(string roleId, string museumId, AccountRequest account)
        {
            if (account == null)
            {
                _logger.LogError("Account request cannot be null.");
                throw new ArgumentNullException(nameof(account), "Account request cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(account.Email))
            {
                _logger.LogError("Email cannot be null or empty.");
                throw new ArgumentException("Email cannot be null or empty.", nameof(account.Email));
            }
            if (string.IsNullOrWhiteSpace(account.Password))
            {
                _logger.LogError("Password cannot be null or empty.");
                throw new ArgumentException("Password cannot be null or empty.", nameof(account.Password));
            }
            var role = await _unit.GetRepository<Role>().FindAsync(x => x.Id == roleId);
            if (role == null)
            {
                _logger.LogError("Role with ID {RoleId} not found.", roleId);
                throw new KeyNotFoundException($"Role with ID {roleId} not found.");
            }
            var museum = await _unit.GetRepository<Museum>().FindAsync(x => x.Id == museumId);
            if (museum == null)
            {
                _logger.LogError("Museum with ID {MuseumId} not found.", museumId);
                throw new KeyNotFoundException($"Museum with ID {museumId} not found.");
            }
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(account.Password);
            var newAccount = new Account
            {
                Email = account.Email,
                Password = hashedPassword,
                FullName = account.FullName,
                Status = EnumStatus.Active,
                CreateAt = DateTime.UtcNow,
                RoleId = role.Id,
                MuseumId = museum.Id
            };
            await _unit.GetRepository<Account>().InsertAsync(newAccount);
            await _unit.SaveChangeAsync();
            _logger.LogInformation("Account with email {AccountEmail} created successfully.", account.Email);
            return newAccount;
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
            var account = await _unit.GetRepository<Account>().FindAsync(x => x.Id == id && x.Status == EnumStatus.Active);
            if (account == null)
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

        public async Task<Account> UpdateAccountAsync(string accountId, AccountRequest account)
        {
            bool isUpdate = false;
            if (account == null)
            {
                _logger.LogError("Account request cannot be null.");
                throw new ArgumentNullException(nameof(account), "Account request cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(account.Email))
            {
                _logger.LogError("Email cannot be null or empty.");
                throw new ArgumentException("Email cannot be null or empty.", nameof(account.Email));
            }
            if (string.IsNullOrWhiteSpace(account.Password))
            {
                _logger.LogError("Password cannot be null or empty.");
                throw new ArgumentException("Password cannot be null or empty.", nameof(account.Password));
            }
            if (string.IsNullOrWhiteSpace(accountId))
            {
                _logger.LogError("Account ID cannot be null or empty.");
                throw new ArgumentException("Account ID cannot be null or empty.", nameof(accountId));
            }
            var existingAccount = await _unit.GetRepository<Account>().FindAsync(x => x.Id == accountId);
            if (existingAccount == null)
            {
                _logger.LogError("Account with ID {AccountId} not found.", accountId);
                throw new KeyNotFoundException($"Account with ID {accountId} not found.");
            }
            if (existingAccount.Email != account.Email)
            {
                existingAccount.Email = account.Email;
                isUpdate = true;
            }
            if (existingAccount.Password != account.Password)
            {
                existingAccount.Password = account.Password;
                isUpdate = true;
            }
            if (existingAccount.FullName != account.FullName)
            {
                existingAccount.FullName = account.FullName;
                isUpdate = true;
            }
            if (isUpdate)
            {
                existingAccount.UpdateAt = DateTime.UtcNow;
                await _unit.GetRepository<Account>().UpdateAsync(existingAccount);
                await _unit.SaveChangeAsync();
                _logger.LogInformation("Account with ID {AccountId} updated successfully.", accountId);
                return existingAccount;
            }
            else
            {
                _logger.LogInformation("No changes detected for account with ID {AccountId}.", accountId);
                throw new InvalidOperationException($"No changes detected for account with ID {accountId}.");
            }


        }


    }
}
