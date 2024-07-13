using Accounting.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Database.Interfaces
{
    public interface ITagManager : IGenericRepository<Tag, int>
    {
        Task<List<Tag>> GetAllAsync();
        Task<Tag> GetByNameAsync(string name);
    }
}