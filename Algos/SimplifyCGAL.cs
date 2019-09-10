using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeshSimplificationComparer
{
    class SimplifyCGAL : Algo
    {
        public SimplifyCGAL(AlgoOptions algoOptions) : base(algoOptions)
        {
        }

        protected override void Run_Impl(AlgoStep step, string inputPath, string outputPath)
        {
            Cli.Run($"{inputPath} {outputPath} {step.quality.Sanitize()}", ao.workspace.exePath);
        }

        public override Ext ext => Ext.off;
    }
}
