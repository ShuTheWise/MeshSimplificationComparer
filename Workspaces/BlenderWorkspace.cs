using System;
using System.Collections.Generic;
using System.IO;

namespace MeshSimplificationComparer
{
    public class BlenderWorkspace : Workspace
    {
        public BlenderWorkspace(string path) : base(path)
        {
            Instance = this;
        }

        public static BlenderWorkspace Instance { get; private set; }

        public void RenderToImage(string inputPath, string renderOutputPath, bool wire = false)
        {
            var scriptPath = Program.scripts[wire ? "renderWire.py" : "render.py"];
            var blendPath = Program.scripts["render.blend"];

            AssertFile(scriptPath);
            AssertFile(blendPath);

            var args = $"{blendPath} -P {scriptPath} -- --inm {inputPath} --outm {renderOutputPath}";
            //Logger.WriteLine(args);
            Run(args);
        }

        private void AssertFile(string path)
        {
            if (File.Exists(path))
                return;

            throw new FileNotFoundException("Missing file", path);
        }
    }
}