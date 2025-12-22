namespace KPO_Cursovoy.Models
{
    public class ComponentSpecification
    {
        public int ComponentId { get; set; }
        public int ValueId { get; set; }
        public SpecificationValue Value { get; set; } = new();
    }
}