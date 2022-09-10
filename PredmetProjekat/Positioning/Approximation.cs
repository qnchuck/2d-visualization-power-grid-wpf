using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using static PredmetProjekat.MainWindow;

namespace PredmetProjekat.Positioning
{
    public class Approximation 
    {
        
        public static Coordinate FindPosition(bool[,] positions, double x,double y)
        {
            Coordinate coordinate = new Coordinate { X = 0, Y = 0 };
            bool foundPosition = false;
            short integerX = (short)(x - 1);
            short integerY = (short)(y - 1);

            if (integerX < 0)
            {
                integerX = 1;
            }
            if (integerY < 0)
            {
                integerY = 1;
            }

            if (!positions[integerX, integerY])
            {
                positions[integerX, integerY] = true;
                coordinate.X = integerX;
                coordinate.Y = integerY;
                return coordinate;
            }
            else
            {
                int circle = 1;
                while (!foundPosition)
                {
                    int rowMinimum = integerY - circle < 0 ? integerY : integerY - circle;
                    int rowMaximum = integerY + circle > positions.GetLength(0) ? integerY : integerY + circle;
                    int columnMinimum = integerX - circle < 0 ? integerX : integerX - circle;
                    int columnMaximum = integerX + circle > positions.GetLength(1) ? integerX : integerX + circle;
                    for (int i = rowMinimum; i <= rowMaximum; i++)
                    {
                        for (int j = columnMinimum; j <= columnMaximum; j++)
                        {
                            if (i != integerY || j != integerX)
                            {
                                if (!positions[j, i])
                                {
                                    positions[j, i] = true;
                                    coordinate.X = (short)j;
                                    coordinate.Y = (short)i;
                                    foundPosition = true;
                                    return coordinate;
                                }
                            }
                        }
                    }
                    circle++;
                }
            }
            return coordinate;
        }

        public static double GetX(double width,double x, double min, double max)
        {
            var position = (x - min) / (max - min);
            return width * position;
        }
        public static double GetCanvasX(double canvasw,double x, double matw)
        {
            
            return canvasw*x/matw;
        }

        public static double GetY(double height,double y, double min, double max)
        {
            var position = (y - min) / (max - min); 
            return height - height * position;
        }
        public static double GetCanvasY(double canvash,double y,double math)
        {
            return canvash * y / math;
        }
        

        public static double GetXYMinMax(XmlNodeList nodeList, bool xy, bool minmax)
        {

            NodeEntity nodeobj = new NodeEntity();
            double noviX, noviY;
            List<double> novixx = new List<double>();
            List<double> noviyy = new List<double>();
            foreach (XmlNode noden in nodeList)
            {
                nodeobj.X = double.Parse(noden.SelectSingleNode("X").InnerText);
                nodeobj.Y = double.Parse(noden.SelectSingleNode("Y").InnerText);
                ToLatLon(nodeobj.X, nodeobj.Y, 34, out noviX, out noviY);
                novixx.Add(noviX);
                noviyy.Add(noviY);
            }
            if (xy == true && minmax == true)
            {
                return novixx.Min();
            }
            else if (xy == true && minmax == false)
            {
                return novixx.Max();
            }
            else if (xy == false && minmax == true)
            {
                return noviyy.Min();
            }
            else
            {
                return noviyy.Max();
            }
        }


        public static void ToLatLon(double utmX, double utmY, int zoneUTM, out double latitude, out double longitude)
        {
            bool isNorthHemisphere = true;

            var diflat = -0.00066286966871111111111111111111111111;
            var diflon = -0.0003868060578;

            var zone = zoneUTM;
            var c_sa = 6378137.000000;
            var c_sb = 6356752.314245;
            var e2 = Math.Pow((Math.Pow(c_sa, 2) - Math.Pow(c_sb, 2)), 0.5) / c_sb;
            var e2cuadrada = Math.Pow(e2, 2);
            var c = Math.Pow(c_sa, 2) / c_sb;
            var x = utmX - 500000;
            var y = isNorthHemisphere ? utmY : utmY - 10000000;

            var s = ((zone * 6.0) - 183.0);
            var lat = y / (c_sa * 0.9996);
            var v = (c / Math.Pow(1 + (e2cuadrada * Math.Pow(Math.Cos(lat), 2)), 0.5)) * 0.9996;
            var a = x / v;
            var a1 = Math.Sin(2 * lat);
            var a2 = a1 * Math.Pow((Math.Cos(lat)), 2);
            var j2 = lat + (a1 / 2.0);
            var j4 = ((3 * j2) + a2) / 4.0;
            var j6 = ((5 * j4) + Math.Pow(a2 * (Math.Cos(lat)), 2)) / 3.0;
            var alfa = (3.0 / 4.0) * e2cuadrada;
            var beta = (5.0 / 3.0) * Math.Pow(alfa, 2);
            var gama = (35.0 / 27.0) * Math.Pow(alfa, 3);
            var bm = 0.9996 * c * (lat - alfa * j2 + beta * j4 - gama * j6);
            var b = (y - bm) / v;
            var epsi = ((e2cuadrada * Math.Pow(a, 2)) / 2.0) * Math.Pow((Math.Cos(lat)), 2);
            var eps = a * (1 - (epsi / 3.0));
            var nab = (b * (1 - epsi)) + lat;
            var senoheps = (Math.Exp(eps) - Math.Exp(-eps)) / 2.0;
            var delt = Math.Atan(senoheps / (Math.Cos(nab)));
            var tao = Math.Atan(Math.Cos(delt) * Math.Tan(nab));

            longitude = ((delt * (180.0 / Math.PI)) + s) + diflon;
            latitude = ((lat + (1 + e2cuadrada * Math.Pow(Math.Cos(lat), 2) - (3.0 / 2.0) * e2cuadrada * Math.Sin(lat) * Math.Cos(lat) * (tao - lat)) * (tao - lat)) * (180.0 / Math.PI)) + diflat;

        }
    }
}
