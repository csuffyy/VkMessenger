using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace VkData.Helpers
{
    public static class DataProtectionExtensions
    {
        public static string Protect(
            this string clearText,
            string optionalEntropy = null,
            DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            if (clearText == null)
                throw new ArgumentNullException(nameof(clearText));
            var clearBytes = Encoding.UTF8.GetBytes(clearText);
            var entropyBytes = string.IsNullOrEmpty(optionalEntropy)
                ? null
                : Encoding.UTF8.GetBytes(optionalEntropy);
            var encryptedBytes = Task.Run(() => ProtectedData.Protect(clearBytes, entropyBytes, scope));
            return Convert.ToBase64String(encryptedBytes.Result);
        }

        public static string Unprotect(
            this string encryptedText,
            string optionalEntropy = null,
            DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            if (encryptedText == null)
                throw new ArgumentNullException(nameof(encryptedText));
            var encryptedBytes = Convert.FromBase64String(encryptedText);
            var entropyBytes = string.IsNullOrEmpty(optionalEntropy)
                ? null
                : Encoding.UTF8.GetBytes(optionalEntropy);
            var clearBytes = ProtectedData.Unprotect(encryptedBytes, entropyBytes, scope);
            return Encoding.UTF8.GetString(clearBytes);
        }

        //public static string Protect(this string s) =>
        //   Task.Run(()=>s.Protect(s)).Result;

        public static Result<string> Unprotect(this Func<string> unsecure)
        {
            return new Try<string, CryptographicException>(
                () => unsecure().Unprotect(),
                _ => string.Empty);
        }
    }
}