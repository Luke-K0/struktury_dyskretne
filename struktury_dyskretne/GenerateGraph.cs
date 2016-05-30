using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace struktury_dyskretne
{
    public partial class GenerateGraph : Form
    {
        int cityRadius;
        int cityDiameter;
        int transmitersNumber;
        int transmiterRadius;
        int transmiterDiameter;
        Random _random = new Random();


        Point OriginPoint;
        List<double> maxDist;
        int[,] Transmiters;
        double[,] Intersections;

        public GenerateGraph(Point _originPoint, int[,] _transmiters, double[,] _intersections, List<double> _maxDist, int citRad, int tranNum, int tranRad)
        {
            OriginPoint = _originPoint;
            Transmiters = _transmiters;
            Intersections = _intersections;
            maxDist = _maxDist;

            cityRadius = citRad;
            transmitersNumber = tranNum;
            transmiterRadius = tranRad;
            cityDiameter = 2 * cityRadius;
            transmiterDiameter = 2 * transmiterRadius;

            InitializeComponent();
        }

        private void GenerateGraph_Paint(object sender, PaintEventArgs e)
        {
            Brush aBrush = (Brush)Brushes.Red;

            for (int i = 0; i < transmitersNumber; i++)
            {
                e.Graphics.FillRectangle(aBrush, Transmiters[0, i], Transmiters[1, i], 1, 1);
                string transmiterId = Convert.ToString(i);
                Pen nodePen = new Pen(Color.Red, 4);
                e.Graphics.DrawString(transmiterId, new Font("Calibri", 12), new SolidBrush(Color.Black), Transmiters[0, i], Transmiters[1, i]);
                e.Graphics.DrawEllipse(nodePen, Transmiters[0, i] - transmiterRadius, Transmiters[1, i] - transmiterRadius, transmiterDiameter, transmiterDiameter);
            }
            for (int i = 0; i < transmitersNumber; i++)
            {
                for (int j = 0; j < transmitersNumber; j++)
                {
                        if (Intersections[i, j] == 1)
                        {
                        Pen graphPen = new Pen(Color.FromArgb(_random.Next(0, 255), _random.Next(0, 255), _random.Next(0, 255)), 2);
                        e.Graphics.DrawLine(graphPen, Transmiters[0, i], Transmiters[1, i], Transmiters[0, j], Transmiters[1, j]);
                        }
                }
            }
            label1.Text = "placeholder";
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
// add colors dependancies