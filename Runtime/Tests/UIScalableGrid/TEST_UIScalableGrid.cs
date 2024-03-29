using System.Collections;
using System.Collections.Generic;
using GG.Camera;
using GG.Core;
using GG.Operators;
using GG.Tests;
using UnityEngine;
using UnityEngine.UI;

namespace GG.Grid
{
    public class TEST_UIScalableGrid : TestController
    {
        #region VARIABLES

        [Header("References")]
        [SerializeField] private GameObject _prefabScalableGrid;
        [SerializeField] private DataConfigUIScalableGrid _config;

        private UIScalableGrid _grid;
        private Color _cellColor;

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
            // Instantiate scalable grid component for every operator
            Canvas uiCanvas = Modules.Get<ModuleCamera>().GetPlayerUICanvas(opIndex);
            _grid = Instantiate(_prefabScalableGrid, uiCanvas.transform).GetComponent<UIScalableGrid>();
            _grid.Init(_config);
            _grid.GetComponent<GridInput>().Init(opIndex);
            
            _cellColor = _grid.CellAtIndex(0).GetComponent<Image>().color;

            _grid.OnCellPointerEnter += OnCellPointerEnter;
            _grid.OnCellPointerExit += OnCellPointerExit;
            _grid.OnCellPointerSelect += OnCellPointerSelect;
            _grid.OnCellPointerSelectRelease += OnCellPointerSelectRelease;
            _grid.OnCellPointerAlternateSelect += OnCellPointerAlternateSelect;
            _grid.OnCellPointerAlternateSelectRelease += OnCellPointerAlternateSelectRelease;
        }

        protected override void CleanupTest()
        {
            base.CleanupTest();
            
            if (!_grid)
                return;
            
            _grid.OnCellPointerEnter -= OnCellPointerEnter;
            _grid.OnCellPointerExit -= OnCellPointerExit;
            _grid.OnCellPointerSelect -= OnCellPointerSelect;
            _grid.OnCellPointerSelectRelease -= OnCellPointerSelectRelease;
            _grid.OnCellPointerAlternateSelect -= OnCellPointerAlternateSelect;
            _grid.OnCellPointerAlternateSelectRelease -= OnCellPointerAlternateSelectRelease;
            
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

        private void OnCellPointerEnter(GridCell cell)
        {
            cell.GetComponent<Image>().color = Color.green;
        }
        
        private void OnCellPointerExit(GridCell cell)
        {
            cell.GetComponent<Image>().color = _cellColor;
        }
        
        private void OnCellPointerSelect(GridCell cell)
        {
            cell.GetComponent<Image>().color = Color.yellow;
        }

        private void OnCellPointerSelectRelease(GridCell cell)
        {
            cell.GetComponent<Image>().color = Color.green;
        }
        
        private void OnCellPointerAlternateSelect(GridCell cell)
        {
            cell.GetComponent<Image>().color = Color.red;
        }
        
        private void OnCellPointerAlternateSelectRelease(GridCell cell)
        {
            cell.GetComponent<Image>().color = Color.green;
        }

        #endregion INPUT TESTS
    }
}
