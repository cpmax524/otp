using Microsoft.EntityFrameworkCore;
using MobileNumLogin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Service.CommonService
{
    internal class CommonService
    {
        private readonly ChristellDbContext _context;
        public CommonService()
        {
            _context = new ChristellDbContext();
        }

        public async Task<string> GetSysAutoNumber(int digit, string ltr, string formType)
        {
            string num = null;
            string newNum = null;

            num = await SelectSysAutoNo(formType);

            if (digit == 10)
            {
                newNum = SetAutoNumber10digit(num, ltr);
            }
            else if (digit == 5)
            {
                newNum = SetAutoNumber5digit(num, ltr);
            }
            else if (digit == 4)
            {
                newNum = SetAutoNumber5digit(num, ltr);
            }

            return newNum;
        }

        public async Task<string> GetSysAutoNumber(int digit, string formType)
        {
            string num = null;
            string newNum = null;

            num = await SelectSysAutoNo(formType);

            if (digit == 2)
            {
                newNum = SetAutoNumber2digit(num);
            }
            else if (digit == 3)
            {
                newNum = SetAutoNumber3digit(num);
            }
            else if (digit == 4)
            {
                newNum = SetAutoNumber4digit(num);
            }
            else if (digit == 5)
            {
                newNum = SetAutoNumber5digit(num);
            }
            else if (digit == 10)
            {
                newNum = SetAutoNumber10digit(num);
            }

            return newNum;
        }

        public async Task<string> SelectSysAutoNo(string formType)
        {
            var autoNoEntry = await _context.SystemAutoNos.FirstOrDefaultAsync(m => m.FormType == formType);

            if (autoNoEntry == null)
                return "";

            // Increment the number
            return autoNoEntry.AutoNum.ToString().Trim();
        }


        public string SetAutoNumber2digit(string num)
        {
            string rtnNumber;
            int oldnum = int.Parse(num);
            int newNum = oldnum + 1;
            string number = newNum.ToString();
            char[] numArray = number.ToCharArray();

            if (numArray.Length == 1)
            {
                rtnNumber = "0" + number;
            }
            else
            {
                rtnNumber = number;
            }
            return rtnNumber;
        }

        //For 3 digits code
        public string SetAutoNumber3digit(string num)
        {
            string rtnNumber;
            int oldnum = int.Parse(num);
            int newNum = oldnum + 1;
            string number = newNum.ToString();
            char[] numArray = number.ToCharArray();

            if (numArray.Length == 1)
            {
                rtnNumber = "00" + number;
            }
            else if (numArray.Length == 2)
            {
                rtnNumber = "0" + number;
            }
            else
            {
                rtnNumber = number;
            }
            return rtnNumber;
        }

        //For 4 digits code
        public string SetAutoNumber4digit(string num)
        {
            string rtnNumber;
            int oldnum = int.Parse(num);
            int newNum = oldnum + 1;
            string number = newNum.ToString();
            char[] numArray = number.ToCharArray();

            if (numArray.Length == 1)
            {
                rtnNumber = "000" + number;
            }
            else if (numArray.Length == 2)
            {
                rtnNumber = "00" + number;
            }
            else if (numArray.Length == 3)
            {
                rtnNumber = "0" + number;
            }
            else
            {
                rtnNumber = number;
            }
            return rtnNumber;
        }

        //For 5 digits code
        public string SetAutoNumber5digit(string num)
        {
            string rtnNumber;
            int oldnum = int.Parse(num);
            int newNum = oldnum + 1;
            string number = newNum.ToString();
            char[] numArray = number.ToCharArray();

            if (numArray.Length == 1)
            {
                rtnNumber = "0000" + number;
            }
            else if (numArray.Length == 2)
            {
                rtnNumber = "000" + number;
            }
            else if (numArray.Length == 3)
            {
                rtnNumber = "00" + number;
            }
            else if (numArray.Length == 4)
            {
                rtnNumber = "0" + number;
            }
            else
            {
                rtnNumber = number;
            }
            return rtnNumber;
        }

        //For 4 digits code with letter
        public string SetAutoNumber5digit(string num, string ltr)
        {
            string rtnNumber = null;
            int oldnum = int.Parse(num);
            int newNum = oldnum + 1;
            string number = newNum.ToString();
            char[] numArray = number.ToCharArray();

            if (numArray.Length == 1)
            {
                rtnNumber = ltr + "000" + number;
            }
            else if (numArray.Length == 2)
            {
                rtnNumber = ltr + "00" + number;
            }
            else if (numArray.Length == 3)
            {
                rtnNumber = ltr + "0" + number;
            }
            else if (numArray.Length == 4)
            {
                rtnNumber = ltr + number;
            }
            return rtnNumber;
        }


        //For 10 digits code
        public string SetAutoNumber10digit(string num, string letr)
        {
            string rtnNumber;
            int oldnum = int.Parse(num);
            int newNum = oldnum + 1;
            string number = newNum.ToString();
            char[] numArray = number.ToCharArray();

            if (numArray.Length == 1)
            {
                rtnNumber = letr + "0000000" + number;
            }
            else if (numArray.Length == 2)
            {
                rtnNumber = letr + "000000" + number;
            }
            else if (numArray.Length == 3)
            {
                rtnNumber = letr + "00000" + number;
            }
            else if (numArray.Length == 4)
            {
                rtnNumber = letr + "0000" + number;
            }
            else if (numArray.Length == 5)
            {
                rtnNumber = letr + "000" + number;
            }
            else if (numArray.Length == 6)
            {
                rtnNumber = letr + "00" + number;
            }
            else if (numArray.Length == 7)
            {
                rtnNumber = letr + "0" + number;
            }
            else
            {
                rtnNumber = letr + number;
            }

            return rtnNumber;
        }

        public string SetAutoNumber10digit(string num)
        {
            string rtnNumber;
            int oldnum = int.Parse(num);
            int newNum = oldnum + 1;
            string number = newNum.ToString();
            char[] numArray = number.ToCharArray();

            if (numArray.Length == 1)
            {
                rtnNumber = "0000000" + number;
            }
            else if (numArray.Length == 2)
            {
                rtnNumber = "000000" + number;
            }
            else if (numArray.Length == 3)
            {
                rtnNumber = "00000" + number;
            }
            else if (numArray.Length == 4)
            {
                rtnNumber = "0000" + number;
            }
            else if (numArray.Length == 5)
            {
                rtnNumber = "000" + number;
            }
            else if (numArray.Length == 6)
            {
                rtnNumber = "00" + number;
            }
            else if (numArray.Length == 7)
            {
                rtnNumber = "0" + number;
            }
            else
            {
                rtnNumber = number;
            }

            return rtnNumber;
        }

        //For 10 digits code
        public string SetAutoNumber10digit2(string num, string letr)
        {
            string rtnNumber;
            int oldnum = int.Parse(num);
            int newNum = oldnum + 1;
            string number = newNum.ToString();
            char[] numArray = number.ToCharArray();

            if (numArray.Length == 1)
            {
                rtnNumber = letr + "00000000" + number;
            }
            else if (numArray.Length == 2)
            {
                rtnNumber = letr + "0000000" + number;
            }
            else if (numArray.Length == 3)
            {
                rtnNumber = letr + "000000" + number;
            }
            else if (numArray.Length == 4)
            {
                rtnNumber = letr + "00000" + number;
            }
            else if (numArray.Length == 5)
            {
                rtnNumber = letr + "0000" + number;
            }
            else if (numArray.Length == 6)
            {
                rtnNumber = letr + "000" + number;
            }
            else if (numArray.Length == 7)
            {
                rtnNumber = letr + "00" + number;
            }
            else if (numArray.Length == 8)
            {
                rtnNumber = letr + "0" + number;
            }
            else
            {
                rtnNumber = letr + number;
            }

            return rtnNumber;
        }
    }
}
