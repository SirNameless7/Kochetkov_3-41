using System.Collections.Generic;

namespace KPO_Cursovoy.Models
{
    public class PcItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public List<ComponentItem> Components { get; set; } = new();
    }
}