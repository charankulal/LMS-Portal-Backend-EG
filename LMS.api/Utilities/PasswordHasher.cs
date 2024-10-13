using System.Security.Cryptography;
using System.Text;

namespace LMS.api.Utilities
{
    public class PasswordHasher
    {
        public string ComputeHash(string input, HashAlgorithm algorithm, Byte[] salt)
        {
            Byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            // Combine salt and input bytes
            Byte[] saltedInput = new Byte[salt.Length + inputBytes.Length];
            salt.CopyTo(saltedInput, 0);
            inputBytes.CopyTo(saltedInput, salt.Length);

            Byte[] hashedBytes = algorithm.ComputeHash(saltedInput);


            StringBuilder hex = new StringBuilder(hashedBytes.Length * 2);
            foreach (byte b in hashedBytes)
                hex.AppendFormat("{0:X2}", b);

            return hex.ToString();

        }

        public  bool ValidatePassword(string inputPassword, string storedHash, Byte[] salt, HashAlgorithm algorithm)
        {
            string hashedInputPassword = ComputeHash(inputPassword, algorithm, salt);
            return hashedInputPassword.Equals(storedHash, StringComparison.OrdinalIgnoreCase);
        }

    }
}
