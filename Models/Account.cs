namespace KPO_Cursovoy.Models
{
    public class Account
    {
        public int AccountId { get; set; }
        public int UserId { get; set; }
        public string Login { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "client";
    }
}
