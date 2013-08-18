using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using QuickGraph;
using System.Windows;

namespace GraphPrinter
{
    public class VisualizationManager
    {
        private static VisualizationManager selfPtr = null;
        private MainWindow window;
        private Thread thread;
        public List<IBidirectionalGraph<object, IEdge<object>>> graphList { get; private set; }

        public static void AddGraph(IBidirectionalGraph<object, IEdge<object>> graph)
        {
            if (selfPtr == null)
            {
                selfPtr = new VisualizationManager();
            }
            while (selfPtr.graphList == null)
            {
                Thread.Sleep(1);
            }

            selfPtr.graphList.Add(graph);
        }

        public VisualizationManager()
        {
            graphList = new List<IBidirectionalGraph<object,IEdge<object>>>();

            thread = new Thread(new ThreadStart(StartVisualizationThread));
            thread.SetApartmentState(ApartmentState.STA);

            thread.Start();
        }

        public void StartVisualizationThread()
        {
            Application app = new Application();
            window = new MainWindow(this);
            app.Run(window);
        }
    }
}
