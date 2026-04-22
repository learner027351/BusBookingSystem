
using BusBooking.ConsoleApp.Services;
using BusBooking.ConsoleApp.UI;

public class Program
{
    static async Task Main(string[] args)
    {
        var api = new ApiClient();
        var menu = new Menu(api);

        await menu.Show();
    }
}