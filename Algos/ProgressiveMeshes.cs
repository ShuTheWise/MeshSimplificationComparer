using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MeshSimplificationComparer
{
    class ProgressiveMeshes : Algo
    {
        public ProgressiveMeshes(AlgoOptions algoOptions) : base(algoOptions)
        {
        }
        public override Ext ext => Ext.obj;

        private void Execute(string command, string filePath, string args)
        {
            Cli.RunWithOutput2($"/c {ao.workspace.exePath}\\{command}.exe {args} > {filePath}", "cmd.exe");
        }

        protected override void Run_Impl(AlgoStep step, string inputPath, string outputPath)
        {
            //path to for (*.m)
            var mFile = Regex.Replace(inputPath, @"\w+$", "m");

            if (!File.Exists(mFile))
            {
                //convert input to (*.m)
                var args = $"{Program.scripts["objtoMesh.pl"]} {inputPath}";
                var output = PerlWorkspace.Instance.RunWithOutput(args);
                File.WriteAllText(mFile, output);
            }
            //TODO : Assert file exits?
            var tmproot = Regex.Replace(inputPath, @"\.\w+$", "");
            var pm_File = $"{tmproot}.pm";
            if (!File.Exists(pm_File))
            {
                var base_m_File = $"{tmproot}.base.m";
                var prog_File = $"{tmproot}.prog";

                //reads the original mesh and randomly samples points over its surface,
                //progressively simplifies it by examining point residual distances, while recording changes to a *.prog file, and
                //writes the resulting base mesh.
                Execute("MeshSimplify", base_m_File, $"{mFile} -prog {prog_File} -simplify");

                var rprog_File = $"{tmproot}.rprog";
                //The next step is to reverse the sequence of stored edge collapses, i.e. forming a progressive sequence of vertex splits:
                Execute("reverselines", rprog_File, $"{tmproot}.prog");

                //We construct a concise progressive mesh by encoding the base mesh together with 
                //the sequence of vertex splits that exactly recover the original mesh:
                Execute("Filterprog", pm_File, $"-fbase {base_m_File} -fprog {rprog_File} -pm");

                ///view progressive mesh
                //Execute("G3dOGL", $"{tmproot}.txt", $"-pm_mode {pm_File}");
            }
            var output_m = $"{tmproot}_{step.qualityStr}.m";
            Execute("FilterPM", output_m, $"{pm_File} -nf {step.faceCount} -outmesh");

            var argsC = $"{Program.scripts["Meshtoobj.pl"]} {output_m}";
            var outputObj = PerlWorkspace.Instance.RunWithOutput(argsC);
            File.WriteAllText(outputPath, outputObj);
        }
    }
}
