using System.Security.Cryptography;
using System.Text;

namespace Catalog.Auth.Extensions
{
    public static class Hasher
    {
        public static string Hash(this string data)
        {
            SHA512 myHash = SHA512.Create();
            byte[] hash = myHash.ComputeHash(Encoding.ASCII.GetBytes(data)); //compute hash
            string val = Convert.ToBase64String(hash);
            return val;
        }
    }
}
