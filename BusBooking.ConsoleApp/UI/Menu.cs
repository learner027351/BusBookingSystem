using BusBooking.ConsoleApp.Services;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusBooking.ConsoleApp.UI
{
    public  class Menu
    {
        private readonly ApiClient _api;

        public Menu(ApiClient api)
        {
            _api = api;
        }

        private void PrintLine(int length = 80)
        {
            Console.WriteLine(new string('-', length));
        }

        private void PrintHeader(string title)
        {
            Console.Clear();
            PrintLine();
            Console.WriteLine(title);
            PrintLine();
        }
        private void Pause()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }


        public async Task Show()
        {
            while (true)
            {
                PrintHeader("BUS BOOKING SYSTEM");

                Console.WriteLine("1. View All Buses");
                Console.WriteLine("2. Search Buses");
                Console.WriteLine("3. Book Seat");
                Console.WriteLine("4. Show Seat Layout");
                Console.WriteLine("5. View My Bookings");
                Console.WriteLine("6. Cancel Booking");
                Console.WriteLine("7. Simulate Concurrent Booking");
                Console.WriteLine("8. Exit");

                PrintLine();
                Console.Write("Enter choice: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await ViewBuses();
                        break;

                    case "2":
                        await Search();
                        break;

                    case "3":
                        await Book();
                        break;

                    case "4":
                        await ShowSeatLayout();
                        break;
                    case "5":
                        await ViewBookings();
                        break;

                    case "6":
                        await Cancel();
                        break;

                    case "7":
                        await SimultaneousBook();
                        break;

                    case "8":
                        return;

                    default:
                        Console.WriteLine("Invalid Choice");
                        break;

                }

            }
    
        }
        private async Task ViewBuses()
        {
            PrintHeader("ALL AVAILABLE BUSES");

            var buses = await _api.GetBuses();

            if (buses.Count == 0)
            {
                Console.WriteLine("No buses available.");
                Pause();
                return;
            }

            //Console.WriteLine($"{"ID",-5} {"Bus No",-10} {"Source",-15} {"Destination",-15} {"AvailableSeats",-10}");
            //PrintLine();

            //foreach (var b in buses)
            //{
            //    string seatInfo = $"{b.AvailableSeats}/{b.TotalSeats}";
            //    Console.WriteLine($"{b.Id,-5} {b.BusNumber,-10} {b.Source,-15} {b.Destination,-15} {seatInfo,-10}");
            //}

            Console.WriteLine($"{"ID",-3} {"Bus",-8} {"Route",-20} {"Date",-12} {"Time",-8} {"Price",-8} {"Seats",-10}");
            PrintLine();

            foreach (var b in buses)
            {
                string route = $"{b.Source}->{b.Destination}";
                string seatInfo = $"{b.AvailableSeats}/{b.TotalSeats}";

                Console.WriteLine($"{b.Id,-3} {b.BusNumber,-8} {route,-20} {b.TravelDate,-12} {b.TravelTime,-8} Rs.{b.Price,-8} {seatInfo,-10}");
            }

            PrintLine();
            Pause();
        }

        private async Task Search()
        {
            PrintHeader("SEARCH BUSES");

            Console.Write("Enter Source: ");
            var source = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(source) || !Regex.IsMatch(source,@"^[A-Za-z]+$"))
            {
                Console.WriteLine("Invalid Source.No use of Special  or Numeric Characters is Allowed");
                Pause();
                
                return;
            }

            Console.Write("Enter Destination: ");
            var dest = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(dest) || !Regex.IsMatch(dest, @"^[A-Za-z]+$"))
            {
                Console.WriteLine("Invalid Destination.No use of Special  or Numeric Characters is Allowed");
                Pause();

                return;
            }
            Console.Write("Enter Travel Date (yyyy-mm-dd): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime date))
            {
                Console.WriteLine("Invalid Date Format");
                Pause();
                return;
            }

            var buses = await _api.SearchBuses(source!, dest!,date);

            if (buses.Count == 0)
            {
                Console.WriteLine("No matching buses found.");
                Pause();
                return;
                
            }

            Console.WriteLine($"\nResults:\n");
            //Console.WriteLine($"{"ID",-5} {"Bus No",-10} {"Source",-15} {"Destination",-15}");
            //PrintLine();

            //foreach (var b in buses)
            //{
            //    Console.WriteLine($"{b.Id,-5} {b.BusNumber,-10} {b.Source,-15} {b.Destination,-15}");
            //}
            Console.WriteLine($"{"ID",-3} {"Bus",-8} {"Date",-12} {"Time",-8} {"Price",-8}");
            PrintLine();

            foreach (var b in buses)
            {
                Console.WriteLine($"{b.Id,-3} {b.BusNumber,-8} {b.TravelDate:yyyy-MM-dd,-12} {b.TravelTime,-8} ₹{b.Price,-7}");
            }

            PrintLine();
            Pause();
        }

        private async Task Book()
        {
            PrintHeader("BOOK SEAT");

            Console.Write("Enter Bus Id: ");
            if (!int.TryParse(Console.ReadLine(), out int busId))
            {
                Console.WriteLine("Invalid Bus Id");
                Pause();
                return;
            }

            Console.Write("Enter User Id: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("Invalid User Id");
                Pause();
                return;
            }

            Console.Write("Enter Seat Number: ");
            if (!int.TryParse(Console.ReadLine(), out int seat))
            {
                Console.WriteLine("Invalid Seat Number");
                Pause();
                return;
            }

            Console.WriteLine("Select Payment Method:");
            Console.WriteLine("1. UPI");
            Console.WriteLine("2. NetBanking");
            Console.WriteLine("3. DebitCard");

            var choice = Console.ReadLine();

            string method = choice switch
            {
                "1" => "UPI",
                "2" => "NetBanking",
                "3" => "DebitCard",
                _ => ""
            };

            if (string.IsNullOrEmpty(method))
            {
                Console.WriteLine("Invalid Payment Method");
                Pause();
                return;
            }

            var result = await _api.BookSeat(busId, userId, seat,method);

            PrintLine();
            Console.WriteLine(result);
            PrintLine();
            Pause();
        }
        private async Task Cancel()
        {
            PrintHeader("CANCEL BOOKING");

            Console.Write("Enter Booking Id:");

            if (!int.TryParse(Console.ReadLine(), out int bookingId))
            {
                Console.WriteLine("Invalid Booking Id");
                Pause();
                return;
            }

            var result = await _api.CancelYourBooking(bookingId);

            PrintLine();
            Console.WriteLine(result);
            PrintLine();

            Pause();
        }
        private async Task ViewBookings()
        {
            PrintHeader("MY BOOKINGS");

            Console.Write("Enter User Id: ");
            if (!int.TryParse(Console.ReadLine(), out int userId))
            {
                Console.WriteLine("Invalid User Id");
                Pause();
                return;
            }

            var bookings = await _api.GetUserBookings(userId);

            if (bookings.Count == 0)
            {
                Console.WriteLine("No bookings found.");
                Pause();
                return;
            }

            //Console.WriteLine($"{"Bus No",-10} {"Seat",-5} {"Route",-25} {"Status",-10}");
            //PrintLine();

            //foreach (var b in bookings)
            //{
            //    string route = $"{b.Source} → {b.Destination}";
            //    Console.WriteLine($"{b.BusNumber,-10} {b.SeatNumber,-5} {route,-25} {b.Status,-10}");
            //}
            Console.WriteLine($"{"Bus",-8} {"Seat",-5} {"Route",-20} {"Status",-10} {"Time",-20} {"PaymentMethod",-30}");
            PrintLine();

            foreach (var b in bookings)
            {
                string route = $"{b.Source}->{b.Destination}";
                Console.WriteLine($"{b.BusNumber,-8} {b.SeatNumber,-5} {route,-20} {b.Status,-10} {b.BookingTime:yyyy - MM - dd HH:mm} {b.PaymentMethod,-30}");
            }

            PrintLine();
            Pause();
        }

        private async Task ShowSeatLayout()
        {
            Console.Write("Enter Bus Id: ");
            int busId = int.Parse(Console.ReadLine()!);

            var seats = await _api.GetSeatLayout(busId);

            Console.WriteLine($"\nSeat Layout for Bus ID: {busId}:\n");

            int columns = 4;

            for (int i = 0; i < seats.Count; i++)
            {
                var seat = seats[i];

                if (seat.IsBooked)
                    Console.ForegroundColor = ConsoleColor.Red;
                else
                    Console.ForegroundColor = ConsoleColor.Green;

                Console.Write($"[{seat.SeatNumber:00}] ");
                Console.ResetColor();

                if ((i + 1) % columns == 0)
                    Console.WriteLine();
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private async Task SimultaneousBook()
        {
            Console.Clear();
            Console.WriteLine("Concurrent user Booking....");
            PrintLine();

            Console.WriteLine("Enter BusID: ");
            int busId = int.Parse(Console.ReadLine()!);

            Console.Write("Enter Seat Number: ");
            int seat = int.Parse(Console.ReadLine()!);

            int user1 = 1;
            int user2 = 2;

            string method = "UPI";

            var task1 = Task.Run(async () =>
            {
                var result = await _api.BookSeat(busId, user1, seat, method);
                Console.WriteLine($"User {user1}: {result}");
            });

            var task2 = Task.Run(async () =>
            {
                var result = await _api.BookSeat(busId, user2, seat, method);
                Console.WriteLine($"User {user2}: {result}");
            });

            await Task.WhenAll(task1, task2);

            PrintLine();
            Console.WriteLine("Test Completed");
            Pause();

        }
    }
}
