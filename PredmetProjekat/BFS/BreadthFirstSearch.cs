using Models;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static PredmetProjekat.MainWindow;

namespace PredmetProjekat.BFS
{

    public class BreadthFirstSearch
    {
        static int[] rowNum = { -1, 0, 0, 1 };
        static int[] colNum = { 0, -1, 1, 0 };

        static bool isValid(bool[,] vis,int row, int col, int rowcol)
        {
           return (row >= 0) && (row < rowcol) &&
                    (col >= 0) && (col < rowcol);
        }
      
        public static Node DoBFS(bool[,] mat, Coordinate src,
                               Coordinate dst, ref Node destination, int size)
        {
            
            // check source and destination cell
            // of the matrix have value 1
            bool[,] visited = new bool[size,size];
            // Mark the source cell as visited
            visited[src.X, src.Y] = true;
            try
            {
                Queue<Node> nq = new Queue<Node>();
                Node node = new Models.Entities.Node(src.X, src.Y)
                {
                    Parent = null
                };
                nq.Enqueue(node);

                Node curr = new Node(1,1);
                while (nq.Count != 0)
                {
                    curr = nq.Peek();
                 
                    for (int i = 0; i < 4; i++)
                    {
                        int row = curr.X + rowNum[i];
                        int col = curr.Y + colNum[i];
                       
                        if (isValid(visited,row, col,size) && (!mat[row, col] || (row == dst.X && col == dst.Y)) &&
                        !visited[row, col])
                        {
                            visited[row, col] = true;

                            Node parent = nq.Peek();
                            Node nodeTemp = new Node(row, col, ref parent)
                            {
                                dst = parent.dst + 1
                            };

                            if (dst.X == row && dst.Y == col)
                            {
                                destination.dst = nodeTemp.dst;
                                destination.Parent = parent;
                                return destination;
                            }
                            nq.Enqueue(nodeTemp);
                        }
                    }
                    nq.Dequeue();
                }
                /*DateTime sec2 = DateTime.Now;//DateTime.Compare(now2, sec1);// 
                TimeSpan tt = sec2 - now2;//DateTime.Compare(now2, sec1);// 
                Console.WriteLine(tt.TotalMilliseconds);*/
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // Return -1 if destination cannot be reached
            return null;
        }

    }
}
