using System;
using System.Collections.Generic;

namespace KPO_Cursovoy.Models
{
    public class ComponentItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CategoryCode { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        //public List<ComponentSpecification> Specifications { get; set; } = new();
    }

}