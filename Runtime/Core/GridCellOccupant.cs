using System;
using UnityEngine;

namespace GG.Grid
{
    /// <summary>
    /// Base class for MonoBehaviour-based grid cell occupants
    /// </summary>
    public abstract class GridCellOccupant : MonoBehaviour, IGridCellOccupant
    {
        #region VARIABLES

        public GridCell Cell { get; set; }
        public Action<GridCellOccupant> OnCellPointerEnter { get; set; }
        public Action<GridCellOccupant> OnCellPointerExit { get; set; }
        public Action<GridCellOccupant> OnCellSelect { get; set; }
        public Action<GridCellOccupant> OnCellSelectRelease { get; set; }
        public Action<GridCellOccupant> OnCellAlternateSelect { get; set; }
        public Action<GridCellOccupant> OnCellAlternateSelectRelease { get; set; }
        public Transform Transform => transform;

        protected Grid _grid;

        #endregion VARIABLES


        #region CALLBACKS

        public void DoGridPointerEnter()
        {
            
        }

        public void DoGridPointerExit()
        {
            
        }

        public virtual void DoCellPointerEnter(GridCell cell)
        {
            OnCellPointerEnter?.Invoke(this);
        }

        public virtual void DoCellPointerExit(GridCell cell)
        {
            OnCellPointerExit?.Invoke(this);
        }

        public virtual void DoCellPointerSelect(GridCell cell)
        {
            OnCellSelect?.Invoke(this);
        }
        
        public virtual void DoCellPointerSelectRelease(GridCell cell)
        {
            OnCellSelectRelease?.Invoke(this);
        }

        public void DoGridPointerReleaseOffGrid()
        {
            
        }

        public virtual void DoCellPointerAlternateSelect(GridCell cell)
        {
            OnCellAlternateSelect?.Invoke(this);
        }
        
        public virtual void DoCellPointerAlternateSelectRelease(GridCell cell)
        {
            OnCellAlternateSelectRelease?.Invoke(this);
        }

        void IGridInputReceiver.DoAlternateSelect() { }

        #endregion CALLBACKS
    }
}