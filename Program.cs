using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;

namespace MeshSimplificationComparer
{
    public delegate void AlgosStepsDel(TimeReportCollection<Algo> algos, TimeReportCollection<AlgoStep> steps);

    public class Program
    {
        private static string originalModelName = "original";

        private static string WorkingDir { get; set; }
        public static string InputFilename { get; private set; }

        public static string Path(Subfolder subfolder, string name, Ext ext = Ext.stl)
        {
            return Path(subfolder, name, ext.ToString());
        }

        private static string Path(Subfolder subfolder, string name, string ext)
        {
            var subFolder = "";
            if (subfolder != Subfolder.none)
            {
                subFolder += $"\\{subfolder}";
            }
            var dir = $"{WorkingDir}{subFolder}";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return $"{dir}\\{name}.{ext}";
        }

        private static void PlaySound(int repeats, SystemSound systemSound, int sleepTime = 1000)
        {
            for (int i = 0; i < repeats; i++)
            {
                systemSound.Play();
                if (i - 1 < repeats)
                    System.Threading.Thread.Sleep(sleepTime);
            }
        }

        static void Main(string[] args)
        {
            //if (args.Length < 1)
            //{
            //    Logger.WriteLine("No input args");
            //    return;
            //}
            bool generateOriginalModelRenders = false;
            foreach (var item in args)
            {
                if (item == "-g")
                {
                    generateOriginalModelRenders = true;
                    break;
                }
            }
            var dirPath = $".\\Models";
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            var allModels = Directory.GetFiles(dirPath);
            var modelsToRunPath = GetUsersChoice(allModels.ToList(), "models");
            foreach (var modelPath in modelsToRunPath)
            {
                Logger.Flush();
                RunMyAlgos(modelPath, generateOriginalModelRenders);
                Logger.WriteToFile(Path(Subfolder.none, $"log_{Regex.Match(modelPath, @"\w+(?=\.\w+$)")}", Ext.txt));
            }
        }

        private static void RunMyAlgos(string inputFile, bool generateBlender)
        {
            Logger.Write("Enter name:");
            InputFilename = Console.ReadLine();
            PlaySound(1, SystemSounds.Hand);
            if (!Regex.IsMatch(InputFilename, @"^\w+$"))
            {
                Logger.WriteLine("Wrong input");
                return;
            }

            var v = System.Configuration.ConfigurationSettings.AppSettings["verbose"];
            bool.TryParse(v, out Cli.verbose);

            WorkingDir = $@"{Environment.CurrentDirectory}\mc_output\{InputFilename}";
            var qualitySteps = new List<float> { 0.5f, 0.25f, 0.1f, 0.05f, 0.02f, 0.01f };
            qualitySteps.Reverse();
            //create an obj copys
            var inputObjPath = Path(Subfolder.none, originalModelName, Ext.obj);
            var inputOffPath = Path(Subfolder.none, originalModelName, Ext.off);
            var inputStlPath = Path(Subfolder.none, originalModelName, Ext.stl);
            var inputPlyPath = Path(Subfolder.none, originalModelName, Ext.ply);
            var cleanLog = Path(Subfolder.none, "logfile_meshlab_clean", Ext.txt);
            var allAlgos = InitializeAlgosAndWorkspaces();

            if (!File.Exists(inputObjPath) || !File.Exists(cleanLog))
                MeshlabWorkspace.Instance.CleanConvert(inputFile, inputObjPath, cleanLog);

            if (generateBlender)
            {
                //generate images for original model
                BlenderWorkspace.Instance.RenderToImage(inputObjPath, Path(Subfolder.none, originalModelName, Ext.png));
                return;
            }

            if (!File.Exists(inputStlPath))
                MeshlabWorkspace.Instance.Run($"-i {inputObjPath} -o {inputStlPath}");
            if (!File.Exists(inputOffPath))
                MeshlabWorkspace.Instance.Run($"-i {inputObjPath} -o {inputOffPath}");
            if (!File.Exists(inputPlyPath))
                MeshlabWorkspace.Instance.Run($"-i {inputObjPath} -o {inputPlyPath}");

            List<AlgoStep> steps = GetSteps(qualitySteps, cleanLog);

            var algosToRun = new TimeReportCollection<Algo>(GetUsersChoice(allAlgos, "programs"));
            var operationsToRun = GetUsersChoice(DeclareOperations(), "operations");

            foreach (var item in operationsToRun)
            {
                Logger.WriteLine($"------------------------\nPerforming: {item.desc}\n------------------------");
                item.algosStepsDel(algosToRun, new TimeReportCollection<AlgoStep>(steps));
            }


            Logger.WriteLine("Finished");
            PlaySound(3, SystemSounds.Exclamation);
        }

