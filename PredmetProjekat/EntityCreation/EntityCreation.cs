using Models;
using Models.Entities;
using PredmetProjekat.Positioning;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;
using static PredmetProjekat.MainWindow;

namespace PredmetProjekat.EntityCreationStrategy
{
    public abstract class EntityCreation
    {
        protected PowerEntity powerEntity = new PowerEntity();
        protected Rectangle rectangle;
        protected GridData gridDataEntities = GridData.Instance;
        protected double convertedX, convertedY;
        protected ToolTip toolTip = new ToolTip();

        public Rectangle CreateEntityRectangleFromXmlNode(
            XmlNode node,
            Approximation nodeApproximation,
            int size, 
            int dimensions
        )
        {
            this.InitPowerEntity(node);
            this.CreateRectangle(dimensions, size);
            this.SetRectangleColor();
            this.AddEntityToEntityList(size,dimensions,nodeApproximation);
            this.CreateRectangleToolTip();
            this.SetToolTipContent();

            return rectangle; 
        }
        protected void InitPowerEntity(XmlNode node)
        {
            powerEntity.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
            powerEntity.Name = node.SelectSingleNode("Name").InnerText;
            powerEntity.X = double.Parse(node.SelectSingleNode("X").InnerText);
            powerEntity.Y = double.Parse(node.SelectSingleNode("Y").InnerText);

            Approximation.ToLatLon(powerEntity.X, powerEntity.Y, 34, out convertedY, out convertedX);

        }
        protected void CreateRectangle(int dimensions, int size)
        {
            rectangle = new Rectangle();
            rectangle.Width = 1.0 * dimensions / size;
            rectangle.Height = 1.0 * dimensions / size;
            rectangle.Name = "ime" + powerEntity.Id.ToString();
        }
        protected abstract void SetRectangleColor();
        protected abstract void SetToolTipContent();
        protected void CreateRectangleToolTip()
        {
            toolTip = new ToolTip();
            toolTip.Background = Brushes.Black;
            toolTip.Foreground = Brushes.White;
            toolTip.BorderBrush = Brushes.Black;
            rectangle.ToolTip = toolTip;
        }
        protected void AddEntityToEntityList(
            int size, int dimensions,
            Approximation nodeApproximation)
        {
            double newX = Approximation.GetX(size, convertedX, nodeApproximation.YMinimum, nodeApproximation.YMaximum),
                   newY = Approximation.GetY(size, convertedY, nodeApproximation.XMinimum, nodeApproximation.XMaximum);

            Coordinate coordinate = Approximation.FindPosition(
                gridDataEntities.positions, newX, newY
                );

            gridDataEntities.positions[coordinate.X, coordinate.Y] = true;
            
            double x = Approximation.GetCanvasX(dimensions, coordinate.X, size);
            double y = Approximation.GetCanvasY(dimensions, coordinate.Y, size);
            rectangle.Margin = new Thickness(x, y, 0, 0);
            NewEntity newEntity = new NewEntity(powerEntity.Id, coordinate.X, coordinate.Y);

            gridDataEntities.entities.TryAdd(newEntity.Id, newEntity);
        }
    }
}
