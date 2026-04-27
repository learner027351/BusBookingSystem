using System;

using BusBooking.Core.DTOs;

namespace BusBooking.Core.Interfaces
{
    public interface IBusService
    {
        Task<bool> AddBus(CreateBusDto dto);

        Task<bool> DeleteBus(int id);
        Task<IEnumerable<BusDto>> GetAllBuses();

        Task<IEnumerable<BusDto>> SearchBuses(string source, string destination,DateTime date);
    }
}
