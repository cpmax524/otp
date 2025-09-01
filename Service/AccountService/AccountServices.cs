using Microsoft.EntityFrameworkCore;
using MobileNumLogin.Models;
using Service.MasterService;
using System;
using System.Threading.Tasks;

namespace Service.AccountService
{
    public class AccountServices : IAccountRepository
    {
        private readonly ChristellDbContext _context;
        private readonly IMasterRepository _masterRepository;

        public AccountServices(ChristellDbContext context, IMasterRepository masterRepository)
        {
            _context = context;
            _masterRepository = masterRepository;
        }

        public async Task<AcrmcustomerInfo?> LoginAsync(string mobileNo)
        {
            return await _context.Set<AcrmcustomerInfo>()
                .FirstOrDefaultAsync(c => c.MobileNo == mobileNo && (c.IsDeleted == null || c.IsDeleted == false));
        }

        public async Task<(bool Success, string Message)> RegisterAsync(AcrmcustomerInfo customer)
        {
            if (await IsMobileExistsAsync(customer.MobileNo!))
            {
                return (false, "Mobile number already registered.");
            }

            customer.RegDate = DateOnly.FromDateTime(DateTime.Now);
            customer.SysDate = DateOnly.FromDateTime(DateTime.Now);
            customer.EnteredDatetime = DateTime.Now;
            customer.IsDeleted = false;

            int newAutoNo = await _masterRepository.UpdateSystemAutoNo("CS");

            customer.RegNo = $"CS{newAutoNo:D8}"; // Format to SC00000001, SC00000002, etc.

            await _context.Set<AcrmcustomerInfo>().AddAsync(customer);
            await _context.SaveChangesAsync();

            return (true, "Registration successful.");
        }

        public async Task<bool> IsMobileExistsAsync(string mobileNo)
        {
            return await _context.Set<AcrmcustomerInfo>()
                .AnyAsync(c => c.MobileNo == mobileNo && (c.IsDeleted == null || c.IsDeleted == false));
        }
    }
}
