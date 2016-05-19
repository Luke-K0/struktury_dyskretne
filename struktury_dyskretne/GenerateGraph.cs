using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
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
        String FreqNum;
        int[,] Transmiters;
        int[,] Intersections;

        //public GenerateGraph(int citRad, int tranNum, int tranRad)
        public GenerateGraph(Point _originPoint, int[,] _transmiters, int[,] _intersections, string _freqNum, int citRad, int tranNum, int tranRad)
        {
            OriginPoint = _originPoint;
            Transmiters = _transmiters;
            Intersections = _intersections;
            FreqNum = _freqNum;

            cityRadius = citRad;
            transmitersNumber = tranNum;
            transmiterRadius = tranRad;
            cityDiameter = 2 * cityRadius;
            transmiterDiameter = 2 * transmiterRadius;

            InitializeComponent();
        }

        private void GenerateGraph_Paint(object sender, PaintEventArgs e)
        {


            e.Graphics.DrawEllipse(Pens.Black, OriginPoint.X, OriginPoint.Y, cityDiameter, cityDiameter);
            Brush aBrush = (Brush)Brushes.Red;

            for (int i = 0; i < transmitersNumber; i++)
            {
                //tempPoint = CalculatePoint();
                e.Graphics.FillRectangle(aBrush, Transmiters[0, i], Transmiters[1, i], 1, 1);
                string transmiterId = Convert.ToString(i);
                e.Graphics.DrawString(transmiterId, new Font("Calibri", 12), new SolidBrush(Color.Black), Transmiters[0, i], Transmiters[1, i]);
                e.Graphics.DrawEllipse(Pens.Red, Transmiters[0, i] - transmiterRadius, Transmiters[1, i] - transmiterRadius, transmiterDiameter, transmiterDiameter);
                //transmiters[0, i] = tempPoint.X;
                //Transmiters[1, i] = tempPoint.Y;
            }
            for (int i = 0; i < transmitersNumber; i++)
            {
                for (int j = 0; j < transmitersNumber; j++)
                {
                    //if (i == j)
                    //{
                    //    intersections[i, j] = 0;
                    //}
                    //else
                    //{
                        //double distancePower = (Math.Pow(transmiters[0, i] - transmiters[0, j], 2) + Math.Pow(transmiters[1, i] - transmiters[1, j], 2));
                        if (Intersections[i, j] == 1)
                        {
                        //Pen graphPen = new Pen(Color.FromArgb(255, (j+i)*10 % 255, (j * 17) % 255), 2);
                        Pen graphPen = new Pen(Color.FromArgb(_random.Next(0, 255), _random.Next(0, 255), _random.Next(0, 255)), 2);

                        e.Graphics.DrawLine(graphPen, Transmiters[0, i], Transmiters[1, i], Transmiters[0, j], Transmiters[1, j]);
                            //intersections[i, j] = 1;
                        }
                        //else
                        //{
                        //    intersections[i, j] = 0;
                        //}
                    //}
                }
            }

            label1.Text = FreqNum;





            

        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Application.Exit();
        }

    }
}
