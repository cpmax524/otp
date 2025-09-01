using Microsoft.EntityFrameworkCore;
using MobileNumLogin.Models;
using System;
using System.Threading.Tasks;

namespace Service.MasterService
{
    public class MasterService : IMasterRepository // ✅ Now implements the interface
    {
        private readonly ChristellDbContext _dbContext;

        public MasterService(ChristellDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> UpdateSystemAutoNo(string formType)
        {
            try
            {
                var existing = await _dbContext.SystemAutoNos.FirstOrDefaultAsync(c => c.FormType == formType);

                if (existing == null)
                    return 0;

                existing.AutoNum += 1;
                existing.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);
                existing.LastModifiedTime = TimeOnly.FromDateTime(DateTime.Now);

                await _dbContext.SaveChangesAsync();
                return existing.AutoNum;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        // Optional: Only add this if you plan to use RegNo generation
        public async Task<string> GetAndUpdateRegNo(string formType)
        {
            var existing = await _dbContext.SystemAutoNos.FirstOrDefaultAsync(c => c.FormType == formType);

            if (existing == null)
                throw new Exception($"Form type '{formType}' not found.");

            int currentNo = existing.AutoNum;
            string regNo = $"{formType}{currentNo.ToString().PadLeft(5, '0')}";

            existing.AutoNum += 1;
            existing.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);
            existing.LastModifiedTime = TimeOnly.FromDateTime(DateTime.Now);

            await _dbContext.SaveChangesAsync();

            return regNo;
        }
    }
}
