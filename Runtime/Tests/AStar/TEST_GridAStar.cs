using GG.Camera;
using GG.Core;
using GG.Operators;
using GG.Tests;
using UnityEngine;
using UnityEngine.UI;

namespace GG.Grid
{
    public class TEST_GridAStar : TestController
    {
        #region VARIABLES

        [Header("References")]
        [SerializeField] private GameObject _prefabScalableGrid;
        [SerializeField] private DataConfigUIScalableGrid _config;

        private UIScalableGrid _grid;
        private GridCell _sourceCell;
        private int[] _currentPath;
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
            _grid.Init(_config);
            _grid.GetComponent<GridInput>().Init(opIndex);
            _colorDefault = _grid.CellViews[0].GetComponent<Image>().color;
            
            _grid.OnCellPointerSelect += OnCellPointerSelect;
            _grid.OnCellPointerAlternateSelect += OnCellPointerSelectAlternate;
            _grid.OnCellPointerEnter += OnCellPointerEnter;
        }

        protected override void CleanupTest()
        {
            base.CleanupTest();
            
            if (!_grid)
                return;
            
            _grid.OnCellPointerSelect -= OnCellPointerSelect;
            _grid.OnCellPointerAlternateSelect -= OnCellPointerSelectAlternate;
            _grid.OnCellPointerEnter -= OnCellPointerEnter;

            Destroy(_grid.gameObject);
        }

        #endregion INITIALIZATION


        #region INPUT TESTS

        private void OnCellPointerSelect(GridCell cell)
        {
            _sourceCell = cell;
        }

        private void OnCellPointerSelectAlternate(GridCell cell)
        {
            if (cell is GridCellTestAStar aCell)
            {
                aCell.Impassible = !aCell.Impassible;
                aCell.GetComponent<Image>().color = aCell.Impassible ? Color.white : _colorDefault;
            }
        }

        private void OnCellPointerEnter(GridCell cell)
        {
            if (_sourceCell == null)
                return;
            
            DataGridAStarResult result = GridAStar.AStar(new DataGridAStarRequest()
            {
                Grid = _grid.Grid,
                View = _grid,
                Source = _sourceCell,
                Destination = cell
            });
            
            // Undo old path
            if (_currentPath != null)
            {
                foreach (int pathCell in _currentPath)
                {
                    if (_grid.CellAtIndex(pathCell) is GridCellTestAStar { Impassible: true })
                        continue;
                    
                    _grid.CellAtIndex(pathCell).GetComponent<Image>().color = _colorDefault;
                }
            }

            foreach (int pathCell in result.Path)
            {
                if (_grid.CellAtIndex(pathCell) is GridCellTestAStar { Impassible: true })
                    continue;
                
                _grid.CellAtIndex(pathCell).GetComponent<Image>().color = Color.red;
            }

            _currentPath = result.Path;
        }

        #endregion INPUT TESTS
    }
}
