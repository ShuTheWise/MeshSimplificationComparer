using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MeshSimplificationComparer
{
    class OpenFlipper : Algo
    {
        private readonly OpenFlipperOptions openFlipperOptions;
        public struct OpenFlipperOptions
        {
            public int type;
            public int order;
        }

        public OpenFlipper(AlgoOptions ao, OpenFlipperOptions openFlipperOptions) : base(ao)
        {
            this.openFlipperOptions = openFlipperOptions;
        }

        protected override void Run_Impl(AlgoStep step, string inputPath, string outputPath)
        {
            var fileName = Regex.Match(inputPath, @"\w+(?:\.\w+)*$").Value;
            var ofs = GenerateOFS(step.faceCount, fileName, outputPath, openFlipperOptions.type, openFlipperOptions.order);
            var ofsPath = Program.Path(Subfolder.none,"decimator", Ext.ofs);
            File.WriteAllText(ofsPath, ofs);
            Cli.Run($@"–no-splash {inputPath} {ofsPath}", ao.workspace.exePath);
        }

        private static string GenerateOFS(int tris, string name, string output, int type = 0, int order = 0)
        {
            string sb = "// creating a scripting object\n" +
            "var constraints = new Object()\n" +
            "// filling the QVariantMap\n" +
            $"constraints[\"decimater_type\"] = {type} \n" +
            $"constraints[\"decimation_order\"] = {order}\n" +
            $"constraints[\"triangles\"] = {tris}\n" +
            "// load e.g. mesh object\n" +
            $"var id = core.getObjectId(\"{name}\")\n" +
            "// perform decimation\n" +
            "decimater.decimate(id,constraints)\n" +
            $"core.saveObject(id,\"{output.Replace("\\", "\\\\")}\")\n" +
            $"core.exitApplication()";
            return sb;
        }
    }
}
