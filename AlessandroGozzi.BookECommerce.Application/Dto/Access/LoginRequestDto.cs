namespace AlessandroGozzi.BookECommerce.Application.Dto.Access
{
    public record LoginRequestDto(string Identifier, string Password)
    {
        public override string ToString() => $"LoginRequestDto {{ Identifier = {Identifier}, Password = [PROTECTED]}}";
    }
}
