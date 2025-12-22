using System;
using System.Collections.Generic;

namespace KPO_Cursovoy.Models
{
    public class Delivery
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public decimal Total { get; set; }
        public Supplier Supplier { get; set; } = new();
        public List<DeliveryItem> Items { get; set; } = new();
    }

    public class DeliveryItem
    {
        public int DeliveryId { get; set; }
        public int ComponentId { get; set; }
        public int Quantity { get; set; }
        public ComponentItem Component { get; set; } = new();
    }
}