using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.Book
{
    public enum BookStatus
    {
        LikeNew = 1,
        Highlighted = 2,
        PencilMarked = 3,
        PenMarked = 4,
        Worn = 5, //USURATO
        TornPages = 6
    }
}
