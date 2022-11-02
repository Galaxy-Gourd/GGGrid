using UnityEngine;

namespace GG.Grid
{
    public interface IGameObjectGrid : IGridInputReceiver
    {
        #region PROPERTIES

        LayerMask GridMask { get; }
        float InputDistance { get; }

        #endregion PROPERTIES


        #region METHODS
        
        /// <summary>
        /// We allow implementing grids to define how a cell is chosen based on a given position and/or hit gameObject
        /// </summary>
        GameObjectGridCell GetClosestCellFromPoint(Vector3 point, GameObject obj);

        #endregion METHODS
    }
}