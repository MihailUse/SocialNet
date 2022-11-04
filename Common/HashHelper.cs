using Isopoh.Cryptography.Argon2;

namespace Common
{
    public class HashHelper
    {
        public static string GetHash(string value) => Argon2.Hash(value, parallelism: Environment.ProcessorCount);
        public static bool Verify(string hash, string value) => Argon2.Verify(hash, value);
    }
}