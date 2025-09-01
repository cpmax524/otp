using MobileNumLogin.Models;
using System.Threading.Tasks;

namespace Service.AccountService
{
    public interface IAccountRepository
    {
        Task<AcrmcustomerInfo?> LoginAsync(string mobileNo);
        Task<(bool Success, string Message)> RegisterAsync(AcrmcustomerInfo customer);
        Task<bool> IsMobileExistsAsync(string mobileNo);
    }
}
