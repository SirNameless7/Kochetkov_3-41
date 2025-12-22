using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KPO_Cursovoy.Models;
using KPO_Cursovoy.Services;

namespace KPO_Cursovoy.Services
{
    public class CompatibilityService
    {
        private readonly DatabaseService _databaseService;

        public CompatibilityService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<CompatibilityResult> CheckCompatibilityAsync(List<ComponentItem> components)
        {
            var rules = await _databaseService.GetCompatibilityRulesAsync();
            var result = new CompatibilityResult { IsCompatible = true };
            foreach (var rule in rules)
            {
                var comp1 = components.FirstOrDefault(c => c.CategoryCode == rule.CategoryCode1);
                var comp2 = components.FirstOrDefault(c => c.CategoryCode == rule.CategoryCode2);

                if (comp1 != null && comp2 != null)
                {
                    bool isCompatible = CheckSimpleCompatibility(comp1, comp2, rule);

                    if (!isCompatible)
                    {
                        result.IsCompatible = false;
                        result.IncompatiblePairs.Add(new IncompatiblePair
                        {
                            Component1 = comp1.Name,
                            Component2 = comp2.Name,
                            Reason = $"Несовместимость {rule.CategoryCode1}-{rule.CategoryCode2}"
                        });
                    }
                }
            }

            return result;
        }

        private bool CheckSimpleCompatibility(ComponentItem comp1, ComponentItem comp2, CompatibilityRule rule)
        {
            if (rule.CategoryCode1 == "CPU" && rule.CategoryCode2 == "MB")
            {
                return (comp1.Name.Contains("Intel") && comp2.Name.Contains("Gigabyte")) ||
                       (comp1.Name.Contains("AMD") && comp2.Name.Contains("ASUS"));
            }

            if (rule.CategoryCode1 == "MB" && rule.CategoryCode2 == "RAM")
            {
                return comp2.Name.Contains("DDR4");
            }

            return true;
        }
    }

    public class CompatibilityResult
    {
        public bool IsCompatible { get; set; }
        public List<IncompatiblePair> IncompatiblePairs { get; set; } = new();
    }

    public class IncompatiblePair
    {
        public string Component1 { get; set; } = string.Empty;
        public string Component2 { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }
}
