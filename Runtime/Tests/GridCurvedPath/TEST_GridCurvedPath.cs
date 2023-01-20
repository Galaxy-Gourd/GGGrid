using System.Collections;
using GG.Camera;
using GG.Core;
using GG.Operators;
using GG.Tests;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GG.Grid
{
    public class TEST_GridCurvedPath : TestController
    {
        #region VARIABLES

        [Header("References")]
        [SerializeField] private GameObject _prefabScalableGrid;
        [SerializeField] private DataConfigUIScalableGrid _config;

        private UIScalableGrid _grid;
        private Color _cellColor;

        private GridCell _source;
        private GridCell _arc1;
        private GridCell _arc2;
        private GridCell _destniation;
        private int _step;
        private bool _pathIsCreated;
        private DataGridCurvedPathResult _result;

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
            
            _cellColor = _grid.CellAtIndex(0).GetComponent<Image>().color;

            _grid.OnCellPointerSelect += OnCellPointerSelect;
        }

        protected override void CleanupTest()
        {
            base.CleanupTest();
            
            if (!_grid)
                return;
            
            _grid.OnCellPointerSelect -= OnCellPointerSelect;
            
            Destroy(_grid.gameObject);
        }

        #endregion INITIALIZATION
        
        
        #region MANUAL TESTING 

        protected override void BeginManualTest()
        {
            
            base.BeginManualTest();
        }

        protected override IEnumerator CR_ManualTest()
        {
            while (true)
            {
                yield return null;
            }
        }

        #endregion MANUAL TESTING


        #region INPUT TESTS

        private void OnCellPointerSelect(GridCell cell)
        {
            switch (_step)
            {
                case 0:
                    _source = cell;
                    SetCellText(_source, "1");
                    _step++; 
                    break;
                case 1: 
                    _arc1 = cell;
                    SetCellText(_arc1, "2");
                    _step++; 
                    break;
                case 2: 
                    _arc2 = cell;
                    _step++; 
                    SetCellText(_arc2, "3");
                    break;
                case 3: 
                    _destniation = cell;
                    SetCellText(_destniation, "4");
                    Curve(); 
                    _step++; 
                    break;
                default: 
                    _step = 0; 
                    foreach (GridCell c in _result.Path)
                    {
                        c.GetComponent<Image>().color =_cellColor;
                    }
                    SetCellText(_source, "");
                    SetCellText(_arc1, "");
                    SetCellText(_arc2, "");
                    SetCellText(_destniation, "");
                    break;
            }
        }

        private void SetCellText(GridCell cell, string text)
        {
            cell.GetComponentInChildren<TMP_Text>().text = text;
        }

        private void Curve()
        {
            _result = GridCurvedPath.CurvedPath(new DataGridCurvedPathRequest()
            {
                View = _grid,
                Source = _source,
                Arc1 = _arc1,
                Arc2 = _arc2,
                Destination = _destniation,
                PathResolution = 1
            });

            foreach (GridCell cell in _result.Path)
            {
                cell.GetComponent<Image>().color = Color.cyan;
            }
        }
        #endregion INPUT TESTS
    }
}
