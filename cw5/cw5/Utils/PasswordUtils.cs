using System;
using System.Security.Cryptography;

namespace cw5.Utils
{
    public class PasswordUtils
    {
        private const int SaltLength = 32;
        private const int KeyLength = 32;
        private const int Iterations = 10000;

        public static string CreateSaltedPasswordHash(string password, byte[] salt)
        {
          
            var hashValue = GenerateHashValue(password, salt);
            return Convert.ToBase64String(hashValue);
        }


        public static byte[] GenerateSalt()
        {
            var randomNumberGenerator = new RNGCryptoServiceProvider();
            var salt = new byte[SaltLength];
            randomNumberGenerator.GetBytes(salt);
            return salt;
        }

        private static byte[] GenerateHashValue(string password, byte[] salt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations);
            return pbkdf2.GetBytes(KeyLength);
        }

        public static bool ValidatePassword(string receivedPassword, string storesPassword, byte[] salt)
        {
            var receivedPasswordHash = CreateSaltedPasswordHash(receivedPassword, salt);
            return storesPassword.Equals(receivedPasswordHash);
        } 
    }
}