namespace KPO_Cursovoy.Models
{
    public class User
    {
        public int UserId { get; set; } 
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string LoyaltyStatus { get; set; } = "обычный";
        public Account? Account { get; set; }
    }
}
