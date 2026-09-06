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
using AlessandroGozzi.BookECommerce.Application.Dto.Auth;
using AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto;
using AlessandroGozzi.BookECommerce.Application.Mappers.VO_Mappers;
using AlessandroGozzi.BookECommerce.WPF.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Stripe;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AlessandroGozzi.BookECommerce.WPF.ViewModels
{
    public partial class PrincipalViewModel : INotifyPropertyChanged
    {
        private readonly IMediator _mediator;
        private readonly Guid _currentUserId;
        private readonly Services.CustomerSession _session;

        private string _customerFullName = string.Empty;
        public string CustomerFullName
        {
            get => _customerFullName;
            set { _customerFullName = value; OnPropertyChanged(); }
        }

        private string _cardHolderName = string.Empty;
        public string CardHolderName
        {
            get => _cardHolderName;
            set { _cardHolderName = value; OnPropertyChanged(); }
        }

        private string _cardHolderSurname = string.Empty;
        public string CardHolderSurname
        {
            get => _cardHolderSurname;
            set { _cardHolderSurname = value; OnPropertyChanged(); }
        }

        private string _cardNumber1 = string.Empty;
        public string CardNumber1
        {
            get => _cardNumber1;
            set { _cardNumber1 = value; OnPropertyChanged(); }
        }

        private string _cardNumber2 = string.Empty;
        public string CardNumber2
        {
            get => _cardNumber2;
            set { _cardNumber2 = value; OnPropertyChanged(); }
        }

        private string _cardNumber3 = string.Empty;
        public string CardNumber3
        {
            get => _cardNumber3;
            set { _cardNumber3 = value; OnPropertyChanged(); }
        }

        private string _cardNumber4 = string.Empty;
        public string CardNumber4
        {
            get => _cardNumber4;
            set { _cardNumber4 = value; OnPropertyChanged(); }
        }

        private string _expiryMonth = string.Empty;
        public string ExpiryMonth
        {
            get => _expiryMonth;
            set { _expiryMonth = value; OnPropertyChanged(); }
        }

        private string _expiryYear = string.Empty;
        public string ExpiryYear
        {
            get => _expiryYear;
            set { _expiryYear = value; OnPropertyChanged(); }
        }

        private bool _hasCard;
        public bool HasCard
        {
            get => _hasCard;
            set
            {
                _hasCard = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsCardEditable));
                OnPropertyChanged(nameof(CardActionButtonText));
                OnPropertyChanged(nameof(CardHeaderTitle));
            }
        }

        public bool IsCardEditable => !HasCard;

        public string CardHeaderTitle => HasCard ? "La tua Carta" : "Aggiungi Carta";

        public string CardActionButtonText => HasCard ? "Rimuovi Carta" : "Aggiungi Carta";

        public ICommand CardActionCommand { get; }

        public PrincipalViewModel(IMediator mediator, Services.CustomerSession session)
        {
            _session = session;
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

            if (session?.CurrentCustomer != null)
            {
                _currentUserId = session.CurrentCustomer.CustomerId;
                CustomerFullName = $"{session.CurrentCustomer.FullName}".Trim();

                if (session.CurrentCustomer.CreditCard is { } creditCard)
                {
                    HasCard = true;

                    CardHolderName = creditCard.OwnerName ?? string.Empty;
                    CardHolderSurname = creditCard.OwnerSurname ?? string.Empty;

                    CardNumber1 = "****";
                    CardNumber2 = "****";
                    CardNumber3 = "****";
                    CardNumber4 = creditCard.Last4Digits ?? "****";

                    if (!string.IsNullOrEmpty(creditCard.ExpiryDate) && creditCard.ExpiryDate.Contains('/'))
                    {
                        var parts = creditCard.ExpiryDate.Split('/');
                        ExpiryMonth = parts[0];
                        ExpiryYear = parts.Length > 1 ? parts[1] : string.Empty;
                    }
                }
                else
                {
                    CardHolderName = string.Empty;
                    CardHolderSurname = string.Empty;
                }
            }

            CardActionCommand = new RelayCommand(ExecuteCardAction);
        }

        private async void ExecuteCardAction()
        {
            if (HasCard)
            {
                var command = new RemoveCreditCardCommand(_currentUserId);
                var result = await _mediator.Send(command);

                if (result.IsSuccess)
                {
                    HasCard = false;
                    _session.UpdateCreditCard(null);
                    ClearCardFields();
                }
            }
            else
            {
                // Aggiunta Carta
                string fullCardNumber = $"{CardNumber1}{CardNumber2}{CardNumber3}{CardNumber4}";
                string expiryDate = $"{ExpiryMonth}/{ExpiryYear}";

                var command = new AddCreditCardCommand(
                    CustomerId: _currentUserId,
                    CardNumber: fullCardNumber,
                    CardHolderName: CardHolderName,
                    CardHolderSurname: CardHolderSurname,
                    ExpiryDate: expiryDate
                );

                var result = await _mediator.Send(command);

                if (result.IsSuccess && result.Value != null)
                {
                    HasCard = true;

                    _session.UpdateCreditCard(result.Value);

                    CardNumber1 = "****";
                    CardNumber2 = "****";
                    CardNumber3 = "****";
                    CardNumber4 = result.Value.Last4Digits;

                    OnPropertyChanged(nameof(CardNumber1));
                    OnPropertyChanged(nameof(CardNumber2));
                    OnPropertyChanged(nameof(CardNumber3));
                    OnPropertyChanged(nameof(CardNumber4));
                }
            }
        }

        private void ClearCardFields()
        {
            CardHolderName = string.Empty;
            CardHolderSurname = string.Empty;
            CardNumber1 = string.Empty;
            CardNumber2 = string.Empty;
            CardNumber3 = string.Empty;
            CardNumber4 = string.Empty;
            ExpiryMonth = string.Empty;
            ExpiryYear = string.Empty;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
