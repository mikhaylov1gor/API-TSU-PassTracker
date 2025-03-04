using System;
using System.Security.Cryptography;
using System.Text;

namespace API_TSU_PassTracker.Infrastructure
{
    public interface IPasswordHasher
    {
        string GenerateHashPassword(string password, string salt);
        string GenerateSalt();

        bool Verify(string password, string hashedPassword, string salt);
    }
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16; 
        private const int WorkFactor = 13;

        public string GenerateHashPassword(string password, string salt)
        {
            var saltedPassword = password + salt;
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(saltedPassword, WorkFactor);

            return hashedPassword;
        }

        public bool Verify(string password, string hashedPassword, string salt)
        {
            var saltedPassword = password + salt;

            return BCrypt.Net.BCrypt.Verify(saltedPassword, hashedPassword);
        }
 

        public string GenerateSalt()
        {
            using var rng = RandomNumberGenerator.Create();
            var saltBytes = new byte[SaltSize];
            rng.GetBytes(saltBytes);

            return Convert.ToBase64String(saltBytes);
        }
    }
}
