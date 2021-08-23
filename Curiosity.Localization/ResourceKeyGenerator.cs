using Curiosity.Tools.Hashing;

namespace Curiosity.Localization
{
    public static class ResourceKeyGenerator
    {
        public static string Generate(string prefix, string text)
        {
            return $"{prefix}_{XXHasher.ComputeStringHash(text)}".ToLower();
        }
    }
}