using GG.Camera;
using GG.Core;
using GG.Operators;
using GG.Tests;
using UnityEngine;
using UnityEngine.UI;

namespace GG.Grid
{
    public class TEST_GridFloodFill : TestController
    {
        #region VARIABLES

        [Header("References")]
        [SerializeField] private GameObject _prefabScalableGrid;
        [SerializeField] private DataConfigUIScalableGrid _config;

        private UIScalableGrid _grid;
        private Color _colorDefault;

        #endregion VARIABLES
        
        
        #region INITIALIZATION

        protected override void DoModulesLoadComplete()
        {
            base.DoModulesLoadComplete();

            for (int i = 0; i < Modules.Get<ModuleOperators>().OperatorsCount; i++)
            {
                SpawnGridForOperator(i);
            }
        }

        private void SpawnGridForOperator(int opIndex)
        {
            // Create grid
            Grid grid = new Grid(_config.GridWidth, _config.GridHeight);

            // Instantiate scalable grid component for every operator
            Canvas uiCanvas = Modules.Get<ModuleCamera>().GetPlayerUICanvas(opIndex);
            _grid = Instantiate(_prefabScalableGrid, uiCanvas.transform).GetComponent<UIScalableGrid>();
            _grid.Init(grid, _config);
            _grid.GetComponent<GridInput>().Init(opIndex);
            _colorDefault = _grid.CellViews[0].GetComponent<Image>().color;

            _grid.OnCellPointerSelect += OnCellPointerSelect;
            _grid.OnCellPointerEnter += OnCellPointerEnter;
        }

        protected override void CleanupTest()
        {
            base.CleanupTest();
            
            if (!_grid)
                return;
            
            _grid.OnCellPointerSelect -= OnCellPointerSelect;
            _grid.OnCellPointerEnter -= OnCellPointerEnter;

            Destroy(_grid.gameObject);
        }

        #endregion INITIALIZATION


        #region INPUT TESTS
        
        private void OnCellPointerSelect(GridCell cell)
        {
            if (cell is GridCellTestFloodFill fCell)
            {
                fCell.Impassible = !fCell.Impassible;
                fCell.GetComponent<Image>().color = fCell.Impassible ? Color.white : _colorDefault;
            }

            // Refresh flood fill
            OnCellPointerEnter(cell);
        }

        private void OnCellPointerEnter(GridCell cell)
        {
            DataGridFloodFillResult result = GridFloodFill.FloodFill(new DataGridFloodFillRequest
            {
                Grid = _grid.Grid,
                View = _grid,
                Range = 5,
                SourceCells = new IGridFloodFillNavigable[]{cell},
                ApplyMovePenalty = true
            });

            foreach (UIScalableGridCell c in _grid.CellViews)
            {
                if (c is GridCellTestFloodFill { Impassible: true })
                {
                    c.GetComponent<Image>().color = Color.white;
                    continue;
                }
                
                bool fill = GridFloodFill.ListContainsIndex(c.Index, result.ValidIndices);
                c.GetComponent<Image>().color = fill ? Color.red : _colorDefault;
            }
        }

        #endregion INPUT TESTS
    }
}
