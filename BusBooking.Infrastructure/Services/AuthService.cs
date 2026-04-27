


namespace BusBooking.Infrastructure.Services
{
    public class AuthService
    {

        public string HashPassword(string PassWord)
        {
            return BCrypt.Net.BCrypt.HashPassword(PassWord);
        }
        public bool VerifyPassword(string PassWord,string HashPassword)
        {

            return BCrypt.Net.BCrypt.Verify(PassWord, HashPassword);
        }
    }
}
