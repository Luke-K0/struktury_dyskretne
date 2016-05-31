using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace struktury_dyskretne
{
    public partial class GenerateGraph : Form
    {
        int nodesNumber;
        int nodeRadius;
        int nodeDiameter;
        Random _random = new Random();


        Point OriginPoint;
        List<double> maxDist;
        int[,] nodes;
        double[,] Intersections;
        double distance;
        string nodesLabel;

        public GenerateGraph(Point _originPoint, int[,] _nodes, double[,] _intersections, List<double> _maxDist, int graphRad, int nodeNum, int nodeRad)
        {
            OriginPoint = _originPoint;
            nodes = _nodes;
            Intersections = _intersections;
            maxDist = _maxDist;

            nodesNumber = nodeNum;
            nodeRadius = nodeRad;
            nodeDiameter = 2 * nodeRadius;

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
                        e.Graphics.DrawLine(graphPen, nodes[0, i], nodes[1, i], nodes[0, j], nodes[1, j]);
                        }
                }
            }
            for (int i = 0; i < nodesNumber; i++)
            {
                if (maxDist[i] == distance)
                {
                    e.Graphics.FillRectangle(aBrush, nodes[0, i], nodes[1, i], 1, 1);
                    e.Graphics.FillEllipse(Brushes.Red, nodes[0, i] - nodeRadius, nodes[1, i] - nodeRadius, nodeDiameter, nodeDiameter);
                }
                else
                {
                    e.Graphics.FillRectangle(aBrush, nodes[0, i], nodes[1, i], 1, 1);
                    e.Graphics.FillEllipse(Brushes.Blue, nodes[0, i] - nodeRadius, nodes[1, i] - nodeRadius, nodeDiameter, nodeDiameter);
                }
            }
            for (int i = 0; i < nodesNumber; i++)
            {
                string transmiterId = Convert.ToString(i);
                e.Graphics.DrawString(transmiterId, new Font("Calibri", 14, FontStyle.Bold), new SolidBrush(Color.Black), nodes[0, i], nodes[1, i]);
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
