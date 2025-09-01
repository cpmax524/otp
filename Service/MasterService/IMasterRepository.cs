using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.MasterService
{
    public interface IMasterRepository
    {
        Task<int> UpdateSystemAutoNo(string formType);
        Task<string> GetAndUpdateRegNo(string formType); // optional, if you use it
    }
}
