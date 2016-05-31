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
        int nodesNumber;
        int transmiterRadius;
        int transmiterDiameter;
        Random _random = new Random();


        Point OriginPoint;
        List<double> maxDist;
        int[,] Transmiters;
        double[,] Intersections;
        double distance;
        string nodesLabel;

        public GenerateGraph(Point _originPoint, int[,] _transmiters, double[,] _intersections, List<double> _maxDist, int citRad, int nodeNum, int tranRad)
        {
            OriginPoint = _originPoint;
            Transmiters = _transmiters;
            Intersections = _intersections;
            maxDist = _maxDist;

            cityRadius = citRad;
            nodesNumber = nodeNum;
            transmiterRadius = tranRad;
            cityDiameter = 2 * cityRadius;
            transmiterDiameter = 2 * transmiterRadius;

            distance = nodesNumber;
            nodesLabel = "";
            for (int i = 0; i < nodesNumber; i++)
            {
                if (maxDist[i] < distance)
                {
                    distance = maxDist[i];
                }
            }
            for (int i = 0; i < nodesNumber; i++)
            {
                if (maxDist[i] == distance)
                {
                    nodesLabel += Convert.ToString(i) + ", ";
                }
            }

            InitializeComponent();
        }

        private void GenerateGraph_Paint(object sender, PaintEventArgs e)
        {
            Brush aBrush = (Brush)Brushes.Red;


            for (int i = 0; i < nodesNumber; i++)
            {
                for (int j = 0; j < nodesNumber; j++)
                {
                        if (Intersections[i, j] == 1)
                        {
                        Pen graphPen = new Pen(Color.MediumAquamarine, 4);
                        e.Graphics.DrawLine(graphPen, Transmiters[0, i], Transmiters[1, i], Transmiters[0, j], Transmiters[1, j]);
                        }
                }
            }
            for (int i = 0; i < nodesNumber; i++)
            {
                if (maxDist[i] == distance)
                {
                    e.Graphics.FillRectangle(aBrush, Transmiters[0, i], Transmiters[1, i], 1, 1);
                    e.Graphics.FillEllipse(Brushes.Red, Transmiters[0, i] - transmiterRadius, Transmiters[1, i] - transmiterRadius, transmiterDiameter, transmiterDiameter);
                }
                else
                {
                    e.Graphics.FillRectangle(aBrush, Transmiters[0, i], Transmiters[1, i], 1, 1);
                    e.Graphics.FillEllipse(Brushes.Blue, Transmiters[0, i] - transmiterRadius, Transmiters[1, i] - transmiterRadius, transmiterDiameter, transmiterDiameter);
                }
            }
            for (int i = 0; i < nodesNumber; i++)
            {
                string transmiterId = Convert.ToString(i);
                e.Graphics.DrawString(transmiterId, new Font("Calibri", 14, FontStyle.Bold), new SolidBrush(Color.Black), Transmiters[0, i], Transmiters[1, i]);
            }

            label4.Text = nodesLabel;
            label5.Text = Convert.ToString(distance);
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
