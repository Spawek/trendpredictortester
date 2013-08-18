using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Timers;
using QuickGraph;


namespace GraphPrinter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public IBidirectionalGraph<object, IEdge<object>> Graph { get; set; }
        private List<IBidirectionalGraph<object, IEdge<object>>> GraphList { get { return visualizationManager_.graphList; } }
        private System.Timers.Timer graphListRefresher = new System.Timers.Timer(50.0d); // 50ms
        private VisualizationManager visualizationManager_;

        public MainWindow(VisualizationManager visualizationManager)
        {
            visualizationManager_ = visualizationManager;

            graphListRefresher.Elapsed += graphListRefresher_Elapsed;
            graphListRefresher.Start();

            Graph = visualizationManager.graphList.First();
            InitializeComponent();
            this.Show();
        }

        void graphListRefresher_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(refreshGraphList));
        }

        private int lastGraphListSize = 0;
        private void refreshGraphList()
        {
            if (GraphList.Count != lastGraphListSize)
            {
                lastGraphListSize = GraphList.Count;
                ListBox_GraphsList.Items.Clear();
                for (int i = 0; i < GraphList.Count; i++)
                {
                    ListBox_GraphsList.Items.Add("Graph" + (i+1).ToString());
                }
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
            {
                int index = Convert.ToInt32(e.AddedItems[0].ToString().Remove(0, 5)) - 1;

                Graph = GraphList[index];
                graphLayout.Graph = Graph;
            }
        }

        //private void CreateGraph()
        //{
        //    BidirectionalGraph<object, IEdge<object>>  graph = new BidirectionalGraph<object, IEdge<object>>();

        //    //add vertices
        //    string[] vertices = new string[5];
        //    for (int i = 0; i < 5; i++)
        //    {
        //        vertices[i] = i.ToString();
        //        graph.AddVertex(vertices[i]);
        //    }

        //    //add edges
        //    graph.AddEdge(new Edge<object>(vertices[0], vertices[1]));
        //    graph.AddEdge(new Edge<object>(vertices[0], vertices[2]));
        //    graph.AddEdge(new Edge<object>(vertices[0], vertices[3]));
        //    graph.AddEdge(new Edge<object>(vertices[0], vertices[4]));
        //    graph.AddEdge(new Edge<object>(vertices[2], vertices[3]));

        //    Graph = graph;
        //}
    }
}
