using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;

namespace struktury_dyskretne
{
    public partial class GraphSettings : Form
    {
        Random _random = new Random();
        Point tempPoint;
        List<int> checkList = new List<int>();
        String freqNum;
        int _originX;
        int _originY;
        int cityRadius;
        int transmitersNumber;
        int transmiterRange;
        int frequenciesNumber = 0;
        int graphWindowSize;
        int[,] transmiters;
        int[,] intersections;
        double transmiterDiameterPower;

        public GraphSettings()
        {
            InitializeComponent();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "gxl files (*.gxl)|*.gxl";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        List<int> idList = new List<int>();
                        List<int> xList = new List<int>();
                        List<int> yList = new List<int>();
                        List<string> setList = new List<string>();

                        using (myStream)
                        {
                            XDocument loadGraph = XDocument.Load(myStream);

                            //parse vars
                            foreach (var node in loadGraph.Descendants("graph").Nodes())
                            {
                                if (node is XComment)
                                {
                                    setList.Add(Convert.ToString(node));
                                }
                            }

                            setList[0] = setList[0].Trim(new Char[] { ' ', '"', '<', '>', '!', '-', '=', 'r', 'a', 'd', 'i', 'u', 's', 'n', 'g', 'e' });
                            setList[1] = setList[1].Trim(new Char[] { ' ', '"', '<', '>', '!', '-', '=', 'r', 'a', 'd', 'i', 'u', 's', 'n', 'g', 'e' });

                            cityRadius = Convert.ToInt32(setList[0]);
                            transmiterRange = Convert.ToInt32(setList[1]);

                            //parse X coordinates 
                            loadGraph.Descendants("attr").Where(p => p.Attribute("name").Value.Contains
                                ("X")).Select(p => new
                            {
                                X = p.Element("int").Value
                            }).ToList().ForEach(p =>
                            {
                                xList.Add(Convert.ToInt32(p.X));
                            });

                            //parse Y coordinates 
                            loadGraph.Descendants("attr").Where(p => p.Attribute("name").Value.Contains
                                ("Y")).Select(p => new
                            {
                                Y = p.Element("int").Value
                            }).ToList().ForEach(p =>
                            {
                                yList.Add(Convert.ToInt32(p.Y));
                            });

                            transmitersNumber = xList.Count;
                            graphWindowSize = 2 * cityRadius + 100;                
                            transmiters = new int[2, transmitersNumber];
                            intersections = new int[transmitersNumber, transmitersNumber];

                            //parse edges
                            loadGraph.Descendants("edge").Select(p => new
                            {
                                to = p.Attribute("to").Value,
                                from = p.Attribute("from").Value
                            }).ToList().ForEach(p =>
                            {
                                intersections[Convert.ToInt32(p.to), Convert.ToInt32(p.from)] = 1;
                                intersections[Convert.ToInt32(p.from), Convert.ToInt32(p.to)] = 1;
                            });

                            for (int i = 0; i < transmitersNumber; i++)
                            {
                                transmiters[0, i] = xList[i];
                                transmiters[1, i] = yList[i];
                            }

                            Point CenterPoint = new Point()
                            {
                                X = graphWindowSize / 2,
                                Y = graphWindowSize / 2
                            };
                            Point OriginPoint = new Point()
                            {
                                X = CenterPoint.X - cityRadius,
                                Y = CenterPoint.Y - cityRadius
                            };

                            //this.Hide();
                            GenerateGraph picture = new GenerateGraph(OriginPoint, transmiters, intersections, freqNum, cityRadius, transmitersNumber, transmiterRange);
                            picture.Width = graphWindowSize + 20;
                            picture.Height = graphWindowSize + 40;
                            picture.Show();
                        }

                        for (int i = 0; i < yList.Count; i++)
                        {
                            using (System.IO.StreamWriter coordinatesWriter = new System.IO.StreamWriter("coordinatesFile.txt", true))
                            {
                                coordinatesWriter.WriteLine(Convert.ToString(yList[i]));
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            cityRadius = Convert.ToInt32(textBox1.Text);
            transmitersNumber = Convert.ToInt32(textBox2.Text);
            transmiterRange = Convert.ToInt32(textBox3.Text);

            graphWindowSize = 2 * cityRadius + 100;

            transmiters = new int[2, transmitersNumber];
            intersections = new int[transmitersNumber, transmitersNumber];

            transmiterDiameterPower = Math.Pow(transmiterRange * 2, 2);

            Point CenterPoint = new Point()
            {
                X = graphWindowSize / 2,
                Y = graphWindowSize / 2
            };
            Point OriginPoint = new Point()
            {
                X = CenterPoint.X - cityRadius,
                Y = CenterPoint.Y - cityRadius
            };

            _originX = CenterPoint.X;
            _originY = CenterPoint.Y;

            for (int i = 0; i < transmitersNumber; i++)
            {
                tempPoint = CalculatePoint();
                transmiters[0, i] = tempPoint.X;
                transmiters[1, i] = tempPoint.Y;
            }

            for (int i = 0; i < transmitersNumber; i++)
            {
                for (int j = 0; j < transmitersNumber; j++)
                {
                    if (i == j)
                    {
                        intersections[i, j] = 0;
                    }
                    else
                    {
                        double distancePower = (Math.Pow(transmiters[0, i] - transmiters[0, j], 2) + Math.Pow(transmiters[1, i] - transmiters[1, j], 2));
                        if (distancePower < transmiterDiameterPower)
                        {
                            intersections[i, j] = 1;
                        }
                        else
                        {
                            intersections[i, j] = 0;
                        }
                    }
                }
            }
            countFreqNum();

            XDocument saveGraph = new XDocument();

            XElement gxl = new XElement("gxl", new XAttribute(XNamespace.Xmlns + "xlink", "http://www.w3.org/1999/xlink"));
            saveGraph.Add(gxl);

            XElement graph = new XElement("graph",
                        new XAttribute("id", "undirected-instance"),
                        new XAttribute("edgeis", "true"),
                        new XAttribute("edgemode", "undirected"),
                        new XAttribute("hypergraph", "false"));
            gxl.Add(graph);

            // insert range attributes to graph as comments
            XComment radius = new XComment("radius = " + cityRadius);
            graph.Add(radius);
            XComment range = new XComment("range = " + transmiterRange);
            graph.Add(range);

            for (int i = 0; i < transmitersNumber; i++)
            {
                XElement node = new XElement("node",
                new XAttribute("id", i),
                new XElement("attr",
                    new XAttribute("name", "X"),
                    new XElement("int", transmiters[0, i])),
                new XElement("attr",
                    new XAttribute("name", "Y"),
                    new XElement("int", transmiters[1, i])));
                graph.Add(node);
            }
            
            for (int i = 0; i < transmitersNumber; i++)
            {
                for (int j = i + 1; j < transmitersNumber; j++)
                {
                    if (intersections[i,j] == 1)
                    {
                        XElement edge = new XElement("edge",
                            new XAttribute("id", i + "-" + j),
                            new XAttribute("to", j),
                            new XAttribute("from", i),
                            new XAttribute("isdirected", "false")
                            );
                        graph.Add(edge);
                    }
                }
            }

            saveGraph.Save("graph.gxl");

            //this.Hide();
            GenerateGraph picture = new GenerateGraph(OriginPoint, transmiters, intersections, freqNum, cityRadius, transmitersNumber, transmiterRange);
            picture.Width = graphWindowSize + 20;
            picture.Height = graphWindowSize + 40;
            picture.Show();
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
                //distancePower = (Math.Pow(transmiters[0, list[pointA]] - transmiters[0, list[pointB]], 2) + Math.Pow(transmiters[1, list[pointA]] - transmiters[1, list[pointB]], 2));

                distancePower = (Math.Pow(transmiters[0, list[pointA]] - transmiters[0, list[pointB]], 2) + Math.Pow(transmiters[1, list[pointA]] - transmiters[1, list[pointB]], 2));
                if (distancePower <= transmiterDiameterPower)
                {
                    value = value + 1;
                    checkList.Add(list[pointB]);
                }
                //if (distancePower > transmiterDiameterPower && value > 3)
                //{
                //    value = value - 1;
                //}
            }
            if (checkList.Count > 1)
            {
                distancePower = (Math.Pow(transmiters[0, checkList[0]] - transmiters[0, checkList[1]], 2) + Math.Pow(transmiters[1, checkList[0]] - transmiters[1, checkList[1]], 2));
                if (distancePower > transmiterDiameterPower)
                {
                    value = value - 1;
                }
                checkList.Clear();
            }

            if (pointB > list.Count())
            {
                //checkList.Clear();
                return value;
            }
            else
            {
                return checkDistance(list, value, pointA, pointB + 1);
            }
        }

        void countFreqNum()
        {
            for (int i = 0; i < transmitersNumber; i++)
            {
                List<int> crossList = new List<int>();

                for (int j = 0; j < transmitersNumber; j++)
                {
                    if (intersections[i, j] == 1)
                    {
                        crossList.Add(j);
                    }
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
                        countedFreqs = checkDistance(crossList, 2, z, z + 1);

                        if (countedFreqs > frequenciesNumber)
                        {
                            frequenciesNumber = countedFreqs;
                        }
                    }
                }
            }
            freqNum = frequenciesNumber.ToString();
        }
    }
}
