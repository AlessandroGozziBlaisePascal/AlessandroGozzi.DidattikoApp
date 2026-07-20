using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlessandroGozzi.BookECommerce.SharedKernel
{
    public record Error
    {
        public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);
        public static readonly Error NullValue = new("General.Null", "Null value was provided", ErrorType.Failure);

        public Error(string code, string description, ErrorType type)
        {
            Code = code;
            Description = description;
            Type = type;
        }

        public string Code { get; }

        public string Description { get; }

        public ErrorType Type { get; }

        public static Error Failure(string code, string description) => new(code, description, ErrorType.Failure);

        public static Error NotFound(string code, string description) => new(code, description, ErrorType.NotFound);

        public static Error StatusConflict(string code, string description) => new(code, description, ErrorType.StatusConflict);

        public static Error Validation(string code, string description) => new(code, description, ErrorType.Validation);
        public static Error PermissionDenied(string code, string description) => new(code, description, ErrorType.PermissionDenied);
    }
}
