using System;
using UnityEngine;

namespace GG.Grid
{
    /// <summary>
    /// Creates visible objects to represent grid in game. Overwrite with specific implementation
    /// </summary>
    public abstract class GridView <TCell, TConfig>: MonoBehaviour, IGridView, IGridInputReceiver 
        where TCell : GridCell 
        where TConfig : DataConfigGridView
    {
        #region VARIABLES

        public Grid Grid => _grid;
        public int CellCount => _cellViews.Length;
        public Vector3 Center => new Vector3(_config.GridCellSize * _grid.GridWidth, 0, _config.GridCellSize * _grid.GridHeight) / 2;
        public GridCell BaseCellAtIndex(int index) => _cellViews[index];
        public TCell CellAtIndex(int index) => _cellViews[index];
        public TCell CellAtCoords(int x, int y) => _cellViews[_grid.GetFlattenedIndexForCoords(x, y)];
        public TCell CellAtCoords(Vector2Int coords) => _cellViews[_grid.GetFlattenedIndexForCoords(coords.x, coords.y)];
        public TCell[] CellViews => _cellViews;

        /// <summary>
        /// Callback fired when the input pointer enters a new cell
        /// </summary>
        public Action OnGridPointerEnter;
        public Action OnGridPointerExit;
        public Action<GridCell> OnCellPointerEnter;
        public Action<GridCell> OnCellPointerExit;
        public Action<GridCell> OnCellPointerSelect;
        public Action<GridCell> OnCellPointerSelectRelease;
        public Action OnPointerSelectReleaseOffGrid;
        public Action<GridCell> OnCellPointerAlternateSelect;
        public Action<GridCell> OnCellPointerAlternateSelectRelease;
        public Action OnViewAlternateSelect;    // Right-click

        protected Grid _grid;
        protected TCell[] _cellViews; // Flattened array of objects for every cell in grid
        protected bool _freezeCallbacks;
        protected TConfig _config;
        protected int _operatorIndex;

        #endregion VARIABLES
        
        
        #region INITIALIZATION

        public virtual void Init(Grid grid, TConfig config, int operatorIndex = 0)
        {
            _grid = grid;
            _config = config;
            _config.OnValidated += OnConfigValidated;
            _cellViews = new TCell[_grid.GridWidth * _grid.GridHeight];
            _operatorIndex = operatorIndex;

            // Spawn a prefab for every cell in the grid
            _grid.IterateGridCoordinates(OnIterateGridCellForViewSpawn);
        }

        protected virtual void OnConfigValidated() 
        {
            
        }

        /// <summary>
        /// Callback from Grid class as it iterates over all cells after spawning them; setup individual cells here
        /// </summary>
        protected virtual void OnIterateGridCellForViewSpawn(int xCoord, int yCoord, int cellIndex)
        {
            // Create and initialize cell
            _cellViews[cellIndex] = InstantiateCell(xCoord, yCoord);
            _cellViews[cellIndex].SetupCell(xCoord, yCoord, cellIndex);
        }

        /// <summary>
        /// Instantiates a new cell in the grid. Override for custom instantiation behavior
        /// </summary>
        protected virtual TCell InstantiateCell(int xCoord, int yCoord)
        {
            Vector3 cellPosition = new Vector3(xCoord * _config.GridCellSize, 0, yCoord * _config.GridCellSize);
            return Instantiate(_config.PrefabCellView, cellPosition, Quaternion.identity).GetComponent<TCell>();
        }

        /// <summary>
        /// We can pass in a cell instance to replace an existing cell; the old cell is returned to caller for destruction or whatever
        /// </summary>
        public virtual TCell ReplaceCellAtCoords(int oldCellX, int oldCellY, TCell newCell)
        {
            // Destroy the old cell
            int index = _grid.GetFlattenedIndexForCoords(oldCellX, oldCellY);
            TCell oldCell = _cellViews[index];
            
            // Assign new cell
            _cellViews[index] = newCell;
            _cellViews[index].SetupCell(oldCellX, oldCellY, index);
            
            // Return the old cell
            return oldCell;
        }

        private void OnDestroy()
        {
            _config.OnValidated -= OnConfigValidated;
        }

        #endregion INITIALIZATION


        #region POINTER

        public virtual void DoGridPointerEnter()
        {
            if (_freezeCallbacks)
                return;
            
            OnGridPointerEnter?.Invoke();
        }

        public virtual void DoGridPointerExit()
        {
            if (_freezeCallbacks)
                return;
            
            OnGridPointerExit?.Invoke();
        }

        public virtual void DoCellPointerEnter(GridCell cell)
        {
            if (_freezeCallbacks)
                return;
            
            OnCellPointerEnter?.Invoke(cell);
            foreach (IGridCellOccupant occupant in cell.Occupants)
            {
                occupant.DoCellPointerEnter(cell);
            }
        }
        
        public virtual void DoCellPointerExit(GridCell cell)
        {
            if (_freezeCallbacks)
                return;
            
            OnCellPointerExit?.Invoke(cell);
            foreach (IGridCellOccupant occupant in cell.Occupants)
            {
                occupant.DoCellPointerExit(cell);
            }
        }

        public virtual void DoCellPointerSelect(GridCell cell)
        {
            if (_freezeCallbacks)
                return;
            
            OnCellPointerSelect?.Invoke(cell);
            foreach (IGridCellOccupant occupant in cell.Occupants)
            {
                occupant.DoCellPointerSelect(cell);
            }
        }
        
        public virtual void DoCellPointerSelectRelease(GridCell cell)
        {
            if (_freezeCallbacks)
                return;
            
            OnCellPointerSelectRelease?.Invoke(cell);
            foreach (IGridCellOccupant occupant in cell.Occupants)
            {
                occupant.DoCellPointerSelectRelease(cell);
            }
        }

        public virtual void DoGridPointerReleaseOffGrid()
        {
            if (_freezeCallbacks)
                return;
            
            OnPointerSelectReleaseOffGrid?.Invoke();
        }

        public virtual void DoCellPointerAlternateSelect(GridCell cell)
        {
            if (_freezeCallbacks)
                return;
            
            OnCellPointerAlternateSelect?.Invoke(cell);
            foreach (IGridCellOccupant occupant in cell.Occupants)
            {
                occupant.DoCellPointerAlternateSelect(cell);
            }
        }
        
        public virtual void DoCellPointerAlternateSelectRelease(GridCell cell)
        {
            if (_freezeCallbacks)
                return;
            
            OnCellPointerAlternateSelectRelease?.Invoke(cell);
            foreach (IGridCellOccupant occupant in cell.Occupants)
            {
                occupant.DoCellPointerAlternateSelectRelease(cell);
            }
        }

        void IGridInputReceiver.DoAlternateSelect()
        {
            OnViewAlternateSelect?.Invoke();
        }

        #endregion POINTER


        #region CALLBACKS

        public void SetCallbacksActive(bool active)
        {
            _freezeCallbacks = !active;
        }

        #endregion CALLBACKS
    }
}