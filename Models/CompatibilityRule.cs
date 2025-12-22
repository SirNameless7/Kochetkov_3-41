namespace KPO_Cursovoy.Models
{
    public class CompatibilityRule
    {
        public int RuleId { get; set; }
        public string CategoryCode1 { get; set; } = string.Empty;
        public string CategoryCode2 { get; set; } = string.Empty;
        public int SpecId1 { get; set; }
        public int SpecId2 { get; set; }
    }
}