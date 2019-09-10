using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeshSimplificationComparer
{
    class MyQem : Algo
    {
        public override Ext ext => Ext.stl;

        public MyQem(AlgoOptions ao) : base(ao)
        {
        }

        protected override void Run_Impl(AlgoStep step, string inputPath, string outputPath)
        {
            Cli.Run($"{inputPath} {outputPath} -t {step.faceCount}", ao.workspace.exePath);
        }
    }
}