        private static List<AlgoStep> GetSteps(List<float> qualitySteps, string cleanLog)
        {
            var steps = new List<AlgoStep>();

            int initialTrisCount = GetTrisCount(cleanLog);

            foreach (var qualityStep in qualitySteps)
            {
                steps.Add(new AlgoStep(initialTrisCount, qualityStep));
            }

            return steps;
        }

        public class AlgosStepsDelDesc
        {
            public readonly AlgosStepsDel algosStepsDel;
            public readonly string desc;

            public AlgosStepsDelDesc(AlgosStepsDel algosStepsDel, string desc)
            {
                this.algosStepsDel = algosStepsDel;
                this.desc = desc;
            }

            public override string ToString()
            {
                return desc;
            }
        }

        private static List<AlgosStepsDelDesc> DeclareOperations()
        {
            var list = new List<AlgosStepsDelDesc>();
            list.Add(new AlgosStepsDelDesc(RunSimplification, "Run simplification algorithms to create simplified meshes"));
            list.Add(new AlgosStepsDelDesc(RunOutputUnifier, "Run unification of output mesh formats (all to .obj)"));
            list.Add(new AlgosStepsDelDesc(RunData, "Run metrics on the simplified meshes (Hausdorff Distance Calcuation)"));
            list.Add(new AlgosStepsDelDesc(RunRenders, "Render images of the simplified meshes (Blender render)"));
            return list;
        }

        private static List<T> GetUsersChoice<T>(List<T> all, string name) where T : class
        {
            Logger.WriteLine($"{all.Count} {name} found: ");
            for (int i = 0; i < all.Count; i++)
            {
                Logger.WriteLine($"{i}: {all[i]}");
            }
            Logger.Write($"Enter which ones you want to run (type separated indexes or press [enter] to run all: ");

            PlaySound(1, SystemSounds.Hand);
            var fl = Console.ReadLine();
            var trim = fl.Trim();
            if (trim == "")
            {
                return all;
            }
            else
            {
                var indexes = new List<int>();
                foreach (Match i in Regex.Matches(trim, @"\d+"))
                {
                    int index;
                    if (int.TryParse(i.Value, out index))
                    {
                        if (index > -1 && index < all.Count)
                        {
                            if (!indexes.Contains(index))
                                indexes.Add(index);
                        }
                    }
                }

                var choice = new List<T>();
                foreach (var i in indexes)
                {
                    choice.Add(all[i]);
                }

                return choice;
            }
        }

        public static Dictionary<string, string> scripts { get; private set; }

