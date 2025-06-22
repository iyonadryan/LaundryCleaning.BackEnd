namespace LaundryCleaning.Service.Auth.CustomModels
{
    public class CurrentUserCustomModel
    {
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new();
    }
}
