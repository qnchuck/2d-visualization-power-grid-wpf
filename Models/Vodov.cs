using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Vodov
    {
        private bool free;
        private long id;

        public Vodov()
        {
            free = true;
            id = 0;
        }
        public Vodov(bool free, long id)
        {
            this.Free = free;
            this.Id = id;
        }

        public bool Free { get => free; set => free = value; }
        public long Id { get => id; set => id = value; }
    }
}
