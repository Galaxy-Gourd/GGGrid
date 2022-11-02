using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GG.Grid
{
    /// <summary>
    /// Utility class to perform flood fill on grid
    /// </summary>
    public static class GridFloodFill
    {
        #region VARIABLES

        public static DataGridFloodFillResult Result { get; private set; }

        private static DataGridFloodFillRequest _request;
        private static List<int> _closed = new ();
        private static List<CellState> _sources = new List<CellState>();
        private static readonly List<CellState> _outerMoveCells = new List<CellState>();

        private struct CellState
        {
            public int Index;
            public int MoveRangeAtCell;
        }

        #endregion VARIABLES


        #region API

        public static DataGridFloodFillResult FloodFill(DataGridFloodFillRequest request)
        {
            ClearCaches(request);
            List<CellState> open = SetupFloodFillProcess();
            List<List<CellState>> rawResults = RunFloodFill(_request, open, _request.Range, _request.ApplyMovePenalty);
            
            // Gather and filter results
            Result = new DataGridFloodFillResult()
            {
                Key = _request.Key,
                ValidIndices = TranslateCellStatesToResults(rawResults),
                OuterIndices = TranslateCellStatesToResults(_outerMoveCells)
            };
            
            if (_request.Filter != null)
            {
                Result = _request.Filter.Filter(Result);
            }

            return Result;
        }

        #endregion API


        #region LOGIC

        private static void ClearCaches(DataGridFloodFillRequest request)
        {
            _closed.Clear();
            _sources.Clear();
            _outerMoveCells.Clear();
            _request = request;
        }

        private static List<CellState> SetupFloodFillProcess()
        {
            List<CellState> open = new List<CellState>();
            _closed = new List<int>();
            foreach (IGridFloodFillNavigable source in _request.SourceCells)
            {
                open.Add(new CellState()
                {
                    Index = source.Index,
                    MoveRangeAtCell = _request.Range
                });
                
                _closed.Add(source.Index);
            }
            
            return open;
        }
        
        private static List<List<CellState>> RunFloodFill(
            DataGridFloodFillRequest request, 
            List<CellState> open, 
            int range,
            bool applyMovePenalty)
        {
            _sources = new List<CellState>(open);
            List<List<CellState>> movementResults = new List<List<CellState>>();
            bool gathering = true;
            while (gathering)
            {
                List<CellState> gatheringNeighbors = new List<CellState>();
                while (open.Count > 0)
                {
                    // Get the open cell with the next highest movement range
                    int nextCellIndex = GetNextSourceCellIndex(open, range);
                    CellState nextCell = open[nextCellIndex];
                    
                    // Add neighbors of this open cell to the neighbors list
                    List<CellState> neighbors = GetValidCellNeighbors(request, nextCell, applyMovePenalty);
                    if (neighbors.Count == 0)
                    {
                        _outerMoveCells.Add(nextCell);
                    }
                    
                    foreach (CellState cell in neighbors)
                    {
                        if (!gatheringNeighbors.Contains(cell))
                        {
                            gatheringNeighbors.Add(cell);
                        }
                    }
                
                    // Close this cell
                    _closed.Add(nextCell.Index);
                    open.RemoveAt(nextCellIndex);
                }
                
                // Now we have new open cells, unless we're finished
                open.Clear();
                if (gatheringNeighbors.Count > 0)
                {
                    open = gatheringNeighbors;
                    movementResults.Add(new List<CellState>(open));
                }
                else
                {
                    gathering = false;
                }
            }

            return movementResults;
        }
        
        private static int GetNextSourceCellIndex(List<CellState> openCells, int maxRange)
        {
            // Iterate through all known neighbors, find the neighbor with the highest move juice remaining
            int index = 0;
            int highestJuice = maxRange;
            for (int i = 0; i < openCells.Count; i++)
            {
                if (openCells[i].Index == -1)
                    continue;
                
                if (openCells[i].MoveRangeAtCell >= highestJuice)
                {
                    index = i;
                    highestJuice = openCells[i].MoveRangeAtCell;
                }
            }

            return index;
        }

        private static List<CellState> GetValidCellNeighbors(
            DataGridFloodFillRequest request, 
            CellState source, 
            bool applyPen)
        {
            List<CellState> neighbors = new List<CellState>();
            foreach (int thisNeighbor in request.Grid.GetGridCellCardinalNeighborIndices(source.Index))
            {
                if (thisNeighbor != -1 &&
                    !_closed.Contains(thisNeighbor) &&
                    request.View.BaseCellAtIndex(thisNeighbor).FloodFillMovePenalty(_request.Key) >= 0 &&
                    !CellStateListDoesContainIndex(neighbors, thisNeighbor))
                {
                    if (_sources.Contains(source))
                    {
                        neighbors.Add(new CellState()
                        {
                            Index = thisNeighbor,
                            MoveRangeAtCell = source.MoveRangeAtCell
                        });
                        
                        continue;
                    }
                    
                    int movePen = request.View.BaseCellAtIndex(source.Index).FloodFillMovePenalty(_request.Key);
                    int newMoveRange = applyPen ? (source.MoveRangeAtCell - (1 + movePen)) : source.MoveRangeAtCell - 1;
                    
                    if (newMoveRange > 0)
                    {
                        neighbors.Add(new CellState()
                        {
                            Index = thisNeighbor,
                            MoveRangeAtCell = newMoveRange
                        });
                    }
                }
            }

            return neighbors;
        }

        private static bool CellStateListDoesContainIndex(List<CellState> cells, int index)
        {
            bool contains = false;
            foreach (CellState cell in cells)
            {
                if (cell.Index == index)
                {
                    contains = true;
                    break;
                }
            }

            return contains;
        }

        private static List<int[]> TranslateCellStatesToResults(List<List<CellState>> cells)
        {
            List<int[]> thisResults = new List<int[]>();
            foreach (List<CellState> row in cells)
            {
                int[] indices = new int[row.Count];
                for (int i = 0; i < row.Count; i++)
                {
                    indices[i] = row[i].Index;
                }

                thisResults.Add(indices);
            }

            return thisResults;
        }

        private static List<int> TranslateCellStatesToResults(List<CellState> cells)
        {
            List<int> thisResults = new List<int>();
            foreach (CellState cell in cells)
            {
                thisResults.Add(cell.Index);
            }

            return thisResults;
        }

        #endregion LOGIC


        #region UTILITY

        public static bool ListContainsIndex(int index, List<int[]> list)
        {
            if (list == null)
                return false;
            
            bool contains = false;
            foreach (int[] group in list)
            {
                if (group.Contains(index))
                {
                    contains = true;
                    break;
                }
            }

            return contains;
        }

        public static int GetFloodDistanceForCell(int index, DataGridFloodFillResult result)
        {
            if (result.ValidIndices == null)
                return -1;
            
            int distance = -1;
            foreach (int[] group in result.ValidIndices)
            {
                if (group.Contains(index))
                {
                    distance = result.ValidIndices.IndexOf(group);
                    break;
                }
            }

            return distance;
        }

        public static List<int> GetAllValidIndicesForResult(DataGridFloodFillResult result)
        {
            if (result.ValidIndices == null)
                return new List<int>();
                
            List<int> indices = new List<int>();
            foreach (int[] group in result.ValidIndices)
            {
                indices.AddRange(group);
            }

            return indices;
        }

        public static IGridFloodFillNavigable[] GetNavigablesForCells(List<int> indices, IGridView view)
        {
            if (indices == null)
                return Array.Empty<IGridFloodFillNavigable>();
            
            IGridFloodFillNavigable[] results = new IGridFloodFillNavigable[indices.Count];
            for (int i = 0; i < indices.Count; i++)
            {
                results[i] = view.BaseCellAtIndex(indices[i]);
            }

            return results;
        }

        #endregion UTILITY
    }
}