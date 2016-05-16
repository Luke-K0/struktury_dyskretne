using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        int _originX;
        int _originY;
        int frequenciesNumber = 0;
        Random _random = new Random();
        Point tempPoint;
        int[,] transmiters;
        int[,] intersections;
        double[,] distancesPower;
        double transmiterDiameterPower;
        List<int> crossListX = new List<int>();
        string coordinatesString = "";
        string intersectionsString = "";
        string distancesString = "";
        string crossListString = "";
        
        public GenerateGraph(int citRad, int tranNum, int tranRad)
        {
            InitializeComponent();
            cityRadius = citRad;
            transmitersNumber = tranNum;
            transmiterRadius = tranRad;
            cityDiameter = 2 * cityRadius;
            transmiterDiameter = 2 * transmiterRadius;
            transmiters = new int[2, transmitersNumber];
            intersections = new int[transmitersNumber, transmitersNumber];
            distancesPower = new double[transmitersNumber, transmitersNumber];
            transmiterDiameterPower = Math.Pow(transmiterDiameter, 2);
        }

        private Point CalculatePoint()
        {
            var angle = _random.NextDouble() * Math.PI * 2;
            var radius = Math.Sqrt(_random.NextDouble()) * cityRadius;
            var x = _originX + radius * Math.Cos(angle);
            var y = _originY + radius * Math.Sin(angle);
            return new Point((int)x, (int)y);
        }

        int checkDistance(List<int> list, int value, int pointA, int pointB)
        {
            double distancePower;
            if (pointB < list.Count)
            {
                distancePower = (Math.Pow(transmiters[0, list[pointA]] - transmiters[0, list[pointB]], 2) + Math.Pow(transmiters[1, list[pointA]] - transmiters[1, list[pointB]], 2));
                if (distancePower < transmiterDiameterPower)
                {
                    value = value + 1;
                }
            }

            if (pointB > list.Count())
            {
                return value;
            }
            else
            {
                return checkDistance(list, value, pointA, pointB + 1);
            }  
        }

        private void GenerateGraph_Paint(object sender, PaintEventArgs e)
        {

            System.IO.File.Delete("coordinatesFile.txt");
            System.IO.File.Delete("intersectionsFile.txt");
            System.IO.File.Delete("distancesFile.txt");
            System.IO.File.Delete("crossListFile.txt");
            System.IO.File.Delete("countFreqFile.txt");


            Point CenterPoint = new Point()
            {
                X = this.ClientRectangle.Width / 2,
                Y = this.ClientRectangle.Height / 2
            };
            Point OriginPoint = new Point()
            {
                X = CenterPoint.X - cityRadius,
                Y = CenterPoint.Y - cityRadius
            };

            e.Graphics.DrawEllipse(Pens.Black, OriginPoint.X, OriginPoint.Y, cityDiameter, cityDiameter);
            Brush aBrush = (Brush)Brushes.Red;

            _originX = CenterPoint.X;
            _originY = CenterPoint.Y;

            for (int i = 0; i < transmitersNumber; i++)
            {
                tempPoint = CalculatePoint();
                e.Graphics.FillRectangle(aBrush, tempPoint.X, tempPoint.Y, 1, 1);
                string transmiterId = Convert.ToString(i);
                e.Graphics.DrawString(transmiterId, new Font("Calibri", 12), new SolidBrush(Color.Black), tempPoint.X, tempPoint.Y);
                e.Graphics.DrawEllipse(Pens.Red, tempPoint.X - transmiterRadius, tempPoint.Y - transmiterRadius, transmiterDiameter, transmiterDiameter);
                transmiters[0, i] = tempPoint.X;
                transmiters[1, i] = tempPoint.Y;
                coordinatesString = "X: " + transmiters[0, i] + "\t Y: " + transmiters[1, i];
                using (System.IO.StreamWriter coordinatesWriter = new System.IO.StreamWriter("coordinatesFile.txt", true))
                {
                    coordinatesWriter.WriteLine(coordinatesString);
                }
                coordinatesString = "";
            }
            for (int i = 0; i < transmitersNumber; i++)
            {
                for (int j = 0; j < transmitersNumber; j++)
                {
                    distancesPower[i, j] = (Math.Pow(transmiters[0, i] - transmiters[0, j], 2) + Math.Pow(transmiters[1, i] - transmiters[1, j], 2));
                    distancesString += distancesPower[i, j].ToString() + " ";
                    if (i == j)
                    {
                        intersections[i, j] = 0;
                        intersectionsString += intersections[i, j].ToString();
                    }
                    else
                    {
                        double distancePower = (Math.Pow(transmiters[0, i] - transmiters[0, j], 2) + Math.Pow(transmiters[1, i] - transmiters[1, j], 2));
                        if (distancePower < transmiterDiameterPower)
                        {
                            e.Graphics.DrawLine(Pens.Red, transmiters[0, i], transmiters[1, i], transmiters[0, j], transmiters[1, j]);
                            intersections[i, j] = 1;
                            intersectionsString += intersections[i, j].ToString();
                        }
                        else
                        {
                            intersections[i, j] = 0;
                            intersectionsString += intersections[i, j].ToString();
                        }
                    }
                }
                using (System.IO.StreamWriter intersectionsWriter = new System.IO.StreamWriter("intersectionsFile.txt", true))
                {
                    intersectionsWriter.WriteLine(intersectionsString);
                }
                intersectionsString = "";
                using (System.IO.StreamWriter distancesWriter = new System.IO.StreamWriter("distancesFile.txt", true))
                {
                    distancesWriter.WriteLine(distancesString);
                }
                distancesString = "";
            }
            for (int i = 0; i < transmitersNumber; i++)
            {
                List<int> crossList = new List<int>();

                using (System.IO.StreamWriter crossListWriter = new System.IO.StreamWriter("crossListFile.txt", true))
                {
                    crossListWriter.Write(Convert.ToString(i) + ": \t");
                }
                for (int j = 0; j < transmitersNumber; j++)
                {
                    if (intersections[i, j] == 1)
                    {
                        crossList.Add(j);
                        crossListString = j + " ";
                        using (System.IO.StreamWriter crossListWriter = new System.IO.StreamWriter("crossListFile.txt", true))
                        {
                            crossListWriter.Write(crossListString);
                        }
                        crossListString = "";
                    }
                }
                using (System.IO.StreamWriter crossListWriter = new System.IO.StreamWriter("crossListFile.txt", true))
                {
                    crossListWriter.Write("\n");
                }
                if ((crossList.Count < 2) && (frequenciesNumber == 0))
                {
                    frequenciesNumber = crossList.Count + 1;
                }
                else if (crossList.Count > 1)
                {
                    for (int z = 0; z < crossList.Count; z++)
                    {
                        int countedFreqs = 0;
                        countedFreqs = checkDistance(crossList, 2, z, z+1);

                        if (countedFreqs > frequenciesNumber)
                        {
                            frequenciesNumber = countedFreqs;
                        }
                    }       
                }
            }
            label1.Text = frequenciesNumber.ToString();
        }
    }
}
