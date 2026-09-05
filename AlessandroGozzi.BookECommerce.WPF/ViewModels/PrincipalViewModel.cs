using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AlessandroGozzi.BookECommerce.Application.Commands.Auth.AddCreditCard;
using AlessandroGozzi.BookECommerce.Application.Commands.Auth.RemoveCreditCard;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto;
using AlessandroGozzi.BookECommerce.WPF.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Stripe;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AlessandroGozzi.BookECommerce.WPF.ViewModels
{
    public partial class PrincipalViewModel : ObservableObject
    {
        private readonly ISender _mediator;
        private readonly Services.CustomerSession _customerSession;

        [ObservableProperty] private string _cardOwner = string.Empty;
        [ObservableProperty] private string _cardNumber1 = string.Empty;
        [ObservableProperty] private string _cardNumber2 = string.Empty;
        [ObservableProperty] private string _cardNumber3 = string.Empty;
        [ObservableProperty] private string _cardNumber4 = string.Empty;
        [ObservableProperty] private string _expiryMonth = string.Empty;
        [ObservableProperty] private string _expiryYear = string.Empty;

        // Gestione dello stato della carta tramite la proprietà automatica
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CardActionButtonText))]
        [NotifyPropertyChangedFor(nameof(IsCardEditable))]
        private bool _hasCard;

        public string CardActionButtonText => HasCard ? "Rimuovi" : "+ Aggiungi";
        public bool IsCardEditable => !HasCard;

        public string CustomerFullName
        {
            get
            {
                var customer = _customerSession.CurrentCustomer;
                if (customer == null) return "OSPITE";
                return $"{customer.FullName}";
            }
        }

        public IAsyncRelayCommand CardActionCommand { get; }

        public PrincipalViewModel(ISender mediator, Services.CustomerSession custSession)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _customerSession = custSession ?? throw new ArgumentNullException(nameof(custSession));

            CardActionCommand = new AsyncRelayCommand(ExecuteCardActionAsync);
            RefreshCardState();
        }

        private void RefreshCardState()
        {
            var card = _customerSession.CurrentCustomer?.CreditCard;

            // Assegna alla proprietà generata HasCard (H Maiuscola)
            HasCard = card != null;

            if (HasCard && card != null)
            {
                CardOwner = card.Owner ?? string.Empty;
                CardNumber1 = "****";
                CardNumber2 = "****";
                CardNumber3 = "****";
                CardNumber4 = card.Last4Digits ?? string.Empty;

                var expiryParts = card.ExpiryDate?.Split('/');
                ExpiryMonth = expiryParts?.Length > 0 ? expiryParts[0] : string.Empty;
                ExpiryYear = expiryParts?.Length > 1 ? expiryParts[1] : string.Empty;
            }
            else
            {
                CardOwner = string.Empty;
                CardNumber1 = string.Empty;
                CardNumber2 = string.Empty;
                CardNumber3 = string.Empty;
                CardNumber4 = string.Empty;
                ExpiryMonth = string.Empty;
                ExpiryYear = string.Empty;
            }

            OnPropertyChanged(nameof(CustomerFullName));
        }

        private async Task ExecuteCardActionAsync()
        {
            var customer = _customerSession.CurrentCustomer;
            if (customer == null) return;

            if (HasCard)
            {
                var result = await _mediator.Send(new RemoveCreditCardCommand(customer.CustomerId));
                if (result?.IsSuccess == true)
                {
                    _customerSession.UpdateCustomerCreditCard(null);
                    RefreshCardState();
                }
            }
            else
            {
                var fullCardNumber = $"{CardNumber1.Trim()}{CardNumber2.Trim()}{CardNumber3.Trim()}{CardNumber4.Trim()}";
                var expiryDate = $"{ExpiryMonth.Trim()}/{ExpiryYear.Trim()}";

                var ownerParts = CardOwner.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                var name = ownerParts.Length > 0 ? ownerParts[0] : string.Empty;
                var surname = ownerParts.Length > 1 ? ownerParts[1] : string.Empty;

                var command = new AddCreditCardCommand(
                    customer.CustomerId,
                    fullCardNumber,
                    name,
                    surname,
                    expiryDate
                );

                var result = await _mediator.Send(command);
                if (result?.IsSuccess == true)
                {
                    var last4Digits = fullCardNumber.Length >= 4
                        ? fullCardNumber.Substring(fullCardNumber.Length - 4)
                        : fullCardNumber;

                    var newCard = new CreditCardDto(CardOwner, last4Digits, expiryDate);
                    _customerSession.UpdateCustomerCreditCard(newCard);
                    RefreshCardState();
                }
            }
        }
    }
}
