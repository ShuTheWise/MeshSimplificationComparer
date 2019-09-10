namespace MeshSimplificationComparer
{
    public class MeshLabQem : Algo
    {
        public MeshLabQem(AlgoOptions ao) : base(ao)
        {
        }

        protected override void Run_Impl(AlgoStep step, string inputPath, string outputPath)
        {
            var scriptPath = Program.Path(Subfolder.none, "filter_qem", Ext.mlx);
            var scriptContent = GetQEMScriptContent(step.faceCount);//, PlanarQuadric: true, QualityWeight: true, QualityThr: 0.5f);
            var logfilePath = Program.Path(Subfolder.none, $"logfile_{ao.name}", Ext.txt);

            var command = $"-l {logfilePath} -i {inputPath} -o {outputPath} -s {scriptPath}";
            MeshlabWorkspace.Instance.RunTemporaryMeshLabScript(scriptContent, scriptPath, command, logfilePath, true);
        }

        private static string GetQEMScriptContent(int TargetNumFaces, float QualityThr = 0.3f, bool PreserveBoundary = false, float BoundaryWeight = 1f, bool PreserveTopology = false, bool PlanarQuadric = false, bool QualityWeight = false)
        {
            string sb = "<!DOCTYPE FilterScript>\n" +
            "<FilterScript>\n" +
            " <filter name=\"Remove Duplicate Vertices\"/>\n" +
            //" <filter name=\"Remove Duplicate Faces\"/>\n" +
            " <filter name=\"Simplification: Quadric Edge Collapse Decimation\">\n" +
            $"  <Param tooltip=\"The desired final number of faces.\" description=\"Target number of faces\" value=\"{TargetNumFaces}\" type=\"RichInt\" name=\"TargetFaceNum\"/>\n" +
            "  <Param tooltip=\"If non zero, this parameter specifies the desired final size of the mesh as a percentage of the initial size.\" description=\"Percentage reduction (0..1)\" value=\"0\" type=\"RichFloat\" name=\"TargetPerc\"/>\n" +
            $"  <Param tooltip=\"Quality threshold for penalizing bad shaped faces.&lt;br>The value is in the range [0..1]&#xa; 0 accept any kind of face (no penalties),&#xa; 0.5  penalize faces with quality &lt; 0.5, proportionally to their shape&#xa;\" description=\"Quality threshold\" value=\"{QualityThr.Sanitize()}\" type=\"RichFloat\" name=\"QualityThr\"/>\n" +
            $"  <Param tooltip=\"The simplification process tries to do not affect mesh boundaries during simplification\" description=\"Preserve Boundary of the mesh\" value=\"{PreserveBoundary.ToLowerString()}\" type=\"RichBool\" name=\"PreserveBoundary\"/>\n" +
            $"  <Param tooltip=\"The importance of the boundary during simplification. Default (1.0) means that the boundary has the same importance of the rest. Values greater than 1.0 raise boundary importance and has the effect of removing less vertices on the border. Admitted range of values (0,+inf). \" description=\"Boundary Preserving Weight\" value=\"{BoundaryWeight.Sanitize()}\" type=\"RichFloat\" name=\"BoundaryWeight\"/>\n" +
            $"  <Param tooltip=\"Try to avoid face flipping effects and try to preserve the original orientation of the surface\" description=\"Preserve Normal\" value=\"false\" type=\"RichBool\" name=\"PreserveNormal\"/>\n" +
            $"  <Param tooltip=\"Avoid all the collapses that should cause a topology change in the mesh (like closing holes, squeezing handles, etc). If checked the genus of the mesh should stay unchanged.\" description=\"Preserve Topology\" value=\"{PreserveTopology.ToLowerString()}\" type=\"RichBool\" name=\"PreserveTopology\"/>\n" +
            $"  <Param tooltip=\"Each collapsed vertex is placed in the position minimizing the quadric error.&#xa; It can fail (creating bad spikes) in case of very flat areas. &#xa;If disabled edges are collapsed onto one of the two original vertices and the final mesh is composed by a subset of the original vertices. \" description=\"Optimal position of simplified vertices\" value=\"true\" type=\"RichBool\" name=\"OptimalPlacement\"/>\n" +
            $"  <Param tooltip=\"Add additional simplification constraints that improves the quality of the simplification of the planar portion of the mesh.\" description=\"Planar Simplification\" value=\"{PlanarQuadric.ToLowerString()}\" type=\"RichBool\" name=\"PlanarQuadric\"/>\n" +
            $"  <Param tooltip=\"Use the Per-Vertex quality as a weighting factor for the simplification. The weight is used as a error amplification value, so a vertex with a high quality value will not be simplified and a portion of the mesh with low quality values will be aggressively simplified.\" description=\"Weighted Simplification\" value=\"{QualityWeight.ToLowerString()}\" type=\"RichBool\" name=\"QualityWeight\"/>\n" +
            $"  <Param tooltip=\"After the simplification an additional set of steps is performed to clean the mesh (unreferenced vertices, bad faces, etc)\" description=\"Post-simplification cleaning\" value=\"true\" type=\"RichBool\" name=\"AutoClean\"/>\n" +
            $"  <Param tooltip=\"The simplification is applied only to the selected set of faces.&#xa; Take care of the target number of faces!\" description=\"Simplify only selected faces\" value=\"false\" type=\"RichBool\" name=\"Selected\"/>\n" +
            " </filter>\n" +
            "</FilterScript>\n";

            return sb;
        }
    }
}