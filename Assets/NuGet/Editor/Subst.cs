using System;
using System.Diagnostics;
using System.IO;

namespace NugetForUnity
{
    public class Subst : IDisposable
    {
        public const string letter = "P";
        private static int count;
        private static string basePath;

        private string path;

        public Subst(string path)
        {
            path = Path.GetFullPath(path);
            if (path.EndsWith("\\"))
            {
                path = path.TrimEnd('\\');
            }

            if (count == 0)
            {
                this.path = path;
                basePath = path;

                Execute($"{letter}: \"{path}\"");
            }
            else if (path.IndexOf(basePath, StringComparison.OrdinalIgnoreCase) != 0)
            {
                throw new Exception("Path must be under basepath");
            }

            count++;
        }

        private int Execute(string args)
        {
            var p = Process.Start(new ProcessStartInfo
            {
                FileName = "subst",
                Arguments = args,
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            });
            p.WaitForExit();
            return p.ExitCode;
        }

        public string MappedRoot => $"{letter}:\\";

        public string GetMappedPath(string path)
        {
            path = Path.GetFullPath(path);
            if (path.IndexOf(basePath, StringComparison.OrdinalIgnoreCase) != 0)
            {
                throw new Exception("Path must be under basepath");
            }

            var relPath = path.Substring(basePath.Length).TrimStart('\\');
            return $"{letter}:\\{relPath}";
        }

        public void Dispose()
        {
            count--;

            if (count == 0)
            {
                Execute($"{letter}: /D");
            }
        }
    }
}