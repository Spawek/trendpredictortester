using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsPlotter
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.Threading.Thread testThread = 
                new System.Threading.Thread(new System.Threading.ThreadStart(Test));
            testThread.Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        }

        static void Test()
        {
            System.Threading.Thread.Sleep(1000);
            Plotter plotter = Plotter.GetInstance();

            plotter.AddPlot(new List<double>() { 5, 4, 3, 2, 1, 0 });
            plotter.AddPlot(new List<double>() { 2, 3, 4, 5, 6, 7 });
        }
    }
}
