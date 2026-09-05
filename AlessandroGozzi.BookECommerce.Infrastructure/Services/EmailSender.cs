using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Services_Helpers;
using MediatR;
using Microsoft.Extensions.Configuration;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AlessandroGozzi.BookECommerce.Infrastructure.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
        {
            string safeSubject = subject.Replace("\"", "'");
            string safeBody = body.Replace("\"", "'");

            string arguments = $"/k \"color 0B && echo. && echo === SIMULAZIONE EMAIL RICEVUTA === && echo. && echo A: {to} && echo OGGETTO: {safeSubject} && echo MESSAGGIO: {safeBody} && echo. && echo ================================== && echo.\"";

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
