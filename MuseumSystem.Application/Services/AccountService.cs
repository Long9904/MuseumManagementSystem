using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MuseumSystem.Application.Dtos;
using MuseumSystem.Application.Dtos.AccountDtos;
using MuseumSystem.Application.Exceptions;
using MuseumSystem.Application.Interfaces;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Entities;
using MuseumSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unit;
        private readonly ILogger<AccountService> _logger;
        private readonly IMapper _mapping;
        public AccountService(IUnitOfWork unit, ILogger<AccountService> logger, IMapper mapping)
        {
            _unit = unit;
            _logger = logger;
            _mapping = mapping;
        }
        public async Task<AccountRespone> CreateAccountAsync(string roleId, string museumId, AccountRequest account)
        {
            if (account == null)
            {
                throw new ArgumentNullException(nameof(account), "Account cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(account.Email))
            {
                throw new ArgumentException("Email cannot be null or empty.", nameof(account.Email));
            }
            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(account.Email, pattern))
            {
                throw new ArgumentException("Invalid email format.", nameof(account.Email));
            }
            if (string.IsNullOrWhiteSpace(account.Password))
            {
                throw new ArgumentException("Password cannot be null or empty.", nameof(account.Password));
            }
            var existingAccount = await _unit.GetRepository<Account>().FindAsync(x => x.Email == account.Email);
            if (existingAccount != null)
            {
                throw new InvalidOperationException($"An account with email {account.Email} already exists.");
            }
            var role = await _unit.GetRepository<Role>().FindAsync(x => x.Id == roleId && x.Status != EnumStatus.Inactive);
            if (role == null)
            {
                throw new KeyNotFoundException($"Role not found.");
            }

            if (role.Name.Equals("SuperAdmin"))
            {
                throw new InvalidAccessException("You can not create account with SuperAdmin role");
            }

            var museum = await _unit.GetRepository<Museum>().FindAsync(x => x.Id == museumId);
            if (museum == null)
            {
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
            return _mapping.Map<AccountRespone>(newAccount);
        }

        public async Task DeleteAccountAsync(string id)
        {
            var account = await _unit.GetRepository<Account>().FindAsync(x => x.Id == id && x.Status == EnumStatus.Active,
                include: x => x.Include(x => x.Role).Include(x => x.Museum));
            if (account == null)
            {
                throw new KeyNotFoundException($"Account with ID {id} not found.");
            }
            if (account.Status == EnumStatus.Inactive)
            {
                account.Status = EnumStatus.Active;
            }
            else
            {
                account.Status = EnumStatus.Inactive;
            }
            await _unit.GetRepository<Account>().UpdateAsync(_mapping.Map<Account>(account));
            await _unit.SaveChangeAsync();
            _logger.LogInformation("Account with ID {AccountId} deleted successfully.", id);
        }

        public async Task<BasePaginatedList<AccountRespone>> GetAllAccountsAsync(int pageIndex, int pageSize)
        {
            var query = _unit.GetRepository<Account>().Entity.Include(x => x.Role).Include(x => x.Museum);
            var rs = await _unit.GetRepository<Account>().GetPagging(query, pageIndex, pageSize);
            return _mapping.Map<BasePaginatedList<AccountRespone>>(rs);
        }

        public async Task<AccountRespone?> GetAccountByIdAsync(string id)
        {
            var account = await _unit.GetRepository<Account>().FindAsync(x => x.Id == id && x.Status == EnumStatus.Active,
                include: x => x.Include(x => x.Role).Include(x => x.Museum));
            if (account == null)
            {
                _logger.LogWarning("Account not found.");
                throw new KeyNotFoundException($"Account with ID {id} not found.");
            }
            return _mapping.Map<AccountRespone>(account);
        }

        public async Task<AccountRespone?> GetAccountByEmailAsync(string email)
        {
            var account = await _unit.GetRepository<Account>().FindAsync(x => x.Email == email,
                include: x => x.Include(x => x.Role).Include(x => x.Museum));
            if (account == null)
            {
                _logger.LogWarning("Account with email {AccountEmail} not found.", email);
                throw new KeyNotFoundException($"Account with email {email} not found.");
            }
            return _mapping.Map<AccountRespone>(account);
        }

        public async Task<AccountRespone> UpdateAccountAsync(string accountId, AccountRequest account)
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
            var accountExisting = await _unit.GetRepository<Account>().FindAsync(x => x.Email == account.Email && x.Id != accountId);
           
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
            var existingAccount = await _unit.GetRepository<Account>().FindAsync(x => x.Id == accountId,
                include: x => x.Include(x => x.Role).Include(x => x.Museum));
            if (existingAccount == null)
            {
                _logger.LogError("Account with ID {AccountId} not found.", accountId);
                throw new KeyNotFoundException($"Account with ID {accountId} not found.");
            }
            if (existingAccount.Email != account.Email && accountExisting == null)
            {
                existingAccount.Email = account.Email;
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
                return _mapping.Map<AccountRespone>(existingAccount);
            }
            else
            {
                _logger.LogInformation("No changes detected for account with ID {AccountId}.", accountId);
                throw new InvalidOperationException($"No changes detected for account with ID {accountId}.");
            }


        }


    }
}
