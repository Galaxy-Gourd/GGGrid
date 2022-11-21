using System;
using System.Collections.Generic;
using UnityEngine;

namespace GG.Grid
{
    /// <summary>
    /// Creates and manages basic 2-dimensional grid of coordinates w/ cell sizes
    /// </summary>
    public class Grid
    {
        #region VARIABLES

        public int GridWidth => _width;
        public int GridHeight => _height;
        public int CellCount => _width * _height;
        public int GetFlattenedIndexForCoords(int x, int y) => x + (_width * y);
        public Vector2Int GetCoordsForFlattenedIndex(int index) => new (index % _width, index / _width);

        // Private
        private readonly int _width;
        private readonly int _height;
        private readonly int[,] _gridArray;

        #endregion VARIABLES
        
        
        #region CONSTRUCTION

        public Grid(int width, int height)
        {
            _width = width;
            _height = height;
            _gridArray = new int[_width, _height];
        }

        #endregion CONSTRUCTION


        #region UTILITY

        /// <summary>
        /// Cycles through all grid coordinates and calls callback action for each cell
        /// </summary>
        /// <param name="cellCallback"></param>
        public void IterateGridCoordinates(Action<int, int, int> cellCallback)
        {
            for (int x = 0; x < _gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < _gridArray.GetLength(1); y++)
                {
                    cellCallback.Invoke(x, y, GetFlattenedIndexForCoords(x, y));
                }
            }
        }

        /// <summary>
        /// Gets neighbor indices for a specified source cell
        /// </summary>
        /// <param name="sourceIndex">The index of the cell whose neighbors we wish to get</param>
        /// <returns>Array of 8 neightboring indices, clockwise starting from top-left. Null/off-grid indices are returned as -1</returns>
        ///
        /*
         *  0 1 2
         *  7 x 3
         *  6 5 4
         */
        public int[] GetGridCellNeighborIndices(int sourceIndex)
        {
            int[] neighbors = new int[8];
            neighbors[0] = GetIndexOfCellInDirection(sourceIndex, GridDirection.TopLeft);
            neighbors[1] = GetIndexOfCellInDirection(sourceIndex, GridDirection.Top);
            neighbors[2] = GetIndexOfCellInDirection(sourceIndex, GridDirection.TopRight);
            neighbors[3] = GetIndexOfCellInDirection(sourceIndex, GridDirection.Right);
            neighbors[4] = GetIndexOfCellInDirection(sourceIndex, GridDirection.BottomRight);
            neighbors[5] = GetIndexOfCellInDirection(sourceIndex, GridDirection.Bottom);
            neighbors[6] = GetIndexOfCellInDirection(sourceIndex, GridDirection.BottomLeft);
            neighbors[7] = GetIndexOfCellInDirection(sourceIndex, GridDirection.Left);
            return neighbors;
        }

        /// <summary>
        /// Gets neighbor indices for a specified source cell in ONLY the 4 cardinal directions
        /// </summary>
        /// <param name="sourceIndex">The index of the cell whose neighbors we wish to get</param>
        /// <returns>Array of 8 neightboring indices, clockwise starting from top. Null/off-grid indices are returned as -1</returns>
        ///
        /*
         *    0 
         *  3 x 1
         *    2 
         */
        public int[] GetGridCellCardinalNeighborIndices(int sourceIndex)
        {
            int[] neighbors = new int[4];
            neighbors[0] = GetIndexOfCellInDirection(sourceIndex, GridDirection.Top);
            neighbors[1] = GetIndexOfCellInDirection(sourceIndex, GridDirection.Right);
            neighbors[2] = GetIndexOfCellInDirection(sourceIndex, GridDirection.Bottom);
            neighbors[3] = GetIndexOfCellInDirection(sourceIndex, GridDirection.Left);
            return neighbors;
        }

        public int GetIndexOfCellInDirection(int sourceIndex, GridDirection direction)
        {
            Vector2Int coords = GetCoordsForFlattenedIndex(sourceIndex);
            switch (direction)
            {
                case GridDirection.TopLeft: return coords.x == 0 || coords.y == _height - 1 ? -1 : sourceIndex + _width - 1;
                case GridDirection.Top: return coords.y == _height - 1 ? -1 : sourceIndex + _width;
                case GridDirection.TopRight: return coords.y == _height - 1 || coords.x == _width - 1 ? -1 : sourceIndex + _width + 1;
                case GridDirection.Right: return coords.x == _width - 1 ? -1 : sourceIndex + 1;
                case GridDirection.BottomRight: return coords.x == _width - 1 || coords.y == 0 ? -1 : sourceIndex - _width + 1;
                case GridDirection.Bottom: return coords.y == 0 ? -1 : sourceIndex - _width;
                case GridDirection.BottomLeft: return coords.x == 0 || coords.y == 0 ? -1 : sourceIndex - _width - 1;
                case GridDirection.Left: return coords.x == 0 ? -1 : sourceIndex - 1;
            }

            return -1;
        }

        /// <summary>
        /// Performs a basic flood fill from the source and returns all valid cells
        /// </summary>
        /// <param name="sourceIndex"></param>
        /// <param name="range"></param>
        /// <param name="includeDiagonals"></param>
        /// <returns>Dictionary - key is distance from source, value is list of indices</returns>
        public Dictionary<int, int[]> GetGridCellNeighborsInRange(int sourceIndex, int range, bool includeDiagonals)
        {
            Dictionary<int, int[]> cellMap = new Dictionary<int, int[]>();
            List<int> activeCells = new List<int>(){sourceIndex};
            List<int> checkedCells = new List<int>();
            int layersCalcd = 0;

            while (layersCalcd < range)
            {
                List<int> newNeighbors = new List<int>();
                foreach (int source in activeCells)
                {
                    // Get the neighbors of this source cell
                    int[] neighbors = includeDiagonals ? GetGridCellNeighborIndices(source) : GetGridCellCardinalNeighborIndices(source);
                    foreach (int thisNeighbor in neighbors)
                    {
                        // If this is a unique nieghbor, add it to the list of new cells to be checked
                        if (thisNeighbor != -1 && !newNeighbors.Contains(thisNeighbor) && !checkedCells.Contains(thisNeighbor))
                        {
                            newNeighbors.Add(thisNeighbor);
                        }
                    }
                }
                
                foreach (int cell in activeCells)
                {
                    if (!checkedCells.Contains(cell))
                    {
                        checkedCells.Add(cell);
                    }
                }
                activeCells.Clear();
                
                // Add new unchecked cells to active cells
                foreach (int neighbor in newNeighbors)
                {
                    if (!checkedCells.Contains(neighbor))
                    {
                        activeCells.Add(neighbor);
                    }
                }

                cellMap.Add(layersCalcd, activeCells.ToArray());
                layersCalcd++;
            }

            return cellMap;
        }

        public List<int> GetGridCellNeighborsInRangeFlattened(int sourceIndex, int range, bool includeDiagonals)
        {
            List<int> flattened = new List<int>();
            Dictionary<int, int[]> results = GetGridCellNeighborsInRange(sourceIndex, range, includeDiagonals);
            foreach (KeyValuePair<int, int[]> result in results)
            {
                flattened.AddRange(result.Value);
            }

            return flattened;
        }

        /// <summary>
        /// Returns true if the given coordinates are contained within the grid
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        public bool CoordsAreWithinGrid(Vector2Int coords)
        {
            if (coords.x < 0 || coords.y < 0)
                return false;

            if (coords.x >= _width || coords.y >= _height)
                return false;

            return true;
        }

        #endregion UTILITY
    }
}