        private static List<Algo> InitializeAlgosAndWorkspaces()
        {
            string SCRIPTS_PATH = ".\\Scripts";
            var helperFiles = new Dictionary<string, string>();
            foreach (var item in Directory.GetFiles(SCRIPTS_PATH))
            {
                //pattern matches filename with extension from path string
                var match = Regex.Match(item, @"\w+\.\w+$");
                if (match.Success)
                {
                    //pattern matches name without extension from ilename with extension string
                    helperFiles.Add(match.Value, item);
                }
            }
            scripts = helperFiles;

            MeshlabWorkspace meshlabWorkspace = new MeshlabWorkspace("meshlab");
            BlenderWorkspace blenderWorkspace = new BlenderWorkspace("blender");
            PerlWorkspace perlWorkspace = new PerlWorkspace("perl");

            //other workspaces
            Workspace qslimWorkspace = new Workspace("qslim");
            Workspace openFlipperWorkspace = new Workspace("openflipper");
            Workspace myQemWorkspace = new Workspace("myqem");
            Workspace foglemanWorkSpace = new Workspace("foglemanqem");
            Workspace cgalWorkspace = new Workspace("cgal");
            Workspace pmWorkspace = new Workspace("pm");

            List<Algo> allAlgos = new List<Algo>();

            //meshlab qem
            allAlgos.Add(new MeshLabQem(new Algo.AlgoOptions(
                meshlabWorkspace,
                "MeshlabQem"
                )));

            //fogleman
            allAlgos.Add(new FoglemanQem(new Algo.AlgoOptions(
                foglemanWorkSpace,
                "Fogleman"
                )));

            //my qem
            allAlgos.Add(new MyQem(new Algo.AlgoOptions(
                myQemWorkspace,
                "MyQem"
                )));

            //open flipper
            var ofo = new OpenFlipper.OpenFlipperOptions
            {
                order = 0,
                type = 0
            };

            allAlgos.Add(new OpenFlipper(new Algo.AlgoOptions(
                openFlipperWorkspace,
                "OpenFlipperDist"
                ), ofo));

            //blender
            allAlgos.Add(new Blender(new Algo.AlgoOptions(
                blenderWorkspace,
                "Blender"
                )));

            //new OpenFlipper(new Algo.AlgoOptions{path = OPEN_FLIPPER_PATH, color= "green", mark ="*", name = "OpenFlipperNormal"}, new OpenFlipper.OpenFlipperOptions{order = 1, type = 0}),
            //new OpenFlipper(new Algo.AlgoOptions{path = OPEN_FLIPPER_PATH, color= "black", mark ="x", name = "OpenFlipperEdge"}, new OpenFlipper.OpenFlipperOptions{order = 2, type = 0}),

            //qslim
            allAlgos.Add(new QSlim(new Algo.AlgoOptions(
                qslimWorkspace,
                "QSlim"
                )));

            allAlgos.Add(new SimplifyCGAL(new Algo.AlgoOptions(
                cgalWorkspace,
                "SimplifyCGAL"
                )));

            allAlgos.Add(new ProgressiveMeshes(new Algo.AlgoOptions(
                pmWorkspace,
                "PM"
                )));

            return allAlgos;
        }

        private static void RunOutputUnifier(TimeReportCollection<Algo> algos, TimeReportCollection<AlgoStep> steps)
        {
            var acceptableExt = Ext.obj;
            foreach (var algo in algos)
            {
                foreach (var step in steps)
                {
                    var ext = algo.ext;
                    if (ext != acceptableExt)
                    {
                        var inputPath = Path(Subfolder.meshes, $"{algo.ao.name}_{step.qualityStr}", algo.ext);
                        var outputPath = Path(Subfolder.meshes, $"{algo.ao.name}_{step.qualityStr}", acceptableExt);
                        MeshlabWorkspace.Instance.CleanConvert(inputPath, outputPath);

                        if (File.Exists(inputPath))
                        {
                            File.Delete(inputPath);
                        }
                    }
                }
            }
        }

