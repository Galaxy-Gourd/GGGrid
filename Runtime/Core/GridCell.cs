using System.Collections.Generic;
using UnityEngine;

namespace GG.Grid
{
    /// <summary>
    /// MonoBehaviour for a single cell of the world grid. Override for specific view implementation
    /// </summary>
    public abstract class GridCell : MonoBehaviour, IGridFloodFillNavigable, IGridCellAStarNavigable
    {
        #region VARIABLES

        // Utility accessors
        public int XCoord { get; private set; }
        public int YCoord { get; private set; }
        public int Index { get; private set; }
        public GridCellPointerState PointerState => _pointerState;
        public List<IGridCellOccupant> Occupants { get; protected set; }

        protected GridCellPointerState _pointerState;

        #endregion VARIABLES


        #region INITIALIZATION

        public virtual void SetupCell(int x, int y, int index)
        {
            XCoord = x;
            YCoord = y;
            Index = index;
            Occupants = new List<IGridCellOccupant>();
            
            SetCellName();
        }

        #endregion INITIALIZATION


        #region STATE

        public virtual void SetCellPointerState(GridCellPointerState state)
        {
            _pointerState = state;
        }

        #endregion STATE


        #region OCCUPANTS

        public virtual void AddOccupant(IGridCellOccupant occupant)
        {
            if (!Occupants.Contains(occupant))
            {
                // Remove from previous cell
                if (occupant.Cell)
                {
                    occupant.Cell.RemoveOccupant(occupant);
                }
                
                Occupants.Add(occupant);
                occupant.Cell = this;
            }
        }

        public virtual void RemoveOccupant(IGridCellOccupant occupant)
        {
            Occupants.Remove(occupant);
            occupant.Cell = null;
        }

        public virtual void RemoveAllOccupants()
        {
            foreach (IGridCellOccupant occupant in Occupants)
            {
                occupant.Cell = null;
            }
            
            Occupants.Clear();
        }

        #endregion OCCUPANTS
        
        
        #region UTILITY
        
        public virtual bool IsAStarNavigable(string requestTag)
        {
            return true;
        }

        public virtual int FloodFillMovePenalty(string key)
        {
            return 0;
        }

        protected virtual void SetCellName()
        {
            transform.name = "cell[" + XCoord + "," + YCoord + "] - " + Index;
        }

        #endregion UTILITY
    }
}