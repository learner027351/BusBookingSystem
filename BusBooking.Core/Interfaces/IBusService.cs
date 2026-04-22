using System;
using System.Collections.Generic;
using System.Text;
using BusBooking.Core.Entities;
using BusBooking.Core.DTOs;

namespace BusBooking.Core.Interfaces
{
    public interface IBusService
    {
        Task<IEnumerable<BusDto>> GetAllBuses();

        Task<IEnumerable<BusDto>> SearchBuses(string source, string destination,DateTime date);
    }
}