        private static void RunSimplification(TimeReportCollection<Algo> algos, TimeReportCollection<AlgoStep> steps)
        {
            foreach (var algo in algos)
            {
                var logFile = Path(Subfolder.meshes, algo.ao.name, Ext.txt);
                if (File.Exists(logFile))
                {
                    File.Delete(logFile);
                }
                foreach (var step in steps)
                {
                    var outputPath = Path(Subfolder.meshes, $"{algo.ao.name}_{step.qualityStr}", algo.ext);
                    var outputPathTxt = Path(Subfolder.logs, $"{algo.ao.name}_{step.qualityStr}", Ext.txt);
                    var inputPath = Path(Subfolder.none, originalModelName, algo.ext);
                    Stopwatch sw = Stopwatch.StartNew();
                    algo.Run(step, inputPath, outputPath);
                    File.WriteAllText(outputPathTxt, sw.ElapsedMilliseconds.ToString());
                    //File.AppendAllText(logFile, $"{step.quality.Sanitize()} {step.StopTimer()}\n");
                }
            }

        }

        private static void RunRenders(TimeReportCollection<Algo> algos, TimeReportCollection<AlgoStep> steps)
        {
            foreach (var algo in algos)
            {
                foreach (var step in steps)
                {
                    var filename = $"{algo.ao.name}_{step.qualityStr}";
                    var outputPath = Path(Subfolder.meshes, filename, Ext.obj);
                    var renderOutputPath = Path(Subfolder.renders, filename, Ext.png);
                    BlenderWorkspace.Instance.RenderToImage(outputPath, renderOutputPath, step.quality <= 0.05);
                }
            }
        }

        private static void RunData(TimeReportCollection<Algo> algos, TimeReportCollection<AlgoStep> steps)
        {
            var tablesPath = Path(Subfolder.data, "table", Ext.tex);
            foreach (var item in Directory.GetFiles($"{WorkingDir}\\{Subfolder.data}"))
            {
                File.Delete(item);
            }

            var algosArray = algos.ToArray();
            string legend = "\\legend{";
            for (int i = 0; i < algosArray.Length; i++)
            {
                Algo alg = algosArray[i];
                if (i > 0)
                    legend += ",";
                legend += alg.ao.name;
            }
            legend += "}";

            var plotPaths = new HashSet<string>();

            foreach (var algo in algos)
            {
                foreach (var step in steps)
                {
                    var outputPath = Path(Subfolder.meshes, $"{algo.ao.name}_{step.qualityStr}", Ext.obj);
                    var outputPathTxt = Path(Subfolder.logs, $"{algo.ao.name}_{step.qualityStr}", Ext.txt);
                    var inputPath = Path(Subfolder.none, originalModelName, algo.ext);
                    algo.ComputeResults(step, inputPath, outputPath, outputPathTxt);
                }

                //convert to csv and and latexv
                //string hausdorffLogFileName = $"results_hausdorff_{algo.ao.name}";
                //var csvPath = Path(Subfolder.data, hausdorffLogFileName, Ext.csv);
                //var texPath = Path(Subfolder.data, hausdorffLogFileName, Ext.tex);
                //save csv and and latex
                //File.AppendAllText(csvPath, algo.results.GetCSVTable());

                var latexTab = algo.results.GetLatexTable(algo.ao.name);
                //File.AppendAllText(texPath, latexTab);
                File.AppendAllText(tablesPath, latexTab);

                var r = algo.PlotResults();
                foreach (Algo.Plot item in r)
                {
                    var plotPath = Path(Subfolder.data, item.name, Ext.tex);
                    if (!plotPaths.Contains(plotPath))
                    {
                        plotPaths.Add(plotPath);
                    }
                    File.AppendAllText(plotPath, item.value);
                }

                var r2 = algo.PlotTimeResults();
                var plotPathTimeResults = Path(Subfolder.data, r2.name, Ext.tex);
                File.AppendAllText(plotPathTimeResults, r2.value);
            }

            foreach (var plotPath in plotPaths)
            {
                File.AppendAllText(plotPath, '\n' + legend);
            }
        }

        public static int GetTrisCount(string cleanLog)
        {
            int trisCount;
            var text = File.ReadAllText(cleanLog);
            var m = Regex.Match(text, @"\d+(?= fn\))");
            trisCount = int.Parse(m.Value);
            return trisCount;
        }
    }
}