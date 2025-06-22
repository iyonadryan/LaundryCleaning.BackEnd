using LaundryCleaning.Service.Common.Models.Entities;
using LaundryCleaning.Service.GraphQL.Users.CustomModels;
using LaundryCleaning.Service.GraphQL.Users.Inputs;

namespace LaundryCleaning.Service.GraphQL.Users.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetUsers (CancellationToken cancellationToken);
        Task<CreateUserCustomModel> CreateUser(CreateUserInput input, CancellationToken cancellationToken);
        Task<string> SendUserNotification(string input, CancellationToken cancellationtoken);
    }
}
