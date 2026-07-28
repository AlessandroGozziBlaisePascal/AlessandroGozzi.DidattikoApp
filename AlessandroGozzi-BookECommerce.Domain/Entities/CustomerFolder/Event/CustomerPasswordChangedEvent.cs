using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.CustomerFolder.Event
{
    public record CustomerPasswordChangedEvent(Guid Id, string OldPassword, string NewPassword) : DomainEvent { }
}
