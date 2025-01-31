using System.Security.Cryptography;
using System.Text;

public static class Util {
    public static string GetSHA256(string text) {
        byte[] buffer = Encoding.UTF8.GetBytes(text);
        byte[] digest = SHA256.HashData(buffer);
        return Convert.ToHexString(digest);
    }
}