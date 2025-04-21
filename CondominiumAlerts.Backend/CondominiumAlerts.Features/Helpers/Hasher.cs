using System.Security.Cryptography;

namespace CondominiumAlerts.Features.Helpers;

public static class Hasher
{
    private const int SaltSize = 128 / 8;
    private const int KeySize = 256 / 8;
    private const int Iterations = 1000;
    private static readonly HashAlgorithmName _algorithmName = HashAlgorithmName.SHA256;

    public static string HashString(string word)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(word, salt, Iterations, _algorithmName, KeySize);
        return string.Join(";", Convert.ToBase64String(salt), Convert.ToBase64String(hash));
    }
}