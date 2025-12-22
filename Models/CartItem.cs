namespace KPO_Cursovoy.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public PcItem? Pc { get; set; }
        public ComponentItem? Component { get; set; }
        public int Quantity { get; set; } = 1;
        public bool IsCustomBuild { get; set; } = false;

        public decimal TotalPrice => Pc != null ? Pc.Price * Quantity : Component != null ? Component.Price * Quantity : 0;
    }
}