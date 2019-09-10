using System.IO;

namespace MeshSimplificationComparer
{
    public class MeshlabWorkspace : Workspace
    {
        public static MeshlabWorkspace Instance { get; private set; }

        public MeshlabWorkspace(string path) : base(path)
        {
            Instance = this;
        }

        public void RunTemporaryMeshLabScript(string scriptContent, string scriptPath, string command, string logfilePath = "", bool deleteFile = true)
        {
            CreateTemporaryMeshLabScriptFile(scriptPath, scriptContent);
            if (deleteFile && !string.IsNullOrEmpty(logfilePath) && File.Exists(logfilePath))
            {
                File.Delete(logfilePath);
            }
            Run(command);
        }

        public void CleanConvert(string inputPath, string outputPath, string logfilePath = "")
        {
            //Count hausdorff 
            var scriptName = "clean";
            var scriptContent = RemoveDuplicatesVerticesScript;
            var scriptPath = Program.Path(Subfolder.none, scriptName, Ext.mlx);

            //var projectPath = workingDir + @"\proj.mlp";
            string args = "";
            if(!string.IsNullOrEmpty(logfilePath))
            {
                args += $"-l {logfilePath} ";
            }
            args += $"-i {inputPath} -o {outputPath} -s {scriptPath}";
            RunTemporaryMeshLabScript(scriptContent, scriptPath, args, logfilePath, true);
        }

        public HausdorffDistance RunFilter_HausdorffDistance(string inputPath, string outputPath, string logfilePath)
        {
            if (File.Exists(logfilePath))
            {
                File.Delete(logfilePath);
            }

            //Count hausdorff 
            var scriptName = "filter_hausdorff";
            var scriptContent = HausdorffScriptContent;
            var scriptPath = Program.Path(Subfolder.none, scriptName, Ext.mlx);

            //var projectPath = workingDir + @"\proj.mlp";
            //logfilePath = GetFilePath("logfile_meshlab_hausdorff", Extension.txt);
            var command = $"-l {logfilePath} -i {outputPath} -i {inputPath} -s {scriptPath}";
            RunTemporaryMeshLabScript(scriptContent, scriptPath, command, logfilePath, false);

            var text = File.ReadAllText(logfilePath);

            return HausdorffDistance.CreateFromMeshLabLog(text);
        }

        public string CreateTemporaryMeshLabScriptFile(string path, string content)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.WriteAllText(path, content);
            File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.Hidden);
            return path;
        }

        public static string RemoveDuplicatesVerticesScript =
            "<!DOCTYPE FilterScript>\n" +
            "<FilterScript>\n" +
            " <filter name=\"Remove Duplicate Vertices\"/>\n" +
            " <filter name=\"Remove Duplicate Faces\"/>\n" +
            "</FilterScript>\n";

        public static string HausdorffScriptContent = "<!DOCTYPE FilterScript>\n" +
        "<FilterScript>\n" +
            " <filter name=\"Hausdorff Distance\">\n" +
            "  <Param value=\"1\" type=\"RichMesh\" name=\"SampledMesh\" tooltip=\"The mesh whose surface is sampled. For each sample we search the closest point on the Target Mesh.\" description=\"Sampled Mesh\"/>\n" +
            "  <Param value=\"0\" type=\"RichMesh\" name=\"TargetMesh\" tooltip=\"The mesh that is sampled for the comparison.\" description=\"Target Mesh\"/>\n" +
            "  <Param value=\"false\" type=\"RichBool\" name=\"SaveSample\" tooltip=\"Save the position and distance of all the used samples on both the two surfaces, creating two new layers with two point clouds representing the used samples.\" description=\"Save Samples\"/>\n" +
            "  <Param value=\"true\" type=\"RichBool\" name=\"SampleVert\" tooltip=\"For the search of maxima it is useful to sample vertices and edges of the mesh with a greater care. It is quite probably the the farthest points falls along edges or on mesh vertexes, and with uniform montecarlo sampling approachesthe probability of taking a sample over a vertex or an edge is theoretically null.&lt;br>On the other hand this kind of sampling could make the overall sampling distribution slightly biased and slightly affects the cumulative results.\" description=\"Sample Vertexes\"/>\n" +
            "  <Param value=\"false\" type=\"RichBool\" name=\"SampleEdge\" tooltip=\"See the above comment.\" description=\"Sample Edges\"/>\n" +
            "  <Param value=\"false\" type=\"RichBool\" name=\"SampleFauxEdge\" tooltip=\"See the above comment.\" description=\"Sample FauxEdge\"/>\n" +
            "  <Param value=\"false\" type=\"RichBool\" name=\"SampleFace\" tooltip=\"See the above comment.\" description=\"Sample Faces\"/>\n" +
            "  <Param value=\"2147483647\" type=\"RichInt\" name=\"SampleNum\" tooltip=\"The desired number of samples. It can be smaller or larger than the mesh size, and according to the choosed sampling strategy it will try to adapt.\" description=\"Number of samples\"/>\n" +
            "  <Param value=\"68.7969\" type=\"RichAbsPerc\" name=\"MaxDist\" tooltip=\"Sample points for which we do not find anything whithin this distance are rejected and not considered neither for averaging nor for max.\" max=\"137.6\" min=\"0\" description=\"Max Distance\"/>\n" +
            " </filter>\n" +
       "</FilterScript>\n";


    }
}