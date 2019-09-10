using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeshSimplificationComparer
{
    public class FoglemanQem : Algo
    {
        public FoglemanQem(AlgoOptions algoOptions) : base(algoOptions)
        {
        }

        public override Ext ext => Ext.stl;


        protected override void Run_Impl(AlgoStep step, string inputPath, string outputPath)
        {
            Cli.Run($"-f {step.quality.Sanitize()} {inputPath} {outputPath}", ao.workspace.exePath);
        }
    }
}
