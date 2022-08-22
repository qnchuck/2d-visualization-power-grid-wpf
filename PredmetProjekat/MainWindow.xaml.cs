using Models;
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

        public double noviX, noviY;
        private bool[,] positions = new bool[400, 400];
        public MainWindow()
        {
            InitializeComponent();

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
                rectangle.Width = 2;
                rectangle.Height = 2;
                rectangle.Name = "ime" + nodeobj.Id.ToString();

                double newX = Approximation.GetX(elementi.Width,noviX, minX, maxX),
                       newY = Approximation.GetY(elementi.Height, noviY, minY, maxY);

                Coordinate coordinate = Approximation.FindPosition(positions,newX, newY);

                positions[coordinate.X/2, coordinate.Y/2] = true;
                //vodovodi[coordinate.x / 2, coordinate.y / 2] = new Vodov(positions[coordinate.x / 2, coordinate.y / 2], nodeobj.Id);
                rectangle.Margin = new Thickness(coordinate.X, coordinate.Y, 0, 0);
                NewEntity newEntity = new NewEntity(nodeobj.Id, coordinate.X, coordinate.Y);

                //entities.Add(newEntity.Id, newEntity);
                //nodes.Add(nodeobj.Id, nodeobj);
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
                rectangle.Width = 2;
                rectangle.Height = 2;
                double newX = Approximation.GetX(elementi.Width,noviX, minX, maxX), newY = Approximation.GetY(elementi.Height,noviY, minY, maxY);

                Coordinate coordinate = Approximation.FindPosition(positions,newX, newY);
                positions[coordinate.X/2, coordinate.Y/2] = true ;
                //vodovodi[coordinate.x / 2, coordinate.y / 2] = new Vodov(positions[coordinate.x / 2, coordinate.y / 2], switchobj.Id);
                NewEntity newEntity = new NewEntity(switchobj.Id, coordinate.X, coordinate.Y);

                //entities.Add(newEntity.Id, newEntity);
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
                rectangle.Width = 2;
                rectangle.Height = 2;
                double newX = Approximation.GetX(elementi.Width, noviX, minX, maxX), newY = Approximation.GetY(elementi.Height, noviY, minY, maxY);
                Coordinate coordinate = Approximation.FindPosition(positions, newX, newY);
                positions[coordinate.X/2, coordinate.Y/2] = true;

                // vodovodi[coordinate.x / 2, coordinate.y / 2] = new Vodov(positions[coordinate.x / 2, coordinate.y / 2], sub.Id);
                NewEntity newEntity = new NewEntity(sub.Id, coordinate.X, coordinate.Y);

               // entities.Add(newEntity.Id, newEntity);
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

        }
    }
}
