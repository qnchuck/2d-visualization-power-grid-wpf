using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class NewEntity
    {
        private long id;
        private int x;
        private int y;

        public NewEntity(long id, int x, int y)
        {
            this.Id = id;
            this.X = x;
            this.Y = y;
        }

        public long Id { get => id; set => id = value; }
        public int X { get => x; set => x = value; }
        public int Y { get => y; set => y = value; }
    }
}
