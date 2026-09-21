using System;
using System.Security.Cryptography;
using System.Text;

namespace Dextor.API.Models.Security
{
    public class PdsaHash
    {
        public enum PdsaHashType
        {
            MD5
        }

        private readonly PdsaHashType _hashType;

        public PdsaHash(PdsaHashType hashType)
        {
            _hashType = hashType;
        }

        public string CreateHash(string plainText, string salt)
        {
            // CRACKED: Salt comes FIRST, followed by the plain text password
            string dataToHash = salt + plainText;

            using (var md5 = MD5.Create())
            {
                // CRACKED: UTF8 Encoding
                byte[] bytes = Encoding.UTF8.GetBytes(dataToHash);
                byte[] hashBytes = md5.ComputeHash(bytes);

                // CRACKED: Base64 Output
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}