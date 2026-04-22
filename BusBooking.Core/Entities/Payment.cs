using System;
using System.Collections.Generic;
using System.Text;

namespace BusBooking.Core.Entities
{
    public enum PaymentMethod
    {
        UPI,
        NetBanking,
        DebitCard
    }
    public class Payment
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }

        public string Status { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        public DateTime PaymentDate { get; set; }

        public Booking Booking { get; set; }
    }
}
