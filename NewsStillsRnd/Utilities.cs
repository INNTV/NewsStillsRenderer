using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace NewsStillsRnd
{
    class Utilities
    {
        public static StreamReader Renderer(RenderObject Obj)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "\"" + Obj.AeRenderPath + "\"";

            DirectoryInfo Dir = new DirectoryInfo(Obj.DestDirectory);
            if (!Dir.Exists)
            {
                Dir.Create();
            }


            proc.StartInfo.Arguments = " -project " + "\"" + Obj.AeProjectPath + "\"" + "   -comp   \"" + Obj.CompositionName + "\" -output " + "\"" + Obj.DestFullFileName + "\"";
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.EnableRaisingEvents = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;

            if (!proc.Start())
            {
                return null;
            }

            StreamReader reader = proc.StandardOutput;
            return reader;

        }

    }
    public class RenderObject
    {
        public string AeRenderPath { get; set; }
        public string DestDirectory { get; set; }
        public string DestFullFileName { get; set; }
        public string AeProjectPath { get; set; }
        public string CompositionName { get; set; }
    }


}
