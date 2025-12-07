using System.Security.Cryptography;
using System.Text;

namespace Vts_Ecommerce.Helpers
{
    /// <summary>
    /// Encryption Service for Two-Way AES Encryption
    /// Used for password encryption/decryption
    /// </summary>
    public static class EncryptionService
    {
        // Encrypt plain text using AES-256 CBC. Returns base64 of IV + cipher bytes.
        public static string Encrypt(string plainText, string key = null)
        {
            if (plainText == null)
                throw new ArgumentNullException(nameof(plainText));

            var keyBytes = GetKeyBytes(key);

            using (var aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.GenerateIV();
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var ms = new MemoryStream())
                {
                    // write IV first
                    ms.Write(aes.IV, 0, aes.IV.Length);

                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs, Encoding.UTF8))
                    {
                        sw.Write(plainText);
                    }

                    var result = ms.ToArray();
                    return Convert.ToBase64String(result);
                }
            }
        }

        // Decrypt base64(IV + cipher) back to plain text
        public static string Decrypt(string cipherText, string key = null)
        {
            if (string.IsNullOrWhiteSpace(cipherText))
                return string.Empty;

            var keyBytes = GetKeyBytes(key);
            var fullCipher = Convert.FromBase64String(cipherText);

            using (var aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                // extract IV (first 16 bytes)
                var iv = new byte[aes.BlockSize / 8];
                Array.Copy(fullCipher, 0, iv, 0, iv.Length);
                aes.IV = iv;

                var cipher = new byte[fullCipher.Length - iv.Length];
                Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);

                using (var ms = new MemoryStream(cipher))
                using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs, Encoding.UTF8))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        private static byte[] GetKeyBytes(string key)
        {
            string finalKey = key;
            if (string.IsNullOrEmpty(finalKey))
            {
                // attempt to read from environment variable first
                finalKey = Environment.GetEnvironmentVariable("EncryptionKey");
            }

            if (string.IsNullOrEmpty(finalKey))
            {
                // fallback to a built-in default key length (NOT recommended for production)
                finalKey = "VtsDefaultEncryptionKey-ChangeMe!!2025";
            }

            // Ensure key is 32 bytes (256 bits)
            var keyBytes = Encoding.UTF8.GetBytes(finalKey);
            if (keyBytes.Length == 32)
                return keyBytes;

            var final = new byte[32];
            for (int i = 0; i < 32; i++)
            {
                final[i] = i < keyBytes.Length ? keyBytes[i] : (byte)0;
            }
            return final;
        }
    }
}

