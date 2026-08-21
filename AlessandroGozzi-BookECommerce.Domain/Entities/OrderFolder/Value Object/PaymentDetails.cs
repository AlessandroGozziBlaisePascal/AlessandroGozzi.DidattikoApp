using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi_BookECommerce.Domain.Entities.CreditCardFolder;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.OrderFolder.Value_Object
{
    public enum PaymentMethod { CreditCard, Wallet, Hybrid}
    public record PaymentDetails
    {
        public PaymentMethod PaymentMethod { get; init; }
        public string Description { get; init; }

        private PaymentDetails(PaymentMethod paymentMethod,  string description)
        {
            PaymentMethod = paymentMethod;
            Description = description;
        }

        public static PaymentDetails FromCreditCard(string displayname) => new PaymentDetails(PaymentMethod.CreditCard, $"Paid from card: {displayname}");
        public static PaymentDetails FromWallet() => new PaymentDetails(PaymentMethod.Wallet, "Paid from internal wallet");
        public static PaymentDetails FromHybridPayment(string displayname) => new PaymentDetails(PaymentMethod.Hybrid, $"Paid with wallet and card; {displayname}");
    }
}
