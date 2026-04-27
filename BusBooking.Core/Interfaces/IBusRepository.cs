using BusBooking.Core.Entities;


namespace BusBooking.Core.Interfaces
{
    public  interface IBusRepository
    {

        Task AddAsync(Bus bus);
        Task DeleteAsync(Bus bus);
        Task<IEnumerable<Bus>> GetAllAsync();
        Task<IEnumerable<Bus>> SearchAsync(string source, string destination,DateTime date);
        Task<Bus?> GetByIdAsync(int id);
    }
}
