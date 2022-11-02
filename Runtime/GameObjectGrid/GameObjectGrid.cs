using UnityEngine;

namespace GG.Grid
{
    /// <summary>
    /// Wrapper for grids composed of GameObject instances
    /// </summary>
    public abstract class GameObjectGrid<TGridCell, TConfig> : GridView<TGridCell, TConfig>, IGameObjectGrid
        where TGridCell : GameObjectGridCell
        where TConfig : DataConfigGameObjectGrid
    {
        #region VARIABLES

        public LayerMask GridMask => _config == null ? LayerMask.NameToLayer("Default") : _config.GridLayers;
        public float InputDistance => _config == null ? 1000 : _config.GridInputDistance;

        #endregion VARIABLES


        #region INPUT
        
        public abstract GameObjectGridCell GetClosestCellFromPoint(Vector3 point, GameObject obj);

        #endregion INPUT
        
        
        #region INITIALIZATION

        public override void Init(Grid grid, TConfig config, int operatorIndex = 0)
        {
            base.Init(grid, config, operatorIndex);
            
            
        }

        #endregion INITIALIZATION

    }
}