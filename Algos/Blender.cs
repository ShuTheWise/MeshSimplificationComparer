using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MeshSimplificationComparer
{
    public class Blender : Algo
    {
        private BlenderWorkspace blenderWorkspace;

        public Blender(AlgoOptions algoOptions) : base(algoOptions)
        {
            blenderWorkspace = (BlenderWorkspace)algoOptions.workspace;
        }

        protected override void Run_Impl(AlgoStep step, string inputPath, string outputPath)
        {
            var args = $@"-P {Program.scripts["decimate.py"]} -- --nfaces {step.faceCount} --inm {inputPath} --outm {outputPath}";
            Cli.Run(args, ao.workspace.exePath);
        }
    }
}
