using LaundryCleaning.Service.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace LaundryCleaning.Service.Services.Implementations
{
    public class PasswordService : IPasswordService
    {
        private readonly IPasswordHasher<object> _passwordHasher;
        public PasswordService(IPasswordHasher<object> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public string GeneratePassword(string password)
        {
            return _passwordHasher.HashPassword(null, password);
        }

        public bool VerifyPassword(string hashedPassword, string password)
        {
            var result = _passwordHasher.VerifyHashedPassword(null, hashedPassword, password);
            // if required, you can handle if result is SuccessRehashNeeded
            return result == PasswordVerificationResult.Success;
        }
    }
}
