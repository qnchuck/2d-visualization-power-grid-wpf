using Models.Entities;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Xml;
using PredmetProjekat.Positioning;
using static PredmetProjekat.MainWindow;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace PredmetProjekat.EntityCreationStrategy
{
    public class ConcreteNodeCreation:EntityCreation
    {
        protected override void SetToolTipContent()
        {
            toolTip.Content = "Node \n ID: " + powerEntity.Id + "  Name: " + powerEntity.Name;
        }

        protected override void SetRectangleColor()
        {
            rectangle.Fill = Brushes.Blue;
        }
    }
}
