using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.CommonService
{
    public interface ICommonRepository
    {
        public Task<string> GetSysAutoNumber(int digit, string ltr, string formType);
        public Task<string> GetSysAutoNumber(int digit, string formType);
    }
}
