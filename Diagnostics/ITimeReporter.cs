using System;
using System.Diagnostics;

namespace MeshSimplificationComparer
{
    public interface ITimeReporter
    {
        string startMessage { get; }
        long time { get; set; }
    }
}