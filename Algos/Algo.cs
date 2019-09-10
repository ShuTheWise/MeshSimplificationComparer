using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MeshSimplificationComparer
{
    public abstract class Algo : ITimeReporter
    {
        public struct AlgoOptions
        {
            public readonly Workspace workspace;
            public readonly string name;

            public AlgoOptions(Workspace workspace, string name)
            {
                this.workspace = workspace;
                this.name = name;
            }
        }
        public readonly AlgoOptions ao;

        public Results results = new Results();
        public virtual Ext ext => Ext.obj;

        public Algo(AlgoOptions algoOptions)
        {
            ao = algoOptions;
        }

        public void Run(AlgoStep step, string inputPath, string outputPath)
        {
            Run_Impl(step, inputPath, outputPath);
            if (!File.Exists(outputPath))
            {
                throw new Exception("Failed to produce output");
            }
        }

        public void ComputeResults(AlgoStep step, string inputPath, string outputPath, string outputPathTxt)
        {
            var hdLog = Program.Path(Subfolder.none, $"result_hausdorff_{ao.name}_{step.qualityStr}", Ext.txt);

            if (File.Exists(hdLog))
                File.Delete(hdLog);

            var hd = MeshlabWorkspace.Instance.RunFilter_HausdorffDistance(inputPath, outputPath, hdLog);
            if (File.Exists(outputPathTxt))
            {
                hd.time = long.Parse(File.ReadAllText(outputPathTxt));
            }

            Results.Result res = new Results.Result()
            {
                path = outputPath,
                step = step,
                hd = hd
            };

            results.bboxDiag = res.hd.bboxDiag;
            results.AddResult(res);

            if (File.Exists(hdLog))
                File.Delete(hdLog);
        }

        public class Plot
        {
            public string name { get; set; }
            public string value { get; set; }
        }

        public List<Plot> PlotResults()
        {
            string header = "\n%" + ao.name + "\n" + $@"\addplot coordinates " + "{\n";

            List<Plot> plots = new List<Plot>();
            int plotsNum = 3;
            for (int i = 0; i < plotsNum; i++)
            {
                plots.Add(new Plot { value = header });
            }

            plots[0].name = "mean";
            plots[1].name = "max";
            plots[2].name = "rms";

            foreach (var item in results.results)
            {
                plots[0].value += $"({item.hd.tris},{item.hd.values.mean.Sanitize()})\n";
                plots[1].value += $"({item.hd.tris},{item.hd.values.max.Sanitize()})\n";
                plots[2].value += $"({item.hd.tris},{item.hd.values.rms.Sanitize()})\n";
            }
            string footer = "};";
            for (int i = 0; i < plotsNum; i++)
            {
                plots[i].value += footer;
            }
            return plots;
        }

        public class Results
        {
            public float bboxDiag;
            public readonly List<Result> results = new List<Result>();

            public void AddResult(Result res)
            {
                results.Add(res);
            }

            public class Result
            {
                public string path;
                public AlgoStep step;
                public HausdorffDistance hd;
            }

            public string GetCSVTable(string sep = ",", string lineEnd = "")
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(HausdorffDistance.Header(sep, lineEnd));
                foreach (var item in results)
                {
                    sb.AppendLine(item.hd.Row(sep, lineEnd));
                }
                return sb.ToString();
            }

            public string GetLatexTable(string rowtitle, string sep = "\t&\t", string lineEnd = " \\\\")
            {
                var hline = "\\hline";

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("\\rowtitle{" + rowtitle + "}\\\\");
                sb.AppendLine(hline);
                foreach (var item in results)
                {
                    var row = item.hd.Row(sep, lineEnd);
                    sb.AppendLine(row);
                }
                sb.AppendLine(hline);
                return sb.ToString();
            }
        }

        protected abstract void Run_Impl(AlgoStep step, string inputPath, string outputPath);

        public string startMessage => $"\n------------\nAlgo: {ao.name}\n------------\n";

        public long time { get; set; }
    }
}