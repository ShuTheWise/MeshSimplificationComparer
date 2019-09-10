namespace MeshSimplificationComparer
{
    public class QSlim : Algo
    {
        public QSlim(AlgoOptions ao) : base(ao)
        {
        }

        protected override void Run_Impl(AlgoStep step, string inputPath, string outputPath)
        {
            Cli.Run($"-t {step.faceCount} -o {outputPath} {inputPath}", ao.workspace.exePath);
        }
    }
}