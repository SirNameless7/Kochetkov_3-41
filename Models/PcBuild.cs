namespace KPO_Cursovoy.Models
{
    public class PcBuild
    {
        public List<ComponentItem> SelectedComponents { get; set; } = new();

        public decimal TotalPrice => SelectedComponents.Sum(c => c.Price);
    }

}
