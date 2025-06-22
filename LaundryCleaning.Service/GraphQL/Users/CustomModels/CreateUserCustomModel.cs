namespace LaundryCleaning.Service.GraphQL.Users.CustomModels
{
    public class CreateUserCustomModel
    {
        public bool Success { get; set; }
        public UserCreatedResponse Data { get; set; }
    }

    public class UserCreatedResponse
    {
        public string Email { get; set; }
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
