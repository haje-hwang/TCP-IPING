using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Server_TCP
{
    class AES
    {
        public const string IV = "InitVector123456";   //should 16byte
        public static string GenerateRandomKey()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] keyBytes = new byte[32]; // AES-256용 32바이트
                rng.GetBytes(keyBytes);
                string key = Convert.ToBase64String(keyBytes);
                return key;
            }
        }
        public static string Encrypt(string plainText, string key, string iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = Encoding.UTF8.GetBytes(iv);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                        cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                        cryptoStream.FlushFinalBlock();

                        return Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
            }
        }

        public static string Decrypt(string cipherText, string key, string iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = Encoding.UTF8.GetBytes(iv);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        byte[] cipherBytes = Convert.FromBase64String(cipherText);
                        cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);
                        cryptoStream.FlushFinalBlock();

                        return Encoding.UTF8.GetString(memoryStream.ToArray());
                    }
                }
            }
        }
    }
}
