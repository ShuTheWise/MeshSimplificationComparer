using System.Reflection;
using System.Text.RegularExpressions;

namespace MeshSimplificationComparer
{
    public struct HausdorffDistance
    {
        public string name;
        public int tris;
        public int verts;
        public float bboxDiag;
        public long time;

        public Values values;

        public struct Values
        {
            public float min;
            public float max;
            public float mean;
            public float rms;

            public string Row(string s, string lineEnd = "") => $"{min.Sanitize()}{s}{max.Sanitize()}{s}{mean.Sanitize()}{s}{rms.Sanitize()}{lineEnd}";

            public static string Header(string s, string lineEnd = "") => $"Min{s}Max{s}Mean{s}Rms{lineEnd}";
        }

        public static string Header(string s, string lineEnd = "") => $"Triangles{s}Vertices{s}{Values.Header(s)}{s}Time{lineEnd}";

        public string Row(string s, string lineEnd = "") => $"{tris}{s}{verts}{s}{values.Row(s)}{s}{(time * 0.001f).Dec().Sanitize()}{lineEnd}";

        public static HausdorffDistance CreateFromMeshLabLog(string text)
        {
            //TODO: FIX Parsing from meshlab file
            var match = Regex.Match(text, "Sampled.*\n.*\n.*\n.*\n");
            var str = match.ToString();
            var identifier = "closest on";
            var name = Regex.Match(str, $@"{identifier}.*").ToString().Substring(identifier.Length).TrimEnd();
            var numbers = Regex.Matches(str, @"\d+\.\d+");

            HausdorffDistance hd;
            hd.name = name;
            hd.tris = int.Parse(Regex.Match(text, @"\d+(?= fn)").Value);
            hd.verts = int.Parse(Regex.Match(text, @"\d+(?= vn)").Value);
            //hd.points = int.Parse(Regex.Match(str, @"\d+(?= pts)").Value);

            hd.bboxDiag = ParseFloat(numbers[4].Value);
            hd.values.min = ParseFloat(numbers[5].Value);
            hd.values.max = ParseFloat(numbers[6].Value);
            hd.values.mean = ParseFloat(numbers[7].Value);
            hd.values.rms = ParseFloat(numbers[8].Value);
            hd.time = 0;

            return hd;
        }

        public static float ParseFloat(string s)
        {
            return float.Parse(s.Replace('.', ','));
        }

        //public string timeString
        //{
        //    get
        //    {
        //        if(time > )

        //        return time.ToString() + " ms";
        //    }
        //}
    }
}
