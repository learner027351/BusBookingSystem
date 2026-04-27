
using System.Text;


using System.Text.Json;
using BusBooking.Core.DTOs;

namespace BusBooking.ConsoleApp.Services
{
    public class ApiClient
    {
        private readonly HttpClient _http;
        private string _token;

        public ApiClient()
        {
            _http =new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback=(_,_,_,_)=>true
            })
            {
                BaseAddress=new Uri("https://localhost:7189/api/")
            };
        }

        public void SetToken(string token)
        {
            _token = token;

            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<List<BusDto>> GetBuses()
        {
            var res = await _http.GetAsync("buses");
            var json = await res.Content.ReadAsStringAsync();


            return JsonSerializer.Deserialize<List<BusDto>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }
        public async Task<string> Register(string username, string password, string role)
        {
            var res = await _http.PostAsync(
                $"auth/register?username={username}&password={password}&role={role}", null);

            return await res.Content.ReadAsStringAsync();
        }
        public async Task<string> AddBus(CreateBusDto dto)
        {
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = await _http.PostAsync("buses", content);

            return await res.Content.ReadAsStringAsync();
        }
        public async Task<string> DeleteBus(int id)
        {
            var res = await _http.DeleteAsync($"buses/{id}");

            return await res.Content.ReadAsStringAsync();
        }
        public async Task<List<BusDto>> SearchBuses(string source, string destination,DateTime date)
        {
            var res = await _http.GetAsync($"buses/search?source={source}&destination={destination}&date={date:yyyy-MM-dd}");
            var json = await res.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<BusDto>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        public async Task<(string token, string role)> Login(string username, string password)
        {
            var res = await _http.PostAsync(
                $"auth/login?username={username}&password={password}", null);

            var json = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
            {
                Console.WriteLine($"Login Failed: {json}");
                return (null, null);
            }

            var data = JsonSerializer.Deserialize<LoginResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return (data.Token, data.Role);
        }
        public async Task<string> BookSeat(int busId, int seat, string method)
        {

            var res = await _http.PostAsync(
                $"booking?busId={busId}&seatNumber={seat}&method={method}", null);

            var content = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
                return $"Failed: {content}";

            return $"Success: {content}";
        }


        public async Task<List<SeatDto>> GetSeatLayout(int busId)
        {
            var res = await _http.GetAsync($"booking/bus/{busId}/seats");
            var json = await res.Content.ReadAsStringAsync();
            

            return JsonSerializer.Deserialize<List<SeatDto>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        public async Task<string> CancelYourBooking(int bookingId)
        {
            var res=await _http.PutAsync($"booking/cancel/{bookingId}", null);

            return await res.Content.ReadAsStringAsync();

        }

        
        public async Task<List<BookingDto>> GetMyBookings()
        {
            var res = await _http.GetAsync("booking/my");
            var json = await res.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<BookingDto>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }
    }
}
 