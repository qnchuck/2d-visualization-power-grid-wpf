using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Coordinate
    {
        public int x, y;
        
        public Coordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
            
        }

        public int X { get => x; set => x = value; }
        public int Y { get => y; set => y = value; }
    }
}
