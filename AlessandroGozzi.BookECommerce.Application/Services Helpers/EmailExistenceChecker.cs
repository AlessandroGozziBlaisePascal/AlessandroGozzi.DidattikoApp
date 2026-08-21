using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Value_Object;
using DnsClient;

namespace AlessandroGozzi.BookECommerce.Application
{
    public static class EmailExistenceChecker
    {
        public static async Task<bool> IsValidAndDelivelableAsync(string email)
        {
            var cleanedMailResult = Email.Create(email);
            if(cleanedMailResult.IsFailure)
            {
                return false;
            }

            try
            {
                var lookUp = new LookupClient();
                var result = await lookUp.QueryAsync(cleanedMailResult.Value.Value.Split('@')[1], QueryType.MX);
                return result.Answers.MxRecords().Any();
            }
            catch
            {
                return false;
            }
        }
    }
}
