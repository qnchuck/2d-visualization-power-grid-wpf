using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class Vod
    {
        private int y2;
        private double distanca;
        private int x1;
        private int y1;
        private int x2;
        private long id;
        private string name;

        public Vod(int x1, int y1, int x2, int y2, double distanca)
        {
            this.X1 = x1;
            this.Y1 = y1;
            this.X2 = x2;
            this.Y2 = y2;
            this.Distanca = distanca;
        }

        public int X1 { get => x1; set => x1 = value; }
        public int Y1 { get => y1; set => y1 = value; }
        public int X2 { get => x2; set => x2 = value; }
        public int Y2 { get => y2; set => y2 = value; }
        public double Distanca { get => distanca; set => distanca = value; }
        public long Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
    }
}
