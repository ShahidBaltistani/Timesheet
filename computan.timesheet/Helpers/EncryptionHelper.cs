using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace computan.timesheet.Helpers
{
    public static class EncryptionHelper
    {
        private static readonly string key = ConfigurationManager.AppSettings["secretkey"];

        public static string EncryptString(string plainText, byte[] key)
        {
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Padding = PaddingMode.PKCS7;
                aes.KeySize = 256; // in bits
                aes.Key = key;
                aes.IV = new byte[128 / 8];

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        public static string DecryptString(string cipherText, byte[] key)
        {
            //byte[] generatedkey = Encoding.UTF8.GetBytes(key).ToArray();
            byte[] buffer = Convert.FromBase64String(cipherText).ToArray();
            string value;
            using (Aes aes = Aes.Create())
            {
                aes.Padding = PaddingMode.PKCS7;
                aes.KeySize = 128; // in bits
                aes.Key = key;
                aes.IV = new byte[128 / 8];

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            value = streamReader.ReadToEnd();
                        }
                    }
                }
            }

            return value;
        }

        public static byte[] GenerateKey()
        {
            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
            byte[] randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }

            return randomBytes;
        }

        public static string GuidPassword()
        {
            Guid guid = Guid.NewGuid();
            string newpass = guid.ToString();
            return newpass;
        }
    }
}