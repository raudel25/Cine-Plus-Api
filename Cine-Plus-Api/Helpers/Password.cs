using System.Security.Cryptography;

namespace Cine_Plus_Api.Helpers;

public static class Password
{
    public static string EncryptPassword(string password)
    {
        var sal = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(sal);
        }

        var pbkdf2 = new Rfc2898DeriveBytes(password, sal, 10000, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(20);

        var hashBytes = new byte[36];
        Array.Copy(sal, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 20);

        return Convert.ToBase64String(hashBytes);
    }

    public static bool CheckPassword(string password,string confirm)
    {
        var hashBytes = Convert.FromBase64String(password);

        var sal = new byte[16];
        Array.Copy(hashBytes, 0, sal, 0, 16);
        var hash = new byte[20];
        Array.Copy(hashBytes, 16, hash, 0, 20);

        var pbkdf2 = new Rfc2898DeriveBytes(confirm, sal, 10000, HashAlgorithmName.SHA256);
        var hashPass = pbkdf2.GetBytes(20);

        for (var i = 0; i < 20; i++)
        {
            if (hash[i] != hashPass[i])
            {
                return false;
            }
        }

        return true;
    }
    
    public static string RandomPassword()
    {
        var alphabet = new List<string>(26 * 2 + 10);

        for (var i = 0; i < 25; i++)
        {
            alphabet.Add(((char)('A' + i)).ToString());
            alphabet.Add(((char)('a' + i)).ToString());
        }

        for (var i = 0; i < 10; i++)
        {
            alphabet.Add(i.ToString());
        }

        var result = "";
        var rnd = new Random();

        for (var i = 0; i < 8; i++) result += alphabet[rnd.Next(alphabet.Count)];

        return result;
    }
}