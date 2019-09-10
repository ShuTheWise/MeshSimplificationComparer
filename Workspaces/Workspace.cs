using System.Configuration;
using System.Diagnostics;

namespace MeshSimplificationComparer
{
    public class Workspace
    {
        public readonly string exePath;

        public Workspace(string key)
        {
            var appsettings = ConfigurationSettings.AppSettings;
            exePath = appsettings[key];
        }

        public virtual void Run(string args)
        {
            Cli.Run(args, exePath);

        }
        public string RunWithOutput(string args)
        {
            return Cli.RunWithOutput(args, exePath);
        }
    }
}