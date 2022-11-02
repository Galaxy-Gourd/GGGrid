using UnityEngine;

namespace GG.Grid
{
    public interface IGridCellOccupant : IGridInputReceiver
    {
        #region PROPERTIES

        GridCell Cell { get; set; }
        Transform Transform { get; }

        #endregion PROPERTIES
    }
}