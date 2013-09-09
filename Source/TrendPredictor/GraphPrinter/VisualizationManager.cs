using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using QuickGraph;
using System.Windows;
using System.Windows.Media;

namespace GraphPrinter
{
    public class Pnt
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Pnt(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

    public class VisualizationManager
    {
        private static VisualizationManager selfPtr = null;
        private MainWindow window;
        private Thread thread;
        public List<IBidirectionalGraph<object, IEdge<object>>> graphList { get; private set; }
        public List<List<Pnt>> actualGraphOutputDataList { get; private set; }
        public List<List<Pnt>> expectedGraphOutputDataList { get; private set; }

        public static void AddGraph(IBidirectionalGraph<object, IEdge<object>> graph, List<double> expectedValues, List<double> actualValues)
        {
            if (selfPtr == null)
            {
                selfPtr = new VisualizationManager();
            }
            while (selfPtr.graphList == null || selfPtr.expectedGraphOutputDataList == null || selfPtr.actualGraphOutputDataList == null)
            {
                Thread.Sleep(1);
            }

            selfPtr.graphList.Add(graph);
            selfPtr.expectedGraphOutputDataList.Add(ConvertDoubleListToPointCollection(expectedValues));
            selfPtr.actualGraphOutputDataList.Add(ConvertDoubleListToPointCollection(actualValues));
        }

        static List<Pnt> ConvertDoubleListToPointCollection(List<double> list)
        {
            List<Pnt> coll = new List<Pnt>(list.Count);

            double i = 1.0d;
            foreach (var item in list)
            {
                coll.Add(new Pnt(i++, item));
            }

            return coll;
        }

        public VisualizationManager()
        {
            graphList = new List<IBidirectionalGraph<object,IEdge<object>>>();
            expectedGraphOutputDataList = new List<List<Pnt>>();
            actualGraphOutputDataList = new List<List<Pnt>>();

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
