using Models;
using PredmetProjekat.BFS;
using PredmetProjekat.Positioning;
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
using System.Xml;

namespace PredmetProjekat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public struct Coordinate
        {
            public int X; public int Y;
        }
        public enum pos { obj, line, none }
        public double noviX, noviY;
        private int dimensions = 200;
        private bool[,] positions = new bool[200, 200];
        public pos[,] positionsLines = new pos[200, 200];
        public bool[,] linesObjects = new bool[200, 200];
        Dictionary<long, NewEntity> entities = new Dictionary<long, NewEntity>();

        Dictionary<long, NodeEntity> nodes = new Dictionary<long, NodeEntity>();
        public MainWindow()
        {
            InitializeComponent();
            positions = new bool[dimensions, dimensions];

            elementi.Width = dimensions;
            elementi.Height = dimensions;
        }

        private void PoligonButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EllipseButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddTextButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RedoButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void elementi_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            elementi.Children.Clear();
            entities = new Dictionary<long, NewEntity>();
           
            int selectedS = SelectSize.SelectedIndex;
            switch (selectedS)
            {
                case 0:
                    dimensions = 200;
                    break;
                case 1:
                    dimensions = 750;
                    break;
                case 2:
                    dimensions = 1500;
                    break;
            }
            positionsLines = new pos[dimensions, dimensions];
            for (int i = 0; i < dimensions; i++)
            {
                for (int j = 0; j < dimensions; j++)
                {
                    positionsLines[i, j] = pos.none;
                }
            }
            elementi.Height = dimensions;
            elementi.Width = dimensions;
            linesObjects = new bool[dimensions, dimensions];
            positions = new bool[dimensions, dimensions];
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("Geographic.xml");

            #region Node
            XmlNodeList nodeList;

            NodeEntity nodeobj = new NodeEntity();

            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Nodes/NodeEntity");

            double minX = Approximation.GetXYMinMax(nodeList, false, true);
            double maxX = Approximation.GetXYMinMax(nodeList, false, false);
            double minY = Approximation.GetXYMinMax(nodeList, true, true);
            double maxY = Approximation.GetXYMinMax(nodeList, true, false);

            foreach (XmlNode node in nodeList)
            {
                nodeobj.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                nodeobj.Name = node.SelectSingleNode("Name").InnerText;
                nodeobj.X = double.Parse(node.SelectSingleNode("X").InnerText);
                nodeobj.Y = double.Parse(node.SelectSingleNode("Y").InnerText);

                Approximation.ToLatLon(nodeobj.X, nodeobj.Y, 34, out noviY, out noviX);
                System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle();
                rectangle.Fill = System.Windows.Media.Brushes.Blue;
                rectangle.Width = 1;
                rectangle.Height = 1;
                rectangle.Name = "ime" + nodeobj.Id.ToString();

                double newX = Approximation.GetX(elementi.Width, noviX, minX, maxX),
                       newY = Approximation.GetY(elementi.Height, noviY, minY, maxY);

                Coordinate coordinate = Approximation.FindPosition(positions, newX, newY);
                positionsLines[coordinate.X, coordinate.Y] = pos.obj;
                positions[coordinate.X, coordinate.Y] = true;
                //vodovodi[coordinate.x / 2, coordinate.y / 2] = new Vodov(positions[coordinate.x / 2, coordinate.y / 2], nodeobj.Id);
                rectangle.Margin = new Thickness(coordinate.X, coordinate.Y, 0, 0);
                NewEntity newEntity = new NewEntity(nodeobj.Id, coordinate.X, coordinate.Y);

                if (dimensions == 1500)
                {
                    // nodes.Add(nodeobj.Id, nodeobj);
                }
                entities.Add(newEntity.Id, newEntity);
               // nodes.Add(nodeobj.Id, nodeobj);
                ToolTip ttip = new ToolTip();

                ttip.Content = "Node \n ID: " + nodeobj.Id + "  Name: " + nodeobj.Name;
                ttip.Background = System.Windows.Media.Brushes.Black;
                ttip.Foreground = System.Windows.Media.Brushes.White;
                ttip.BorderBrush = System.Windows.Media.Brushes.Black;
                rectangle.ToolTip = ttip;

                elementi.Children.Add(rectangle);
                int indeks = elementi.Children.IndexOf(rectangle);
                // index.Add(nodeobj.Id.ToString(), indeks);
            }

            #endregion

            #region Switch
            SwitchEntity switchobj = new SwitchEntity();

            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Switches/SwitchEntity");

            minX = Approximation.GetXYMinMax(nodeList, false, true);
            maxX = Approximation.GetXYMinMax(nodeList, false, false);
            minY = Approximation.GetXYMinMax(nodeList, true, true);
            maxY = Approximation.GetXYMinMax(nodeList, true, false);


            foreach (XmlNode node in nodeList)
            {
                switchobj.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                switchobj.Name = node.SelectSingleNode("Name").InnerText;
                switchobj.X = double.Parse(node.SelectSingleNode("X").InnerText);
                switchobj.Y = double.Parse(node.SelectSingleNode("Y").InnerText);
                switchobj.Status = node.SelectSingleNode("Status").InnerText;

                Approximation.ToLatLon(switchobj.X, switchobj.Y, 34, out noviY, out noviX);
                System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle();
                rectangle.Fill = System.Windows.Media.Brushes.Green;
                rectangle.Width = 1;
                rectangle.Height = 1;
                double newX = Approximation.GetX(elementi.Width, noviX, minX, maxX), newY = Approximation.GetY(elementi.Height, noviY, minY, maxY);

                Coordinate coordinate = Approximation.FindPosition(positions, newX, newY);

                positionsLines[coordinate.X, coordinate.Y] = pos.obj;
                positions[coordinate.X, coordinate.Y] = true;
                //vodovodi[coordinate.x / 2, coordinate.y / 2] = new Vodov(positions[coordinate.x / 2, coordinate.y / 2], switchobj.Id);
                NewEntity newEntity = new NewEntity(switchobj.Id, coordinate.X, coordinate.Y);

                entities.Add(newEntity.Id, newEntity);
                // switches.Add(switchobj.Id, switchobj);

                rectangle.Name = "ime" + switchobj.Id.ToString() + "Q" + "true";

                rectangle.Margin = new Thickness(coordinate.X, coordinate.Y, 0, 0);

                ToolTip ttip = new ToolTip();

                ttip.Content = "Switch \n ID: " + switchobj.Id + "  Name: " + switchobj.Name + " Status: " + switchobj.Status;
                ttip.Background = System.Windows.Media.Brushes.Black;
                ttip.Foreground = System.Windows.Media.Brushes.White;
                ttip.BorderBrush = System.Windows.Media.Brushes.Black;
                rectangle.ToolTip = ttip;

                elementi.Children.Add(rectangle);

                int indeks = elementi.Children.IndexOf(rectangle);
                //index.Add(switchobj.Id.ToString(), indeks);

            }

            #endregion

            #region Substation
            SubstationEntity sub = new SubstationEntity();

            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Substations/SubstationEntity");

            minX = Approximation.GetXYMinMax(nodeList, false, true);
            maxX = Approximation.GetXYMinMax(nodeList, false, false);
            minY = Approximation.GetXYMinMax(nodeList, true, true);
            maxY = Approximation.GetXYMinMax(nodeList, true, false);

            foreach (XmlNode node in nodeList)
            {
                sub.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                sub.Name = node.SelectSingleNode("Name").InnerText;
                sub.X = double.Parse(node.SelectSingleNode("X").InnerText);
                sub.Y = double.Parse(node.SelectSingleNode("Y").InnerText);

                Approximation.ToLatLon(sub.X, sub.Y, 34, out noviY, out noviX);
                System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle();
                rectangle.Fill = System.Windows.Media.Brushes.Brown;
                rectangle.Width = 1;
                rectangle.Height = 1;
                double newX = Approximation.GetX(elementi.Width, noviX, minX, maxX), newY = Approximation.GetY(elementi.Height, noviY, minY, maxY);
                Coordinate coordinate = Approximation.FindPosition(positions, newX, newY);
                positions[coordinate.X, coordinate.Y] = true;

                positionsLines[coordinate.X, coordinate.Y] = pos.obj;
                // vodovodi[coordinate.x / 2, coordinate.y / 2] = new Vodov(positions[coordinate.x / 2, coordinate.y / 2], sub.Id);
                NewEntity newEntity = new NewEntity(sub.Id, coordinate.X, coordinate.Y);

                entities.Add(newEntity.Id, newEntity);
                //substations.Add(sub.Id, sub);

                rectangle.Margin = new Thickness(coordinate.X, coordinate.Y, 0, 0);
                rectangle.Name = "ime" + sub.Id.ToString() + "Q" + "true";

                ToolTip ttip = new ToolTip();

                ttip.Content = "Substation\nID: " + sub.Id + "  Name: " + sub.Name;
                ttip.Background = System.Windows.Media.Brushes.Black;
                ttip.Foreground = System.Windows.Media.Brushes.White;
                ttip.BorderBrush = System.Windows.Media.Brushes.Black;
                rectangle.ToolTip = ttip;

                elementi.Children.Add(rectangle);

                int indeks = elementi.Children.IndexOf(rectangle);
                //index.Add(sub.Id.ToString(), indeks);
            }

            #endregion

            #region Route

            LineEntity l = new LineEntity();
            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Lines/LineEntity");
            Dictionary<Vod, Vod> vodovizaSort = new Dictionary<Vod, Vod>();
            int ss=0;

            foreach (XmlNode node in nodeList)
            {
                l.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                l.Name = node.SelectSingleNode("Name").InnerText;
                if (node.SelectSingleNode("IsUnderground").InnerText.Equals("true"))
                {
                    l.IsUnderground = true;
                }
                else
                {
                    l.IsUnderground = false;
                }
                l.R = float.Parse(node.SelectSingleNode("R").InnerText);
                l.ConductorMaterial = node.SelectSingleNode("ConductorMaterial").InnerText;
                l.LineType = node.SelectSingleNode("LineType").InnerText;
                l.ThermalConstantHeat = long.Parse(node.SelectSingleNode("ThermalConstantHeat").InnerText);
                l.FirstEnd = long.Parse(node.SelectSingleNode("FirstEnd").InnerText);
                l.SecondEnd = long.Parse(node.SelectSingleNode("SecondEnd").InnerText);

               /* if (dimensions == 1500)
                {
                    if (!nodes.ContainsKey(l.FirstEnd) &&entities.ContainsKey(l.FirstEnd)&& entities.ContainsKey(l.SecondEnd))
                    {
                        int x1 = entities[l.FirstEnd].X;
                        int x2 = entities[l.SecondEnd].X;
                        int y1 = entities[l.FirstEnd].Y;
                        int y2 = entities[l.SecondEnd].Y;
                        double dist = distance(new System.Windows.Point(x1, y1),
                        new System.Windows.Point(x2, y2));

                        Vod Vod = new Vod(x1, y1, x2, y2, dist);
                        if (!vodovizaSort.ContainsKey(Vod))
                        {
                            Vod.Id = l.Id;
                            // Vod.Name = l.FirstEnd + "_" + l.SecondEnd;
                            vodovizaSort.Add(Vod, Vod);
                        }
                    }
                }else*/
                if (entities.ContainsKey(l.FirstEnd) && entities.ContainsKey(l.SecondEnd))
                {
                    int x1 = entities[l.FirstEnd].X;
                    int x2 = entities[l.SecondEnd].X;
                    int y1 = entities[l.FirstEnd].Y;
                    int y2 = entities[l.SecondEnd].Y;
                    double dist = distance(new System.Windows.Point(x1, y1),
                    new System.Windows.Point(x2,y2));

                    Vod Vod = new Vod(x1, y1, x2,y2, dist);
                    if (vodovizaSort.Where(x=>x.Key.X1==x1&&x.Key.X2==x2&&x.Key.Y1==y1&&x.Key.Y2==y2).FirstOrDefault().Key==null)
                    {
                        Vod.Id = l.Id;
                       // Vod.Name = l.FirstEnd + "_" + l.SecondEnd;
                        vodovizaSort.Add(Vod, Vod);
                    }
                    
                }
            }

            vodovizaSort.OrderBy(x => x.Key.Distanca);
            List<Vod> vodovizaSort1 = vodovizaSort.Keys.ToList();
            List<Vod> vodoviZaBrisanje = new List<Vod>();
            linesObjects = (bool[,])positions.Clone();
            DateTime sec1 = DateTime.Now;
            Console.WriteLine(vodovizaSort.Count());
            for (int i = 0; i < vodovizaSort1.Count; i++)
            {
                Vod node = vodovizaSort1[i];
                Polyline polyline = new Polyline();
                Coordinate source = new Coordinate {X= node.X1,Y= node.Y1 };
                Coordinate dest = new Coordinate { X = node.X2, Y = node.Y2 };
                Node destination = new Node(dest.X,dest.Y);
                BreadthFirstSearch.DoBFS(linesObjects, source, dest,ref destination,dimensions);
               
                if (!(destination.Parent is null))
                {
                    vodovizaSort.Remove(node);
                    int xInt = dest.X;
                    int yInt = dest.Y;
                    int cnt = destination.dst;
                    for (int j = 0; j < cnt; j++)
                    {
                        linesObjects[destination.X, destination.Y] = true;
                        polyline.Points.Add(new Point(destination.X + 0.5, destination.Y + 0.5));

                        destination = destination.Parent;
                    }
                    /*do
                    {
                        linesObjects[destination.X, destination.Y] = true;
                        polyline.Points.Add(new Point(destination.X + 0.5, destination.Y + 0.5));

                        destination = destination.Parent;
                    } while (destination.Parent != null);*/

                    polyline.Points.Add(new Point(destination.X + 0.5, destination.Y + 0.5));
                    polyline.Points.Add(new Point(source.X + 0.5, source.Y + 0.5));
                    polyline.Stroke = Brushes.Black;
                    polyline.StrokeThickness = 0.5;
                    elementi.Children.Add(polyline);
                }
            }
           
           /* foreach (var item in vodoviZaBrisanje)
            {
                vodovizaSort.Remove(item);
            }*/
            Console.WriteLine(vodovizaSort.Count());
            linesObjects = (bool[,])positions.Clone();
            List<Vod> vodovizaSort2 = vodovizaSort.Keys.ToList();
            for (int i = 0; i < vodovizaSort2.Count; i++)
            {

                Vod node = vodovizaSort2[i];
                Polyline polyline = new Polyline();
                Coordinate source = new Coordinate { X = node.X1, Y = node.Y1 };
                Coordinate dest = new Coordinate { X = node.X2, Y = node.Y2 };
                Node destination = new Node(dest.X, dest.Y);
                BreadthFirstSearch.DoBFS(linesObjects, source, dest, ref destination, dimensions);

                if (destination.Parent != null)
                {
                    vodoviZaBrisanje.Add(node);
                    int xInt = dest.X;
                    int yInt = dest.Y;
                    int cnt = destination.dst;
                    for (int j = 0; j < cnt; j++)
                    {
                       // linesObjects[destination.X, destination.Y] = true;
                        polyline.Points.Add(new Point(destination.X + 0.5, destination.Y + 0.5));

                        destination = destination.Parent;
                    }
                    polyline.Points.Add(new Point(destination.X + 0.5, destination.Y + 0.5));
                    polyline.Points.Add(new Point(source.X + 0.5, source.Y + 0.5));
                    polyline.Stroke = Brushes.Black;
                    polyline.StrokeThickness = 0.5;
                    elementi.Children.Add(polyline);
                }
            }
            #endregion
            DateTime now2 = DateTime.Now;
            TimeSpan ttime = now2 - sec1;//DateTime.Compare(now2, sec1);// 
            Console.WriteLine(ttime.TotalMilliseconds);

        }
        public double distance(System.Windows.Point p1, System.Windows.Point p2)
        {
            return Math.Sqrt((Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2))); 

        }
    }
}