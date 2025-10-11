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
        Task<BasePaginatedList<AccountRespone>> GetAllAccountsAsync(int pageIndex, int pageSize);
        Task<AccountRespone?> GetAccountByIdAsync(string id);
        Task<AccountRespone?> GetAccountByEmailAsync(string email);
        Task<AccountRespone> CreateAccountAsync(string roleId, string museumId,AccountRequest account);
        Task<AccountRespone> UpdateAccountAsync(string accountId, AccountRequest account);
        Task DeleteAccountAsync(string id);
      

    }
}
