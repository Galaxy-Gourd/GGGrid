using System.Collections.Generic;
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

        public Vector3 Center { get; private set; }
        public float GridWorldWidth => _config.GridCellSize * _grid.GridWidth;
        public float GridWorldHeight => _config.GridCellSize * _grid.GridHeight;
        public LayerMask GridMask => _config == null ? LayerMask.NameToLayer("Default") : _config.GridLayers;
        public float InputDistance => _config == null ? 1000 : _config.GridInputDistance;

        private float _cellDiagonal;

        #endregion VARIABLES


        #region INPUT
        
        public abstract GameObjectGridCell GetClosestCellFromPoint(Vector3 point, GameObject obj);

        #endregion INPUT
        
        
        #region INITIALIZATION

        public override void Init(Grid grid, TConfig config, int operatorIndex = 0)
        {
            base.Init(grid, config, operatorIndex);
            
            Center = new Vector3(_config.GridCellSize * _grid.GridWidth, 0, _config.GridCellSize * _grid.GridHeight) / 2;
            _cellDiagonal = Mathf.Sqrt((_config.GridCellSize * _config.GridCellSize) + (_config.GridCellSize * _config.GridCellSize)) / 2f;
        }

        protected override TGridCell InstantiateCell(int xCoord, int yCoord)
        {
            Vector3 cellPosition = new Vector3(xCoord * _config.GridCellSize, 0, yCoord * _config.GridCellSize);
            return Instantiate(_config.PrefabCellView, cellPosition, Quaternion.identity).GetComponent<TGridCell>();
        }

        #endregion INITIALIZATION


        #region UTILITY

        public TGridCell GetEncapsulatingCell(Vector2 point)
        {
            TGridCell eCell = _cellViews[0];
            foreach (TGridCell cell in _cellViews)
            {
                Vector2 ps = new Vector2(cell.Transform.position.x, cell.Transform.position.z);
                if (Vector2.Distance(ps, point) < _cellDiagonal)
                {
                    eCell = cell;
                    break;
                }
            }
            return eCell;
        }

        /// <summary>
        /// Returns the smallest group of cells that fully encapsulate the given point with the given radius
        /// </summary>
        public List<TGridCell> GetEncapsulatingCells(Vector2 point, float radius)
        {
            List<TGridCell> cells = new List<TGridCell>();
            foreach (TGridCell cell in _cellViews)
            {
                Vector2 ps = new Vector2(cell.Transform.position.x, cell.Transform.position.z);
                float distance = Vector2.Distance(ps, point);
                if (distance < _cellDiagonal + radius)
                {
                    cells.Add(cell);
                }
            }

            return cells;
        }

        #endregion UTILITY
    }
}