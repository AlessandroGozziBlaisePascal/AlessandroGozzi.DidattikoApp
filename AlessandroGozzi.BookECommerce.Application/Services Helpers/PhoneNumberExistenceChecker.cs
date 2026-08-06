using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhoneNumbers;

namespace AlessandroGozzi.BookECommerce.Application
{
    public static class PhoneNumberExistenceChecker
    {
        public static bool IsValidMobile(string rawPhoneNumber, string defaultCountry = "IT")
        {
            var phoneNumberUtil = PhoneNumberUtil.GetInstance();

            try
            {
                var number = phoneNumberUtil.Parse(rawPhoneNumber, defaultCountry);
                bool isValid = phoneNumberUtil.IsValidNumber(number);
                var type = phoneNumberUtil.GetNumberType(number);

                bool isMobile = type == PhoneNumberType.MOBILE || type == PhoneNumberType.FIXED_LINE_OR_MOBILE;

                return isValid && isMobile;
            }
            catch (NumberParseException)
            {
                return false;
            }
        }
    }
}
