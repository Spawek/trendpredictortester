﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading;

namespace WindowsFormsPlotter
{
	public partial class Plotter : Form
    {
        public const double CHART_HEIGHT_MULTIPLER = 0.9d;

		private static Plotter singletonInstance = null;
        private static Thread singletonThread = null;
		public static Plotter GetInstance()
		{
			if (singletonInstance == null)
			{
                singletonThread = new Thread(new ThreadStart(RunForm));
                singletonThread.Start();
			}

            //wait for form to be created
            while (singletonInstance == null)
            {
                Thread.Sleep(1);
            }

			return singletonInstance;
		}

        public static void RunForm()
        {
            singletonInstance = new Plotter();
            singletonInstance.Visible = true;
            singletonInstance.WindowState = FormWindowState.Normal;
            singletonInstance.Show();

            Application.Run(singletonInstance);
        }

		public Plotter()
		{
			InitializeComponent();
		}

		List<Chart> currCharts = new List<Chart>();
		private void AddChartInRuntime(Chart chart)
		{
			if (this.InvokeRequired)
			{ //not-form thread
				this.Invoke((MethodInvoker)delegate { AddChartInRuntime(chart); });
			}
			else
			{ //form thread
				currCharts.Add(chart);
				ReplotCharts();
				this.Controls.Add(chart);
				this.Refresh();
			}
		}

		private void ReplotCharts()
		{
			int windowWidth = this.Size.Width;
			int windowHeight = this.Size.Height;

			int plotsNo = currCharts.Count;

			for (int i = 0; i < plotsNo; i++)
			{
				currCharts[i].Width = windowWidth;
				currCharts[i].Left = 0;
                currCharts[i].Height = (int)(CHART_HEIGHT_MULTIPLER * (double)windowHeight / (double)plotsNo);
				currCharts[i].Top = windowHeight / plotsNo * i;
				currCharts[i].Visible = true;
			}
		}

		public void AddPlot(List<double> values, string title = "")
		{
			Chart chart = new Chart();
			Series series = new Series();
            ChartArea chartArea1 = new ChartArea();
            chartArea1.Name = "ChartArea1";
            chart.ChartAreas.Add(chartArea1);
            series.BorderWidth = 2;
            series.BorderDashStyle = ChartDashStyle.Solid;
            series.ChartType = SeriesChartType.Line;
            series.Color = Color.Green;
			for (int i = 0; i < values.Count; i++)
			{
				series.Points.AddXY(i, values[i]);
			}
            chart.BorderlineColor = Color.Red;
            chart.BorderlineWidth = 1;
			chart.Series.Add(series);
            chart.Titles.Add(title);
            chart.Invalidate();
            chart.Palette = ChartColorPalette.Fire;
            chartArea1.AxisY.Minimum = values.Min();
            chartArea1.AxisY.Maximum = values.Max();
			AddChartInRuntime(chart);
		}

	}
}
