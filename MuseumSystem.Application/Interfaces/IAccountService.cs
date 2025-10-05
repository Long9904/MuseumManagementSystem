using Microsoft.AspNetCore.Identity.Data;
using MuseumSystem.Application.Dtos.AccountDtos;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Interfaces
{
    public interface IAccountService
    {
        Task<BasePaginatedList<Account>> GetAllAccountsAsync(int pageIndex, int pageSize);
        Task<Account?> GetAccountByIdAsync(string id);
        Task<Account?> GetAccountByEmailAsync(string email);
        Task<Account> CreateAccountAsync(string roleId, AccountRequest account);
        Task<Account> UpdateAccountAsync(string accountId, AccountRequest account);
        Task DeleteAccountAsync(string id);
      

    }
}
