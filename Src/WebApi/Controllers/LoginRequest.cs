using FluentValidation;

namespace WebApi.Controllers
{
    public enum GranType
    {
        Password,
        RefreshToken
    }

    public record LoginRequest
    {
        public string Password { get; set; }
        public string Username { get; set; }
        public GranType GrantType { get; set; }
        public string Token { get; set; }
        public string ExpiredToken { get; set; }
    }

    public class PersonValidator : AbstractValidator<LoginRequest>
    {
        public PersonValidator()
        {
            RuleFor(x => x.Password).NotEmpty().When(x => x.GrantType == GranType.Password);
            RuleFor(x => x.Username).NotEmpty().When(x => x.GrantType == GranType.Password);
            RuleFor(x => x.GrantType).NotNull();
            RuleFor(x => x.Token).NotEmpty().When(x => x.GrantType == GranType.RefreshToken);
            RuleFor(x => x.ExpiredToken).NotEmpty().When(x => x.GrantType == GranType.RefreshToken);
        }
    }
}
