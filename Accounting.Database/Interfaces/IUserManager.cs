﻿using Accounting.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Database.Interfaces
{
    public interface IUserManager : IGenericRepository<User, int>
    {
        Task<List<User>> GetAllAsync(int organizationId);
        Task<User> GetAsync(int userId);
        Task<User> GetAsync(string email, bool searchTenants);
        Task<int> UpdatePasswordAsync(int userId, string passwordHash);
    }
}