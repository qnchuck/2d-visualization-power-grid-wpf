using Models;
using Models.Entities;
using PredmetProjekat.BFS;
using PredmetProjekat.EntityCreationStrategy;
using PredmetProjekat.Positioning;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
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
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;
using WpfApp1;

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

        bool objplstxt = false;
        UIElement uIElement = new UIElement();
        UIElement uIElement2 = new UIElement();
        List<UIElement> children = new List<UIElement>();
        bool undoEl = false;
        int numberOfEntities;
        private enum button { nothing, ellipse, poligon, addtext, undo, redo, clear }
        List<button> buttons = new List<button>();

        public List<System.Windows.Point> points = new List<System.Windows.Point>();
        private static int size = 500;
        public enum pos { obj, line, none }
        public double noviX, noviY;
        private int dimensions = 400;
        private GridData GridDataEntities = GridData.Instance;

        public MainWindow()
        {
            InitializeComponent();
          
            elementi.Width = dimensions;
            elementi.Height = dimensions;
        }


        private void elementi_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(buttons.Count == 0) return;
            if (e.ChangedButton == MouseButton.Right)
            {
                switch (buttons[buttons.Count() - 1])
                {
                    case button.ellipse:
                        DrawEllipse drawEllipse = new DrawEllipse(e.GetPosition(elementi).X, e.GetPosition(elementi).Y, -1);
                        drawEllipse.Show();
                        undoEl = false;
                        buttons.Add(button.nothing);
                        break;
                    case button.poligon:
                        points.Add(e.GetPosition(elementi));
                        undoEl = false;
                        break;
                    case button.addtext:
                        AddText addText = new AddText(e.GetPosition(elementi), false);
                        undoEl = false;
                        addText.Show();
                        buttons.Add(button.nothing);
                        break;
                }
            }
            else if (buttons[buttons.Count - 1] == button.poligon)
            {
                DrawPoligon dp = new DrawPoligon(points, -1);
                dp.Show();
                buttons.Add(button.nothing);
            }

        }

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            if (elementi.Children.Count > numberOfEntities)
            {
                TextBlock tb = (elementi.Children[elementi.Children.Count - 1] as TextBlock);
                if (tb != null)
                {
                    if (!undoEl && tb.Name == "text" + (elementi.Children.Count - 2).ToString())
                    {
                        uIElement = elementi.Children[elementi.Children.Count - 2];
                        uIElement2 = elementi.Children[elementi.Children.Count - 1];
                        objplstxt = true;
                        elementi.Children.RemoveAt(elementi.Children.Count - 1);
                        elementi.Children.RemoveAt(elementi.Children.Count - 1);
                        undoEl = true;

                        buttons.Add(button.undo);
                    }
                }
                else if (!undoEl)
                {
                    uIElement = elementi.Children[elementi.Children.Count - 1];
                    elementi.Children.RemoveAt(elementi.Children.Count - 1);
                    undoEl = true;
                    objplstxt = false;
                    buttons.Add(button.undo);
                }
            }
            else
            if (!undoEl && buttons[buttons.Count() - 1] == button.clear)
            {
                objplstxt = false;
                foreach (var c in children)
                {
                    elementi.Children.Add(c);

                }
                undoEl = true;

                buttons.Add(button.undo);
            }

        }

        private void AddTextButton_Click(object sender, RoutedEventArgs e)
        {
            buttons.Add(button.addtext);
        }

        private void EllipseButton_Click(object sender, RoutedEventArgs e)
        {
            buttons.Add(button.ellipse);
        }

        private void PoligonButton_Click(object sender, RoutedEventArgs e)
        {
            buttons.Add(button.poligon);
        }

        private void RedoButton_Click(object sender, RoutedEventArgs e)
        {
            if (undoEl == true && buttons[buttons.Count - 1] == button.undo && !elementi.Children.Contains(uIElement))
            {
                buttons.Add(button.redo);
                elementi.Children.Add(uIElement);
                if (objplstxt)
                    elementi.Children.Add(uIElement2);
                undoEl = false;
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            buttons.Add(button.clear);
            undoEl = false;
            for (int i = numberOfEntities; i < elementi.Children.Count; i++)
            {
                children.Add(elementi.Children[i]);
            }
            elementi.Children.RemoveRange(numberOfEntities, elementi.Children.Count-numberOfEntities);
           
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
                rectangle.Uid = node.SelectSingleNode("Id").InnerText;
                elementi.Children.Add(rectangle);
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
                    node,nodeApproximation,size,dimensions
                    );
                rectangle.Uid = node.SelectSingleNode("Id").InnerText;
                elementi.Children.Add(rectangle);
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

                rectangle.Uid = node.SelectSingleNode("Id").InnerText;
                elementi.Children.Add(rectangle);
            }

            #endregion

            #region Route

            LineEntity l = new LineEntity();
            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Lines/LineEntity");
            Dictionary<Vod, Vod> vodovizaSort = new Dictionary<Vod, Vod>();
            int ss = 0;
            int ss1 = 0;

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
                    if (vodovizaSort.Where(x => x.Key.X1 == x1 && x.Key.X2 == x2 && x.Key.Y1 == y1 && x.Key.Y2 == y2).FirstOrDefault().Key == null &&
                        vodovizaSort.Where(x => x.Key.X2 == x1 && x.Key.X1 == x2 && x.Key.Y2 == y1 && x.Key.Y1 == y2).FirstOrDefault().Key == null
                        )
                    {
                        Vod.Id = l.Id;
                        Vod.FirstEnd = l.FirstEnd;
                        Vod.SecondEnd = l.SecondEnd;
                        GridDataEntities.vodoviClick.TryAdd(Vod.Id, Vod);
                        vodovizaSort.Add(Vod, Vod);
                    }
                    else {
                        ss++;
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
                BreadthFirstSearch.DoBFS(GridDataEntities.linesObjects, source, dest, ref destination,size);
               
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
                        GridDataEntities.linesIds[destination.X, destination.Y].Add(node.Id);
                        
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
                    GridDataEntities.linesIds[destination.X, destination.Y].Add(node.Id);
                    polyline.Uid = node.Id.ToString();
                    polyline.MouseRightButtonUp += Polyline_Click;
                    polyline.Points.Add(p); //polyline.Points.Add(new Point(source.X + 0.5, source.Y + 0.5));
                    polyline.Stroke = Brushes.Black;
                    polyline.StrokeThickness = 0.5 * dimensions / size;
                    ToolTip ttip = new ToolTip();
                    ttip.Content = "Line \nID: " + node.Id;
                    ttip.Background = System.Windows.Media.Brushes.Black;
                    ttip.Foreground = System.Windows.Media.Brushes.White;
                    ttip.BorderBrush = System.Windows.Media.Brushes.Black;
                    polyline.ToolTip = ttip;
                    GridDataEntities.lines.TryAdd(node.Id, new List<Polyline> { polyline });
                    elementi.Children.Add(polyline);
                }
            }

            Console.WriteLine(vodovizaSort.Count());
            GridDataEntities.linesObjects = (bool[,])GridDataEntities.positions.Clone();
            List<Vod> vodovizaSortDrugiKrug = vodovizaSort.Keys.ToList();
            List<Ellipse> elipse = new List<Ellipse>();
            for (int i = 0; i < vodovizaSortDrugiKrug.Count; i++)
            {

                Vod node = vodovizaSortDrugiKrug[i];

                Polyline polyline = new Polyline();

                polyline.MouseRightButtonUp += Polyline_Click;
                Coordinate source = new Coordinate { X = node.X1, Y = node.Y1 };
                Coordinate dest = new Coordinate { X = node.X2, Y = node.Y2 };
                Node destination = new Node(dest.X, dest.Y);

                GridDataEntities.linesIds[destination.X, destination.Y].Add(node.Id);
                BreadthFirstSearch.DoBFS(GridDataEntities.linesObjects, source, dest, ref destination, size);
                bool isOverlaping = false;
                if (destination.Parent != null)
                {

                    int xInt = dest.X;
                    int yInt = dest.Y;
                    int cnt = destination.dst;
                    double x;
                    double y;
                    Point p;
                    Coordinate previous = new Coordinate { X = destination.X, Y = destination.Y };


                    for (int j = 0; j < cnt; j++)
                    {

                        x = Approximation.GetCanvasX(dimensions, destination.X, size) + 0.5 * dimensions / size;
                        y = Approximation.GetCanvasY(dimensions, destination.Y, size) + 0.5 * dimensions / size;
                        p = new Point(x, y);
                        int xRise = 0;
                        int yRise = 0;


                        if (GridDataEntities.linesIds[destination.X, destination.Y].Count != 0 )
                        {
                            List<long> idslines = new List<long>();
                            double xx = Approximation.GetCanvasX(dimensions, destination.X, size);
                            double yy = Approximation.GetCanvasY(dimensions, destination.Y, size);


                            Ellipse rectangle = new Ellipse();
                            rectangle.Fill = Brushes.Yellow;
                            rectangle.Width = 1.0 * dimensions / size;
                            rectangle.Height = 1.0 * dimensions / size;
                            rectangle.Margin = new Thickness(xx, yy, 0, 0);

                            bool foundIntersection = false;
                            idslines = GridDataEntities.linesIds[previous.X, previous.Y].
                                Intersect(GridDataEntities.linesIds[destination.X, destination.Y]).ToList();

                            if (idslines.Count == 0)
                            {
                                if (!(destination.X == source.X && destination.Y == source.Y) && !(destination.X == dest.X && destination.Y == dest.Y))
                                {
                                    elipse.Add(rectangle);
                                }
                                
                                double nx = Approximation.GetCanvasX(dimensions, destination.Parent.X, size) + 0.5 * dimensions / size;
                                double ny = Approximation.GetCanvasY(dimensions, destination.Parent.Y, size) + 0.5 * dimensions / size;
                                Point np = new Point(nx, ny);

                                polyline.Uid = node.Id.ToString();
                                polyline.MouseRightButtonUp += Polyline_Click;
                                polyline.Points.Add(p);
                                polyline.Points.Add(np);
                                polyline.Uid = l.Id.ToString();

                                polyline.Stroke = Brushes.Black;
                                polyline.StrokeThickness = 0.5 * dimensions / size;

                                polyline.ToolTip = CreateToolTip(node.Id);
                                elementi.Children.Add(polyline);

                                polyline = new Polyline();
                                polyline.Uid = node.Id.ToString();
                                polyline.MouseRightButtonUp += Polyline_Click;
                                polyline.Stroke = Brushes.Black;
                                polyline.StrokeThickness = 0.5 * dimensions / size;

                                foundIntersection = true;
                            }

                            if (!foundIntersection && GridDataEntities.linesIds[destination.X, destination.Y].
                                Intersect(GridDataEntities.linesIds[destination.Parent.X, destination.Parent.Y]).ToList().Count == 0
                                && !(destination.Parent.X == source.X && destination.Parent.Y == source.Y)
                                )
                            {
                                double nx = Approximation.GetCanvasX(dimensions, previous.X, size) + 0.5 * dimensions / size;
                                double ny = Approximation.GetCanvasY(dimensions, previous.Y, size) + 0.5 * dimensions / size;
                                Point np = new Point(nx, ny);
                                polyline.Points.Add(np);
                                polyline.Points.Add(p);

                                polyline.MouseRightButtonUp += Polyline_Click;
                                if (!(destination.X == source.X && destination.Y == source.Y) && !(destination.X == dest.X && destination.Y == dest.Y))
                                {
                                    elipse.Add(rectangle);
                                }
                            }


                        }
                        else
                        
                        if (GridDataEntities.linesIds[destination.X, destination.Y].Count == 0)
                        {
                            bool anything = false;
                            if (GridDataEntities.linesIds[previous.X, previous.Y].Count > 1)
                            {
                                double nx = Approximation.GetCanvasX(dimensions, previous.X, size) + 0.5 * dimensions / size;
                                double ny = Approximation.GetCanvasY(dimensions, previous.Y, size) + 0.5 * dimensions / size;
                                Point np = new Point(nx, ny);
                                polyline.Points.Add(np);
                                polyline.Points.Add(p);
                                anything = true;
                            }
                            if (GridDataEntities.linesIds[destination.Parent.X, destination.Parent.Y].Count > 0)
                            {
                                double nx = Approximation.GetCanvasX(dimensions, destination.Parent.X, size) + 0.5 * dimensions / size;
                                double ny = Approximation.GetCanvasY(dimensions, destination.Parent.Y, size) + 0.5 * dimensions / size;
                                Point np = new Point(nx, ny);

                                anything = true;
                                polyline.Points.Add(p);
                                polyline.Points.Add(np);
                                polyline.Uid = node.Id.ToString();

                                polyline.MouseRightButtonUp += Polyline_Click;
                                polyline.Stroke = Brushes.Black;
                                polyline.StrokeThickness = 0.5 * dimensions / size;
                                
                                polyline.ToolTip = CreateToolTip(node.Id); 
                                elementi.Children.Add(polyline);
                                polyline = new Polyline();
                                polyline.Uid = node.Id.ToString();

                                polyline.MouseRightButtonUp += Polyline_Click;
                                polyline.Stroke = Brushes.Black;
                                polyline.StrokeThickness = 0.5 * dimensions / size;

                            }
                            if (!anything)
                            {
                                polyline.Points.Add(p);
                            }
                        }


                        GridDataEntities.linesIds[destination.X, destination.Y].Add(node.Id);
                        previous.X = destination.X;
                        previous.Y = destination.Y;
                        destination = destination.Parent;
                    }

                    GridDataEntities.linesIds[destination.X, destination.Y].Add(node.Id);
                    x = Approximation.GetCanvasX(dimensions, destination.X, size) + 0.5 * dimensions / size;
                    y = Approximation.GetCanvasY(dimensions, destination.Y, size) + 0.5 * dimensions / size;
                    p = new Point(x, y);

                    polyline.Points.Add(p);
                    polyline.Uid = node.Id.ToString();

                    polyline.Stroke = Brushes.Black;
                    polyline.StrokeThickness = 0.5 * dimensions / size;
                  /*  ToolTip ttip = new ToolTip();
                    ttip.Content = "Line \nID: " + node.Id;
                    ttip.Background = System.Windows.Media.Brushes.Black;
                    ttip.Foreground = System.Windows.Media.Brushes.White;
                    ttip.BorderBrush = System.Windows.Media.Brushes.Black;*/
                    polyline.ToolTip = CreateToolTip(node.Id);
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
            Console.WriteLine("Vreme potrebno za iscrtavanje na platnu dimenzija " + dimensions + "x" + dimensions+ " iznosi :"  + ttime.TotalSeconds);
            numberOfEntities = elementi.Children.Count;

           // Canvas myCanvas = elementi; // Your Canvas object
          //  string filePath = "C:\\temp\\Image.png"; // Your desired file path
           // SaveCanvasAsImage(myCanvas, filePath, dimensions.ToString());
        }
        public double distance(System.Windows.Point p1, System.Windows.Point p2)
        {
            return Math.Sqrt((Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2)));

        }
        private void CaptureScreenshot_Click(object sender, RoutedEventArgs e)
        {

            double originalLeft = Canvas.GetLeft(elementi);
            double originalTop = Canvas.GetTop(elementi);



            elementi.Measure(new System.Windows.Size(elementi.Width, elementi.Height));
            elementi.Arrange(new Rect(new System.Windows.Size(elementi.Width, elementi.Height)));


            double offsetX = (skrolbar.ViewportWidth - elementi.ActualWidth) / 2;
            double offsetY = (skrolbar.ViewportHeight - elementi.ActualHeight) / 2;
            Canvas.SetLeft(elementi, offsetX);
            Canvas.SetTop(elementi, offsetY);

            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)elementi.Width, (int)elementi.Height, 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(elementi);


            Canvas.SetLeft(elementi, originalLeft);
            Canvas.SetTop(elementi, originalTop);


            PngBitmapEncoder pngImage = new PngBitmapEncoder();
            pngImage.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
            string fileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
            string filePath = System.IO.Path.Combine(@"C:\Users\HP\Desktop\predmet projekat\PredmetProjekat", fileName);
            using (Stream fileStream = File.Create(filePath))
            {
                pngImage.Save(fileStream);
            }

            MessageBox.Show("Screenshot captured and saved successfully!");
        }
        private void Polyline_Click(object sender, MouseButtonEventArgs e)
        {
            Polyline clickedPolyline = (Polyline)sender;
            string polylineId = clickedPolyline.Uid;
            Vod vod = GridDataEntities.vodoviClick[long.Parse(polylineId)];

            for (int i = 0; i < elementi.Children.Count; i++)
            {
                if (elementi.Children[i].Uid == vod.FirstEnd.ToString())
                {
                    System.Windows.Shapes.Rectangle rectangle = elementi.Children[i] as System.Windows.Shapes.Rectangle;
                    StorePreviousColor(rectangle);
                    rectangle.Fill = System.Windows.Media.Brushes.Orange;
                    StartColorTimer(rectangle);
                }
                if (elementi.Children[i].Uid == vod.SecondEnd.ToString())
                {
                    System.Windows.Shapes.Rectangle rectangle = elementi.Children[i] as System.Windows.Shapes.Rectangle;
                    StorePreviousColor(rectangle);
                    rectangle.Fill = System.Windows.Media.Brushes.Orange;
                    StartColorTimer(rectangle);
                }
            }
        }

        private Dictionary<System.Windows.Shapes.Rectangle, System.Windows.Media.Brush> previousColors = new Dictionary<System.Windows.Shapes.Rectangle, System.Windows.Media.Brush>();

        private void StorePreviousColor(System.Windows.Shapes.Rectangle rectangle)
        {
            previousColors[rectangle] = rectangle.Fill;
        }

        private void StartColorTimer(System.Windows.Shapes.Rectangle rectangle)
        { 
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(5000);
            timer.Tick += (sender, e) =>
            { 
                rectangle.Fill = previousColors[rectangle];
                 
                timer.Stop();
            };
             
            timer.Start();
        }


        ToolTip CreateToolTip(long id)
        {

            ToolTip ttip = new ToolTip();
            ttip.Content = "Line \nID: " + id.ToString();
            ttip.Background = System.Windows.Media.Brushes.Black;
            ttip.Foreground = System.Windows.Media.Brushes.White;
            ttip.BorderBrush = System.Windows.Media.Brushes.Black;
            return ttip;
        }

      
    }
}