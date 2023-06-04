using Models;
using Models.Entities;
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
    public class ConcreteSubstationCreation : EntityCreation
    {
        protected override void SetToolTipContent()
        {
            toolTip.Content = "Substation \n ID: " + powerEntity.Id + "  Name: " + powerEntity.Name;
        }

        protected override void SetRectangleColor()
        {
            rectangle.Fill = Brushes.Brown;
        }
    }
}
