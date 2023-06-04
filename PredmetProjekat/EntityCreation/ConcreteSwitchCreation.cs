using Models.Entities;
using Models;
using PredmetProjekat.Positioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;
using System.Xml;
using static PredmetProjekat.MainWindow;

namespace PredmetProjekat.EntityCreationStrategy
{
    public class ConcreteSwitchCreation : EntityCreation
    {
        protected override void SetToolTipContent()
        {
            toolTip.Content = "Switch \n ID: " + powerEntity.Id + "  Name: " + powerEntity.Name;
        }

        protected override void SetRectangleColor()
        {
            rectangle.Fill = Brushes.Green;
        }
    }
}
