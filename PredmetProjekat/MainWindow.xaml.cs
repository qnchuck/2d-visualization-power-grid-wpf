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
        private static int size = 600;
        public enum pos { obj, line, none }
        public double noviX, noviY;
        private int dimensions = 400;
        private bool[,] positions = new bool[size, size];
        //private Dictionary<Coordinate, List<long>> linesids = new Dictionary<Coordinate, List<long>>();
        public bool[,] linesObjects = new bool[size, size];
        private List<long>[,] linesids = new List<long>[size, size];
        Dictionary<long, NewEntity> entities = new Dictionary<long, NewEntity>();

        Dictionary<long, List<Polyline>> lines= new Dictionary<long, List<Polyline>>();
        public MainWindow()
        {
            InitializeComponent();
            positions = new bool[dimensions, dimensions];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    linesids[i,j] = new List<long>();
                }
            }
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
            lines = new Dictionary<long, List<Polyline>>();
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

            elementi.Height = dimensions;
            elementi.Width = dimensions;
            linesObjects = new bool[size, size];
            positions = new bool[size, size];
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
                Rectangle rectangle = new Rectangle();
                rectangle.Fill = Brushes.Blue;
                rectangle.Width = 1.0 * dimensions / size;
                rectangle.Height = 1.0 * dimensions / size;
                rectangle.Name = "ime" + nodeobj.Id.ToString();

                double newX = Approximation.GetX(size, noviX, minX, maxX),
                       newY = Approximation.GetY(size, noviY, minY, maxY);

                Coordinate coordinate = Approximation.FindPosition(positions, newX, newY);
                positions[coordinate.X, coordinate.Y] = true;
                //vodovodi[coordinate.x / 2, coordinate.y / 2] = new Vodov(positions[coordinate.x / 2, coordinate.y / 2], nodeobj.Id);

                double x = Approximation.GetCanvasX(dimensions, coordinate.X, size);
                double y = Approximation.GetCanvasY(dimensions, coordinate.Y, size);
                rectangle.Margin = new Thickness(x, y, 0, 0);
                NewEntity newEntity = new NewEntity(nodeobj.Id, coordinate.X, coordinate.Y);

                if (dimensions == 1500)
                {
                    // nodes.Add(nodeobj.Id, nodeobj);
                }
                entities.Add(newEntity.Id, newEntity);
               // nodes.Add(nodeobj.Id, nodeobj);
                ToolTip ttip = new ToolTip();

                ttip.Content = "Node \n ID: " + nodeobj.Id + "  Name: " + nodeobj.Name;
                ttip.Background = Brushes.Black;
                ttip.Foreground = Brushes.White;
                ttip.BorderBrush = Brushes.Black;
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
                rectangle.Fill = Brushes.Green;
                rectangle.Width = 1.0 * dimensions / size;
                rectangle.Height = 1.0 * dimensions / size;
                double newX = Approximation.GetX(size, noviX, minX, maxX), newY = Approximation.GetY(size, noviY, minY, maxY);

                Coordinate coordinate = Approximation.FindPosition(positions, newX, newY);

                positions[coordinate.X, coordinate.Y] = true;
                //vodovodi[coordinate.x / 2, coordinate.y / 2] = new Vodov(positions[coordinate.x / 2, coordinate.y / 2], switchobj.Id);
                NewEntity newEntity = new NewEntity(switchobj.Id, coordinate.X, coordinate.Y);

                entities.Add(newEntity.Id, newEntity);
                // switches.Add(switchobj.Id, switchobj);

                rectangle.Name = "ime" + switchobj.Id.ToString() + "Q" + "true";


                double x = Approximation.GetCanvasX(dimensions, coordinate.X, size);
                double y = Approximation.GetCanvasY(dimensions, coordinate.Y, size);
                rectangle.Margin = new Thickness(x, y, 0, 0);
                ToolTip ttip = new ToolTip();

                ttip.Content = "Switch \n ID: " + switchobj.Id + "  Name: " + switchobj.Name + " Status: " + switchobj.Status;
                ttip.Background = Brushes.Black;
                ttip.Foreground = Brushes.White;
                ttip.BorderBrush = Brushes.Black;
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
                Rectangle rectangle = new Rectangle();
                rectangle.Fill = Brushes.Brown;
                rectangle.Width = 1.0 * dimensions / size;
                rectangle.Height = 1.0 * dimensions / size;
                double newX = Approximation.GetX(size, noviX, minX, maxX), newY = Approximation.GetY(size, noviY, minY, maxY);
                Coordinate coordinate = Approximation.FindPosition(positions, newX, newY);
               
                positions[coordinate.X, coordinate.Y] = true;

                
                NewEntity newEntity = new NewEntity(sub.Id, coordinate.X, coordinate.Y);

                entities.Add(newEntity.Id, newEntity);
                //substations.Add(sub.Id, sub);
                double x = Approximation.GetCanvasX(dimensions, coordinate.X, size);
                double y = Approximation.GetCanvasY(dimensions, coordinate.Y, size);
                rectangle.Margin = new Thickness(x,y, 0, 0);
                rectangle.Name = "ime" + sub.Id.ToString() + "Q" + "true";

                ToolTip ttip = new ToolTip();

                ttip.Content = "Substation\nID: " + sub.Id + "  Name: " + sub.Name;
                ttip.Background = Brushes.Black;
                ttip.Foreground = Brushes.White;
                ttip.BorderBrush = Brushes.Black;
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
            int ss = 0;

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
                    double dist = distance(new Point(x1, y1), new Point(x2,y2));

                    Vod Vod = new Vod(x1, y1, x2,y2, dist);
                    if (vodovizaSort.Where(x=>x.Key.X1==x1&&x.Key.X2==x2&&x.Key.Y1==y1&&x.Key.Y2==y2).FirstOrDefault().Key == null)
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
                Coordinate source = new Coordinate { X = node.X1,Y = node.Y1 };
                Coordinate dest = new Coordinate { X = node.X2, Y = node.Y2 };
                Node destination = new Node(dest.X,dest.Y);
                BreadthFirstSearch.DoBFS(linesObjects, source, dest,ref destination,size);
               
                if (!(destination.Parent is null))
                {
                    vodovizaSort.Remove(node);
                    int xInt = dest.X;
                    int yInt = dest.Y;
                    int cnt = destination.dst; double x;
                    double y;
                    Point p;
                    for (int j = 0; j < cnt; j++)
                    {
                        if (j > 0)
                        {
                            linesids[destination.X, destination.Y].Add(node.Id);
                        }
                        linesObjects[destination.X, destination.Y] = true;
                       
                        x = Approximation.GetCanvasX(dimensions, destination.X, size) + 0.5 * dimensions / size;
                        y = Approximation.GetCanvasY(dimensions, destination.Y, size) + 0.5 * dimensions / size;
                        p = new Point(x, y);

                        polyline.Points.Add(p);
                        destination = destination.Parent;
                        
                    }
                    x = Approximation.GetCanvasX(dimensions, destination.X, size) + 0.5 * dimensions / size;
                    y = Approximation.GetCanvasY(dimensions, destination.Y, size) + 0.5 * dimensions / size;
                    p = new Point(x, y);

                    polyline.Points.Add(p); //polyline.Points.Add(new Point(source.X + 0.5, source.Y + 0.5));
                    polyline.Stroke = Brushes.Black;
                    polyline.StrokeThickness = 0.5 * dimensions / size;
                    lines.Add(node.Id, new List<Polyline> { polyline });
                    elementi.Children.Add(polyline);
                }
            }

            Console.WriteLine(vodovizaSort.Count());
            linesObjects = (bool[,])positions.Clone();
            List<Vod> vodovizaSort2 = vodovizaSort.Keys.ToList();
            List<Ellipse> elipse = new List<Ellipse>();  
            for (int i = 0; i < vodovizaSort2.Count; i++)
            {

                Vod node = vodovizaSort2[i];
                Polyline polyline = new Polyline();
                Coordinate source = new Coordinate { X = node.X1, Y = node.Y1 };
                Coordinate dest = new Coordinate { X = node.X2, Y = node.Y2 };
                Node destination = new Node(dest.X, dest.Y);
                BreadthFirstSearch.DoBFS(linesObjects, source, dest, ref destination, size);

                if (destination.Parent != null)
                {
                    //vodoviZaBrisanje.Add(node);
                    int xInt = dest.X;
                    int yInt = dest.Y;
                    int cnt = destination.dst;
                    double x;
                    double y;
                    Point p;
                    Coordinate previous = new Coordinate{ X = destination.X, Y = destination.Y };

                    for (int j = 0; j < cnt; j++)
                    {

                        x = Approximation.GetCanvasX(dimensions, destination.X, size) + 0.5 * dimensions / size;
                        y = Approximation.GetCanvasY(dimensions, destination.Y, size) + 0.5 * dimensions / size;
                        p = new Point(x, y);
                        int xRise = 0;
                        int yRise = 0;
                        if (linesids[destination.X, destination.Y].Count != 0 && j>0)
                        {
                            List<long> idslines = new List<long>();
                            double xx = Approximation.GetCanvasX(dimensions, destination.X, size);
                            double yy = Approximation.GetCanvasY(dimensions, destination.Y, size);


                            Ellipse rectangle = new Ellipse();
                            rectangle.Fill = Brushes.Yellow;
                            rectangle.Width = 1.0 * dimensions / size;
                            rectangle.Height = 1.0 * dimensions / size;
                            rectangle.Margin = new Thickness(xx, yy, 0, 0);
                            idslines = linesids[previous.X , previous.Y].Intersect(linesids[destination.X, destination.Y]).ToList();
                            // idslines = idslines.Intersect(linesids[destination.X, destination.Y]).ToList();
                            bool foundIntersection = false;
                            if (idslines.Count == 0)
                            {
                                elipse.Add(rectangle);
                                foundIntersection = true;
                            }

                            if( !foundIntersection && linesids[destination.X, destination.Y].Intersect(linesids[destination.Parent.X, destination.Parent.Y]).ToList().Count==0)
                            {
                                elipse.Add(rectangle);
                            }
                            

                            /*if (destination.X > destination.Parent.X)
                            {
                                xRise = 1;
                            }
                            else if (destination.X < destination.Parent.X)
                            {
                                xRise = 2;
                            }
                            if (destination.Y > destination.Parent.Y)
                            {
                                yRise = 1;
                            }
                            else if (destination.Y < destination.Parent.Y)
                            {
                                yRise = 2;
                            }
                            //bool right = 1;  left = 2, down = 3, up = 4, 
                            int direction = 0;
                            if (xRise == 1 && yRise == 0)
                                direction = 1;
                            else if (xRise == 2 && yRise == 0)
                                direction = 2;
                            else if (xRise == 0 && yRise == 1)
                                direction = 3;
                            else if (xRise == 0 && yRise == 2)
                                direction = 4;
                            double xx = Approximation.GetCanvasX(dimensions, destination.X, size);
                            double yy = Approximation.GetCanvasY(dimensions, destination.Y, size);


                            Ellipse rectangle = new Ellipse();
                            rectangle.Fill = Brushes.Yellow;
                            rectangle.Width = 1.0 * dimensions / size;
                            rectangle.Height = 1.0 * dimensions / size;
                            rectangle.Margin = new Thickness(xx, yy, 0, 0);

                            // maybe it is the same thing for up and down --- and left and right direction
                            List<long> idslines = new List<long>();
                            bool foundIntersection = false;
                            if (direction == 3 || direction == 4  && j >0)
                            {
                               
                                if (destination.X == 0 || destination.X == size - 1 || destination.Y == 0 || destination.Y == size - 1)
                                {

                                }
                                else
                                {
                                    idslines = linesids[destination.X + 1, destination.Y].Intersect(linesids[destination.X - 1, destination.Y]).ToList();
                                    idslines = idslines.Intersect(linesids[destination.X, destination.Y]).ToList();
                                    if (idslines.Count != 0)
                                    {
                                        elipse.Add(rectangle);
                                        foundIntersection = true;
                                    }
                                    
                                    idslines = linesids[destination.X , destination.Y].Intersect(linesids[destination.X - 1, destination.Y]).ToList();
                                   // idslines = idslines.Intersect(linesids[destination.X, destination.Y]).ToList();
                                    if (idslines.Count != 0 && !foundIntersection)
                                    {
                                        elipse.Add(rectangle);
                                        foundIntersection = true;
                                    }
                                    idslines = linesids[destination.X , destination.Y].Intersect(linesids[destination.X + 1, destination.Y]).ToList();
                                   // idslines = idslines.Intersect(linesids[destination.X, destination.Y]).ToList();
                                    if (idslines.Count != 0 && !foundIntersection)
                                    {
                                        elipse.Add(rectangle);
                                    }
                                }
                            }
                            else if (direction == 1 || direction == 2)
                            {
                                if (destination.X == 0 || destination.X == size - 1 || destination.Y == 0 || destination.Y == size - 1)
                                {

                                }
                                else
                                {
                                    idslines = linesids[destination.X, destination.Y + 1].Intersect(linesids[destination.X, destination.Y - 1]).ToList();
                                    idslines = idslines.Intersect(linesids[destination.X, destination.Y]).ToList();
                                    if (idslines.Count != 0)
                                    {
                                        elipse.Add(rectangle);
                                        foundIntersection = true;
                                    }

                                    idslines = linesids[destination.X, destination.Y].Intersect(linesids[destination.X, destination.Y - 1]).ToList();

                                    if (idslines.Count != 0 && !foundIntersection)
                                    {
                                        elipse.Add(rectangle);
                                        foundIntersection = true;
                                    }

                                    idslines = linesids[destination.X, destination.Y].Intersect(linesids[destination.X, destination.Y + 1]).ToList();
                                    
                                    if (idslines.Count != 0 && !foundIntersection)
                                    {
                                        elipse.Add(rectangle);
                                        foundIntersection = true;
                                    }

                                }
                            }





*/
                            /*if (j>0 &&linesids[destination.Parent.X, destination.Parent.Y].Count == 0 && linesids[destination.X, destination.Y].Count > 1 
                                && destination.Parent.Parent != null)
                            {
                                double xx = Approximation.GetCanvasX(dimensions, destination.X, size);
                                double yy = Approximation.GetCanvasY(dimensions, destination.Y, size);


                                Ellipse rectangle = new Ellipse();
                                rectangle.Fill = Brushes.Yellow;
                                rectangle.Width = 1.0 * dimensions / size;
                                rectangle.Height = 1.0 * dimensions / size;
                                rectangle.Margin = new Thickness(xx, yy, 0, 0);
                                elipse.Add(rectangle);
                            }else 
                             if (j>0 && linesids[destination.Parent.X, destination.Parent.Y].Count > 1 && linesids[destination.X, destination.Y].Count == 1 
                                 && destination.Parent.Parent != null)
                             {
                                double xx = Approximation.GetCanvasX(dimensions, destination.X, size);
                                double yy = Approximation.GetCanvasY(dimensions, destination.Y, size);


                                Ellipse rectangle = new Ellipse();
                                rectangle.Fill = Brushes.Yellow;
                                rectangle.Width = 1.0 * dimensions / size;
                                rectangle.Height = 1.0 * dimensions / size;
                                rectangle.Margin = new Thickness(xx, yy, 0, 0);
                                elipse.Add(rectangle);
                             }*/
                        }

                        linesids[destination.X, destination.Y].Add(node.Id);
                        polyline.Points.Add(p);
                        previous.X = destination.X;
                        previous.Y = destination.Y;
                        destination = destination.Parent;
                    }

                    linesids[destination.X, destination.Y].Add(node.Id);
                    x = Approximation.GetCanvasX(dimensions, destination.X, size) + 0.5 * dimensions / size;
                    y = Approximation.GetCanvasY(dimensions, destination.Y, size) + 0.5 * dimensions / size;
                    p = new Point(x, y);

                    polyline.Points.Add(p);

                    polyline.Stroke = Brushes.Black;
                    polyline.StrokeThickness = 0.5 * dimensions / size;
                    elementi.Children.Add(polyline);
                }
            }
            for (int i = 0; i < elipse.Count; i++)
            {
                elementi.Children.Add(elipse[i]);
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