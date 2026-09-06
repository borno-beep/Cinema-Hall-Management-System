using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CinemaHallSystem.DAL;
using CinemaHallSystem.Models;

namespace CinemaHallSystem.BLL
{
    public class BookingManager
    {
        private BookingDAL bookingDAL = new BookingDAL();
        private ShowSeatDAL showSeatDAL = new ShowSeatDAL();
        private PaymentDAL paymentDAL = new PaymentDAL();
        private FoodOrderDAL foodOrderDAL = new FoodOrderDAL();

        public List<ShowSeat> GetShowSeats(int showId)
        {
            return showSeatDAL.GetByShowId(showId);
        }

        public List<int> LockSeats(List<int> showSeatIds)
        {
            var lockedIds = new List<int>();
            foreach (var id in showSeatIds)
            {
                if (showSeatDAL.LockSeat(id) > 0)
                {
                    lockedIds.Add(id);
                }
            }
            return lockedIds;
        }

        public int CompleteBooking(int userId, int showId, List<int> showSeatIds, string paymentMethod, decimal totalAmount, FoodOrder foodOrder = null, List<FoodOrderItem> foodItems = null)
        {
            using var conn = DBConnection.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                var booking = new Booking
                {
                    UserID = userId,
                    ShowID = showId,
                    BookingDate = DateTime.Now,
                    TotalAmount = totalAmount,
                    Status = "Confirmed"
                };

                var payment = new Payment
                {
                    Amount = totalAmount,
                    PaymentMethod = paymentMethod,
                    PaymentDate = DateTime.Now,
                    Status = "Success"
                };

                int bookingId = bookingDAL.CreateBookingTransaction(booking, showSeatIds, payment, conn, tran);

                if (foodOrder != null && foodItems != null && foodItems.Count > 0)
                {
                    foodOrder.BookingID = bookingId;
                    foodOrder.OrderDate = DateTime.Now;
                    foodOrderDAL.CreateFoodOrderTransaction(foodOrder, foodItems, conn, tran);
                }

                tran.Commit();
                return bookingId;
            }
            catch
            {
                tran.Rollback();
                foreach (var id in showSeatIds)
                {
                    try { showSeatDAL.UpdateStatus(id, "Available"); } catch { }
                }
                throw;
            }
        }

        /// <summary>
        /// Cancels a booking: sets Booking status to Cancelled, Payment to Refunded,
        /// and releases all booked seats back to Available.
        /// </summary>
        public bool CancelBooking(int bookingId)
        {
            try
            {
                // Update booking status
                var booking = bookingDAL.GetById(bookingId);
                if (booking == null) return false;
                booking.Status = "Cancelled";
                bookingDAL.Update(booking);

                // Update payment status
                var payment = paymentDAL.GetByBookingId(bookingId);
                if (payment != null)
                {
                    payment.Status = "Refunded";
                    paymentDAL.Update(payment);
                }

                // Release seats — query BookingSeats to find ShowSeatIDs, then set Available
                using var conn = DBConnection.GetConnection();
                conn.Open();
                string query = "SELECT ShowSeatID FROM BookingSeats WHERE BookingID = @BookingID";
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@BookingID", bookingId);
                using var reader = cmd.ExecuteReader();
                var seatIds = new List<int>();
                while (reader.Read())
                {
                    seatIds.Add(Convert.ToInt32(reader["ShowSeatID"]));
                }
                reader.Close();

                foreach (var seatId in seatIds)
                {
                    showSeatDAL.UpdateStatus(seatId, "Available");
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Unlocks seats that were locked during checkout (e.g. if user cancels payment).
        /// </summary>
        public void UnlockSeats(List<int> showSeatIds)
        {
            foreach (var id in showSeatIds)
            {
                try { showSeatDAL.UpdateStatus(id, "Available"); } catch { }
            }
        }

        public List<Booking> GetUserBookings(int userId)
        {
            return bookingDAL.GetByUserId(userId);
        }

        public List<Booking> GetAllBookings()
        {
            return bookingDAL.GetAll();
        }

        public Booking GetBookingDetails(int bookingId)
        {
            return bookingDAL.GetById(bookingId);
        }
    }
}
