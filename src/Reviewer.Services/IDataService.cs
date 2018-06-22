using System;
using Reviewer.SharedModels;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Reviewer.Services
{
    public interface IDataService
    {
        Task<List<Business>> GetBusinesses();
        Task InsertBusiness(Business business);
        Task UpdateBusiness(Business business);
    }
}
