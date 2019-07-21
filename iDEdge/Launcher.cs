using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iDEdge.Netease;

namespace iDEdge.Launcher
{
    class Launcher
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                Process.Start("idedge.wizard.exe");
            Nease.Main(args);
        }
    }
}
