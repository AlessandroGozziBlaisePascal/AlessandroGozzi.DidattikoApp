using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Services_Helpers;
using AlessandroGozzi.BookECommerce.Domain.AggregateRoots.Customers.ValueObjects;
using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace AlessandroGozzi.BookECommerce.Infrastructure.Services
{
    public class SMSSender : ISMSSender
    {
        public Task SendSmsAsync(string to, string message, CancellationToken cancellationToken = default)
        {
            // Pulisce il messaggio per evitare problemi con caratteri speciali nella riga di comando
            string safeMessage = message.Replace("\"", "'");

            // Parametri CMD: /k mantiene la finestra aperta, color 0A imposta il testo verde stile console
            string arguments = $"/k \"color 0A && echo. && echo === SIMULAZIONE SMS RICEVUTO === && echo. && " +
                $"echo A: {to} && echo MESSAGGIO: {safeMessage} && echo. && echo ================================== && echo.\"";

            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = arguments,
                UseShellExecute = true
            });

            return Task.CompletedTask;
        }
    }
}
