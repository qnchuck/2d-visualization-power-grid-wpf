using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
   
    public class Node
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Node Parent { get; set; }
        public int dst;
        
        public Node(int x, int y, ref Node parent)
        {
            dst = 0;
            Parent = parent;
            X = x;
            Y = y;
        }
        public Node(int x, int y)
        {
            dst = 0;
            Parent = null;
            X = x;
            Y = y;
        }
      /*  public void setAsParent(ref Node node)
        {
            this.Parent = node;
        }*/
    }
    
}
