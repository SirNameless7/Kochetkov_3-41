using System;

namespace KPO_Cursovoy.Models
{
    public enum PaymentStatus
    {
        Pending,
        Partial,
        Paid,
        Failed
    }

    public enum PaymentMethod
    {
        Cash,
        Transfer
    }

    public enum PaymentType
    {
        Full,
        Installment
    }

    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public PaymentMethod Method { get; set; } = PaymentMethod.Cash;
        public PaymentType Type { get; set; } = PaymentType.Full;
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    }
}