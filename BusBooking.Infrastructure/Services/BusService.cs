using BusBooking.Core.DTOs;
using BusBooking.Core.Entities;
using BusBooking.Core.Interfaces;
using BusBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusBooking.Infrastructure.Services
{
    public class BusService:IBusService
    {
        private readonly IBusRepository _busRepo;
        private readonly BusBookDbContext _context;

        public BusService(IBusRepository busRepo,BusBookDbContext context)
        {
            _busRepo = busRepo;
            _context = context;
        }
        public async Task<IEnumerable<BusDto>> GetAllBuses()
        {
            //return await _busRepo.GetAllAsync();
            var buses=await _busRepo.GetAllAsync();
            var res = new List<BusDto>();


            foreach(var bus in buses)
            {
                var bookedSeats = await _context.Bookings
                    .CountAsync(b => b.BusId == bus.Id && b.Status != "Cancelled");

                var totalSeats = int.TryParse(bus.TotalSeats, out var seats) ? seats : 0;

                res.Add(new BusDto
                {
                    Id = bus.Id,
                    BusNumber=bus.BusNumber,
                    Source=bus.Source,
                    Destination=bus.Destination,
                    TotalSeats=totalSeats,
                    AvailableSeats=totalSeats-bookedSeats,
                    TravelDate = bus.TravelDate,
                    TravelTime = bus.TravelTime,
                    Price = bus.Price
                });
            }
            return res;
        }

        public async Task<IEnumerable<BusDto>> SearchBuses(string source,string destination,DateTime date)
        {
            //return await _busRepo.SearchAsync(source, destination);
            
            var buses=await _busRepo.SearchAsync(source, destination,date);

            var res = new List<BusDto>();

            foreach(var bus in buses)
            {
                var bookedSeats=await _context.Bookings.CountAsync(
                    b=>b.BusId==bus.Id&&b.Status!="Cancelled");

                res.Add(new BusDto
                {
                    Id = bus.Id,
                    BusNumber = bus.BusNumber,
                    Source = bus.Source,
                    Destination = bus.Destination,
                    TotalSeats = Convert.ToInt32(bus.TotalSeats),
                    AvailableSeats = Convert.ToInt32(bus.TotalSeats) - bookedSeats,
                    TravelDate = bus.TravelDate,
                    TravelTime = bus.TravelTime,
                    Price = bus.Price
                });
            }
            return res;
        }
    }
}
