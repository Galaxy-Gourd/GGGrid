using System;
using System.Collections.Generic;
using UnityEngine;

namespace GG.Grid
{
    /// <summary>
    /// Utility class to perform A* pathfinding on grid
    /// </summary>
    public static class GridAStar
    {
        #region VARIABLES

        public static DataGridAStarResult Result { get; private set; }
        
        private static DataGridAStarRequest _request;
        private static List<DataGridAStarNode> _path = new();
        private static readonly List<DataGridAStarNode> _nodes = new();
        private static readonly List<DataGridAStarNode> _openList = new();
        private static readonly List<DataGridAStarNode> _closedList = new();
        private const int CONST_MOVECOST_STRAIGHT = 10;
        private const int CONST_MOVECOST_DIAGONAL = 14;

        #endregion VARIABLES


        #region API

        public static DataGridAStarResult AStar(DataGridAStarRequest request)
        {
            ClearCaches(request);
            DataGridAStarNode destination = SetupAStarProcess();
            RunAStar(destination);
            
            // Gather results
            Result = new DataGridAStarResult()
            {
                Path = GetPathIndicesFromPathNodes(_path)
            };

            return Result;
        }

        #endregion API


        #region LOGIC

        private static void ClearCaches(DataGridAStarRequest request)
        {
            _openList.Clear();
            _closedList.Clear();
            _path.Clear();
            _nodes.Clear();
            _request = request;
        }

        private static DataGridAStarNode SetupAStarProcess()
        {
            // Add list of nodes corresponding to grid cells
            for(int i = 0; i < _request.View.CellCount; i++)
            {
                _nodes.Add(new DataGridAStarNode
                {
                    Cell = _request.View.BaseCellAtIndex(i),
                    FromNode = null,
                    GCost = int.MaxValue,
                    HCost = _request.Source.IsAStarNavigable(_request.RequestTag) ? 0 : int.MaxValue
                });
            }
            
            DataGridAStarNode startNode = GetNodeDataForCell(_request.Source);
            DataGridAStarNode endNode = GetNodeDataForCell(_request.Destination);
            startNode.GCost = 0;
            startNode.HCost = CalculateDistanceCost(startNode, endNode);
            _openList.Add(startNode);

            return endNode;
        }

        private static void RunAStar(DataGridAStarNode destination)
        {
            while (_openList.Count > 0)
            {
                DataGridAStarNode currentNode = GetLowestFCostNode();
                if (currentNode == destination)
                {
                    // Finished
                    _path = CalculatePath(destination);
                    break;
                }

                _openList.Remove(currentNode);
                _closedList.Add(currentNode);

                // Cycle through neighbors of current cell
                foreach (int neighbor in _request.Grid.GetGridCellNeighborIndices(currentNode.Cell.Index))
                {
                    // Skip out of bounds and impassible cells
                    if (neighbor == -1 || !_request.View.BaseCellAtIndex(neighbor).IsAStarNavigable(_request.RequestTag))
                        continue;

                    DataGridAStarNode neighborNode = GetNodeDataForCell(_request.View.BaseCellAtIndex(neighbor));
                    if (_closedList.Contains(neighborNode))
                        continue;

                    int tentativeGCost = currentNode.GCost + CalculateDistanceCost(currentNode, neighborNode);
                    if (tentativeGCost < neighborNode.GCost)
                    {
                        neighborNode.FromNode = currentNode;
                        neighborNode.GCost = tentativeGCost;
                        neighborNode.HCost = CalculateDistanceCost(neighborNode, destination);

                        if (!_openList.Contains(neighborNode))
                        {
                            _openList.Add(neighborNode);
                        }
                    }
                }
            }
        }

        #endregion
        
        
        #region UTILITY

        private static DataGridAStarNode GetNodeDataForCell(IGridCellAStarNavigable cell)
        {
            return _nodes[cell.Index];
        }

        private static int CalculateDistanceCost(DataGridAStarNode nodeA, DataGridAStarNode nodeB)
        {
            int xDistance = Mathf.Abs(nodeA.Cell.XCoord - nodeB.Cell.XCoord);
            int yDistance = Mathf.Abs(nodeA.Cell.YCoord - nodeB.Cell.YCoord);
            int remaining = Mathf.Abs(xDistance - yDistance);
            return CONST_MOVECOST_DIAGONAL * Mathf.Min(xDistance, yDistance) + CONST_MOVECOST_STRAIGHT * remaining;
        }

        private static DataGridAStarNode GetLowestFCostNode()
        {
            DataGridAStarNode node = _openList[0];
            for (int i = 1; i < _openList.Count; i++)
            {
                if (_openList[i].FCost < node.FCost)
                {
                    node = _openList[i];
                }
            }

            return node;
        }

        private static List<DataGridAStarNode> CalculatePath(DataGridAStarNode endNode)
        {
            _path.Add(endNode);
            DataGridAStarNode currentNode = endNode;
            while (currentNode.FromNode != null)
            {
                _path.Add(currentNode.FromNode);
                currentNode = currentNode.FromNode;
            }

            _path.Reverse();
            return _path;
        }

        private static int[] GetPathIndicesFromPathNodes(List<DataGridAStarNode> nodePath)
        {
            int[] path = new int[nodePath.Count];
            for (int i = 0; i < nodePath.Count; i++)
            {
                path[i] = nodePath[i].Cell.Index;
            }

            return path;
        }

        #endregion UTILITY
    }
}