using System;
using GG.Core;
using UnityEngine;

namespace GG.Grid
{
    /// <summary>
    /// Base class for a grid that can dynamically resize to fit different UI container sizes
    /// </summary>
    public class UIScalableGrid : GridView<UIScalableGridCell, DataConfigUIScalableGrid>, ITickable
    {
        #region VARIABLES

        [Header("References")]
        [SerializeField] protected RectTransform _contentRect;
        [SerializeField] protected RectTransform _inputRect;
        [SerializeField] private Transform _cellParent;
        
        public TickGroup TickGroup => TickGroup.UIUpdate;
        public float CellSize { get; private set; }
        public float CellSpacing => _config.CellSpacing;
        public Action OnCellsUpdated;

        protected RectTransform _rect;
        private bool _updateFlag;

        #endregion VARIABLES


        #region INITIALIZATION

        protected virtual void OnEnable()
        {
            TickRouter.Register(this);
            if (_rect && _config)
                _updateFlag = true;
        }

        protected virtual void OnDisable()
        {
            TickRouter.Unregister(this);
        }

        public override void Init(DataConfigUIScalableGrid config, int operatorIndex = 0)
        {
            base.Init(config, operatorIndex);

            _rect = GetComponent<RectTransform>();
            _updateFlag = true;
        }

        protected override UIScalableGridCell InstantiateCell(int xCoord, int yCoord)
        {
            return Instantiate(_config.PrefabCellView, _cellParent).GetComponent<UIScalableGridCell>();
        }

        #endregion INITIALIZATION


        #region CALLBACKS

        private void OnRectTransformDimensionsChange()
        {
            _updateFlag = true;
        }
        
        protected override void OnConfigValidated()
        {
            base.OnConfigValidated();

            _updateFlag = true;
        }

        #endregion CALLBACKS


        #region RESIZE

        private void UpdateCellSizes()
        {
            if (_grid == null)
                return;
            
            Vector2 anchor = _contentRect.rect.center;
            float availableWidth = _contentRect.rect.width - ((_grid.GridWidth + 1) * _config.CellSpacing) - (_config.BorderSpacing * 2);
            float maxCellWidth = availableWidth / _grid.GridWidth;

            float availableHeight = _contentRect.rect.height - ((_grid.GridHeight + 1) * _config.CellSpacing) - (_config.BorderSpacing * 2);
            float maxCellHeight = availableHeight / _grid.GridHeight;
            
            CellSize = Mathf.Min(maxCellHeight, maxCellWidth);
            float cellContainerWidth = (_grid.GridWidth * CellSize) + ((_grid.GridWidth + 1) * _config.CellSpacing);
            float cellContainerHeight = (_grid.GridHeight * CellSize) + ((_grid.GridHeight + 1) * _config.CellSpacing);

            // The input rect should envelope the cells
            float cellsWidth = (CellSize * _grid.GridWidth) + (_config.CellSpacing * _grid.GridWidth);
            float cellsHeight = (CellSize * _grid.GridHeight) + (_config.CellSpacing * _grid.GridHeight);
            _inputRect.sizeDelta = 
                new Vector2(cellsWidth * _config.CellAreaInputNormalizedSize, cellsHeight * _config.CellAreaInputNormalizedSize);
            _inputRect.anchoredPosition = Vector2.zero;

            for (int i = 0; i < _cellViews.Length; i++)
            {
                UIScalableGridCell cell = _cellViews[i];
                cell.CellRect.sizeDelta = new Vector2(CellSize, CellSize);
                float xPos = (anchor.x + (cell.XCoord * CellSize) + (_config.CellSpacing + (cell.XCoord * _config.CellSpacing)) 
                              + (CellSize / 2)) - (cellContainerWidth / 2);
                float yPos = (anchor.y - (cell.YCoord * CellSize) - (_config.CellSpacing + (cell.YCoord * _config.CellSpacing)) 
                              - (CellSize / 2)) + (cellContainerHeight / 2);
                cell.CellRect.anchoredPosition = new Vector2 (xPos, yPos);
            }
            
            OnCellsUpdated?.Invoke();
        }

        /// <summary>
        /// We can manually set the grid size to match for a specific cell size, ie if trying to visually match different grids
        /// </summary>
        public void SetGridSizeManually(float targetCellSize)
        {
            float targetWidth = 
                (_grid.GridWidth * targetCellSize) + ((_grid.GridWidth + 1) * _config.CellSpacing) + (_config.BorderSpacing * 2);
            float targetHeight = 
                (_grid.GridHeight * targetCellSize) + ((_grid.GridHeight + 1) * _config.CellSpacing) + (_config.BorderSpacing * 2);
            
            _rect.sizeDelta = new Vector2(targetWidth, targetHeight);
            CellSize = targetCellSize;
            
            OnCellsUpdated?.Invoke();
        }

        #endregion RESIZE


        #region TICK

        public void Tick(float delta)
        {
            if (_updateFlag)
            {
                _updateFlag = false;
                UpdateCellSizes();
            }

            OnTick(delta);
        }

        protected virtual void OnTick(float delta) { }

        #endregion TICK
    }
}