using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;

namespace AISSystem
{
    public class HashHelper
    {
        /// <summary>
        /// Create random salt
        /// </summary>
        /// <returns></returns>
        public static string CreateRandomSalt()
        {
            Byte[] saltBytes = new Byte[4];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(saltBytes);

            return Convert.ToBase64String(saltBytes);
        }

        /// <summary>
        /// Compute salted password hash
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static string ComputeSaltedHash(string password, string salt)
        {
            // Create Byte array of password string
            UnicodeEncoding encoder = new UnicodeEncoding();
            Byte[] secretBytes = encoder.GetBytes(password);

            // Create a new salt
            Byte[] saltBytes = Convert.FromBase64String(salt);

            // append the two arrays
            Byte[] toHash = new Byte[secretBytes.Length + saltBytes.Length];
            Array.Copy(secretBytes, 0, toHash, 0, secretBytes.Length);
            Array.Copy(saltBytes, 0, toHash, secretBytes.Length, saltBytes.Length);

            SHA512 sha512 = SHA512.Create();
            Byte[] computedHash = sha512.ComputeHash(toHash);

            return Convert.ToBase64String(computedHash);
        }

        public static  string EncodePassword(string passtext, string passwordSalt)
        {
            //byte[] bytePASS = Encoding.Unicode.GetBytes(passtext);
            //byte[] byteSALT = Convert.FromBase64String(passwordSalt);
            //byte[] byteRESULT = new byte[byteSALT.Length + bytePASS.Length + 1];

            //System.Buffer.BlockCopy(byteSALT, 0, byteRESULT, 0, byteSALT.Length);
            //System.Buffer.BlockCopy(bytePASS, 0, byteRESULT, byteSALT.Length, bytePASS.Length);

            //HashAlgorithm ha = HashAlgorithm.Create(Membership.HashAlgorithmType);
            //return (Convert.ToBase64String(ha.ComputeHash(byteRESULT)));

            // Create a new instance of the hash crypto service provider.
            HashAlgorithm hashAlg = new SHA256CryptoServiceProvider();
            // Convert the data to hash to an array of Bytes.
            byte[] bytValue = System.Text.Encoding.UTF8.GetBytes(passtext );
            // Compute the Hash. This returns an array of Bytes.
            byte[] bytHash = hashAlg.ComputeHash(bytValue);
            // Optionally, represent the hash value as a base64-encoded string, 
            // For example, if you need to display the value or transmit it over a network.
            string base64 = Convert.ToBase64String(bytHash);

            return base64;

        }
    }
}
