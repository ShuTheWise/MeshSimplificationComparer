using System.Globalization;

namespace MeshSimplificationComparer
{
    public static class FloatEx
    {
        //Format: Dot
        public static string Sanitize(this float s)
        {
            return s.ToString(new CultureInfo("en-US"));
        }

        public static string ToLowerString(this bool _bool)
        {
            return _bool.ToString().ToLower();
        }

        public static float Dec(this float s)
        {
            return float.Parse(s.ToString("0.##"));
        }
    }
}
