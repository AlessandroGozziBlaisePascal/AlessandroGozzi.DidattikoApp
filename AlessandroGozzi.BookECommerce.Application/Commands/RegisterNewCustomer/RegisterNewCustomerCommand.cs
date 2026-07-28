using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.Application.Dto.Aggregate_Roots_Dto;
using AlessandroGozzi.BookECommerce.Application.Dto.VO_Dto;
using AlessandroGozzi.BookECommerce.SharedKernel;
using MediatR;

namespace AlessandroGozzi.BookECommerce.Application.Commands.RegisterNewCustomer
{
    public record RegisterNewCustomerCommand(string Name,
        string Surname,
        string Email,
        string PhoneNumber,
        AddressDto Address,
        string TaxCode,
        string Password,
        string ConfirmPassword): IRequest<Result<CustomerDto>>
    {
    }
}
