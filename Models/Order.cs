using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace KPO_Cursovoy.Models
{
    public enum OrderStatus
    {
        New,
        WaitingPayment,
        Paid,
        Processing,
        Completed,
        Cancelled
    }

    public class Order
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int? PcId { get; set; }

        public bool IsCustomBuild { get; set; } = false;

        public decimal TotalAmount { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public OrderStatus Status { get; set; } = OrderStatus.New;

        public List<OrderComponent> Components { get; set; } = new();

        public List<OrderService> Services { get; set; } = new();

        [NotMapped]
        public bool IsPaymentRequired => Status == OrderStatus.WaitingPayment;
    }

    public class OrderComponent
    {
        public int OrderId { get; set; }
        public int ComponentId { get; set; }
        public int Quantity { get; set; } = 1;
        public ComponentItem? Component { get; set; }
    }

    public class OrderService
    {
        public int OrderId { get; set; }
        public int ServiceId { get; set; }
        public ServiceItem? Service { get; set; }
    }

}
