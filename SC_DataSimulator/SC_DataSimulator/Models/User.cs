namespace SC_DataSimulator.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool RememberLogin { get; set; }
        public string ReturnUrl { get; set;} = string.Empty;
    }
}
