using BusBooking.ConsoleApp.Services;
using BusBooking.Core.DTOs;
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

                Console.WriteLine("1. Login");
                Console.WriteLine("2. Register");
                Console.WriteLine("3. Exit");

                Console.Write("Enter your choice: ");

                var choice = Console.ReadLine();

                if (choice == "1")
                    await LoginFlow();
                else if (choice == "2")
                    await RegisterFlow();
                else
                    return;

            }
    
        }
        private async Task LoginFlow()
        {
            Console.Write("Username: ");
            var username = Console.ReadLine();

            if (!Regex.IsMatch(username ?? "", @"^[a-zA-Z0-9]+$"))
            {
                Console.WriteLine(" Error: The username must contain only A-Z or a-z characters.");
                return;
            }

            Console.Write("Password: ");
            var password = Console.ReadLine();

            if (!Regex.IsMatch(password ?? "", @"^[a-zA-Z0-9]+$"))
            {
                Console.WriteLine(" Error: The username must contain only A-Z or a-z characters.");
                return;
            }


            var (token, role) = await _api.Login(username!, password!);

            //var (token, role) = await _api.Login(username!, password!);

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Login failed. Try again.");
                Pause();
                return;
            }

            _api.SetToken(token);

            if (role == "Admin")
                await AdminMenu();
            else
                await UserMenu();
        }
        private async Task AdminMenu()
        {
            while (true)
            {
                PrintHeader("ADMIN PANEL");

                Console.WriteLine("1. Add Bus");
                Console.WriteLine("2. Delete Bus");
                Console.WriteLine("3. View All Buses");
                Console.WriteLine("4. Logout");

                Console.Write("Enter your choice: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await AddBus();
                        break;

                    case "2":
                        await DeleteBus();
                        break;
                    case "3":
                        await ViewBuses();
                        break;
                    case "4":
                        return;
                }
            }
        }
        private async Task UserMenu()
        {
            while (true)
            {
                PrintHeader("USER PANEL");

                Console.WriteLine("1. View All Buses");
                Console.WriteLine("2. Search Buses");
                Console.WriteLine("3. Book Seat");
                Console.WriteLine("4. View My Bookings");
                Console.WriteLine("5. Cancel Booking");
                Console.WriteLine("6. Seat Layout");
                Console.WriteLine("7. Simulate Concurrent Booking");
                Console.WriteLine("8. Logout");

                Console.Write("Enter your Choice: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": await ViewBuses(); break;
                    case "2": await Search(); break;
                    case "3": await Book(); break;
                    case "4": await ViewBookings(); break;
                    case "5": await Cancel(); break;
                    case "6": await ShowSeatLayout(); break;
                    case "7": await SimultaneousBook(); break;
                    case "8": return;
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

            var istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");


            Console.WriteLine($"{"ID",-3} {"Bus",-8} {"Route",-20} {"Date",-12} {"Time",-8} {"Price",-8} {"Seats",-10}");
            PrintLine();

            foreach (var b in buses)
            {
                string route = $"{b.Source}->{b.Destination}";
                string seatInfo = $"{b.AvailableSeats}/{b.TotalSeats}";

                var istDate = TimeZoneInfo.ConvertTimeFromUtc(b.TravelDate, istZone);
                var istTime = b.TravelTime;

                Console.WriteLine($"{b.Id,-3} {b.BusNumber,-8} {route,-20} {istDate:yyyy-MM-dd} {istTime,-8} Rs.{b.Price,-8} {seatInfo,-10}");
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

            var result = await _api.BookSeat(busId, seat,method);

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

            
            var bookings=await _api.GetMyBookings();

            if (bookings.Count == 0)
            {
                Console.WriteLine("No bookings found.");
                Pause();
                return;
            }

            


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
        private async Task RegisterFlow()
        {
            Console.Write("Username: ");
            var username = Console.ReadLine();

            Console.Write("Password: ");
            var password = Console.ReadLine();

            Console.Write("Role (Admin/User): ");
            var role = Console.ReadLine();

            var res = await _api.Register(username!, password!, role!);

            Console.WriteLine(res);
            Pause();
        }

        private async Task ShowSeatLayout()
        {
            Console.Write("Enter Bus Id: ");
            int busId = int.Parse(Console.ReadLine()!);

            var seats = await _api.GetSeatLayout(busId);

            Console.WriteLine($"\nSeat Layout for Bus ID: {busId}:\n");

            int columns = 4;
            int aisleIndex = 2;

            for (int i = 0; i < seats.Count; i++)
            {
                var seat = seats[i];
                int postionInRow = i % columns;

                if (postionInRow==aisleIndex)
                {
                    
                    Console.Write("  ");
                }

                Console.ForegroundColor = seat.IsBooked ? ConsoleColor.Red : ConsoleColor.Green;
                

                Console.Write($"[{seat.SeatNumber:00}] ");
                Console.ResetColor();

                if (postionInRow==columns-1)
                {
                    Console.WriteLine();

                }
                    
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
        private async Task AddBus()
        {
            Console.Write("Bus Number: ");
            var busNumber = Console.ReadLine();

            Console.Write("Source: ");
            var source = Console.ReadLine();

            Console.Write("Destination: ");
            var destination = Console.ReadLine();

            Console.Write("Total Seats: ");
            int seats = int.Parse(Console.ReadLine()!);

            Console.Write("Travel Date (yyyy-mm-dd): ");
            DateTime date = DateTime.Parse(Console.ReadLine()!);

            Console.Write("Travel Time (HH:mm:ss): ");
            TimeSpan time = TimeSpan.Parse(Console.ReadLine()!);

            Console.Write("Price: ");
            decimal price = decimal.Parse(Console.ReadLine()!);

            var res = await _api.AddBus(new CreateBusDto
            {
                BusNumber = busNumber!,
                Source = source!,
                Destination = destination!,
                TotalSeats = seats,
                TravelDate = date,
                TravelTime = time,
                Price = price
            });

            Console.WriteLine(res);
            Pause();
        }
        private async Task DeleteBus()
        {
            Console.Write("Enter Bus Id: ");
            int id = int.Parse(Console.ReadLine()!);

            var res = await _api.DeleteBus(id);

            Console.WriteLine(res);
            Pause();
        }

        
        private async Task SimultaneousBook()
        {
            Console.Write("Bus Id: ");
            int busId = int.Parse(Console.ReadLine()!);

            Console.Write("Seat Number: ");
            int seat = int.Parse(Console.ReadLine()!);

            string method = "UPI";

            var client1 = new ApiClient();
            var client2 = new ApiClient();

            string User1 = "Ritik";
            string User2 = "Bhushan";

            var login1 = await client1.Login("Ritik", "Ritik2022");
            var login2 = await client2.Login("Bhushan", "Bhushan2023");

            client1.SetToken(login1.token);
            client2.SetToken(login2.token);

            var t1 = client1.BookSeat(busId, seat, method);
            var t2 = client2.BookSeat(busId, seat, method);

            var results = await Task.WhenAll(t1, t2);

            Console.WriteLine($"{User1} -> {results[0]}");
            Console.WriteLine($"{User2} > {results[1]}");

            Pause();
        }
    }
}
