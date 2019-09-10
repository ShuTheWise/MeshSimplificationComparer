using System;
using System.Diagnostics;

namespace MeshSimplificationComparer
{
    public class PerlWorkspace : Workspace
    {
        public static PerlWorkspace Instance { get; set; }

        public PerlWorkspace(string path) : base(path)
        {
            Instance = this;
        }
    }
}