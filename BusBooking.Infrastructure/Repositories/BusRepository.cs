using BusBooking.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using BusBooking.Core.Entities;
using BusBooking.Infrastructure.Data;

namespace BusBooking.Infrastructure.Repositories
{
    public class BusRepository:IBusRepository
    {

        private readonly BusBookDbContext _context;

        public BusRepository(BusBookDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Bus>> GetAllAsync()
        => await _context.Buses.ToListAsync();

        public async Task<IEnumerable<Bus>> SearchAsync(string source, string destination,DateTime date)
            => await _context.Buses
                .Where(b => b.Source == source && b.Destination == destination && b.TravelDate.Date == date.Date)
                .ToListAsync();

        public async Task<Bus?> GetByIdAsync(int id)
            => await _context.Buses.FindAsync(id);


        public async Task AddAsync(Bus bus)
        {
            await _context.Buses.AddAsync(bus);
        }

        public async Task DeleteAsync(Bus bus)
        {
            _context.Buses.Remove(bus);
        }

    }
}
