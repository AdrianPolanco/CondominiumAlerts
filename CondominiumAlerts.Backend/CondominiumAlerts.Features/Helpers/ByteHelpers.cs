using System.Security.Cryptography;

namespace CondominiumAlerts.Features.Helpers
{
    public static class ByteHelpers
    {
        public static string GenerateBase64String(int length)
        {
            // Convert to base64 and take only first 11 characters
            string base64 = Convert.ToBase64String(RandomNumberGenerator.GetBytes(length));
            return base64.Substring(0, length);
        }
    }
}