using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using BusBooking.Core.Entities;

using System.Text.Json;
using BusBooking.Core.DTOs;

namespace BusBooking.ConsoleApp.Services
{
    public class ApiClient
    {
        private readonly HttpClient _http;

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

        public async Task<List<BusDto>> GetBuses()
        {
            var res = await _http.GetAsync("buses");
            var json = await res.Content.ReadAsStringAsync();


            return JsonSerializer.Deserialize<List<BusDto>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }
        public async Task<List<BusDto>> SearchBuses(string source, string destination,DateTime date)
        {
            var res = await _http.GetAsync($"buses/search?source={source}&destination={destination}&date={date:yyyy-MM-dd}");
            var json = await res.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<BusDto>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        public async Task<string> BookSeat(int busId,int userId,int seat,string method)
        {
            var res = await _http.PostAsync(
                $"booking?busId={busId}&userId={userId}&seatNumber={seat}&method={method}", null);

            return await res.Content.ReadAsStringAsync();
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

        public async Task<List<BookingDto>>GetUserBookings(int userId)
        {
            var res=await _http.GetAsync($"booking/user/{userId}");
            var json = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error: {json}");
                return new List<BookingDto>();
            }

            try
            {
                return JsonSerializer.Deserialize<List<BookingDto>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })! ?? new List<BookingDto>();
            }

            catch (Exception ex) {

                Console.WriteLine($"JSON Error:{ ex.Message}");
                return new List<BookingDto>();
            }
        }
    }
}
 