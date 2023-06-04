using Models;
using Models.Entities;
using PredmetProjekat.BFS;
using PredmetProjekat.EntityCreationStrategy;
using PredmetProjekat.Positioning;
using System;
using System.Collections.Concurrent;
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
        private static int size = 500;
        public enum pos { obj, line, none }
        public double noviX, noviY;
        private int dimensions = 400;

        // private bool[,] positions = new bool[size, size];
        // public bool[,] linesObjects = new bool[size, size];
        // private List<long>[,] linesids = new List<long>[size, size];
        // Dictionary<long, NewEntity> entities = new Dictionary<long, NewEntity>();
        private GridData GridDataEntities = GridData.Instance;
       // Dictionary<long, List<Polyline>> lines= new Dictionary<long, List<Polyline>>();
        public MainWindow()
        {
            InitializeComponent();
          
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
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    GridDataEntities.linesIds[i, j] = new List<long>();
                }
            }
            elementi.Children.Clear();
            GridDataEntities.entities = new ConcurrentDictionary<long, NewEntity>();
            GridDataEntities.lines = new ConcurrentDictionary<long, List<Polyline>>();
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
            GridDataEntities.linesObjects = new bool[size, size];
            GridDataEntities.positions = new bool[size, size];
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("Geographic.xml");

            #region Node
            XmlNodeList nodeList;

            NodeEntity nodeobj = new NodeEntity();

            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Nodes/NodeEntity");
            Approximation nodeApproximation = Approximation.GetXYMinMax(nodeList);

            ConcreteNodeCreation concreteNode = new ConcreteNodeCreation();
            foreach (XmlNode node in nodeList)
            {
                Rectangle rectangle = concreteNode.CreateEntityRectangleFromXmlNode(
                    node,
                    nodeApproximation,
                    size,
                    dimensions
                    );
                elementi.Children.Add(rectangle);
                int indeks = elementi.Children.IndexOf(rectangle);
                // index.Add(nodeobj.Id.ToString(), indeks);
            }

            #endregion

            #region Switch
            SwitchEntity switchobj = new SwitchEntity();

            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Switches/SwitchEntity");

            Approximation switchApproximation = Approximation.GetXYMinMax(nodeList);

            ConcreteSwitchCreation concreteSwitch = new ConcreteSwitchCreation();

            foreach (XmlNode node in nodeList)
            {

                Rectangle rectangle = concreteSwitch.CreateEntityRectangleFromXmlNode(
                    node,
                    nodeApproximation,
                    size,
                    dimensions
                    );
                elementi.Children.Add(rectangle);

                int indeks = elementi.Children.IndexOf(rectangle);

            }

            #endregion

            #region Substation
            SubstationEntity sub = new SubstationEntity();

            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Substations/SubstationEntity");
            Approximation subApproximation = Approximation.GetXYMinMax(nodeList);

            ConcreteSubstationCreation concreteSubstation = new ConcreteSubstationCreation();
            foreach (XmlNode node in nodeList)
            {
                Rectangle rectangle = concreteSubstation.CreateEntityRectangleFromXmlNode(
                    node,
                    nodeApproximation,
                    size,
                    dimensions
                    );
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

                if (GridDataEntities.entities.ContainsKey(l.FirstEnd) && GridDataEntities.entities.ContainsKey(l.SecondEnd))
                {
                    int x1 = GridDataEntities.entities[l.FirstEnd].X;
                    int x2 = GridDataEntities.entities[l.SecondEnd].X;
                    int y1 = GridDataEntities.entities[l.FirstEnd].Y;
                    int y2 = GridDataEntities.entities[l.SecondEnd].Y;
                    double dist = distance(new Point(x1, y1), new Point(x2,y2));

                    Vod Vod = new Vod(x1, y1, x2,y2, dist);
                    if (vodovizaSort.Where(x=>x.Key.X1==x1&&x.Key.X2==x2&&x.Key.Y1==y1&&x.Key.Y2==y2).FirstOrDefault().Key == null &&
                        vodovizaSort.Where(x => x.Key.X2 == x1 && x.Key.X1 == x2 && x.Key.Y2 == y1 && x.Key.Y1 == y2).FirstOrDefault().Key == null
                        )
                    {
                        Vod.Id = l.Id;
                       
                        //Vod.Name = l.FirstEnd + "_" + l.SecondEnd;
                        vodovizaSort.Add(Vod, Vod);
                    }
                }
            }

            vodovizaSort.OrderBy(x => x.Key.Distanca);
            List<Vod> vodovizaSortCopy = vodovizaSort.Keys.ToList();

            GridDataEntities.linesObjects = (bool[,])GridDataEntities.positions.Clone();
            DateTime sec1 = DateTime.Now;
            Console.WriteLine(vodovizaSort.Count());

            for (int i = 0; i < vodovizaSortCopy.Count; i++)
            {
                Vod node = vodovizaSortCopy[i];
                Polyline polyline = new Polyline();
                Coordinate source = new Coordinate { X = node.X1,Y = node.Y1 };
                Coordinate dest = new Coordinate { X = node.X2, Y = node.Y2 };
                Node destination = new Node(dest.X,dest.Y);
                BreadthFirstSearch.DoBFS(GridDataEntities.linesObjects, source, dest,ref destination,size);
               
                if (!(destination.Parent is null))
                {
                    vodovizaSort.Remove(node);
                    int xInt = dest.X;
                    int yInt = dest.Y;
                    int cnt = destination.dst; double x;
                    double y;
                    Point p;
                    for (int j = 0; j < cnt ; j++)
                    {
                        if (j > 0 )
                        {
                            GridDataEntities.linesIds[destination.X, destination.Y].Add(node.Id);
                        }
                        GridDataEntities.linesObjects[destination.X, destination.Y] = true;
                        
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
                    GridDataEntities.lines.TryAdd(node.Id, new List<Polyline> { polyline });
                    elementi.Children.Add(polyline);
                }
            }

            Console.WriteLine(vodovizaSort.Count());
            GridDataEntities.linesObjects = (bool[,])GridDataEntities.positions.Clone();
            List<Vod> vodovizaSort2 = vodovizaSort.Keys.ToList();
            List<Ellipse> elipse = new List<Ellipse>();
            for (int i = 0; i < vodovizaSort2.Count; i++)
            {
                
                Vod node = vodovizaSort2[i];
                
                Polyline polyline = new Polyline();
                Coordinate source = new Coordinate { X = node.X1, Y = node.Y1 };
                Coordinate dest = new Coordinate { X = node.X2, Y = node.Y2 };
                Node destination = new Node(dest.X, dest.Y);
                BreadthFirstSearch.DoBFS(GridDataEntities.linesObjects, source, dest, ref destination, size);
                
                if (destination.Parent != null)
                {

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
                        if (GridDataEntities.linesIds[destination.X, destination.Y].Count != 0 && j > 0 )
                        {
                            List<long> idslines = new List<long>();
                            double xx = Approximation.GetCanvasX(dimensions, destination.X, size);
                            double yy = Approximation.GetCanvasY(dimensions, destination.Y, size);


                            Ellipse rectangle = new Ellipse();
                            rectangle.Fill = Brushes.Yellow;
                            rectangle.Width = 1.0 * dimensions / size;
                            rectangle.Height = 1.0 * dimensions / size;
                            rectangle.Margin = new Thickness(xx, yy, 0, 0);
                            idslines = GridDataEntities.linesIds[previous.X , previous.Y].Intersect(GridDataEntities.linesIds[destination.X, destination.Y]).ToList();
                            // idslines = idslines.Intersect(GridDataEntities.linesIds[destination.X, destination.Y]).ToList();
                            bool foundIntersection = false;
                            
                            if (idslines.Count == 0)
                            {
                                elipse.Add(rectangle);
                                foundIntersection = true;
                            }

                            if(!foundIntersection && GridDataEntities.linesIds[destination.X, destination.Y].
                                Intersect(GridDataEntities.linesIds[destination.Parent.X, destination.Parent.Y]).ToList().Count==0
                                && !(destination.Parent.X==source.X && destination.Parent.X==source.Y))
                            {
                                elipse.Add(rectangle);
                            }
                            

                        }

                        GridDataEntities.linesIds[destination.X, destination.Y].Add(node.Id);
                        polyline.Points.Add(p);
                        previous.X = destination.X;
                        previous.Y = destination.Y;
                        destination = destination.Parent;
                    }

                    GridDataEntities.linesIds[destination.X, destination.Y].Add(node.Id);
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