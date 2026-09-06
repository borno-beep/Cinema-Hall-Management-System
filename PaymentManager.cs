using System.Collections.Generic;
using CinemaHallSystem.DAL;
using CinemaHallSystem.Models;

namespace CinemaHallSystem.BLL
{
    public class PaymentManager
    {
        private PaymentDAL paymentDAL = new PaymentDAL();

        public Payment GetPaymentByBooking(int bookingId)
        {
            return paymentDAL.GetByBookingId(bookingId);
        }
    }
}
