using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Commands.Auth.AddCreditCard;
using AlessandroGozzi.BookECommerce.MobileGUI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;

namespace AlessandroGozzi.BookECommerce.MobileGUI.ViewModels
{
    public partial class PrincipalViewModel : ObservableObject
    {
        private readonly IMediator _mediator;
        private readonly CustomerSession _customerSession;

        [ObservableProperty]
        private string _customerFullName = "Mario Rossi";

        // Stato del menu a tendina della carta
        [ObservableProperty]
        private bool _isCardSectionExpanded = false;

        [ObservableProperty]
        private string _accordionIcon = "▼";

        // Campi Mock Carta
        [ObservableProperty] private string _cardHolderName = string.Empty;
        [ObservableProperty] private string _cardHolderSurname = string.Empty;
        [ObservableProperty] private string _cardNumberPart1 = string.Empty;
        [ObservableProperty] private string _cardNumberPart2 = string.Empty;
        [ObservableProperty] private string _cardNumberPart3 = string.Empty;
        [ObservableProperty] private string _cardNumberPart4 = string.Empty;
        [ObservableProperty] private string _expiryMonth = string.Empty;
        [ObservableProperty] private string _expiryYear = string.Empty;
        [ObservableProperty] private string _cvv = string.Empty;

        public PrincipalViewModel(IMediator mediator, CustomerSession customerSession)
        {
            _mediator = mediator;
            _customerSession = customerSession;
        }

        [RelayCommand]
        private void ToggleCardSection()
        {
            IsCardSectionExpanded = !IsCardSectionExpanded;
            AccordionIcon = IsCardSectionExpanded ? "▲" : "▼";
        }

        //[RelayCommand]
        //private async Task SaveCreditCardAsync()
        //{
        //    var fullCardNumber = $"{CardNumberPart1}{CardNumberPart2}{CardNumberPart3}{CardNumberPart4}";
        //    var expiry = $"{ExpiryMonth}/{ExpiryYear}";

        //    if (string.IsNullOrWhiteSpace(CardHolderName) || fullCardNumber.Length < 16)
        //    {
        //        await Shell.Current.DisplayAlert("Attenzione", "Inserisci tutti i dettagli della carta.", "OK");
        //        return;
        //    }

        //    var command = new AddCreditCardCommand(
        //        $"{CardHolderName} {CardHolderSurname}",
        //        fullCardNumber,
        //        expiry,
        //        Cvv);

        //    var result = await _mediator.Send(command);

        //    if (result.IsFailure)
        //    {
        //        await Shell.Current.DisplayAlert("Errore Carta", result.Error.Description, "OK");
        //        return;
        //    }

        //    await Shell.Current.DisplayAlert("Sicurezza", "Metodo di pagamento memorizzato con successo.", "OK");
        //    IsCardSectionExpanded = false;
        //    AccordionIcon = "▼";
        //}

        [RelayCommand]
        private async Task GoToBuyBooksAsync() => await Shell.Current.GoToAsync("CatalogPage");

        [RelayCommand]
        private async Task GoToSellBooksAsync() => await Shell.Current.GoToAsync("SellPage");

        [RelayCommand]
        private async Task LogoutAsync()
        {
            _customerSession?.ClearSession();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
