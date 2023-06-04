using Models.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Models
{

    public class GridData:IGridData
    {
        private static readonly Lazy<GridData> instance = new Lazy<GridData>(
            () => new GridData());
        private readonly int size = 500;
        public ConcurrentDictionary<long, NewEntity> entities;
        public ConcurrentDictionary<long, List<Polyline>> lines; 
        public List<long>[,] linesIds;
        public bool[,] positions;
        public bool[,] linesObjects;
        public static GridData Instance => instance.Value;
    
        private GridData()
        {
            
            entities = new ConcurrentDictionary<long,NewEntity>();
            linesIds = new List<long>[size, size];
            lines = new ConcurrentDictionary<long, List<Polyline>>();
            positions = new bool[size, size];
            linesObjects = new bool[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    linesIds[i, j] = new List<long>();
                }
            }
            
        }
    }
}
