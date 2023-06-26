using System.Text;

namespace AcesCore
{
    public static class IdGenerator
    {
        public static string GenerateGameId()
        {
            Random random = new();
            StringBuilder stringBuilder = new();
            for (int i = 0; i < 6; i++)
            {
                char character = (char)random.Next('A', 'Z');
                stringBuilder.Append(character);
            }

            return stringBuilder.ToString();
        }

        public static string GenerateGenericId(string prefix)
        {
            string key = string.Concat(Guid.NewGuid().ToString("N").Select(c => (char)(c + 17)));
            StringBuilder sb = new();
            for (int i = 0; i < key.Length; i += 2)
            {
                sb.Append(key[i]);
            }

            return $"{prefix}_{sb}";
        }
    }
}
