namespace LaundryCleaning.Service.GraphQL.Users.Inputs
{
    public class CreateUserInput
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
