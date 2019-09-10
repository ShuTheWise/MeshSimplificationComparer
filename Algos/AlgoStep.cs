using System;

namespace MeshSimplificationComparer
{
    public class AlgoStep : ITimeReporter
    {
        public long time { get; set; }

        public readonly float quality;
        public readonly int faceCount;
        public readonly string qualityStr;

        public AlgoStep(int initialTrisCount, float quality)
        {
            this.quality = quality;
            faceCount = (int)(initialTrisCount * quality);
            qualityStr = quality.Sanitize();
            qualityStr = qualityStr.Replace(".", "");
        }

        public string startMessage => $"Step:\t{quality.Sanitize()}\t->\t{faceCount} faces\t";
    }
}