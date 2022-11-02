using UnityEngine;

namespace GG.Grid
{
    public interface IGridView
    {
        #region PROPERTIES

        Grid Grid { get; }
        int CellCount { get; }
        Vector3 Center { get; }
        GridCell BaseCellAtIndex(int index);

        #endregion PROPERTIES
    }
}