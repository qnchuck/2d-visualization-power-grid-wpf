﻿using Models;
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
            bool[,] visited = new bool[size,size];
            visited[src.X, src.Y] = true;
            
            Queue<Node> nq = new Queue<Node>();
            Node node = new Models.Entities.Node(src.X, src.Y) { Parent = null };
            nq.Enqueue(node);
            while (nq.Count != 0)
            {
                Node curr = nq.Peek();
                 
                for (int i = 0; i < 4; i++)
                {
                    int row = curr.X + rowNum[i];
                    int col = curr.Y + colNum[i];
                       
                    if (isValid(visited,row, col,size) && (!mat[row, col] || (row == dst.X && col == dst.Y)) &&
                    !visited[row, col])
                    {
                        visited[row, col] = true;

                        Node parent = curr;
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
            return null;
        }

    }
}
