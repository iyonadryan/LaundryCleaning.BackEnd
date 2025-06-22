namespace LaundryCleaning.Service.Services.Interfaces
{
    public interface IPasswordService
    {
        string GeneratePassword(string password);
        bool VerifyPassword(string hashedPassword, string password);
    }
}
