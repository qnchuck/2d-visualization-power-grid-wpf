using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace PredmetProjekat.Positioning
{
    interface IApproximate
    {
        void FindPosition(bool[,] positions, out Point point);
        void ToLatLon(double utmX, double utmY, int zoneUTM, out double latitude, out double longitude);
        double GetXYMinMax(XmlNodeList nodeList, bool xy, bool minmax);
        double GetX(double x, double min, double max);
        double GetY(double y, double min, double max);
        
    }
}
