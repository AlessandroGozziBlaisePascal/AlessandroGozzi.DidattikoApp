using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AlessandroGozzi.BookECommerce.SharedKernel;

namespace AlessandroGozzi_BookECommerce.Domain.Entities.BookFolder.Value_Object
{
    public record ImageUrl
    {
        private const string DefaultImage = "default-book.jpg";

        public string Value { get; init; }

        private ImageUrl(string value)
        {
            Value = value;
        }

        public static Result<ImageUrl> Create(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Result.Success(new ImageUrl(DefaultImage));
            }

            var trimmedValue = value.Trim();

            var validExtensionsRegex = new Regex(@"\.(jpg|jpeg|png|webp)$", RegexOptions.IgnoreCase);
            if (!validExtensionsRegex.IsMatch(trimmedValue))
            {
                return Result.Failure<ImageUrl>(new Error(
                    "ImageUrl.InvalidFormat",
                    "L'immagine deve avere un'estensione valida (.jpg, .jpeg, .png, .webp).",
                    ErrorType.Validation));
            }

            return Result.Success(new ImageUrl(trimmedValue));
        }

        public override string ToString() => Value;
    }

}
