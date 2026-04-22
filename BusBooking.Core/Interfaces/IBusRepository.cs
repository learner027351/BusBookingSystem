using BusBooking.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusBooking.Core.Interfaces
{
    public  interface IBusRepository
    {
        Task<IEnumerable<Bus>> GetAllAsync();
        Task<IEnumerable<Bus>> SearchAsync(string source, string destination,DateTime date);
        Task<Bus?> GetByIdAsync(int id);
    }
}
