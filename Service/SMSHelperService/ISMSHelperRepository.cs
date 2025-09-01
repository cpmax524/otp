using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.SMSHelperService
{
    public interface ISMSHelperRepository
    {
        public Task<(bool IsSuccess, string ServerRef)> SendSMSAsync(string campaignName, int customerId, string message, string phoneNumber);
        public Task<bool> SendOTPAsync(int customerId, string phoneNumber, string otp);
    }
}
