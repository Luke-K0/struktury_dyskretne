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
        String centerNode;
        int _originX;
        int _originY;
        int graphSize = 300;
        int nodesNumber;
        int nodeSize = 5;
        int graphWindowSize;
        int[,] nodes;
        double[,] intersections;
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

                            graphSize = Convert.ToInt32(setList[0]);
                            nodeSize = Convert.ToInt32(setList[1]);

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

                            nodesNumber = xList.Count;
                            graphWindowSize = 2 * graphSize + 100;                
                            nodes = new int[2, nodesNumber];
                            intersections = new double[nodesNumber, nodesNumber];

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

                            for (int i = 0; i < nodesNumber; i++)
                            {
                                nodes[0, i] = xList[i];
                                nodes[1, i] = yList[i];
                            }

                            Point CenterPoint = new Point()
                            {
                                X = graphWindowSize / 2,
                                Y = graphWindowSize / 2
                            };
                            Point OriginPoint = new Point()
                            {
                                X = CenterPoint.X - graphSize,
                                Y = CenterPoint.Y - graphSize
                            };

                            //this.Hide();
                            GenerateGraph picture = new GenerateGraph(OriginPoint, nodes, intersections, centerNode, graphSize, nodesNumber, nodeSize);
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
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            nodesNumber = Convert.ToInt32(textBox2.Text);
            graphWindowSize = 2 * graphSize + 100;
            nodes = new int[2, nodesNumber];
            intersections = new double[nodesNumber, nodesNumber];

            transmiterDiameterPower = Math.Pow(nodeSize * 2, 2);

            Point CenterPoint = new Point()
            {
                X = graphWindowSize / 2,
                Y = graphWindowSize / 2
            };
            Point OriginPoint = new Point()
            {
                X = CenterPoint.X - graphSize,
                Y = CenterPoint.Y - graphSize
            };

            _originX = CenterPoint.X;
            _originY = CenterPoint.Y;

            for (int i = 0; i < nodesNumber; i++)
            {
                tempPoint = CalculatePoint();
                nodes[0, i] = tempPoint.X;
                nodes[1, i] = tempPoint.Y;
            }

            for (int i = 0; i < nodesNumber; i++)
            {
                for (int j = i; j < nodesNumber; j++)
                {
                    if (i == j)
                    {
                        intersections[i, j] = 0;
                    }
                    else
                    { 
                        if (_random.Next(0, 100) % 2 == 0)
                        {
                            intersections[i, j] = 1;
                            intersections[j, i] = 1;
                        }
                        else
                        {
                            intersections[i, j] = 0;
                            intersections[j, i] = 0;
                        }
                    }
                }
            }


            dijstra dist = new dijstra(intersections, nodesNumber);

            var item = dist.dist;
            List<string> tempList = new List<string>();

            for (int i = 0; i < item.Length; i++)
            {
                tempList.Add("Node " + i + " Path Distance = " + item[i]);
            }

            System.IO.File.Delete("listFile.txt");
            System.IO.File.Delete("intersectionsFile.txt");
            string intersectionsString = "";

            for (int i = 0; i < nodesNumber; i++)
            {
                for (int j = 0; j < nodesNumber; j++)
                {
                    intersectionsString += intersections[i, j].ToString();
                }
                using (System.IO.StreamWriter intersectionsWriter = new System.IO.StreamWriter("intersectionsFile.txt", true))
                {
                    intersectionsWriter.WriteLine(intersectionsString);
                }
                intersectionsString = "";
            }


            for (int i = 0; i < tempList.Count; i++)
            {
                using (System.IO.StreamWriter coordinatesWriter = new System.IO.StreamWriter("listFile.txt", true))
                {
                    coordinatesWriter.WriteLine(tempList[i]);
                }
            }

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
            XComment radius = new XComment("radius = " + graphSize);
            graph.Add(radius);
            XComment range = new XComment("range = " + nodeSize);
            graph.Add(range);

            for (int i = 0; i < nodesNumber; i++)
            {
                XElement node = new XElement("node",
                new XAttribute("id", i),
                new XElement("attr",
                    new XAttribute("name", "X"),
                    new XElement("int", nodes[0, i])),
                new XElement("attr",
                    new XAttribute("name", "Y"),
                    new XElement("int", nodes[1, i])));
                graph.Add(node);
            }
            
            for (int i = 0; i < nodesNumber; i++)
            {
                for (int j = i + 1; j < nodesNumber; j++)
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
            GenerateGraph picture = new GenerateGraph(OriginPoint, nodes, intersections, centerNode, graphSize, nodesNumber, nodeSize);
            picture.Width = graphWindowSize + 20;
            picture.Height = graphWindowSize + 40;
            picture.Show();
        }

        private Point CalculatePoint()
        {
            var angle = _random.NextDouble() * Math.PI * 2;
            var radius = Math.Sqrt(_random.NextDouble()) * graphSize;
            var x = _originX + radius * Math.Cos(angle);
            var y = _originY + radius * Math.Sin(angle);
            return new Point((int)x, (int)y);
        }

        double shiftRows(double[,] array)
        {
            for (int i = 0; i < nodesNumber; i++)
            {
                double tempVal;
                for (int j = 0; j < nodesNumber; j++)
                {
                    //do logic
                }

            }
            return 0;
        }
    }

    public class dijstra
    {
        public dijstra(double[,] graphArray, int nodeNum)
        {
            initial(0, nodeNum);
            while (queue.Count > 0)
            {
                int u = getNextVertex();

                for (int i = 0; i < nodeNum; i++)
                {
                    if (graphArray[u, i] > 0)
                    {
                        if (dist[i] > dist[u] + graphArray[u, i])
                        {
                            dist[i] = dist[u] + graphArray[u, i];
                        }
                    }
                }
            }
        }

        public double[] dist { get; set; }

        int getNextVertex()
        {
            var min = double.PositiveInfinity;
            int vertex = -1;

            foreach (int val in queue)
            {
                if (dist[val] <= min)
                {
                    min = dist[val];
                    vertex = val;
                }
            }

            queue.Remove(vertex);

            return vertex;

        }
        List<int> queue = new List<int>();

        public void initial(int s, int len)
        {
            dist = new double[len];

            for (int i = 0; i < len; i++)
            {
                dist[i] = double.PositiveInfinity;
                queue.Add(i);
            }

            dist[0] = 0;
        }
    }
}
